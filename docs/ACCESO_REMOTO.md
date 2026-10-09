# Acceso local, LAN y ngrok

Para empezar desde el portable y conectar el celular paso a paso, consulta [GUIA_PANEL_Y_CELULAR.md](GUIA_PANEL_Y_CELULAR.md), con capturas del panel. Abre primero `PanelGaraje.exe`: el panel inicia el servidor y **Abrir local** abre la interfaz donde conectas el COM.

El panel Windows inicia y detiene el servidor. La interfaz web es la misma en PC y móvil, con los mismos controles. El servidor mantiene una única conexión serie al UNO; no abras simultáneamente el monitor serie del IDE.

## En la computadora

Iniciar local → Abrir local. Puerto predeterminado 8773. El acceso directo por localhost está permitido sin PIN. Desde otro equipo o a través de un proxy se exige PIN. Un puerto ocupado genera error: el panel no adopta otro servidor existente.

## En la misma red

1. Detén servidor si estaba en modo local.
2. Inicia LAN / Hotspot. El panel escucha en las interfaces de red disponibles; no crea la zona móvil de Windows.
3. Usa la IP del adaptador de tu red privada. PC y teléfono deben estar comunicados, sin aislamiento de clientes Wi-Fi.
4. Escanea QR LAN e introduce el PIN del panel. QR contiene URL, no PIN.

La LAN usa HTTP: utilízala en una red de confianza. No abras puertos en el router. Si cambias de red o PIN, reinicia el servidor para invalidar sesiones anteriores.

## Por Internet con ngrok

1. Descarga ngrok para Windows desde https://ngrok.com/download/windows y coloca `ngrok.exe` en `support/ngrok/` junto al portable.
2. Obtén tu authtoken en tu propia cuenta. Usa el campo protegido del panel y «Guardar authtoken». Esto ejecuta la configuración oficial de ngrok y guarda la credencial en el perfil del usuario mediante ngrok; no se incluye en el proyecto.
3. Inicia el servidor y pulsa iniciar ngrok. El panel obtiene la URL pública HTTPS de la API local de ngrok y genera su QR.
4. Abre esa URL en el teléfono e introduce el PIN. Cualquier persona con URL y PIN tendrá acceso a todas las funciones, incluida calibración.
5. Detén ngrok cuando termines. Detener servidor también detiene el túnel iniciado por este panel. No administra otros procesos ngrok ajenos.

No se ha configurado una cuenta ni publicado un túnel en la entrega. Disponibilidad, límites y posibles avisos de ngrok dependen de su servicio y tu cuenta. Mantén computadora, USB y servidor activos para acceso remoto.

## Comportamiento de acceso

PIN aleatorio de ocho cifras por panel; solo se regenera con servidor detenido. Sesión con cookie HttpOnly, SameSite=Strict, duración máxima de ocho horas; Secure cuando el proxy informa HTTPS. Se limitan intentos de PIN. El servidor no permite CORS y exige cabecera propia para cambios.

No existe rol de solo lectura: comparte el PIN únicamente con quienes puedan accionar la maqueta. La respuesta del firmware confirma que aceptó una orden; no demuestra movimiento físico. Si se pierde conexión, la interfaz deja de presentar la telemetría como actual.
