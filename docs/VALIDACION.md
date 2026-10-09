# Validación · 7 de octubre de 2026

## Release 1.0.2 · 8 de octubre de 2026

- Construcción completa ejecutada con `scripts/build_windows.ps1` en Windows 11 x64: entorno virtual, dependencias, PyInstaller y panel .NET autocontenido.
- 19 pruebas de backend aprobadas; prueba JavaScript de los 1601 pulsos y entradas inválidas aprobada.
- Portable extraído en carpeta nueva: health, HTML/JS, demo y STOP aprobados, sin conectar Arduino.
- ZIP de fuentes extraído en carpeta nueva: instalador PowerShell, creación de `.venv`, instalación de pyserial y arranque HTTP aprobados.
- CH341SER.EXE descargado desde WCH y verificado por SHA256 y Authenticode: firma válida de Nanjing Qinheng Microelectronics Co., Ltd. No se ejecutó el instalador del driver.
- ZIP de fuentes revisado: sin `.build`, `.venv`, ejecutables, entregas, cachés ni historial del traslado. Archivos SHA256 generados para ambos ZIP.
- El panel usa `.venv/Scripts/python.exe` cuando se ejecuta desde fuentes y ese entorno existe; su compilación fue verificada. La revisión visual y el flujo completo del panel nativo siguen pendientes.

Las pruebas de esta sección son locales. El workflow de GitHub Actions aún no ha ejecutado en GitHub. No se repitió la compilación del firmware porque no cambió. Las pruebas físicas y de los demás equipos enumeradas abajo siguen pendientes.

## Comprobado en software

| Prueba | Resultado |
|---|---|
| Arduino CLI, AVR UNO, Servo | Compila sin advertencias del proyecto; 9888 bytes flash (30 %), 643 bytes globales RAM (31 %) |
| Controlador C++ real, compilado a WASM y ejecutado con Node | 26 comprobaciones aprobadas |
| Python unittest | 19 pruebas aprobadas: protocolo, ACK, timeout sin reintento, autenticación, HTTP, demo y configuración |
| Panel C# | Build Release y publish Windows x64 autocontenido exitosos |
| Portable desde ZIP extraído en carpeta nueva | Servidor, health, HTML/JS, demo y STOP correctos |
| Navegador | Abrir, pausar, recargar manteniendo pausa, mantenimiento, seleccionar 2 servos, aplicar/guardar simulado, salir y reanudar |
| Diseño adaptable | Capturas de escritorio y viewport móvil de 390 px; sin desbordamiento horizontal observado en vista general |

El simulador no replica con exactitud los filtros ni valida electrónica. Las pruebas C++ usan el mismo `control.h` incluido en el sketch, pero no ejecutan Servo/EEPROM sobre una placa real. La prueba portable se ejecutó en Windows 11; no prueba aún el equipo HP con Windows 10. El panel nativo compiló; su revisión visual completa en Windows queda pendiente. La automatización de un diálogo de confirmación del navegador tuvo un timeout; se recuperó con una pestaña nueva y se verificó el estado resultante.

## Pruebas físicas pendientes

1. Verificar polaridades, regulador a 5 V y GND común antes de energizar motores.
2. Cargar firmware en UNO real, identificar COM y confirmar versión `GARAGE-1.0.0`.
3. Comprobar lecturas exteriores con cinta métrica y estado crudo/polaridad IR; probar falta de eco y fondo libre.
4. Calibrar un servo sin carga; repetir para el segundo desacoplado; verificar extremos y puntos intermedios sin esfuerzos.
5. Probar apertura, entrada, abandono de entrada, cierre y reapertura con obstáculos de juguete.
6. Verificar pausa durante movimiento, recarga web, desconexión serie y reinicio de UNO: una pausa guardada debe impedir arranque automático.
7. Guardar ajustes, apagar y leer tras reiniciar para demostrar persistencia EEPROM real.
8. Habilitar arranque autónomo solo tras revisar mecánica: comprobar inicio, funcionamiento sin servidor y posibles reinicios por conexión USB.
9. Medir caída de tensión, corriente, calentamiento y autonomía con 4 AA bajo carga. No bloquear deliberadamente los ejes durante periodos prolongados.
10. Probar teléfono físico en LAN, lectura del QR, PIN y, opcionalmente, ngrok con cuenta propia. Ningún túnel externo fue publicado.

## Repetir pruebas desde fuentes

```powershell
python -m unittest discover -s tests -p 'test_*.py'
arduino-cli compile --fqbn arduino:avr:uno --warnings all firmware/GarajeInteligente
dotnet build panel_garaje/PanelGaraje.csproj -c Release
node --check interfaz_garaje/static/app.js
python tools/check_portable.py
```

`tests/control_tests.cpp` expone `run_tests`; puede compilarse con Clang para wasm32, sin biblioteca estándar, y ejecutarse mediante WebAssembly en Node. Devuelve cero si pasa y el índice de comprobación si falla. El paquete se arma con `tools/package.py` después de publicar panel y servidor; no incluye tokens.

## Interfaz 1.0.1
Selector grados/microsegundos validado con los 1601 valores enteros de 700 a 2300 µs, límites y entradas inválidas. En navegador demo se verificó aplicar 56,25 grados como 1200 µs y cambiar de unidad conservando el valor. Firmware y formato EEPROM sin cambios. Prueba física pendiente.

