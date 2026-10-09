# Garaje Inteligente 1.0.2

Entrega para Windows x64 y código fuente preparado para GitHub.

- Portable con servidor y panel autocontenidos.
- Driver CH340/CH341 oficial con procedencia, hash e instalación manual.
- Instalador de dependencias del servidor en entorno virtual y lanzador Windows.
- Scripts de compilación y empaquetado con rutas relativas.
- Documentación de instalación, publicación y verificaciones SHA256.
- Panel actualizado para utilizar el entorno virtual local al iniciar el servidor desde fuentes.

Validado localmente: 19 pruebas del servidor, 1601 conversiones de calibración, construcción del portable, ejecución desde ZIP limpio e instalación/arranque desde una extracción limpia de fuentes. El driver tiene firma Authenticode válida de WCH; no se instaló durante estas pruebas.

Extraer todo el ZIP Windows y abrir `PanelGaraje.exe`. Para ejecutar desde fuentes, instalar Python y usar `INSTALAR_DEPENDENCIAS.cmd` seguido de `INICIAR_SERVIDOR.cmd`.

Se conserva el selector de grados nominales/microsegundos. El firmware continúa en `GARAGE-1.0.0`; esta entrega no cambia la lógica ni el formato de EEPROM.

Ver `docs/VALIDACION.md` para las pruebas realizadas. Pendientes: pruebas físicas, instalación del driver en el equipo destino, revisión completa del panel nativo, teléfono real y HP Windows 10. La aplicación no está firmada digitalmente. No contiene ngrok ni credenciales.
