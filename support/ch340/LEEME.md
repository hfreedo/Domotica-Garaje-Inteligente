# Driver USB serie CH340 / CH341

Instalador original de **Nanjing Qinheng Microelectronics Co., Ltd. (WCH)**, descargado el 8 de octubre de 2026. La firma Authenticode resultó **Valid** en el equipo de preparación. No se ejecutó ni se instaló el controlador durante la preparación.

- Página oficial: https://www.wch-ic.com/downloads/CH341SER.EXE.html?type=en
- Descarga oficial: https://www.wch-ic.com/download/file?id=65
- Archivo: `CH341SER.EXE`, 799096 bytes.
- SHA256: `458c37bdafbe4ce3cd0baf728c232b4b765b36d7463956e9b94cdf099212cad1`

El fabricante indica compatibilidad con CH340/CH341 y Windows, incluidos Windows 10/11. Usar solo si el conversor USB de la placa corresponde a esa familia.

## Instalar en el equipo destino

1. Conectar la placa con un cable USB de datos y revisar **Administrador de dispositivos → Puertos (COM y LPT)**. Si ya aparece y funciona, no hace falta reinstalar.
2. Si falta el controlador, abrir `CH341SER.EXE` de esta carpeta. Comprobar que el editor es WCH/Nanjing Qinheng; Windows puede solicitar permiso de administrador.
3. En el instalador del fabricante, pulsar **INSTALL**. Desconectar y volver a conectar la placa, y reiniciar solo si el instalador lo pide.
4. Confirmar el número COM y seleccionarlo en el panel del garaje. Cerrar el monitor serie de Arduino antes de conectarse.

En el repositorio solo se incluyen estas instrucciones y el manifiesto. Para obtener el mismo binario, ejecutar `powershell -NoProfile -ExecutionPolicy Bypass -File scripts/descargar_ch340.ps1` desde la raíz. El script verifica hash y firma, sin instalarlo. Si WCH cambia el archivo, se detiene y requiere revisar la nueva versión.

El driver conserva los términos de su fabricante; no está relicenciado como parte del proyecto. La verificación de firma y hash no sustituye la prueba de funcionamiento en el equipo destino.
