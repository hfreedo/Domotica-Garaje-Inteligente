# Instalación

## Portable Windows x64

Extraer todo el ZIP 1.0.2 a una carpeta con permiso de escritura. Abrir `PanelGaraje.exe`; mantener `GarajeServidor.exe` y `_internal` a su lado. No requiere Python, pip ni .NET instalados. No ejecutar desde dentro del ZIP. La validación local corresponde a Windows 11; Windows 10 y el equipo HP requieren una prueba propia.

## Fuentes del servidor

Instalar Python 3.11 o posterior desde https://www.python.org/downloads/windows/ con PATH habilitado. Ejecutar `INSTALAR_DEPENDENCIAS.cmd` y después `INICIAR_SERVIDOR.cmd`. El instalador usa Internet y crea un entorno `.venv` propio del proyecto. No instala paquetes globales, drivers, Arduino IDE ni ngrok.

Alternativa manual:

```powershell
python -m venv .venv
.venv/Scripts/python.exe -m pip install -r requirements.txt
.venv/Scripts/python.exe interfaz_garaje/server.py
```

La dependencia de ejecución es `pyserial==3.5`; HTTP, JSON y demás servicios utilizan la biblioteca estándar de Python. Node solo sirve para las pruebas JavaScript. Los documentos ya están generados; regenerarlos requiere herramientas adicionales de Word/PDF ajenas al servidor.

## Arduino y CH340

Instalar Arduino IDE desde https://www.arduino.cc/en/software y la biblioteca Servo desde su gestor. Abrir `firmware/GarajeInteligente/GarajeInteligente.ino`, seleccionar Arduino UNO y el puerto correcto. EEPROM está incluida en el núcleo AVR. Cerrar el monitor serie antes de conectar el servidor. No cargar firmware ni calibrar con los servos acoplados a la puerta.

Si el conversor USB de la placa es CH340/CH341, seguir `support/ch340/LEEME.md`. Una UNO con otro conversor puede necesitar otro controlador. La instalación de un driver no demuestra que la placa esté conectada ni valida sensores o motores.

## Compilar el portable

Necesita Windows x64, Python, Git, Internet y .NET SDK 9 (https://dotnet.microsoft.com/download/dotnet/9.0). Trabajar desde un clon Git; si se extrajo el ZIP de fuentes, ejecutar primero `git init`. Desde la raíz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/descargar_ch340.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/build_windows.ps1
.venv/Scripts/python.exe tools/check_portable.py
```

El script instala las dependencias de `requirements-build.txt`, compila el servidor con PyInstaller, publica el panel autocontenido y arma el ZIP. Descargar y verificar el driver con `scripts/descargar_ch340.ps1` antes de empaquetar. No cambiar el nombre de un archivo HTML a EXE: el empaquetador verifica su hash y cabecera.

## Problemas habituales

- Python abre Microsoft Store: instalar Python y reabrir la consola, o corregir su alias en Windows.
- Puerto 8773 ocupado: detener la instancia anterior o elegir otro puerto desde el panel.
- No aparece COM: probar un cable USB de datos, revisar Administrador de dispositivos y el modelo del conversor.
- Acceso al COM denegado: cerrar Arduino Monitor Serie y otras instancias del servidor.
- Teléfono: usar la misma red y el modo LAN del panel. El servidor no configura automáticamente firewall ni hotspot.
- ngrok es opcional; seguir `docs/ACCESO_REMOTO.md`, usando cuenta y token propios.
