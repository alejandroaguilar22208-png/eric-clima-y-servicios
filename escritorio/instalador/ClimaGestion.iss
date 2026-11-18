#define Nombre "ClimaGestión"
#define Version "1.0.0"
#define Empresa "Eric Clima y Servicios"
#define Exe "EricClima.Escritorio.exe"

[Setup]
AppId={{6F3A2C1B-8E4D-4A7B-9C55-2D1E0F7A9B31}
AppName={#Nombre}
AppVersion={#Version}
AppPublisher={#Empresa}
DefaultDirName={autopf}\ClimaGestion
DefaultGroupName={#Nombre}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
OutputDir=salida
OutputBaseFilename=Instalar-ClimaGestion
SetupIconFile=..\EricClima.Escritorio\climagestion.ico
UninstallDisplayIcon={app}\{#Exe}
UninstallDisplayName={#Nombre}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "escritorio"; Description: "Crear un acceso directo en el escritorio"; GroupDescription: "Accesos directos:"

[Files]
Source: "publicado\{#Exe}"; DestDir: "{app}"; Flags: ignoreversion
Source: "publicado\Manual.pdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "publicado\questpdf_fonts_lato.br"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "publicado\conexion.txt"; DestDir: "{app}"; Flags: onlyifdoesntexist uninsneveruninstall
Source: "..\..\database\climagestion.sql"; DestDir: "{app}\base de datos"; Flags: ignoreversion
Source: "LEEME.txt"; DestDir: "{app}"; Flags: ignoreversion isreadme

[Icons]
Name: "{group}\{#Nombre}"; Filename: "{app}\{#Exe}"
Name: "{group}\Manual de usuario"; Filename: "{app}\Manual.pdf"
Name: "{group}\Configurar conexión a la base"; Filename: "notepad.exe"; Parameters: """{app}\conexion.txt"""
Name: "{group}\Desinstalar {#Nombre}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#Nombre}"; Filename: "{app}\{#Exe}"; Tasks: escritorio

[Run]
Filename: "{app}\{#Exe}"; Description: "Abrir {#Nombre} ahora"; Flags: nowait postinstall skipifsilent
