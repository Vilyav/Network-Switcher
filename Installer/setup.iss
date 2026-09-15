[Setup]
SourceDir=..
AppName=NetworkSwitcher
AppVersion=1.0
DefaultDirName={autopf}\NetworkSwitcher
DefaultGroupName=NetworkSwitcher
OutputBaseFilename=NetworkSwitcher_Setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
SetupIconFile=Resources\miyabi.ico

[Files]
Source: "bin\Release\NetworkSwitcher.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\NetworkSwitcher.exe.config"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\NetworkSwitcher"; Filename: "{app}\NetworkSwitcher.exe"; IconFilename: "{app}\NetworkSwitcher.exe"
Name: "{commondesktop}\NetworkSwitcher"; Filename: "{app}\NetworkSwitcher.exe"; IconFilename: "{app}\NetworkSwitcher.exe"

[Code]
var
  // Страница путей
  PagePaths: TWizardPage;
  EditZapretDir: TNewEdit;
  EditWarpExe: TNewEdit;
  EditAmneziaExe: TNewEdit;
  EditAmneziaConf: TNewEdit;

  // Страница стратегий
  PageStrategies: TWizardPage;
  RadioDefaultStrat: TRadioButton;
  RadioCustomStrat: TRadioButton;
  EditStrat1Name: TNewEdit;
  EditStrat1Bat: TNewEdit;
  EditStrat2Name: TNewEdit;
  EditStrat2Bat: TNewEdit;

// --- Вспомогательные функции ---

function ExtractFileNameWithoutExt(const FilePath: String): String;
var
  FileName: String;
  DotPos: Integer;
begin
  FileName := ExtractFileName(FilePath);
  DotPos := Pos('.', FileName);
  if DotPos > 0 then
    Result := Copy(FileName, 1, DotPos - 1)
  else
    Result := FileName;
end;

// --- Обработчики нажатий кнопок «Обзор...» ---

procedure BrowseZapretDirClick(Sender: TObject);
var
  Dir: String;
begin
  Dir := EditZapretDir.Text;
  if BrowseForFolder('Выберите папку Zapret', Dir, True) then
    EditZapretDir.Text := Dir;
end;

procedure BrowseWarpExeClick(Sender: TObject);
var
  FileName: String;
begin
  FileName := EditWarpExe.Text;
  if GetOpenFileName('Выберите warp-cli.exe', FileName, 'C:\Program Files\Cloudflare\Cloudflare WARP', 'Исполняемые файлы (*.exe)|*.exe|Все файлы (*.*)|*.*', 'exe') then
    EditWarpExe.Text := FileName;
end;

procedure BrowseAmneziaExeClick(Sender: TObject);
var
  FileName: String;
begin
  FileName := EditAmneziaExe.Text;
  if GetOpenFileName('Выберите amneziawg.exe', FileName, 'C:\Program Files\AmneziaWG', 'Исполняемые файлы (*.exe)|*.exe|Все файлы (*.*)|*.*', 'exe') then
    EditAmneziaExe.Text := FileName;
end;

procedure BrowseAmneziaConfClick(Sender: TObject);
var
  FileName: String;
begin
  FileName := EditAmneziaConf.Text;
  if GetOpenFileName('Выберите файл конфигурации AmneziaWG', FileName, 'C:\Program Files\AmneziaWG', 'Файлы конфигурации (*.conf)|*.conf|Все файлы (*.*)|*.*', 'conf') then
    EditAmneziaConf.Text := FileName;
end;

// --- Переключение доступности полей стратегий ---

procedure UpdateStrategiesEnabledState;
var
  EnableCustom: Boolean;
begin
  EnableCustom := RadioCustomStrat.Checked;
  EditStrat1Name.Enabled := EnableCustom;
  EditStrat1Bat.Enabled := EnableCustom;
  EditStrat2Name.Enabled := EnableCustom;
  EditStrat2Bat.Enabled := EnableCustom;
end;

procedure StrategyRadioOnClick(Sender: TObject);
begin
  UpdateStrategiesEnabledState;
end;

// --- Запись настроек ---

procedure UpdateConfigKey(const ConfigPath, KeyName, KeyValue: String);
var
  Lines: TArrayOfString;
  I: Integer;
  SearchStr: String;
begin
  if LoadStringsFromFile(ConfigPath, Lines) then
  begin
    SearchStr := 'key="' + KeyName + '"';
    for I := 0 to GetArrayLength(Lines) - 1 do
    begin
      if Pos(SearchStr, Lines[I]) > 0 then
      begin
        Lines[I] := '    <add key="' + KeyName + '" value="' + KeyValue + '" />';
        Break;
      end;
    end;
    SaveStringsToUTF8File(ConfigPath, Lines, False);
  end;
