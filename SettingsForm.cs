using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NetworkSwitcher
{
    /// <summary>
    /// Модальное окно настроек: пути к внешним инструментам, имена слотов,
    /// импорт/экспорт конфигурации, автозапуск Windows и управление туннелем AmneziaWG.
    /// </summary>
    public partial class SettingsForm : Form
    {
        /// <summary>Ссылка на главную форму для обновления подписей трея после сохранения.</summary>
        private readonly MainForm mainForm;

        /// <summary>
        /// Создаёт окно настроек, загружает текущие значения из конфигурации,
        /// привязывает обработчики кнопок и обзорных диалогов.
        /// </summary>
        /// <param name="mainForm">Экземпляр главной формы.</param>
        public SettingsForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;

            btnExport.Click += btnExport_Click;
            btnImport.Click += btnImport_Click;

            btnBrowseZapret.Click += (s, e) => BrowseFolder(txtZapretPath);
            btnBrowseWarp.Click += (s, e) => BrowseFile(txtWarpPath, "Исполняемые файлы (*.exe)|*.exe");
            btnBrowseAmneziaExe.Click += (s, e) => BrowseFile(txtAmneziaExePath, "Исполняемые файлы (*.exe)|*.exe");
            btnBrowseAmneziaConf.Click += (s, e) => BrowseFile(txtAmneziaConfPath, "Конфигурация WireGuard/Amnezia (*.conf)|*.conf");

            chkUseCustomNames.CheckedChanged += (s, e) => UpdateSlotFieldsEnabled();

            LoadConfigToUI();
        }

        /// <summary>
        /// Включает/выключает возможность редактирования названий и батников слотов
        /// в зависимости от состояния <c>chkUseCustomNames</c>.
        /// </summary>
        private void UpdateSlotFieldsEnabled()
        {
            bool enabled = chkUseCustomNames.Checked;

            txtSlot1Name.Enabled = enabled;
            txtSlot1Bat.Enabled = enabled;
            txtSlot2Name.Enabled = enabled;
            txtSlot2Bat.Enabled = enabled;
        }

        /// <summary>Загружает значения из <c>appSettings</c> в элементы управления формы.</summary>
        private void LoadConfigToUI()
        {
            txtZapretPath.Text = ConfigHelper.Get("ZapretFolderPath");
            chkUseCustomNames.Checked = ConfigHelper.GetBool("UseCustomNames");
            txtSlot1Name.Text = ConfigHelper.Get("Slot1Name", "Стратегия 9 (ALT9)");
            txtSlot1Bat.Text = ConfigHelper.Get("Slot1Bat", "general (ALT9).bat");
            txtSlot2Name.Text = ConfigHelper.Get("Slot2Name", "Стратегия 11 (ALT11)");
            txtSlot2Bat.Text = ConfigHelper.Get("Slot2Bat", "general (ALT11).bat");
            txtWarpPath.Text = ConfigHelper.Get("WarpCliPath");
            txtAmneziaExePath.Text = ConfigHelper.Get("AmneziaExePath");
            txtAmneziaConfPath.Text = ConfigHelper.Get("AmneziaConfPath");
            txtAmneziaTunnelName.Text = ConfigHelper.Get("AmneziaTunnelName");
            chkAutoStart.Checked = ConfigHelper.GetBool("AutoStartWithWindows");

            UpdateSlotFieldsEnabled();
        }

        /// <summary>
        /// Сохраняет значения из UI в конфигурацию, обновляет автозапуск Windows
        /// и закрывает окно.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFromUIToConfig();

            bool autoStart = chkAutoStart.Checked;
            SetAutoStartTask(autoStart);
            mainForm.UpdateTrayMenuNames();

            MessageBox.Show("Настройки сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        /// <summary>
        /// Копирует значения полей формы в <c>appSettings</c>.
        /// Имя туннеля AmneziaWG приводится к корректному виду (без расширения .conf и недопустимых символов).
        /// </summary>
        private void SaveFromUIToConfig()
        {
            ConfigHelper.Set("ZapretFolderPath", txtZapretPath.Text);
            ConfigHelper.Set("UseCustomNames", chkUseCustomNames.Checked.ToString());
            ConfigHelper.Set("Slot1Name", txtSlot1Name.Text);
            ConfigHelper.Set("Slot1Bat", txtSlot1Bat.Text);
            ConfigHelper.Set("Slot2Name", txtSlot2Name.Text);
            ConfigHelper.Set("Slot2Bat", txtSlot2Bat.Text);
            ConfigHelper.Set("WarpCliPath", txtWarpPath.Text);
            ConfigHelper.Set("AmneziaExePath", txtAmneziaExePath.Text);
            ConfigHelper.Set("AmneziaConfPath", txtAmneziaConfPath.Text);
            ConfigHelper.Set("AmneziaTunnelName", NormalizeTunnelName(txtAmneziaTunnelName.Text));
            ConfigHelper.Set("AutoStartWithWindows", chkAutoStart.Checked.ToString());
        }

        /// <summary>
        /// Нормализует имя туннеля AmneziaWG: обрезает пробелы, удаляет расширение <c>.conf</c>
        /// и недопустимые символы имени файла.
        /// </summary>
        /// <param name="name">Исходное имя, введённое пользователем.</param>
        /// <returns>Безопасное имя туннеля, пригодное для использования в имени службы Windows.</returns>
        private static string NormalizeTunnelName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            name = name.Trim();

            if (name.EndsWith(".conf", StringComparison.OrdinalIgnoreCase))
                name = Path.GetFileNameWithoutExtension(name);

            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c.ToString(), "");

            return name;
        }

        #region Логика Импорта и Экспорта

        /// <summary>
        /// Экспортирует текущие настройки из UI в JSON-файл (ключ-значение).
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON Конфигурация (*.json)|*.json";
                sfd.FileName = "NetworkSwitcher_Config.json";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var configData = new Dictionary<string, string>
                        {
                            { "ZapretFolderPath", txtZapretPath.Text },
                            { "UseCustomNames", chkUseCustomNames.Checked.ToString() },
                            { "Slot1Name", txtSlot1Name.Text },
                            { "Slot1Bat", txtSlot1Bat.Text },
                            { "Slot2Name", txtSlot2Name.Text },
                            { "Slot2Bat", txtSlot2Bat.Text },
                            { "WarpCliPath", txtWarpPath.Text },
                            { "AmneziaExePath", txtAmneziaExePath.Text },
                            { "AmneziaConfPath", txtAmneziaConfPath.Text },
                            { "AmneziaTunnelName", NormalizeTunnelName(txtAmneziaTunnelName.Text) },
                            { "AutoStartWithWindows", chkAutoStart.Checked.ToString() }
                        };

                        var serializer = new JavaScriptSerializer();
                        string json = serializer.Serialize(configData);

                        File.WriteAllText(sfd.FileName, json);
                        MessageBox.Show("Настройки успешно экспортированы!", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Импортирует настройки из JSON-файла и перезаписывает текущую конфигурацию приложения.
        /// </summary>
        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON Конфигурация (*.json)|*.json";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(ofd.FileName);
                        var serializer = new JavaScriptSerializer();
                        var configData = serializer.Deserialize<Dictionary<string, string>>(json);

                        if (configData != null)
                        {
                            foreach (var item in configData)
                            {
                                ConfigHelper.Set(item.Key, item.Value);
                            }

                            LoadConfigToUI();
                            mainForm.UpdateTrayMenuNames();
                            MessageBox.Show("Настройки успешно импортированы!", "Импорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region Вспомогательные методы выбора файлов/папок

        /// <summary>Открывает диалог выбора папки и записывает результат в указанное текстовое поле.</summary>
        /// <param name="targetTextBox">Поле, в которое будет записан выбранный путь.</param>
        private void BrowseFolder(TextBox targetTextBox)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    targetTextBox.Text = fbd.SelectedPath;
                }
            }
        }

        /// <summary>Открывает диалог выбора файла с указанным фильтром и записывает результат в текстовое поле.</summary>
        /// <param name="targetTextBox">Поле, в которое будет записан выбранный путь.</param>
        /// <param name="filter">Фильтр диалога (например, "*.exe").</param>
        private void BrowseFile(TextBox targetTextBox, string filter)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = filter;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    targetTextBox.Text = ofd.FileName;
                }
            }
        }

        #endregion

        #region Автозапуск Windows

        /// <summary>Имя задачи в Планировщике заданий Windows для автозапуска приложения.</summary>
        private const string AutoStartTaskName = "NetworkSwitcherAutostart";

        /// <summary>
        /// Настраивает автозапуск через Планировщик заданий Windows.
        /// Ключ реестра HKCU\...\Run НЕ подходит для программ, требующих прав администратора:
        /// Windows не может тихо поднять UAC для элементов автозапуска в реестре, поэтому
        /// приложение с requireAdministrator в манифесте либо не запустится вовсе,
        /// либо запуск будет ненадёжным. Задача с "Выполнять с наивысшими правами"
        /// решает это корректно.
        /// </summary>
        /// <param name="enable"><c>true</c> — создать задачу, <c>false</c> — удалить.</param>
        private void SetAutoStartTask(bool enable)
        {
            // На всякий случай подчищаем старую запись автозапуска через реестр,
            // оставшуюся от предыдущих версий приложения, чтобы не было двойного запуска.
            RemoveLegacyRegistryAutostart();

            try
            {
                ProcessStartInfo psi;

                if (enable)
                {
                    string exePath = Application.ExecutablePath;

                    // Внутренние кавычки вокруг пути к exe нужно экранировать бэкслешем,
                    // т.к. они должны остаться литеральными внутри значения /TR, которое само
                    // обрамлено внешними кавычками (путь может содержать пробелы).
                    string arguments =
                        $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{AutoStartTaskName}\" /TR \"\\\"{exePath}\\\" --minimized\"";

                    psi = new ProcessStartInfo
                    {
                        FileName = "schtasks.exe",
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                }
                else
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = "schtasks.exe",
                        Arguments = $"/Delete /F /TN \"{AutoStartTaskName}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                }

                using (var p = Process.Start(psi))
                {
                    p?.WaitForExit(5000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось настроить автозапуск через Планировщик заданий: {ex.Message}",
                    "Ошибка автозапуска",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>Удаляет устаревшую запись автозапуска из реестра HKCU (если осталась).</summary>
        private void RemoveLegacyRegistryAutostart()
        {
            try
            {
                string regKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                string appName = "NetworkSwitcher";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(regKeyPath, true))
                {
                    if (key?.GetValue(appName) != null)
                    {
                        key.DeleteValue(appName);
                    }
                }
            }
            catch
            {
                // Не критично, если старой записи не было или не удалось удалить.
            }
        }

        #endregion
    }
}
