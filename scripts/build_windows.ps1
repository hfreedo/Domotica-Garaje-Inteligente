$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
Set-Location -LiteralPath $projectRoot
& "$PSScriptRoot\instalar_dependencias.ps1" -Build
if ($LASTEXITCODE -ne 0) { throw 'No se pudieron instalar las dependencias de compilacion.' }
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Instale .NET SDK 9 para compilar el panel.' }
$staticAssets = (Join-Path $projectRoot 'interfaz_garaje/static') + ';static'
& '.\.venv\Scripts\python.exe' -m PyInstaller --noconfirm --clean --name GarajeServidor --onedir --console --add-data $staticAssets --distpath .build/server-dist --workpath .build/pyinstaller --specpath .build interfaz_garaje/server.py
if ($LASTEXITCODE -ne 0) { throw 'Fallo PyInstaller.' }
& dotnet publish panel_garaje/PanelGaraje.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .build/panel
if ($LASTEXITCODE -ne 0) { throw 'Fallo la compilacion del panel.' }
& '.\.venv\Scripts\python.exe' tools/package.py
if ($LASTEXITCODE -ne 0) { throw 'Fallo el empaquetado.' }