end;

// --- Инициализация элементов управления ---

procedure InitializeWizard;
var
  Btn: TNewButton;
begin
  // --- Страница 1: Пути ---
  PagePaths := CreateCustomPage(wpSelectDir, 'Настройка путей к программам', 'Укажите расположения необходимых утилит');

  // Zapret Folder
  with TNewStaticText.Create(PagePaths) do
  begin
    Parent := PagePaths.Surface;
    Caption := 'Папка Zapret:';
    Left := 0; Top := 0;
  end;
  EditZapretDir := TNewEdit.Create(PagePaths);
  EditZapretDir.Parent := PagePaths.Surface;
  EditZapretDir.Text := 'C:\zapret';
  EditZapretDir.Left := 0; EditZapretDir.Top := 18; EditZapretDir.Width := 320;

  Btn := TNewButton.Create(PagePaths);
  Btn.Parent := PagePaths.Surface;
  Btn.Caption := 'Обзор...';
  Btn.Left := 330; Btn.Top := 16; Btn.Width := 75;
  Btn.OnClick := @BrowseZapretDirClick;

  // Cloudflare WARP CLI
  with TNewStaticText.Create(PagePaths) do
  begin
    Parent := PagePaths.Surface;
    Caption := 'Путь к warp-cli.exe:';
    Left := 0; Top := 50;
  end;
  EditWarpExe := TNewEdit.Create(PagePaths);
  EditWarpExe.Parent := PagePaths.Surface;
  EditWarpExe.Text := 'C:\Program Files\Cloudflare\Cloudflare WARP\warp-cli.exe';
  EditWarpExe.Left := 0; EditWarpExe.Top := 68; EditWarpExe.Width := 320;

  Btn := TNewButton.Create(PagePaths);
  Btn.Parent := PagePaths.Surface;
  Btn.Caption := 'Обзор...';
  Btn.Left := 330; Btn.Top := 66; Btn.Width := 75;
  Btn.OnClick := @BrowseWarpExeClick;

  // AmneziaWG Exe
  with TNewStaticText.Create(PagePaths) do
  begin
    Parent := PagePaths.Surface;
    Caption := 'Путь к amneziawg.exe:';
    Left := 0; Top := 100;
  end;
  EditAmneziaExe := TNewEdit.Create(PagePaths);
  EditAmneziaExe.Parent := PagePaths.Surface;
  EditAmneziaExe.Text := 'C:\Program Files\AmneziaWG\amneziawg.exe';
  EditAmneziaExe.Left := 0; EditAmneziaExe.Top := 118; EditAmneziaExe.Width := 320;

  Btn := TNewButton.Create(PagePaths);
  Btn.Parent := PagePaths.Surface;
  Btn.Caption := 'Обзор...';
  Btn.Left := 330; Btn.Top := 116; Btn.Width := 75;
  Btn.OnClick := @BrowseAmneziaExeClick;

  // AmneziaWG Conf
  with TNewStaticText.Create(PagePaths) do
  begin
    Parent := PagePaths.Surface;
    Caption := 'Путь к .conf файлу AmneziaWG:';
    Left := 0; Top := 150;
  end;
  EditAmneziaConf := TNewEdit.Create(PagePaths);
  EditAmneziaConf.Parent := PagePaths.Surface;
  EditAmneziaConf.Text := 'C:\Program Files\AmneziaWG\WARP.conf';
  EditAmneziaConf.Left := 0; EditAmneziaConf.Top := 168; EditAmneziaConf.Width := 320;

  Btn := TNewButton.Create(PagePaths);
  Btn.Parent := PagePaths.Surface;
  Btn.Caption := 'Обзор...';
  Btn.Left := 330; Btn.Top := 166; Btn.Width := 75;
  Btn.OnClick := @BrowseAmneziaConfClick;

  // --- Страница 2: Стратегии Zapret ---
  PageStrategies := CreateCustomPage(PagePaths.ID, 'Настройка стратегий Zapret', 'Выберите батники для слотов управления');

  RadioDefaultStrat := TRadioButton.Create(PageStrategies);
  RadioDefaultStrat.Parent := PageStrategies.Surface;
  RadioDefaultStrat.Caption := 'Использовать стандартные (general (ALT9).bat / general (ALT11).bat)';
  RadioDefaultStrat.Left := 0; RadioDefaultStrat.Top := 10; RadioDefaultStrat.Width := 400;
  RadioDefaultStrat.Checked := True;
  RadioDefaultStrat.OnClick := @StrategyRadioOnClick;

  RadioCustomStrat := TRadioButton.Create(PageStrategies);
  RadioCustomStrat.Parent := PageStrategies.Surface;
  RadioCustomStrat.Caption := 'Указать свои имена и .bat файлы';
  RadioCustomStrat.Left := 0; RadioCustomStrat.Top := 35; RadioCustomStrat.Width := 400;
  RadioCustomStrat.OnClick := @StrategyRadioOnClick;

  with TNewStaticText.Create(PageStrategies) do
  begin
    Parent := PageStrategies.Surface;
    Caption := 'Имя Слота 1 / .bat файл:';
    Left := 20; Top := 70;
  end;
  EditStrat1Name := TNewEdit.Create(PageStrategies);
  EditStrat1Name.Parent := PageStrategies.Surface;
  EditStrat1Name.Text := 'Стратегия 9 (ALT9)';
  EditStrat1Name.Left := 20; EditStrat1Name.Top := 88; EditStrat1Name.Width := 150;

  EditStrat1Bat := TNewEdit.Create(PageStrategies);
  EditStrat1Bat.Parent := PageStrategies.Surface;
  EditStrat1Bat.Text := 'general (ALT9).bat';
  EditStrat1Bat.Left := 180; EditStrat1Bat.Top := 88; EditStrat1Bat.Width := 180;

  with TNewStaticText.Create(PageStrategies) do
  begin
    Parent := PageStrategies.Surface;
    Caption := 'Имя Слота 2 / .bat файл:';
    Left := 20; Top := 120;
  end;
  EditStrat2Name := TNewEdit.Create(PageStrategies);
  EditStrat2Name.Parent := PageStrategies.Surface;
  EditStrat2Name.Text := 'Стратегия 11 (ALT11)';
  EditStrat2Name.Left := 20; EditStrat2Name.Top := 138; EditStrat2Name.Width := 150;

  EditStrat2Bat := TNewEdit.Create(PageStrategies);
  EditStrat2Bat.Parent := PageStrategies.Surface;
  EditStrat2Bat.Text := 'general (ALT11).bat';
  EditStrat2Bat.Left := 180; EditStrat2Bat.Top := 138; EditStrat2Bat.Width := 180;

  // Инициализация начального состояния блокировки полей
  UpdateStrategiesEnabledState;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ConfigFile: String;
  TunnelName: String;
