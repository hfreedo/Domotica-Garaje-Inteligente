# Garaje Inteligente · CELE

Maqueta educativa con Arduino UNO, HC-SR04, sensor infrarrojo y uno o dos servos SG90. Incluye firmware autónomo, interfaz web en español y panel para Windows con acceso local o LAN.

![Interfaz del garaje](docs/interfaz-demo.jpg)

## Empezar en Windows

En **Releases**, descargar `GarajeInteligente_v1.0.2_Windows.zip`, extraer todo y abrir `PanelGaraje.exe`. Python y .NET vienen incluidos: el portable no requiere instalar sus dependencias.

Si el Arduino usa CH340 y no aparece su puerto COM, consultar `support/ch340/LEEME.md`. El release 1.0.2 incluye el instalador original del fabricante con firma verificada; nunca se ejecuta automáticamente.

Probar primero la demostración. Para montaje y calibración, leer [la guía](docs/MONTAJE_Y_CALIBRACION.md). Las pruebas físicas siguen pendientes; la posición mostrada es estimada y STOP conserva los pulsos, no corta la alimentación.

## Ejecutar desde fuentes

1. Instalar [Python 3.11 o posterior](https://www.python.org/downloads/windows/), con la opción **Add Python to PATH**.
2. Ejecutar `INSTALAR_DEPENDENCIAS.cmd`: crea `.venv` e instala `pyserial==3.5` con conexión a Internet.
3. Ejecutar `INICIAR_SERVIDOR.cmd`. Abre la interfaz en `http://127.0.0.1:8773`; cerrar con Ctrl+C.

El servidor por sí solo no necesita .NET. Para construir el panel y el portable, ver [instalación y compilación](docs/INSTALACION.md). El firmware usa Arduino UNO y la biblioteca Servo; su carga se hace explícitamente desde Arduino IDE.

## Documentación

- [Instalación, dependencias y solución de problemas](docs/INSTALACION.md)
- [Montaje y calibración](docs/MONTAJE_Y_CALIBRACION.md)
- [Acceso remoto opcional](docs/ACCESO_REMOTO.md)
- [Validación y límites de las pruebas](docs/VALIDACION.md)
- [Preparar GitHub y publicar un release](docs/PUBLICAR_GITHUB.md)
- [Notas del release 1.0.2](docs/RELEASE_1.0.2.md)
- [Investigación editable](investigacion/Guia_investigacion_garaje_Paraguay.docx) y [PDF](investigacion/Guia_investigacion_garaje_Paraguay.pdf)

La alimentación de los servos es externa y comparte GND con UNO. La compatibilidad de los SG90 concretos con cuatro AA directas está pendiente de verificación.

## Estructura y pruebas

`firmware/`: UNO; `interfaz_garaje/`: servidor y web; `panel_garaje/`: panel C#; `tests/`: pruebas; `scripts/` y `tools/`: instalación y empaquetado. `entregas/`, `.build/`, entornos virtuales y archivos privados quedan excluidos de Git.

```powershell
.venv/Scripts/python.exe -m unittest discover -s tests -p 'test_*.py'
node tests/test_servo_units.cjs
```

La licencia de publicación del código propio aún no está definida; no se concede una licencia de terceros en nombre de sus autores. Consultar [atribuciones](ATRIBUCIONES.md). Los componentes externos conservan sus términos respectivos.
