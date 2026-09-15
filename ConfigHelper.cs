using System.Configuration;

namespace NetworkSwitcher
{
    /// <summary>
    /// Предоставляет типизированный доступ к секции <c>appSettings</c> конфигурационного
    /// файла приложения (App.config / NetworkSwitcher.exe.config).
    /// </summary>
    /// <remarks>
    /// Все изменения сохраняются через <see cref="Configuration.Save()"/> в .exe.config,
    /// поэтому методы записи требуют прав администратора (приложение всегда запускается с ними —
    /// см. app.manifest).
    /// </remarks>
    public static class ConfigHelper
    {
        /// <summary>
        /// Возвращает строковое значение параметра из <c>appSettings</c>.
        /// </summary>
        /// <param name="key">Ключ параметра конфигурации.</param>
        /// <param name="defaultValue">Значение, возвращаемое, если ключ отсутствует или пуст.</param>
        /// <returns>Значение параметра либо <paramref name="defaultValue"/>.</returns>
        public static string Get(string key, string defaultValue = "")
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        /// <summary>
        /// Возвращает логическое значение параметра из <c>appSettings</c>.
        /// </summary>
        /// <param name="key">Ключ параметра конфигурации.</param>
        /// <param name="defaultValue">Значение, возвращаемое, если ключ отсутствует или содержит
        /// непарсящееся значение.</param>
        /// <returns>Распарсенное значение либо <paramref name="defaultValue"/>.</returns>
        public static bool GetBool(string key, bool defaultValue = false)
        {
            string raw = ConfigurationManager.AppSettings[key];
            return bool.TryParse(raw, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// Устанавливает (или добавляет) значение ключа в <c>appSettings</c> и сохраняет конфигурацию.
        /// </summary>
        /// <param name="key">Ключ параметра конфигурации.</param>
        /// <param name="value">Новое строковое значение параметра.</param>
        public static void Set(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
