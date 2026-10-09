param([switch]$Build)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
Set-Location -LiteralPath $projectRoot
try {
    $pythonCommand = Get-Command python -ErrorAction SilentlyContinue
    if ($pythonCommand) { $bootstrap = $pythonCommand.Source; $prefix = @() }
    elseif (Get-Command py -ErrorAction SilentlyContinue) { $bootstrap = 'py'; $prefix = @('-3') }
    else { throw 'Instale Python 3.11 o posterior desde https://www.python.org/downloads/windows/ y habilite Add Python to PATH.' }
    & $bootstrap @prefix -c 'import sys; assert sys.version_info >= (3,11)'
    if ($LASTEXITCODE -ne 0) { throw 'Python no esta disponible o su version no es compatible.' }
    if (!(Test-Path '.venv/Scripts/python.exe')) {
        & $bootstrap @prefix -m venv .venv
        if ($LASTEXITCODE -ne 0) { throw 'No se pudo crear .venv.' }
    }
    $requirements = 'requirements.txt'
    if ($Build) { $requirements = 'requirements-build.txt' }
    & '.\.venv\Scripts\python.exe' -m pip install -r $requirements
    if ($LASTEXITCODE -ne 0) { throw 'Fallo la instalacion de dependencias. Compruebe su conexion.' }
    & '.\.venv\Scripts\python.exe' -c "import serial; print('pyserial', serial.VERSION)"
    if ($LASTEXITCODE -ne 0) { throw 'Fallo la verificacion de pyserial.' }
    Write-Host 'Listo. Ejecute INICIAR_SERVIDOR.cmd. No se instalo ningun driver ni se conecto Arduino.'
} catch { Write-Error $_; exit 1 }
