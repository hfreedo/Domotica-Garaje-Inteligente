@echo off
cd /d "%~dp0"
if not exist ".venv\Scripts\python.exe" (
  echo Primero ejecute INSTALAR_DEPENDENCIAS.cmd.
  pause
  exit /b 1
)
".venv\Scripts\python.exe" interfaz_garaje\server.py
set "result=%errorlevel%"
if not "%result%"=="0" pause
exit /b %result%
