; ============================================
;   VPN Widget Installer - by code.itzpa1
;   App/Web Developer | IG: @code.itzpa1
; ============================================

[Setup]
AppName=VPN Widget
AppVersion=1.0.0
AppPublisher=code.itzpa1 (App/Web Developer)
AppPublisherURL=https://instagram.com/code.itzpa1
AppSupportURL=https://github.com/itzpa1
AppComments=VPN IP & Location Floating Widget | Built by code.itzpa1
AppCopyright=© 2025
DefaultDirName={userappdata}\VPNWidget
DefaultGroupName=VPN Widget
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename=VPNWidget-Setup
Compression=lzma
SolidCompression=yes
SetupIconFile=VpnWidget.ico
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\VpnWidget.exe

[Files]
Source: "publish\VpnWidget.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "VpnWidget.ico"; DestDir:"{app}"
Source: "Credits.txt"; DestDir:"{app}"

; =============================
; Shortcuts
; =============================
[Icons]
Name: "{userdesktop}\VPN Widget"; Filename: "{app}\VpnWidget.exe"; Tasks: desktopicon
; Startup shortcut (optional)
; Name: "{userstartup}\VPN Widget"; Filename: "{app}\VpnWidget.exe"

[Tasks]
Name: "desktopicon"; Description: "Create Desktop Shortcut"; Flags: unchecked

; =============================
; After Install
; =============================
[Run]
Filename: "{app}\VpnWidget.exe"; Description: "Run VPN Widget now"; Flags: nowait postinstall skipifsilent
Filename: "notepad.exe"; Parameters: "{app}\Credits.txt"; Flags: postinstall shellexec runasoriginaluser
Filename: "https://instagram.com/code.itzpa1"; Description: "Open Instagram"; Flags: postinstall shellexec runasoriginaluser unchecked
Filename: "https://github.com/itzpa1"; Description: "Open GitHub"; Flags: postinstall shellexec runasoriginaluser unchecked

[UninstallRun]
Filename: "taskkill.exe"; Parameters: "/IM VpnWidget.exe /F"; Flags: runhidden

; =============================
; Startup (Registry)
; =============================
[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "VpnWidget"; ValueData: """{app}\VpnWidget.exe"""; Flags: uninsdeletevalue

; =============================
; Create/Update Credits File
; =============================
[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  CreditsFile: String;
begin
  if CurStep = ssInstall then
  begin
    CreditsFile := ExpandConstant('{app}\Credits.txt');
    SaveStringToFile(CreditsFile,
      '------------------------------'#13#10 +
      '  VPN Widget by code.itzpa1'#13#10 +
      '------------------------------'#13#10#13#10 +
      'Author: code.itzpa1 (App/Web Developer)'#13#10 +
      'Instagram: https://instagram.com/code.itzpa1'#13#10 +
      'GitHub: https://github.com/itzpa1'#13#10#13#10 +
      'Thank you for installing VPN Widget!', False);
  end;
end;
