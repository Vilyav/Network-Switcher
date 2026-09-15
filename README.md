# NetworkSwitcher

Утилита для удобного переключения и управления сетевыми стратегиями Zapret, а также туннелями Cloudflare WARP и AmneziaWG.

---

## 🛠️ Используемые компоненты и полезные ссылки

Для работы программы используются следующие утилиты, сервисы и генераторы конфигураций:

* **Zapret (Запрет для Discord и YouTube)**  
  * [Страница релизов zapret-discord-youtube (GitHub)](https://github.com/Flowseal/zapret-discord-youtube/releases) — готовые сборки батников и утилит Zapret.
* **AmneziaWG**  
  * [Страница релизов AmneziaWG Windows Client (GitHub)](https://github.com/amnezia-vpn/amneziawg-windows-client/releases) — инсталляторы WireGuard-клиента с защитой от блокировок.
  * [Генератор конфигураций WARP для AmneziaWG](https://warp-generation.github.io) — веб-сервис для быстрого создания готовых `.conf` файлов туннеля WARP.
* **Cloudflare WARP**  
  * [Центр загрузок Cloudflare Client](https://developers.cloudflare.com/cloudflare-one/team-and-resources/devices/cloudflare-one-client/download/) — официальные дистрибутивы приложения Cloudflare WARP для Windows.

---

## ⚙️ Требования к окружению

Перед запуском или установкой утилиты убедитесь, что в вашей системе подготовлены компоненты:

1. **Zapret** — распакован в любую удобную папку (по умолчанию `C:\zapret` или папка с релизом `zapret-discord-youtube`).
2. **AmneziaWG** — установлен в систему, сгенерирован и импортирован `.conf` файл конфигурации (можно получить через [warp-generation.github.io](https://warp-generation.github.io)).
3. **Cloudflare WARP** — установлен клиент (для работы через `warp-cli.exe`).

---

## 🚀 Установка и первичная настройка

1. Запустите `NetworkSwitcher_Setup.exe` от имени **Администратора**.
2. На странице **Настройка путей** укажите:
   * **Папка Zapret**: каталог с батниками (например, `C:\Users\...\zapret-discord-youtube-1.10.2`).
   * **Warp CLI**: путь к `warp-cli.exe` (по умолчанию `C:\Program Files\Cloudflare\Cloudflare WARP\warp-cli.exe`).
   * **AmneziaWG Exe**: путь к `amneziawg.exe` (по умолчанию `C:\Program Files\AmneziaWG\amneziawg.exe`).
   * **Конфиг AmneziaWG**: путь к вашему `.conf` файлу. *Имя туннеля извлечётся и зафиксируется автоматически из названия файла.*
3. На странице **Настройка стратегий** выберите режим работы (стандартные имена батников `general (ALT9).bat` / `general (ALT11).bat` или укажите кастомные).

---

## 📋 Использование

* Запустите **NetworkSwitcher** через ярлык на рабочем столе или в меню «Пуск».
* Все пути, имена слотов и батников можно в любой момент отредактировать в окне **«Настройки системы»** внутри приложения.
* Настройки автоматически сохраняются в `NetworkSwitcher.exe.config`.

---

## 📄 Лицензия

Проект распространяется под лицензией MIT.