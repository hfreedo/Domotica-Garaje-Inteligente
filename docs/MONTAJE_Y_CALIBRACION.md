# Montaje y puesta en marcha

## Selector de unidades de la interfaz 1.0.1

En mantenimiento, «Unidad de calibración» permite editar los cuatro extremos y la posición de prueba en grados nominales o microsegundos. La equivalencia es lineal: 0° = 700 µs, 45° = 1100 µs, 90° = 1500 µs, 135° = 1900 µs y 180° = 2300 µs. Se muestran también los valores equivalentes. Los grados representan una consigna, no el ángulo medido del eje ni de la puerta; no garantizan un recorrido físico de 180°.

Cambiar de unidad no mueve los motores ni altera el pulso exacto existente. Al escribir grados se redondea al microsegundo más cercano. RAM y EEPROM siguen guardando microsegundos; no hace falta volver a cargar el sketch. El selector es una opción de visualización de cada página y vuelve a microsegundos al recargar. La separación mínima entre extremos sigue siendo 100 µs, equivalente a 11,25° nominales. Guarda después de aplicar para conservar cambios reales.

## Alimentación

UNO por USB de la computadora. HC-SR04 e infrarrojo de 5 V del UNO. Servos desde un circuito separado:

```text
4 × AA de 1,5 V → interruptor → regulador 5 V → rojo de servo(s)
negativo pilas ─────────────── GND regulador → marrón/negro servo(s)
                                          └→ GND Arduino
PC USB → UNO → 5 V → sensores
             → señales → sensores y servo(s)
```

Utiliza un regulador **buck-boost** si quieres mantener 5 V conforme baja la tensión de las pilas. Como punto de partida, un módulo de 5 V/3 A cuya corriente sea realmente continua; hay que verificar su capacidad, calentamiento y caída de tensión con estos servos. Cuatro AA y un módulo barato no garantizan esa corriente. Ajusta y mide los 5 V antes de conectar motores. No uses el pequeño regulador del UNO para alimentar los servos.

Un condensador de 470–1000 µF, de al menos 10 V, cerca de los servos puede ayudar frente a transitorios; respeta su polaridad. No sustituye una fuente adecuada. Usa conexiones firmes y una línea de alimentación suficientemente corta. El positivo de la alimentación de los servos **no se une al 5 V del UNO**. Sí se unen las tierras.

Si hay zumbido persistente, bloqueo, calentamiento o reinicios, corta la alimentación de servos y revisa mecánica y suministro. Un contrapeso o una puerta más ligera suele resolver más que añadir un segundo SG90. Dos servos enfrentados no garantizan duplicar el par útil.

## Pines iniciales

### Distribución desde el bloque POWER

Usa un pin **GND del bloque POWER** del UNO como referencia de tierra y conéctalo a una bornera o línea común GND. A esa misma línea conecta GND del HC-SR04, GND del infrarrojo, marrón/negro de cada servo y negativo de la salida de la alimentación externa. Dibuja las ramas en paralelo; no uses el GND situado junto a AREF para este esquema. Los GND del UNO son eléctricamente comunes, pero esta elección facilita seguir el dibujo.

Conecta el pin **5V del bloque POWER** a otra línea para VCC del HC-SR04 y VCC del infrarrojo. Los rojos de los servos van exclusivamente al positivo de la alimentación externa adecuada, nunca a esta línea 5V del UNO. No utilices VIN, 3.3V ni IOREF como sustitutos.

La línea común permite distribuir los cables sin introducir varios conductores en un solo pin. Une el retorno de los servos directamente con el negativo de su fuente en esa distribución: su corriente no debe atravesar la placa Arduino para volver a la fuente.

| Elemento | Conexión UNO |
|---|---|
| HC-SR04 VCC / GND | Línea 5V / línea GND desde POWER |
| HC-SR04 TRIG | D4 |
| HC-SR04 ECHO | D2 |
| Infrarrojo VCC / GND | Línea 5V / línea GND desde POWER, si admite 5 V |
| Infrarrojo OUT digital | D7 |
| Señal servo 1 | D9 |
| Señal servo 2 opcional | D10 |
| Rojo de servos | 5 V regulados externos |
| Marrón/negro de servos | GND común |

Confirma los rótulos reales de cada módulo; el orden de pines puede variar. El IR suele detectar con nivel LOW; puede cambiarse a HIGH en la interfaz. Su potenciómetro ajusta el umbral óptico; su salida digital no mide centímetros.

Los pines configurables válidos son D2–D12 y A0–A5 (números 14–19). D0/D1 se reservan para serie y D13 para evitar conflictos con el LED de placa. No pueden repetirse. El pin de servo 2 queda reservado aunque uses un solo servo. Servo utiliza Timer1; no agregues funciones PWM en D9/D10 sin revisar esa interacción.

## Sensores y geometría

