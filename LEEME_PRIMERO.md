# Garaje inteligente UNO · 1.0.2

**Orden de inicio del portable:** `PanelGaraje.exe → Iniciar local (solo PC) o Iniciar LAN / Hotspot (PC y celular) → Abrir local → seleccionar COM → Conectar`. Para cambiar de local a LAN, detener primero el servidor. Para el teléfono: misma red → QR LAN → PIN del panel. Sigue [la guía con capturas](docs/GUIA_PANEL_Y_CELULAR.md).

El portable incluye Python y .NET. No ejecute el instalador de dependencias para usar `PanelGaraje.exe`. Para ejecutar desde fuentes, consulte `docs/INSTALACION.md`. El driver CH340 oficial está en `support/ch340`; se instala manualmente solo si la placa lo necesita.

Proyecto educativo: firmware autónomo, interfaz web adaptable al celular y panel de Windows independiente para administrar servidor, PIN, QR y acceso remoto.

## Empieza sin conectar motores

1. Extrae **todo** el ZIP portable. Abre `PanelGaraje.exe`; conserva `GarajeServidor.exe` y `_internal` a su lado.
2. En el panel, inicia la demostración y abre la interfaz local. Prueba abrir, pausar, reanudar y los sensores simulados.
3. Lee [MONTAJE_Y_CALIBRACION.md](docs/MONTAJE_Y_CALIBRACION.md) antes de conectar las pilas.
4. Carga `firmware/GarajeInteligente/GarajeInteligente.ino` con Arduino IDE, placa **Arduino UNO**, biblioteca **Servo**. `EEPROM` viene con el núcleo AVR. Cierra el monitor serie antes de conectar la interfaz.
5. Sal de demo, selecciona el puerto del UNO y conecta. Configura y calibra con los servos desacoplados. El firmware inicial no mueve los motores hasta habilitar la calibración y ordenar movimiento.

El portable incluye sus dependencias de Python y .NET para Windows x64. Se construyó y verificó el servidor en este equipo Windows 11; queda pendiente probar el conjunto en tu HP con Windows 10 y con el Arduino físico.

## Qué hace

- Detecta el vehículo exterior con rango, confirmación e histéresis ajustables.
- Acciona uno o dos servos posicionales con extremos independientes; los dos pueden moverse en sentidos opuestos.
- Registra presencia interior y cierra después del tiempo configurado, siempre con exterior libre confirmado. También cierra tras una aproximación abandonada.
- Un obstáculo exterior o una lectura ultrasónica inválida durante el cierre provoca reapertura. La falta de eco nunca se interpreta como vía libre.
- Permite abrir desde el teléfono para salir. El infrarrojo interior no abre automáticamente.
- Guarda configuración en EEPROM únicamente por orden explícita. La pausa también se conserva para inhibir el arranque tras un reinicio.
- Continúa su ciclo sin interfaz ni servidor, mientras siga alimentado. Para iniciar automáticamente después de encender hay que habilitar y guardar **arranque autónomo**.

**La posición mostrada es ordenada/estimada, no medida.** Los SG90 no informan su posición real. Esta maqueta no incorpora protección certificada contra atrapamientos.

## Panel e interfaz

El **panel de Windows** administra el servidor y los accesos. La **interfaz del navegador** controla el Arduino y configura el garaje. Comparten datos en la computadora; el UNO no se conecta por sí solo a Wi-Fi.

Para LAN: detén el servidor local y usa «Iniciar LAN / Hotspot». El teléfono debe estar en la misma red; selecciona en el panel la IP de la conexión correcta y escanea el QR. Introduce el PIN mostrado. El panel no crea el hotspot ni modifica el firewall. Si Windows solicita acceso, usa únicamente la red privada en la que confías.

Para Internet: consulta [ACCESO_REMOTO.md](docs/ACCESO_REMOTO.md). ngrok es opcional y no viene incluido. No se publicó ningún túnel durante la entrega.

## Archivos

| Carpeta | Contenido |
|---|---|
| firmware | Sketch UNO y controlador de estados |
| interfaz_garaje | Servidor Python, protocolo e interfaz HTML/CSS/JS |
| panel_garaje | Fuente C# del panel independiente |
| tests | Pruebas de lógica, protocolo, conexión y HTTP |
| docs | Montaje, acceso remoto y evidencias de validación |
| entregas | Portable Windows y ZIP |

Para ejecutar desde fuentes: instala Python 3.11 o posterior y `pyserial==3.5`, después ejecuta `python interfaz_garaje/server.py --no-browser`. Abre `http://127.0.0.1:8773`. Para compilar el panel se necesita SDK .NET 9 y acceso a NuGet para QRCoder.

## Límites importantes

Los servos se alimentan aparte: **4 pilas AA → regulador de 5 V → servos**, con tierra común con UNO. No conectes el positivo del portapilas al pin 5V del Arduino. La duración de las pilas y la capacidad de levantar tu puerta deben medirse con la maqueta real.

Pausar detiene el cambio de posición ordenada y bloquea el automatismo; conserva los pulsos. No corta la alimentación. Mantenimiento retira los pulsos, por lo que debes sostener la puerta. Al encender o reiniciar se desconoce la posición: el primer movimiento puede ser brusco. Con arranque autónomo habilitado, ese movimiento inicial es abrir.

Consulta [VALIDACION.md](docs/VALIDACION.md) para separar las pruebas de software de las pruebas físicas pendientes.