begin
  if CurStep = ssPostInstall then
  begin
    ConfigFile := ExpandConstant('{app}\NetworkSwitcher.exe.config');

    // Извлекаем имя туннеля из выбранного .conf файла
    TunnelName := ExtractFileNameWithoutExt(EditAmneziaConf.Text);

    UpdateConfigKey(ConfigFile, 'ZapretFolderPath', EditZapretDir.Text);
    UpdateConfigKey(ConfigFile, 'WarpCliPath', EditWarpExe.Text);
    UpdateConfigKey(ConfigFile, 'AmneziaExePath', EditAmneziaExe.Text);
    UpdateConfigKey(ConfigFile, 'AmneziaConfPath', EditAmneziaConf.Text);
    UpdateConfigKey(ConfigFile, 'AmneziaTunnelName', TunnelName);

    if RadioCustomStrat.Checked then
    begin
      UpdateConfigKey(ConfigFile, 'UseCustomNames', 'true');
      UpdateConfigKey(ConfigFile, 'Slot1Name', EditStrat1Name.Text);
      UpdateConfigKey(ConfigFile, 'Slot1Bat', EditStrat1Bat.Text);
      UpdateConfigKey(ConfigFile, 'Slot2Name', EditStrat2Name.Text);
      UpdateConfigKey(ConfigFile, 'Slot2Bat', EditStrat2Bat.Text);
    end
    else
    begin
      UpdateConfigKey(ConfigFile, 'UseCustomNames', 'false');
      UpdateConfigKey(ConfigFile, 'Slot1Name', 'Стратегия 9 (ALT9)');
      UpdateConfigKey(ConfigFile, 'Slot1Bat', 'general (ALT9).bat');
      UpdateConfigKey(ConfigFile, 'Slot2Name', 'Стратегия 11 (ALT11)');
      UpdateConfigKey(ConfigFile, 'Slot2Bat', 'general (ALT11).bat');
    end;
  end;
end;