Coloca el ultrasónico para ver el vehículo y no la propia hoja durante su recorrido. Con el acceso vacío debe haber un **eco válido de fondo** más allá de «máxima + margen». Si apunta al vacío y no recibe eco, el cierre queda inhibido intencionalmente. Cambia su orientación o coloca un fondo dentro del alcance útil; no desactives esa protección para ocultar lecturas inválidas.

El IR observa la entrada al interior. Una presencia se recuerda durante el ciclo; puede seguir activo con el vehículo estacionado y no impide cerrar. Por esa misma razón **no protege el umbral**: si quisieras detectar atrapamientos tendrías que añadir una barrera independiente en el paso.

## Calibrar uno o dos servos

1. Deja el eje desacoplado y sostiene la puerta. Alimenta UNO, carga el sketch y conecta la interfaz al COM correcto.
2. Entra en mantenimiento. Selecciona 1 o 2 servos y pulsa **Aplicar en RAM**. No cambia el cableado físico.
3. Prueba cada servo por separado, sin carga: comienza con 1500 µs y avanza en pasos pequeños, por ejemplo 20 µs. El primer pulso puede causar un salto. No busques los topes internos.
4. Determina qué pulso corresponde al cierre y a la apertura con tu montaje. Los 1100/1900 µs iniciales son ejemplos, no una calibración de tus motores. El firmware admite 700–2300 µs; eso no significa que cada SG90 tolere todos esos valores.
5. Para motores enfrentados, normalmente uno sube y el otro baja su pulso: por ejemplo servo 1 cerrado 1100/abierto 1900; servo 2 cerrado 1900/abierto 1100. Ajusta individualmente; no basta asumir que ambos son idénticos.
6. Escribe los cuatro extremos, marca «He calibrado…», aplica en RAM y guarda en EEPROM. Para dos servos, prueba también puntos intermedios desacoplados. Si sus trayectorias no coinciden, corrige mecánica o usa un solo actuador con mayor par.
7. Quita la alimentación antes de fijar brazos y eje; sitúalos en posiciones mecánicas compatibles. Reconecta con la zona libre. Sal de mantenimiento y usa **Reanudar y abrir**. Desde posición desconocida la primera orden es directamente al extremo abierto; el desplazamiento inicial no se puede suavizar con certeza sin realimentación.
8. Prueba ciclos completos sin vehículo y después con vehículo de juguete. No debe haber esfuerzo contra topes ni zumbido sostenido. El tiempo de recorrido regula la progresión de las órdenes, no garantiza velocidad física bajo carga.
9. Solo después de estas pruebas activa arranque autónomo y guarda, si lo deseas. Al reiniciar ordenará abrir tras aproximadamente 3 s. Una pausa persistida lo inhibe hasta **Reanudar y abrir**.

Para cambiar pines: entra en mantenimiento, desconecta la alimentación de servos, aplica y guarda los nuevos pines, corta alimentación del sistema y recablea. Comprueba todo antes de energizar. La aplicación no puede saber si el cableado coincide con el formulario.

## Ciclo y ajustes iniciales

Rango 3–25 cm durante 300 ms → apertura. Exterior libre requiere distancia válida mayor que 30 cm durante 1500 ms. Una lectura por debajo del mínimo también bloquea cierre, aunque no active apertura inicial.

Con puerta abierta y exterior libre confirmado: espera 5 s si hubo detección interior; 10 s si no la hubo. Estos plazos se reinician cuando deja de estar libre el exterior. Durante cierre, cualquier lectura no libre (incluida inválida) ordena reabrir. Los tiempos son configurables. «Solicitar cierre» solo se acepta estando abierta y con exterior libre; no fuerza el cierre sobre una detección.

Para salir, abre desde interfaz/panel de acceso. El IR interior no autoriza apertura por sí solo. Si el vehículo sigue estacionado frente al IR, eso cuenta como presencia en el siguiente ciclo; no se distingue la identidad ni la dirección de un vehículo.

## Memoria y reinicios

- **Formulario:** cambios todavía sin enviar.
- **RAM del UNO:** Aplicar; se pierde al reiniciar si no se guarda.
- **EEPROM:** Guardar conserva configuración validada con versión y CRC. Recuperar carga lo guardado. Restablecer aplica valores iniciales en RAM; no borra EEPROM hasta Guardar.
- **Pausa:** se escribe por separado al pausar/entrar en mantenimiento y se libera al reanudar. No se escribe en cada medición.
- **Demo:** todo es memoria de la computadora. Guardar en demo no escribe un UNO ni prueba la EEPROM real.

Cerrar navegador o detener servidor no apaga el automatismo del UNO. Retirar USB sí corta su alimentación, aunque las pilas de los servos continúen conectadas. Abrir/cerrar el puerto USB puede reiniciar algunas placas UNO: con arranque autónomo habilitado puede producir apertura. Prueba ese comportamiento antes del montaje final.
