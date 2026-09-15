using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkSwitcher
{
    /// <summary>
    /// Главное окно приложения: тумблеры Zapret (2 слота), Cloudflare WARP и AmneziaWG,
    /// журнал событий, переключение темы и значок в системном трее.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>Текущая тема интерфейса: <c>true</c> — тёмная, <c>false</c> — светлая. Сохраняется в конфиге.</summary>
        private bool isDarkTheme;

        /// <summary>Флаг реентерабельной защиты: <c>true</c>, пока идёт принудительная синхронизация UI с реальным состоянием служб.</summary>
        private bool isUpdatingStrategy;

        /// <summary>Момент (UTC), до которого таймер синхронизации не трогает галочку WARP — даём warp-cli время подключиться.</summary>
        private DateTime warpToggleGraceUntilUtc = DateTime.MinValue;

        /// <summary>Таймер периодической сверки состояния тумблеров с реальным состоянием процессов и служб.</summary>
        private System.Windows.Forms.Timer syncTimer;

        /// <summary>Значок приложения в системном трее.</summary>
        private NotifyIcon trayIcon;

        /// <summary>Контекстное меню значка в трее.</summary>
        private ContextMenuStrip trayMenu;

        /// <summary>Пункт меню трея для Слота 1.</summary>
        private ToolStripMenuItem itemSlot1;

        /// <summary>Пункт меню трея для Слота 2.</summary>
        private ToolStripMenuItem itemSlot2;

        /// <summary>Пункт меню трея для Cloudflare WARP.</summary>
        private ToolStripMenuItem itemWarp;

        /// <summary>Пункт меню трея для AmneziaWG.</summary>
        private ToolStripMenuItem itemAmnezia;

        /// <summary><c>true</c>, если приложение запущено с флагом <c>--minimized</c> (автозапуск Windows) и должно стартовать в трее.</summary>
        private readonly bool startMinimized;

        /// <summary>
        /// Создаёт главную форму, инициализирует обработчики, трей, тему из конфигурации
        /// и таймер синхронизации состояния.
        /// </summary>
        /// <param name="startMinimized">Запускать ли форму сразу свёрнутой в трей.</param>
        public MainForm(bool startMinimized = false)
        {
            InitializeComponent();
            this.startMinimized = startMinimized;

            if (chkSlot1 != null) chkSlot1.CheckedChanged += (s, e) => OnSlotChanged(1);
            if (chkSlot2 != null) chkSlot2.CheckedChanged += (s, e) => OnSlotChanged(2);
            if (chkWarp != null) chkWarp.CheckedChanged += (s, e) => OnCloudflareChanged();
            if (chkAmnezia != null) chkAmnezia.CheckedChanged += (s, e) => OnAmneziaChanged();

            if (btnTurnOffAll != null) btnTurnOffAll.Click += (s, e) => TurnOffAll();
            if (btnRestart != null) btnRestart.Click += (s, e) => RestartServices();
            if (btnThemeToggle != null) btnThemeToggle.Click += (s, e) => ToggleTheme();

            InitTrayIcon();

            // Тема восстанавливается из конфигурации (см. ключ DarkTheme в App.config)
            isDarkTheme = ConfigHelper.GetBool("DarkTheme");
            ApplyTheme(isDarkTheme);
            if (btnThemeToggle != null) btnThemeToggle.Text = isDarkTheme ? "☀️" : "🌙";

            syncTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            syncTimer.Tick += async (s, e) => await SyncStateQuietlyAsync();

            this.Load += async (s, e) =>
            {
                if (this.startMinimized) HideToTray();
                UpdateSlotLabels();

                await RestoreLastStateAsync();

                await CheckCurrentStateAsync();
                syncTimer.Start();
            };
        }

        #region Трей

        /// <summary>
        /// Создаёт значок в трее с контекстным меню (переключатели, «Отключить всё»,
        /// «Открыть», «Выход») и привязывает его к двойному клику.
        /// </summary>
        private void InitTrayIcon()
        {
            RemoveTrayIcon();

            trayMenu = new ContextMenuStrip();

            itemSlot1 = new ToolStripMenuItem(ConfigHelper.Get("Slot1Name", "Стратегия 1"), null, (s, e) => { if (chkSlot1 != null) chkSlot1.Checked = itemSlot1.Checked; }) { CheckOnClick = true };
            itemSlot2 = new ToolStripMenuItem(ConfigHelper.Get("Slot2Name", "Стратегия 2"), null, (s, e) => { if (chkSlot2 != null) chkSlot2.Checked = itemSlot2.Checked; }) { CheckOnClick = true };
            itemWarp = new ToolStripMenuItem("Cloudflare WARP", null, (s, e) => { if (chkWarp != null) chkWarp.Checked = itemWarp.Checked; }) { CheckOnClick = true };
            itemAmnezia = new ToolStripMenuItem("AmneziaWG", null, (s, e) => { if (chkAmnezia != null) chkAmnezia.Checked = itemAmnezia.Checked; }) { CheckOnClick = true };

            trayMenu.Items.Add(itemSlot1);
            trayMenu.Items.Add(itemSlot2);
            trayMenu.Items.Add(itemWarp);
            trayMenu.Items.Add(itemAmnezia);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("Отключить всё", null, (s, e) => TurnOffAll()));
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem("Открыть", null, (s, e) => RestoreFromTray()));
            trayMenu.Items.Add(new ToolStripMenuItem("Выход", null, OnExitClick));

            trayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                ContextMenuStrip = trayMenu,
                Visible = true,
                Text = "Network Switcher"
            };

            trayIcon.DoubleClick += (s, e) => RestoreFromTray();

            this.FormClosed += (s, e) => RemoveTrayIcon();
        }

        /// <summary>Скрывает и уничтожает значок в трее (при повторной инициализации или выходе).</summary>
        private void RemoveTrayIcon()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }

        /// <summary>Обработчик пункта «Выход»: полностью завершает приложение.</summary>
        private void OnExitClick(object sender, EventArgs e)
        {
            RemoveTrayIcon();
            Application.ExitThread();
            Environment.Exit(0);
        }

        /// <summary>Обновляет подписи пунктов трея и тумблеров из актуальной конфигурации.</summary>
        public void UpdateTrayMenuNames() => UpdateSlotLabels();

        /// <summary>
        /// Читает пользовательские названия слотов из конфигурации и применяет их
        /// к пунктам меню трея и текстам тумблеров.
        /// </summary>
        private void UpdateSlotLabels()
        {
            string slot1Name = ConfigHelper.Get("Slot1Name", "Стратегия 1");
            string slot2Name = ConfigHelper.Get("Slot2Name", "Стратегия 2");

            if (itemSlot1 != null) itemSlot1.Text = slot1Name;
            if (itemSlot2 != null) itemSlot2.Text = slot2Name;

            if (chkSlot1 != null) chkSlot1.Text = slot1Name;
            if (chkSlot2 != null) chkSlot2.Text = slot2Name;
        }

        /// <summary>Прячет окно, оставляя приложение работать в трее.</summary>
        private void HideToTray()
        {
            this.Hide();
            this.ShowInTaskbar = false;
        }

        /// <summary>Разворачивает окно из трея и выводит его на передний план.</summary>
        public void RestoreFromTray()
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        /// <summary>
        /// Перехватывает закрытие окна пользователем: вместо выхода сворачивает приложение в трей.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideToTray();
            }
            else
            {
                RemoveTrayIcon();
                base.OnFormClosing(e);
            }
        }

        #endregion

        #region Логирование

        /// <summary>
        /// Добавляет строку с меткой времени в журнал событий.
        /// Безопасно вызывается из любого потока (при необходимости делает маршаллинг в UI-поток).
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="isError"><c>true</c>, если это сообщение об ошибке.</param>
        public void Log(string message, bool isError = false)
        {
            if (txtLogs == null) return;

            if (txtLogs.InvokeRequired)
            {
                txtLogs.Invoke(new Action(() => Log(message, isError)));
                return;
            }

            txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLogs.SelectionStart = txtLogs.TextLength;
            txtLogs.ScrollToCaret();
        }

        #endregion

        #region Управление Zapret

        /// <summary>
        /// Останавливает текущий процесс zapret и запускает указанный .bat-файл стратегии
        /// в скрытом окне консоли.
        /// </summary>
        /// <param name="batFileName">Имя (или полный путь) .bat файла стратегии.</param>
        /// <returns><c>true</c>, если после запуска процесс zapret действительно работает.</returns>
        private bool StartZapretProcess(string batFileName)
        {
            StopZapretProcess();

            if (string.IsNullOrWhiteSpace(batFileName))
            {
                Log("❌ Ошибка: В настройках не указан файл батника!", true);
                return false;
            }

            string zapretDir = ConfigHelper.Get("ZapretFolderPath");
            string batPath = Path.IsPathRooted(batFileName) ? batFileName : Path.Combine(zapretDir, batFileName);

            if (!File.Exists(batPath))
            {
                Log($"❌ Ошибка: Не найден батник {batFileName}", true);
                return false;
            }

            Log($"⚙️ Запуск {Path.GetFileName(batPath)} в скрытом режиме...");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"\"{batPath}\"\"",
                    WorkingDirectory = zapretDir,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(psi);
                Thread.Sleep(2000);

                if (IsZapretRunning())
                {
                    Log($"✅ Zapret успешно запущен!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log($"❌ Ошибка запуска: {ex.Message}", true);
            }

            return false;
        }

        /// <summary>
        /// Принудительно завершает процессы zapret (winws.exe / zapret.exe)
        /// и останавливает/удаляет драйвер WinDivert.
        /// </summary>
        private void StopZapretProcess()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c taskkill /f /im winws.exe /im zapret.exe >nul 2>&1 & net stop WinDivert >nul 2>&1 & sc delete WinDivert >nul 2>&1",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(psi)) { p?.WaitForExit(2000); }
            }
            catch { }
        }

        /// <summary>
        /// Определяет, запущен ли сейчас процесс zapret (winws.exe или zapret.exe).
        /// </summary>
        /// <returns><c>true</c>, если процесс найден.</returns>
        private bool IsZapretRunning()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessId FROM Win32_Process WHERE Name = 'winws.exe' OR Name = 'zapret.exe'"))
                {
                    return searcher.Get().Count > 0;
                }
            }
            catch { return false; }
        }

        /// <summary>WinAPI: получает заголовок окна по дескриптору.</summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        /// <summary>WinAPI: перебирает все окна верхнего уровня.</summary>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        /// <summary>Callback-прототип для <see cref="EnumWindows"/>.</summary>
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// Определяет, какой слот Zapret сейчас активен (1 или 2).
        /// Сначала сверяется с состоянием тумблеров, затем ищет имя .bat файла в заголовках
        /// окон и в командной строке процесса.
        /// </summary>
        /// <returns>Номер активного слота (1 или 2); 0 — zapret не запущен.</returns>
        private int GetActiveZapretSlot()
        {
            if (!IsZapretRunning()) return 0;

            if (chkSlot1 != null && chkSlot1.Checked) return 1;
            if (chkSlot2 != null && chkSlot2.Checked) return 2;

            string slot1Bat = ConfigHelper.Get("Slot1Bat", "");
            string slot2Bat = ConfigHelper.Get("Slot2Bat", "");

            string slot1Name = Path.GetFileNameWithoutExtension(slot1Bat);
            string slot2Name = Path.GetFileNameWithoutExtension(slot2Bat);

            int detectedSlot = 0;

            EnumWindows((hWnd, lParam) =>
            {
                var sb = new System.Text.StringBuilder(256);
                GetWindowText(hWnd, sb, 256);
                string title = sb.ToString();

                if (!string.IsNullOrEmpty(title))
                {
                    if (!string.IsNullOrEmpty(slot1Name) && title.IndexOf(slot1Name, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        detectedSlot = 1;
                        return false;
                    }
                    if (!string.IsNullOrEmpty(slot2Name) && title.IndexOf(slot2Name, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        detectedSlot = 2;
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);

            if (detectedSlot != 0) return detectedSlot;

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE Name = 'winws.exe' OR Name = 'zapret.exe'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string cmd = obj["CommandLine"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(slot2Name) && cmd.IndexOf(slot2Name, StringComparison.OrdinalIgnoreCase) >= 0) return 2;
                        if (!string.IsNullOrEmpty(slot1Name) && cmd.IndexOf(slot1Name, StringComparison.OrdinalIgnoreCase) >= 0) return 1;
                    }
                }
            }
            catch { }

            return 1;
        }

        #endregion

        #region Восстановление состояния при запуске

        /// <summary>
        /// Поднимает те службы/стратегии, которые были активны в прошлый раз (хранится в
        /// Properties.Settings.Default: Slot1Active/Slot2Active/WarpActive/AmneziaActive).
        /// Нужно, потому что сами по себе Zapret и Cloudflare WARP не переживают перезагрузку ПК
        /// (в отличие от службы AmneziaWG, которая, будучи один раз установлена через
        /// /installtunnelservice, поднимается Windows самостоятельно). Без этого метода после
        /// перезагрузки нужно было вручную включать стратегию.
        /// </summary>
        private async Task RestoreLastStateAsync()
        {
            try
            {
                if (chkSlot1 != null && Properties.Settings.Default.Slot1Active)
                {
                    chkSlot1.Checked = true; // OnSlotChanged синхронный — выполнится полностью до возврата отсюда
                }
                else if (chkSlot2 != null && Properties.Settings.Default.Slot2Active)
                {
                    chkSlot2.Checked = true;
                }

                if (chkWarp != null && Properties.Settings.Default.WarpActive)
                {
                    chkWarp.Checked = true;
                    // OnCloudflareChanged — async void, даём ему время реально отработать
                    // (внутри у него уже есть Task.Delay(1500) + сами команды warp-cli)
                    await Task.Delay(2500);
                }

                if (chkAmnezia != null && Properties.Settings.Default.AmneziaActive)
                {
                    chkAmnezia.Checked = true;
                    await Task.Delay(2500);
                }
            }
            catch (Exception ex)
            {
                Log($"⚠️ Не удалось восстановить прошлое состояние: {ex.Message}", true);
            }
        }

        #endregion

        #region Синхронизация состояния

        /// <summary>
        /// Начальная проверка состояния при загрузке формы: сверяет тумблеры
        /// с реальным состоянием процессов и служб.
        /// </summary>
        private async Task CheckCurrentStateAsync()
        {
            Log("🔍 Проверка состояния...");
            await SyncStateQuietlyAsync();
            Log("✔️ Готово к работе.");
        }

        /// <summary>
        /// Тихая сверка состояния тумблеров с реальностью (без лишних сообщений в журнал).
        /// При расхождении обновляет тумблеры, пункты трея и пользовательские настройки.
        /// </summary>
        private async Task SyncStateQuietlyAsync()
        {
            if (isUpdatingStrategy) return;

            int activeSlot = GetActiveZapretSlot();

            string warpCli = ConfigHelper.Get("WarpCliPath");
            bool cfActive = false;
            if (File.Exists(warpCli))
            {
                var cfStatus = await ExecuteCommandWithOutputAsync(warpCli, "status");
                cfActive = cfStatus.IndexOf("Status update: Connected", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           cfStatus.IndexOf("Подключено", StringComparison.OrdinalIgnoreCase) >= 0;
            }

            string awgExe = ConfigHelper.Get("AmneziaExePath");
            string tunnelName = ConfigHelper.Get("AmneziaTunnelName");
            bool awgActive = false;
            if (File.Exists(awgExe))
            {
                awgActive = await IsAmneziaServiceRunningAsync(tunnelName);
            }

            bool s1Checked = chkSlot1 != null && chkSlot1.Checked;
            bool s2Checked = chkSlot2 != null && chkSlot2.Checked;
            bool cfChecked = chkWarp != null && chkWarp.Checked;
            bool awgChecked = chkAmnezia != null && chkAmnezia.Checked;

            // Пока действует "льготный период" после ручного включения WARP, не сверяем его статус:
            // warp-cli может устанавливать соединение не мгновенно, а таймер синхронизации тикает
            // каждые 3 секунды — без этой паузы галочка гаснет раньше, чем WARP успевает подключиться.
            bool skipWarpSync = DateTime.UtcNow < warpToggleGraceUntilUtc;
            bool cfMismatch = !skipWarpSync && cfChecked != cfActive;

            if (s1Checked != (activeSlot == 1) || s2Checked != (activeSlot == 2) || cfMismatch || awgChecked != awgActive)
            {
                isUpdatingStrategy = true;

                if (chkSlot1 != null) chkSlot1.Checked = (activeSlot == 1);
                if (chkSlot2 != null) chkSlot2.Checked = (activeSlot == 2);
                if (chkWarp != null && !skipWarpSync) chkWarp.Checked = cfActive;
                if (chkAmnezia != null) chkAmnezia.Checked = awgActive;

                Properties.Settings.Default.Slot1Active = (activeSlot == 1);
                Properties.Settings.Default.Slot2Active = (activeSlot == 2);
                if (!skipWarpSync) Properties.Settings.Default.WarpActive = cfActive;
                Properties.Settings.Default.AmneziaActive = awgActive;
                Properties.Settings.Default.Save();

                SyncTrayState();
                isUpdatingStrategy = false;
                UpdateStatus();
            }
        }

        /// <summary>Копирует текущее состояние тумблеров в пункты меню трея.</summary>
        private void SyncTrayState()
        {
            if (itemSlot1 != null && chkSlot1 != null) itemSlot1.Checked = chkSlot1.Checked;
            if (itemSlot2 != null && chkSlot2 != null) itemSlot2.Checked = chkSlot2.Checked;
            if (itemWarp != null && chkWarp != null) itemWarp.Checked = chkWarp.Checked;
            if (itemAmnezia != null && chkAmnezia != null) itemAmnezia.Checked = chkAmnezia.Checked;
        }

        #endregion

        #region Обработка переключений

        /// <summary>
        /// Выполняет консольную команду и возвращает объединённый stdout+stderr.
        /// </summary>
        /// <param name="fileName">Исполняемый файл.</param>
        /// <param name="arguments">Аргументы командной строки.</param>
        /// <returns>Текст вывода команды; пустая строка при ошибке запуска.</returns>
        private async Task<string> ExecuteCommandWithOutputAsync(string fileName, string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var process = Process.Start(psi))
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();
                    process.WaitForExit(2000);
                    return (output + error).Trim();
                }
            }
            catch { return ""; }
        }

        /// <summary>
        /// Выполняет консольную команду без чтения вывода, ожидая завершения до 3 секунд.
        /// </summary>
        /// <param name="fileName">Исполняемый файл.</param>
        /// <param name="arguments">Аргументы командной строки.</param>
        private void ExecuteCommand(string fileName, string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = ConfigHelper.Get("ZapretFolderPath"),
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(psi)) { p?.WaitForExit(3000); }
            }
            catch (Exception ex)
            {
                Log($"Ошибка выполнения {fileName}: {ex.Message}", true);
            }
        }

        /// <summary>
        /// Обработчик переключения слотов Zapret. Слоты взаимоисключающие: при включении одного
        /// второй выключается; при выключении обоих zapret останавливается, а WARP (если был
        /// включён поверх слота) тоже отключается.
        /// </summary>
        /// <param name="slot">Номер слота (1 или 2), состояние которого изменилось.</param>
        private void OnSlotChanged(int slot)
        {
            if (isUpdatingStrategy) return;

            isUpdatingStrategy = true;
            try
            {
                string slot1Bat = ConfigHelper.Get("Slot1Bat");
                string slot2Bat = ConfigHelper.Get("Slot2Bat");

                if (slot == 1 && chkSlot1 != null && chkSlot1.Checked)
                {
                    if (chkSlot2 != null) chkSlot2.Checked = false;
                    bool success = StartZapretProcess(slot1Bat);
                    if (!success && chkSlot1 != null) chkSlot1.Checked = false;
                }
                else if (slot == 2 && chkSlot2 != null && chkSlot2.Checked)
                {
                    if (chkSlot1 != null) chkSlot1.Checked = false;
                    bool success = StartZapretProcess(slot2Bat);
                    if (!success && chkSlot2 != null) chkSlot2.Checked = false;
                }
                else if ((chkSlot1 == null || !chkSlot1.Checked) && (chkSlot2 == null || !chkSlot2.Checked))
                {
                    StopZapretProcess();
                    Log("Все службы выключены.");

                    if (chkWarp != null && chkWarp.Checked)
                    {
                        chkWarp.Checked = false;
                        ExecuteCommand(ConfigHelper.Get("WarpCliPath"), "disconnect");
                        Properties.Settings.Default.WarpActive = false;
                        Properties.Settings.Default.Save();
                    }
                }
            }
            finally
            {
                SyncTrayState();
                isUpdatingStrategy = false;
            }

            Properties.Settings.Default.Slot1Active = chkSlot1 != null && chkSlot1.Checked;
            Properties.Settings.Default.Slot2Active = chkSlot2 != null && chkSlot2.Checked;
            Properties.Settings.Default.Save();

            UpdateStatus();
        }

        /// <summary>
        /// Обработчик переключения Cloudflare WARP. WARP не включается без активной стратегии
        /// Zapret и взаимоисключается с AmneziaWG.
        /// </summary>
        private async void OnCloudflareChanged()
        {
            if (isUpdatingStrategy || chkWarp == null) return;

            isUpdatingStrategy = true;
            try
            {
                string warpCli = ConfigHelper.Get("WarpCliPath");

                if (chkWarp.Checked)
                {
                    bool anyStrategyActive = (chkSlot1 != null && chkSlot1.Checked) || (chkSlot2 != null && chkSlot2.Checked);

                    if (!anyStrategyActive)
                    {
                        // Без активной стратегии Zapret WARP в этой сети не сможет установить соединение —
                        // явно сообщаем причину вместо того, чтобы пытаться подключиться и через пару секунд
                        // молча откатить галочку обратно (это и выглядело как "включился и сразу выключился").
                        Log("⚠️ Cloudflare WARP не может подключиться без активной стратегии Zapret. Сначала включите Слот 1 или Слот 2.", true);
                        chkWarp.Checked = false;
                    }
                    else
                    {
                        if (chkAmnezia != null && chkAmnezia.Checked)
                        {
                            chkAmnezia.Checked = false;
                            Properties.Settings.Default.AmneziaActive = false;
                            Properties.Settings.Default.Save();
                            ExecuteCommand(ConfigHelper.Get("AmneziaExePath"), $"/uninstalltunnelservice \"{ConfigHelper.Get("AmneziaTunnelName")}\"");
                            Log("ℹ️ AmneziaWG автоматически отключена.");
                        }

                        Log("Включение Cloudflare WARP...");
                        // Даём таймеру синхронизации 10 секунд не трогать эту галочку —
                        // реальное подключение WARP может занимать больше 1.5 секунд.
                        warpToggleGraceUntilUtc = DateTime.UtcNow.AddSeconds(10);

                        await Task.Run(() => ExecuteCommand(warpCli, "connect"));

                        await Task.Delay(1500);
                        var status = await ExecuteCommandWithOutputAsync(warpCli, "status");
                        if (status.IndexOf("Connected", StringComparison.OrdinalIgnoreCase) >= 0 || status.IndexOf("Подключено", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Log("✅ Cloudflare WARP успешно запущен!");
                        }
                        else
                        {
                            Log("ℹ️ Cloudflare WARP отправлен запрос на запуск.");
                        }
                    }
                }
                else
                {
                    Log("Остановка Cloudflare WARP...");
                    await Task.Run(() => ExecuteCommand(warpCli, "disconnect"));
                    Log("🛑 Cloudflare WARP отключен.");
                }
            }
            finally
            {
                SyncTrayState();
                isUpdatingStrategy = false;
            }

            Properties.Settings.Default.WarpActive = chkWarp != null && chkWarp.Checked;
            Properties.Settings.Default.Save();

            UpdateStatus();
        }

        /// <summary>
        /// Определяет, работает ли служба туннеля AmneziaWG
        /// (<c>AmneziaWGTunnel$&lt;имя&gt;</c>).
        /// </summary>
        /// <param name="tunnelName">Имя туннеля (без расширения).</param>
        /// <returns><c>true</c>, если служба туннеля установлена и находится в состоянии Running.</returns>
        private async Task<bool> IsAmneziaServiceRunningAsync(string tunnelName)
        {
            string serviceName = $"AmneziaWGTunnel${tunnelName}";

            return await Task.Run(() =>
            {
                try
                {
                    using (var sc = new ServiceController(serviceName))
                    {
                        // Обращение к API SCM напрямую, а не парсинг текста "sc query" —
                        // текстовый вывод sc.exe оказался ненадёжным для поиска "RUNNING"
                        // (зависит от локали/кодовой страницы консоли на конкретной системе),
                        // из-за чего реально запущенный туннель определялся как выключенный.
                        return sc.Status == ServiceControllerStatus.Running;
                    }
                }
                catch (InvalidOperationException)
                {
                    // Такой службы не существует — туннель не установлен.
                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Обработчик переключения AmneziaWG. Взаимоисключается с WARP;
        /// повторно не устанавливает службу, если она уже работает.
        /// </summary>
        private async void OnAmneziaChanged()
        {
            if (isUpdatingStrategy || chkAmnezia == null) return;

            isUpdatingStrategy = true;
            try
            {
                string awgExe = ConfigHelper.Get("AmneziaExePath");
                string confPath = ConfigHelper.Get("AmneziaConfPath");
                string tunnelName = ConfigHelper.Get("AmneziaTunnelName");

                if (chkAmnezia.Checked)
                {
                    if (chkWarp != null && chkWarp.Checked)
                    {
                        chkWarp.Checked = false;
                        Properties.Settings.Default.WarpActive = false;
                        Properties.Settings.Default.Save();
                        ExecuteCommand(ConfigHelper.Get("WarpCliPath"), "disconnect");
                        Log("ℹ️ Cloudflare автоматически отключен.");
                    }

                    bool alreadyRunning = await IsAmneziaServiceRunningAsync(tunnelName);
                    if (alreadyRunning)
                    {
                        // Служба AmneziaWG уже установлена и работает (например, поднялась сама
                        // с прошлого сеанса как служба Windows). Повторный /installtunnelservice
                        // не нужен — именно из-за него вылезало нативное уведомление о том, что
                        // туннель "уже установлен и запущен".
                        Log($"ℹ️ AmneziaWG ({tunnelName}) уже установлен и запущен.");
                    }
                    else
                    {
                        Log($"Включение AmneziaWG ({tunnelName})...");
                        await Task.Run(() => ExecuteCommand(awgExe, $"/installtunnelservice \"{confPath}\""));

                        await Task.Delay(1500);
                        Log($"✅ AmneziaWG ({tunnelName}) успешно запущен!");
                    }
                }
                else
                {
                    bool alreadyRunning = await IsAmneziaServiceRunningAsync(tunnelName);
                    if (!alreadyRunning)
                    {
                        Log("ℹ️ AmneziaWG уже остановлена.");
                    }
                    else
                    {
                        Log("Остановка AmneziaWG...");
                        await Task.Run(() => ExecuteCommand("cmd.exe", $"/c \"\"{awgExe}\" /uninstalltunnelservice \"{tunnelName}\" >nul 2>&1\""));
                        Log("🛑 AmneziaWG отключена.");
                    }
                }
            }
            finally
            {
                SyncTrayState();
                isUpdatingStrategy = false;
            }

            Properties.Settings.Default.AmneziaActive = chkAmnezia != null && chkAmnezia.Checked;
            Properties.Settings.Default.Save();

            UpdateStatus();
        }

        /// <summary>Отключает все инструменты: zapret, WARP и туннель AmneziaWG.</summary>
        private void TurnOffAll()
        {
            Log("Выключение всех сервисов...");

            isUpdatingStrategy = true;
            if (chkSlot1 != null) chkSlot1.Checked = false;
            if (chkSlot2 != null) chkSlot2.Checked = false;
            if (chkWarp != null) chkWarp.Checked = false;
            if (chkAmnezia != null) chkAmnezia.Checked = false;
            SyncTrayState();
            isUpdatingStrategy = false;

            StopZapretProcess();
            ExecuteCommand(ConfigHelper.Get("WarpCliPath"), "disconnect");

            string awgExe = ConfigHelper.Get("AmneziaExePath");
            string tunnelName = ConfigHelper.Get("AmneziaTunnelName");
            ExecuteCommand("cmd.exe", $"/c \"\"{awgExe}\" /uninstalltunnelservice \"{tunnelName}\" >nul 2>&1\"");

            Properties.Settings.Default.Slot1Active = false;
            Properties.Settings.Default.Slot2Active = false;
            Properties.Settings.Default.WarpActive = false;
            Properties.Settings.Default.AmneziaActive = false;
            Properties.Settings.Default.Save();

            UpdateStatus();
            Log("✔️ Все службы выключены.");
        }

        /// <summary>
        /// Перезапускает текущий набор активных инструментов: сначала глушит всё,
        /// затем по памяти включает то, что было активно.
        /// </summary>
        private void RestartServices()
        {
            bool s1 = chkSlot1 != null && chkSlot1.Checked;
            bool s2 = chkSlot2 != null && chkSlot2.Checked;
            bool cf = chkWarp != null && chkWarp.Checked;
            bool awg = chkAmnezia != null && chkAmnezia.Checked;

            Log("🔄 Перезапуск...");
            TurnOffAll();
            Thread.Sleep(1500);

            if (s1 && chkSlot1 != null) chkSlot1.Checked = true;
            else if (s2 && chkSlot2 != null) chkSlot2.Checked = true;

            if (cf && chkWarp != null) chkWarp.Checked = true;
            if (awg && chkAmnezia != null) chkAmnezia.Checked = true;
        }

        /// <summary>
        /// Обновляет строку статуса внизу окна в соответствии с активными тумблерами
        /// и применённой темой.
        /// </summary>
        private void UpdateStatus()
        {
            if (lblStatus == null) return;

            string slot1Name = ConfigHelper.Get("Slot1Name", "Стратегия 1");
            string slot2Name = ConfigHelper.Get("Slot2Name", "Стратегия 2");

            string activeName = "";

            if (chkSlot1 != null && chkSlot1.Checked) activeName = slot1Name;
            else if (chkSlot2 != null && chkSlot2.Checked) activeName = slot2Name;

            if (chkWarp != null && chkWarp.Checked)
            {
                activeName = string.IsNullOrEmpty(activeName) ? "Cloudflare" : $"{activeName} + Cloudflare";
            }

            if (chkAmnezia != null && chkAmnezia.Checked)
            {
                activeName = string.IsNullOrEmpty(activeName) ? "AmneziaWG" : $"{activeName} + AmneziaWG";
            }

            if (string.IsNullOrEmpty(activeName))
            {
                lblStatus.Text = "🔴 Все службы отключены";
                lblStatus.ForeColor = isDarkTheme ? Color.Coral : Color.DarkRed;
            }
            else
            {
                lblStatus.Text = $"🟢 {activeName}";
                lblStatus.ForeColor = isDarkTheme ? Color.LightGreen : Color.Green;
            }
        }

        /// <summary>
        /// Переключает тёмную/светлую тему, применяет её и сохраняет выбор в конфигурации
        /// (ключ <c>DarkTheme</c>).
        /// </summary>
        private void ToggleTheme()
        {
            isDarkTheme = !isDarkTheme;
            ApplyTheme(isDarkTheme);

            ConfigHelper.Set("DarkTheme", isDarkTheme.ToString());

            if (btnThemeToggle != null) btnThemeToggle.Text = isDarkTheme ? "☀️" : "🌙";
        }

        /// <summary>Применяет цветовую схему окна, журнала, тумблеров и переключает тему кастомных контролов.</summary>
        /// <param name="dark"><c>true</c> — тёмная тема, <c>false</c> — светлая.</param>
        private void ApplyTheme(bool dark)
        {
            Color bg = dark ? Color.FromArgb(30, 30, 32) : Color.FromArgb(250, 250, 252);
            Color fg = dark ? Color.FromArgb(240, 240, 240) : Color.FromArgb(20, 20, 20);

            this.BackColor = bg;
            this.ForeColor = fg;

            if (lblTitle != null) lblTitle.ForeColor = fg;

            if (txtLogs != null)
            {
                txtLogs.BackColor = dark ? Color.FromArgb(18, 18, 20) : Color.FromArgb(242, 243, 245);
                txtLogs.ForeColor = dark ? Color.FromArgb(200, 200, 200) : Color.FromArgb(50, 50, 50);
            }

            if (chkSlot1 != null) chkSlot1.ForeColor = fg;
            if (chkSlot2 != null) chkSlot2.ForeColor = fg;
            if (chkWarp != null) chkWarp.ForeColor = fg;
            if (chkAmnezia != null) chkAmnezia.ForeColor = fg;

            // Кастомные тумблеры имеют собственные цвета дорожки/бегунка — переключаем и их
            chkSlot1?.ApplyTheme(dark);
            chkSlot2?.ApplyTheme(dark);
            chkWarp?.ApplyTheme(dark);
            chkAmnezia?.ApplyTheme(dark);

            UpdateStatus();
        }

        /// <summary>Открывает модальное окно настроек.</summary>
        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(this);
            settingsForm.ShowDialog();
        }

        #endregion
    }
}
