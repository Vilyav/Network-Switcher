using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkSwitcher
{
    /// <summary>
    /// Точка входа приложения: обеспечивает запуск единственного экземпляра,
    /// повышение прав до администратора, первичное автоопределение путей
    /// к внешним инструментам и запуск главной формы.
    /// </summary>
    internal static class Program
    {
        /// <summary>Имя глобального мьютекса, гарантирующего единственный запущенный экземпляр.</summary>
        private const string MutexName = "Global\\NetworkSwitcher_UniqueMutex_App";

        /// <summary>Имя именованного канала для сигнала «развернуть окно» от повторного запуска.</summary>
        private const string PipeName = "NetworkSwitcher_RestorePipe";

        /// <summary>Стандартные расположения папки zapret, проверяемые при автоопределении.</summary>
        private static readonly string[] ZapretFolderCandidates =
        {
            @"C:\zapret",
            @"C:\Program Files\zapret",
            @"C:\Program Files (x86)\zapret"
        };

        /// <summary>Стандартные расположения warp-cli.exe, проверяемые при автоопределении.</summary>
        private static readonly string[] WarpCliCandidates =
        {
            @"C:\Program Files\Cloudflare\Cloudflare WARP\warp-cli.exe",
            @"C:\Program Files (x86)\Cloudflare\Cloudflare WARP\warp-cli.exe"
        };

        /// <summary>Стандартные расположения amneziawg.exe, проверяемые при автоопределении.</summary>
        private static readonly string[] AmneziaCandidates =
        {
            @"C:\Program Files\AmneziaWG\amneziawg.exe",
            @"C:\Program Files (x86)\AmneziaWG\amneziawg.exe"
        };

        /// <summary>
        /// Главная точка входа приложения.
        /// </summary>
        /// <param name="args">Аргументы командной строки; <c>--minimized</c> — запуск свёрнутым в трей.</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Проверяем, запущен ли уже один экземпляр
            using (Mutex mutex = new Mutex(true, MutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    // Программа уже работает! Отправляем сигнал развернуться и выходим
                    SendRestoreSignal();
                    return;
                }

                // Проверка на права администратора
                if (!IsAdministrator())
                {
                    RestartAsAdministrator();
                    return;
                }

                // Перед показом UI подставляем реальные пути к zapret / WARP / AmneziaWG,
                // если в конфиге остались несуществующие значения по умолчанию.
                EnsureInitialConfiguration();

                bool startMinimized = args.Length > 0 && args[0] == "--minimized";
                MainForm mainForm = new MainForm(startMinimized);

                StartPipeServer(mainForm);

                Application.Run(mainForm);
            }
        }

        /// <summary>
        /// Перезапускает текущий процесс с правами администратора (через UAC-запрос).
        /// Если пользователь отклонил запрос — показывает сообщение об ошибке.
        /// </summary>
        private static void RestartAsAdministrator()
        {
            string exeName = Process.GetCurrentProcess().MainModule.FileName;

            var startInfo = new ProcessStartInfo(exeName)
            {
                Verb = "runas",
                UseShellExecute = true
            };

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                MessageBox.Show("Для работы программы требуются права администратора!",
                                "Ошибка доступа",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Проверяет ключевые пути конфигурации и при необходимости автоматически определяет
        /// места установки Zapret, Cloudflare WARP и AmneziaWG, записывая найденные пути в конфиг.
        /// </summary>
        /// <remarks>
        /// Заменяет аналогичную логику, которая ранее была заложена в установщик:
        /// определение на первом запуске надёжнее, т.к. не зависит от того, в какой момент
        /// пользователь установил внешние инструменты. Перезаписываются только пути,
        /// указывающие на несуществующие файлы/папки.
        /// </remarks>
        private static void EnsureInitialConfiguration()
        {
            DetectZapretFolder();
            DetectWarpCli();
            DetectAmnezia();
        }

        /// <summary>
        /// Если <c>ZapretFolderPath</c> указывает на несуществующую папку — ищет zapret
        /// в стандартных расположениях и сохраняет первое найденное.
        /// </summary>
        private static void DetectZapretFolder()
        {
            string current = ConfigHelper.Get("ZapretFolderPath");
            if (Directory.Exists(current)) return;

            string detected = FindFirstExisting(ZapretFolderCandidates, Directory.Exists);
            if (detected != null)
                ConfigHelper.Set("ZapretFolderPath", detected);
        }

        /// <summary>
        /// Если <c>WarpCliPath</c> указывает на несуществующий файл — ищет warp-cli.exe
        /// в стандартных расположениях и сохраняет первый найденный.
        /// </summary>
        private static void DetectWarpCli()
        {
            string current = ConfigHelper.Get("WarpCliPath");
            if (File.Exists(current)) return;

            string detected = FindFirstExisting(WarpCliCandidates, File.Exists);
            if (detected != null)
                ConfigHelper.Set("WarpCliPath", detected);
        }

        /// <summary>
        /// Если путь к amneziawg.exe указывает на несуществующий файл — ищет его в стандартных
        /// расположениях. Дополнительно, если конфиг туннеля не найден, пытается подобрать
        /// первый <c>*.conf</c> рядом с исполняемым файлом и вывести имя туннеля из его имени.
        /// </summary>
        private static void DetectAmnezia()
        {
            string exeConfigKey = "AmneziaExePath";
            string confConfigKey = "AmneziaConfPath";
            string tunnelConfigKey = "AmneziaTunnelName";

            string currentExe = ConfigHelper.Get(exeConfigKey);
            if (File.Exists(currentExe) && File.Exists(ConfigHelper.Get(confConfigKey))) return;

            string detectedExe = FindFirstExisting(AmneziaCandidates, File.Exists);
            if (detectedExe == null) return;

            ConfigHelper.Set(exeConfigKey, detectedExe);

            // Подбираем .conf рядом с amneziawg.exe, если текущий путь конфига недействителен
            if (!File.Exists(ConfigHelper.Get(confConfigKey)))
            {
                try
                {
                    string exeDir = Path.GetDirectoryName(detectedExe);
                    string[] confFiles = Directory.GetFiles(exeDir, "*.conf");
                    if (confFiles.Length > 0)
                    {
                        string confPath = confFiles[0];
                        ConfigHelper.Set(confConfigKey, confPath);

                        // Имя туннеля — имя файла без расширения
                        string tunnelName = Path.GetFileNameWithoutExtension(confPath);
                        ConfigHelper.Set(tunnelConfigKey, tunnelName);
                    }
                }
                catch
                {
                    // Не удалось перечислить файлы в папке (нет прав, папка недоступна и т.п.) —
                    // не критично, просто не смогли автоматически подобрать .conf. Само приложение
                    // при этом не должно падать ещё до показа главного окна.
                }
            }
        }

        /// <summary>
        /// Возвращает первый элемент массива, удовлетворяющий предикату, либо <c>null</c>.
        /// </summary>
        /// <param name="candidates">Список путей-кандидатов.</param>
        /// <param name="exists">Предикат проверки существования (файл/папка).</param>
        private static string FindFirstExisting(string[] candidates, Func<string, bool> exists)
        {
            foreach (string candidate in candidates)
            {
                if (exists(candidate)) return candidate;
            }
            return null;
        }

        /// <summary>
        /// Отправляет работающему экземпляру приложения сигнал «RESTORE» через именованный канал,
        /// чтобы тот развернул своё окно из трея.
        /// </summary>
        private static void SendRestoreSignal()
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                {
                    client.Connect(1000); // Ждем подключение максимум 1 сек
                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine("RESTORE");
                        writer.Flush();
                    }
                }
            }
            catch
            {
                // Если не удалось связаться, просто выходим
            }
        }

        /// <summary>
        /// Запускает фоновый сервер именованных каналов: принимает сигналы «RESTORE»
        /// от повторных запусков и разворачивает главное окно в UI-потоке.
        /// </summary>
        /// <param name="form">Экземпляр главной формы.</param>
        private static void StartPipeServer(MainForm form)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        using (var server = new NamedPipeServerStream(PipeName, PipeDirection.In))
                        {
                            server.WaitForConnection();
                            using (var reader = new StreamReader(server))
                            {
                                string message = reader.ReadLine();
                                if (message == "RESTORE")
                                {
                                    // Вызываем RestoreFromTray в основном UI-потоке
                                    form.Invoke(new Action(() => form.RestoreFromTray()));
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Игнорируем ошибки сервера пайпов при закрытии
                    }
                }
            });
        }

        /// <summary>
        /// Определяет, запущен ли текущий процесс с правами администратора.
        /// </summary>
        /// <returns><c>true</c>, если у процесса есть административные привилегии.</returns>
        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}