[Setup]
AppName=GoodNight-PC
AppVersion=2.0.0
AppPublisher=GoodNight-PC
DefaultDirName={commonpf}\GoodNight-PC
DefaultGroupName=GoodNight-PC
OutputDir=.
OutputBaseFilename=GoodNight-PC-Setup
SetupIconFile=icon.ico
Compression=lzma2
SolidCompression=yes

[Files]
Source: "bin\Release\net10.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\GoodNight-PC"; Filename: "{app}\GoodNight-PC.exe"
Name: "{commondesktop}\GoodNight-PC"; Filename: "{app}\GoodNight-PC.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "在桌面创建快捷方式"

[Run]
Filename: "{app}\GoodNight-PC.exe"; Description: "运行 GoodNight-PC"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

