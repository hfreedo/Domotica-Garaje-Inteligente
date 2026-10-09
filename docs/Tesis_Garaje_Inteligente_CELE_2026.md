# REPÚBLICA DEL PARAGUAY
## MINISTERIO DE EDUCACIÓN Y CIENCIAS (MEC)
### CENTRO EDUCATIVO LA ESPERANZA (CELE)
#### BACHILLERATO TÉCNICO EN INFORMÁTICA (BTI)

---

# GARAJE INTELIGENTE UNO: PROTOTIPO DOMÓTICO ESCOLAR DE CONTROL DE ACCESO VEHICULAR CON SENSADO ULTRASONICO, REGISTRO INFRARROJO Y GESTIÓN LOCAL MÓVIL EN ARDUINO UNO

### Informe de Investigación Tecnológica y Memoria Técnica del Proyecto

**Modalidad:** Robótica Científica y Aplicaciones Embebidas  
**Evento:** Feria de Ciencias y Tecnología SERCAP 2026  
**Curso:** 3.º Curso de la Educación Media Técnica  
**Énfasis:** Bachillerato Técnico en Informática  
**Institución:** Centro Educativo La Esperanza  
**Lugar y Fecha:** Luque / Asunción, Paraguay — Octubre de 2026  

---

## HOJA DE DICTAMEN DE EVALUACIÓN

El jurado examinador de la **Feria de Ciencias y Tecnología SERCAP 2026**, tras analizar el presente informe de investigación técnica, la carpeta de campo, el prototipo mecatrónico a escala y la defensa realizada por los estudiantes del 3.º curso del Bachillerato Técnico en Informática del Centro Educativo La Esperanza, resuelve otorgar la calificación de:

**CALIFICACIÓN FINAL:** ______________________________  
**PUNTAJE OBTENIDO:** ________ / 100 puntos

**Observaciones del Tribunal Evaluador:**  
_________________________________________________________________________________  
_________________________________________________________________________________  
_________________________________________________________________________________  

| _________________________________________ | _________________________________________ |
| :---: | :---: |
| **Firma del Evaluador 1** | **Firma del Evaluador 2** |
| Especialidad Electrónica / Informática | Metodología de la Investigación |

| _________________________________________ |
| :---: |
| **Firma del Coordinador de Feria SERCAP** |
| Coordinación General de Feria de Ciencias y Tecnología |

---

## DEDICATORIA

*A nuestras familias, que nos apoyaron con paciencia infinita durante las largas tardes y fines de semana en el taller escolar, financiando la compra de los componentes, tolerando el desorden de cables y herramientas en nuestras casas y alentándonos en cada momento en que el prototipo parecía no responder.*

*A nuestros compañeros del Bachillerato Técnico en Informática, con quienes compartimos el aprendizaje diario de la programación y el entusiasmo de convertir líneas de código en movimientos reales en el mundo físico.*

---

## AGRADECIMIENTOS

Al **Centro Educativo La Esperanza (CELE)**, a su directiva y cuerpo docente, por brindarnos las instalaciones de laboratorio, los instrumentos de medición y el espacio pedagógico para experimentar con microcontroladores y electrónica aplicada.

A nuestro **Docente Tutor y Orientador de Taller**, por guiarnos con paciencia cuando los motores reseteaban el Arduino, por exigirnos rigor metrológico en lugar de simples demostraciones que funcionaran «por casualidad», y por enseñarnos que en la ciencia escolar un fallo bien documentado y comprendido vale mucho más que una simulación perfecta sin base física.

A los docentes del área técnica de informática, por inculcarnos la disciplina del control de versiones, la modularización de código y el valor del software de código abierto en la solución de problemas comunitarios.

---

## RESUMEN EJECUTIVO

El presente proyecto de investigación aplicada y desarrollo tecnológico presenta el diseño, montaje, calibración y evaluación experimental de un prototipo mecatrónico de garaje residencial automatizado a escala, denominado **Garaje Inteligente UNO**. El sistema aborda la problemática cotidiana de inseguridad y congestionamiento en accesos vehiculares de zonas residenciales de Asunción y el Departamento Central, donde el descenso del conductor para la apertura de portones manuales incrementa el riesgo de hechos delictivos y bloquea calzadas angostas. Frente a los costosos sistemas industriales comerciales y a la inestabilidad de la conectividad a internet en el ámbito escolar y barrial, se desarrolló una solución híbrida de bajo costo basada en el microcontrolador **Arduino UNO R3** (ATmega328P). 

El prototipo integra un sensor ultrasónico **HC-SR04** para la detección frontal exterior en el rango calibrado de 3 a 25 cm (con confirmación temporal de 300 ms e histéresis de seguridad de 5 cm), un sensor óptico infrarrojo reflectivo interior para registrar el ingreso efectivo del vehículo, y hasta dos servomotores **Tower Pro SG90** para la apertura de la compuerta basculante. La seguridad se garantiza mediante una estricta regla de inhibición de cierre ante lecturas inválidas o falta de eco, retención del estado de pausa en la memoria EEPROM y desacoplamiento de fuentes mediante un paquete de 4 pilas AA con regulador conmutado de 5 V independiente para los actuadores. Opcionalmente, una computadora intermedia ejecuta un servidor local liviano en Python y una interfaz web adaptable al celular con autenticación por PIN y código QR sobre una red Wi-Fi local sin requerir internet.

La metodología experimental consistió en una batería de 40 ensayos aleatorizados a cuatro distancias de referencia (5, 15, 20 y 35 cm) y 6 escenarios de pruebas de contingencia física. Los resultados demostraron una efectividad de apertura del 100% dentro del rango útil (30/30 aciertos en un tiempo promedio de 0,58 s) e inmunidad absoluta a falsos positivos a 35 cm (0/10 aperturas), validando la hipótesis operativa de trabajo. Se concluye que el diseño propuesto demuestra viabilidad funcional, solidez metrológica y accesibilidad económica (costo total de 248.000 PYG), constituyendo una propuesta educativa y tecnológica transferible a la realidad paraguaya.

**Palabras clave:** Domótica escolar, Arduino UNO, Sensor HC-SR04, Control de acceso, Red local sin internet.

---

## ABSTRACT

This applied research and engineering project presents the design, assembly, calibration, and experimental evaluation of a scale-model automated residential garage prototype, named **Garaje Inteligente UNO**. The system addresses the common safety hazards and traffic congestion in residential vehicle accesses within the Greater Asunción and Central Department areas, where drivers stepping out to manually open gates are exposed to street robbery and obstruct narrow roadways. Facing prohibitive commercial automation costs and intermittent internet connectivity in local neighborhoods and schools, an affordable hybrid solution was developed around the **Arduino UNO R3** (ATmega328P) microcontroller.

The prototype incorporates an **HC-SR04** ultrasonic sensor for exterior front detection within a calibrated range of 3 to 25 cm (featuring a 300 ms confirmation window and a 5 cm safety hysteresis), an interior reflective infrared sensor to verify vehicle parking, and up to two **Tower Pro SG90** servomotors driving an overhead tilt door. Operational safety is enforced through strict firmware rules: closing is prohibited upon invalid acoustic readings or loss of echo, manual pause states are persisted in EEPROM, and power rails are decoupled using 4 AA batteries and an independent 5V switching regulator with a shared ground reference. Optionally, a host PC runs a lightweight Python server and a mobile-friendly web UI authenticated via PIN and QR codes over an offline local LAN or hotspot without cloud dependencies.

The experimental methodology comprised 40 randomized bench trials across four reference distances (5, 15, 20, and 35 cm) alongside 6 physical contingency stress scenarios. The results demonstrated a 100% opening success rate within the programmed detection zone (30/30 hits with an average latency of 0.58 s) and complete immunity to false positives outside the boundary at 35 cm (0/10 openings), fully supporting the operational hypothesis. It is concluded that the proposed design offers verifiable functional reliability, metrological soundness, and financial accessibility (total bill of materials of 248,000 PYG), providing a viable educational and technological model tailored to the Paraguayan context.

**Keywords:** Educational home automation, Arduino UNO, HC-SR04 sensor, Access control, Offline local network.

---

## ÍNDICE GENERAL

* **Preliminares**
  * Hoja de Dictamen de Evaluación
  * Dedicatoria y Agradecimientos
  * Resumen Ejecutivo
  * Abstract
* **Capítulo I: El Problema y los Objetivos**
  * 1.1 Planteamiento y contextualización del problema en Gran Asunción
  * 1.2 Pregunta principal de investigación y subpreguntas técnicas
  * 1.3 Justificación socio-técnica y educativa en el BTI
  * 1.4 Delimitación y alcance del proyecto escolar
  * 1.5 Objetivo general
  * 1.6 Objetivos específicos
  * 1.7 Hipótesis operativa de trabajo
* **Capítulo II: Marco Teórico y Referencial**
  * 2.1 Antecedentes nacionales y antecedentes de robótica escolar en Paraguay
  * 2.2 Diagnóstico de conectividad y justificación del diseño fuera de la nube (INE 2025)
  * 2.3 Fundamentos del sensado ultrasónico de distancia y mitigación de errores
  * 2.4 Detección óptica por reflexión infrarroja y límites de discriminación
  * 2.5 Servomotores de modelismo y análisis físico del momento de carga
  * 2.6 Microcontrolador ATmega328P, gestión de memoria no volátil y desmitificación de la IA
  * 2.7 Marco normativo y seguridad eléctrica (Norma Paraguaya NP 2 028 96 y régimen MBTS)
* **Capítulo III: Marco Metodológico y Diseño Experimental**
  * 3.1 Enfoque, tipo y diseño de la investigación
  * 3.2 Población, muestra y unidad experimental de análisis
  * 3.3 Operacionalización de variables
  * 3.4 Batería experimental de 40 ensayos aleatorizados
  * 3.5 Protocolo de pruebas funcionales de contingencia (6 escenarios de taller)
  * 3.6 Instrumentos de medición y recolección de datos
  * 3.7 Presupuesto analítico de materiales e insumos en Guaraníes (PYG)
* **Capítulo IV: Desarrollo Tecnológico, Arquitectura y Montaje**
  * 4.1 Arquitectura electromecánica general del prototipo
  * 4.2 Esquema de conexionado y asignación de pines
  * 4.3 Desacoplamiento eléctrico de fuentes y filtrado capacitivo
  * 4.4 Lógica del firmware embebido y Máquina de Estados Finitos
  * 4.5 Reglas de confirmación temporal, histéresis y protección ante falta de eco
  * 4.6 Ecosistema de control complementario: servidor Python e interfaz web móvil por QR
  * 4.7 Procedimiento de calibración angular y sincronización de servomotores
* **Capítulo V: Resultados Experimentales, Discusión y Conclusiones**
  * 5.1 Resultados cuantitativos de la batería de 40 ensayos de distancia
  * 5.2 Análisis de latencia temporal y tiempos de respuesta física
  * 5.3 Resultados de los 6 escenarios de contingencia y seguridad
  * 5.4 Evaluación de persistencia en EEPROM y autonomía sin servidor
  * 5.5 Contrastación formal de la hipótesis de investigación
  * 5.6 Discusión crítica y limitaciones honestas de la maqueta
  * 5.7 Proyecciones y propuestas de escalamiento
  * 5.8 Conclusiones generales
  * 5.9 Recomendaciones pedagógicas y técnicas
* **Bloque de Carpeta de Campo**
  * 1. Cronograma de actividades escolares (Diagrama de Gantt)
  * 2. Bitácora de taller y registro vivencial de incidentes técnicos (12 hitos)
  * 3. Ficha de laboratorio del ensayo unitario representativo
  * 4. Lista de chequeo (Checklist) para stand de feria
* **Referencias Bibliográficas**
* **Anexos Técnicos**
  * Anexo A: Diagrama de flujo de la Máquina de Estados Finitos
  * Anexo B: Tabla de correspondencia entre microsegundos y grados nominales
  * Anexo C: Código fuente íntegro de la lógica de control (`control.h`)

---

## ÍNDICE DE TABLAS Y FIGURAS

* **Tabla 1:** Asignación de pines de control en la placa Arduino UNO R3
* **Tabla 2:** Presupuesto analítico del prototipo en moneda local (Guaraníes - PYG)
* **Tabla 3:** Matriz de operacionalización de variables de investigación
* **Tabla 4:** Resultados de la batería de 40 ensayos de detección por distancia
* **Tabla 5:** Resumen estadístico de rendimiento, latencia y efectividad por distancia
* **Tabla 6:** Resultados de la batería de 6 pruebas funcionales de contingencia
* **Tabla 7:** Matriz de correspondencia entre calibración en microsegundos y grados nominales
* **Tabla 8:** Cronograma general de actividades del proyecto escolar (Abril - Octubre 2026)
* **Tabla 9:** Bitácora resumida de taller con incidentes y decisiones técnicas adoptadas
* **Tabla 10:** Ficha de ensayo de laboratorio representativo (Ensayo N.º 24)
* **Figura 1:** Esquema conceptual de desacoplamiento de fuentes y masas comunes
* **Figura 2:** Diagrama de bloques de la arquitectura general del sistema
* **Figura 3:** Diagrama de estados de la Máquina de Estados Finitos embebida

---

---

# CAPÍTULO I: EL PROBLEMA Y LOS OBJETIVOS

## 1.1 Planteamiento y contextualización del problema en Gran Asunción

En las ciudades del Departamento Central y los barrios residenciales de Asunción (tales como Luque, San Lorenzo, Fernando de la Mora, Capiatá y Lambaré), el acceso vehicular a las viviendas unifamiliares continúa dependiendo mayoritariamente de portones manuales de rejas metálicas o madera. Esta situación cotidiana plantea dos problemáticas críticas que afectan de manera directa la seguridad y la convivencia comunitaria:

1. **Vulnerabilidad ante la delincuencia urbana:** Cuando un conductor arriba a su domicilio durante horas de la noche o en condiciones climáticas adversas, se ve obligado a detener el automóvil en la vía pública, descender con el motor encendido o la llave en mano, abrir el candado o pasador, empujar las hojas del portón, ingresar el vehículo y volver a salir para cerrar. Este lapso de exposición, que suele oscilar entre 45 y 90 segundos, es aprovechado con frecuencia por asaltantes al paso (*motochorros* o *tortoleros*), generando situaciones de alto riesgo para la integridad física de las familias.
2. **Congestionamiento en arterias residenciales angostas:** La morfología urbana de muchos barrios paraguayos se caracteriza por calles estrechas de un solo carril de circulación efectiva, muchas de ellas empedradas o asfaltadas sin banquina. Detener un vehículo en doble fila mientras el conductor realiza la apertura manual del portón genera cuellos de botella instantáneos, bocinazos, maniobras imprudentes de adelantamiento y peligro constante de colisiones por alcance.

Frente a esta realidad, la alternativa que ofrece el mercado local consiste en la instalación de motores electromecánicos comerciales (de marcas reconocidas en plaza como Rossi, Peccinin o PPA). Sin embargo, un kit de automatización instalado, con cremalleras o brazos telescópicos y mandos a distancia por radiofrecuencia (RF), representa una inversión que supera holgadamente los 1.800.000 a 2.500.000 Guaraníes (PYG). Este costo resulta prohibitivo para familias de ingresos medios y trabajadores, así como para inquilinos que no pueden realizar modificaciones estructurales permanentes en las viviendas.

Por otra parte, las soluciones modernas basadas en Internet de las Cosas (IoT) y aplicaciones móviles comerciales sufren en Paraguay de un obstáculo determinante: la extrema volatilidad de la conectividad a internet. Aunque una gran parte de la población cuenta con planes de datos en telefonía celular, las redes Wi-Fi residenciales experimentan interrupciones frecuentes por cortes del tendido eléctrico o microcortes de servicio. Un portón que dependa obligatoriamente de la «nube» o de servidores remotos para abrirse queda completamente inoperativo ante la caída del proveedor de internet, dejando al usuario bloqueado fuera de su hogar.

En el ámbito formativo del **Bachillerato Técnico en Informática (BTI)** del **Centro Educativo La Esperanza**, surge la necesidad pedagógica y tecnológica de no limitarse a estudiar conceptos abstractos de software en un pizarrón o simulador de computadora. Los estudiantes de educación media técnica deben ser capaces de enfrentarse al mundo real del hardware, investigando cómo componentes electrónicos accesibles en el mercado de componentes paraguayo pueden integrarse en un sistema mecatrónico autónomo, confiable, seguro y económicamente viable.

## 1.2 Pregunta principal de investigación y subpreguntas técnicas

Con el propósito de orientar la investigación bajo parámetros verificables de ingeniería escolar, se formuló la siguiente **pregunta principal de investigación**:

> *¿Con qué efectividad porcentual y en qué tiempo de respuesta inicia la apertura un prototipo a escala de garaje automatizado basado en Arduino UNO al posicionar un vehículo a distancias de 5, 15, 20 y 35 cm de un sensor ultrasónico HC-SR04, y cómo responde el sistema ante contingencias de seguridad de cierre bajo condiciones controladas de taller escolar?*

De dicha interrogante se derivan las siguientes **subpreguntas técnicas específicas**:
* ¿Cuál es la proporción de aperturas exitosas dentro de un plazo de 2 segundos en cada una de las distancias de prueba establecidas?
* ¿Cuál es la latencia física real medida desde el instante en que el vehículo queda en posición estable hasta el inicio visible del movimiento mecánico del portón?
* ¿De qué manera reacciona la lógica del microcontrolador cuando un objeto ingresa al garaje frente al sensor infrarrojo, en comparación a una aproximación exterior que es abandonada?
* ¿Cómo responde el automatismo ante la pérdida súbita de eco ultrasónico o la detección de un obstáculo durante la maniobra de descenso de la puerta?
* ¿Es capaz el prototipo de retener de forma permanente su calibración y su estado de seguridad ante cortes de energía eléctrica mediante el uso de la memoria EEPROM?

## 1.3 Justificación socio-técnica y educativa en el BTI

El desarrollo del proyecto **Garaje Inteligente UNO** se justifica desde tres dimensiones fundamentales:

* **Dimensión Social y Comunitaria:** La automatización de accesos vehiculares no debe considerarse un artículo de lujo, sino una medida preventiva de seguridad ciudadana y de ordenamiento urbano. Desarrollar y documentar un prototipo de bajo costo demuestra a la comunidad educativa que la tecnología es accesible y que pueden idearse soluciones locales adaptadas a las limitaciones de infraestructura del país.
* **Dimensión Técnica y de Ingeniería Embebida:** El proyecto exige una integración rigurosa entre software y hardware. Obliga a los estudiantes a abandonar la programación lineal y bloqueante (el uso ingenuo de `delay()`) para adoptar arquitecturas basadas en **Máquinas de Estados Finitos (FSM)** gobernadas por temporizadores de milisegundos (`millis()`), desacoplamiento galvánico de cargas inductivas, control de transitorios eléctricos mediante capacitores de filtrado y comunicación serie asíncrona bidireccional.
* **Dimensión Pedagógica y Metodológica:** En el marco de la feria **SERCAP 2026**, este trabajo busca transformar la tradicional «maqueta escolar que solo se enciende para la foto» en un verdadero ejercicio de investigación aplicada. Se promueve el método científico experimental, el registro honesto de fallos y la recopilación de datos numéricos reproducibles mediante instrumentos de taller.

## 1.4 Delimitación y alcance del proyecto escolar

Para evitar falsas expectativas y delimitar con absoluta honestidad el carácter del trabajo, se establecen los siguientes límites:

1. **Nivel de maqueta a escala:** El prototipo físico está construido a escala reducida utilizando una estructura de madera terciada/MDF y una compuerta basculante liviana accionada por servomotores plásticos de modelismo Tower Pro SG90. *No se trata de un portón industrial a tamaño real ni de un producto comercial homologado.*
2. **Entorno de ensayo:** Las mediciones se realizaron en el banco de trabajo del laboratorio del colegio, en condiciones de interior, sobre una superficie plana, utilizando un vehículo de juguete estandarizado de 18 cm de longitud.
3. **Carácter de la detección:** El sensor ultrasónico mide distancia lineal frontal por tiempo de rebote acústico; *no discrimina la marca, modelo o matrícula del automóvil*. Asimismo, el sensor infrarrojo interior detecta reflectancia óptica simple y no cuenta con haz matricial para certificar el umbral contra atrapamientos corporales.
4. **Independencia de la nube:** La arquitectura se diseñó para operar de forma 100% autónoma en el microcontrolador. La computadora y el teléfono móvil actúan exclusivamente como herramientas de supervisión y apertura manual a través de una red local cerrada (LAN/Hotspot), sin conexión a servidores de internet externos.

## 1.5 Objetivo general

Diseñar, construir, calibrar y evaluar experimentalmente un prototipo mecatrónico a escala de garaje automatizado basado en Arduino UNO R3, utilizando sensado ultrasónico exterior, confirmación infrarroja interior, actuación servomecánica y control móvil en red local, caracterizando su tiempo de respuesta y comportamiento ante fallos para su presentación en la feria SERCAP 2026.

## 1.6 Objetivos específicos

1. **Diseñar e integrar el circuito electromecánico del prototipo**, implementando un desacoplamiento estricto entre la alimentación lógica del microcontrolador (5 V USB) y la alimentación de potencia de los servomotores (fuente externa regulada con GND común).
2. **Desarrollar el firmware embebido en lenguaje C++ para Arduino UNO**, estructurado sobre una Máquina de Estados Finitos no bloqueante con filtros temporales de confirmación, histéresis de distancia y rutinas de salvaguarda ante lecturas inválidas.
3. **Implementar un sistema de supervisión y gestión local multiplataforma**, compuesto por un servidor en Python y una interfaz web adaptable accesible desde teléfonos móviles mediante código QR y PIN de seguridad sobre la red local.
4. **Evaluar experimentalmente el desempeño de la detección exterior mediante una batería de 40 ensayos aleatorizados**, comparando la tasa de apertura exitosa y la latencia física a distancias de 5, 15, 20 y 35 cm.
5. **Comprobar la robustez del sistema ante seis escenarios de contingencia de taller**, verificando la reapertura por obstáculos, la inhibición por falta de eco y la persistencia de configuraciones en la memoria EEPROM tras la pérdida de alimentación.

## 1.7 Hipótesis operativa de trabajo

Para contrastar el comportamiento del sistema mediante pruebas estadísticas descriptivas, se formuló la siguiente hipótesis operativa previa a la recolección de datos:

> **Hipótesis ($H_1$):** *Al posicionar de forma estable un vehículo de prueba frente al sensor ultrasónico HC-SR04, el sistema iniciará físicamente la apertura del portón en un tiempo menor o igual a 2,0 segundos en al menos 9 de cada 10 intentos (efectividad $≥ 90%$) en las distancias interiores al rango programado (5, 15 y 20 cm), y registrará 0 aperturas en 10 intentos (0% de falsos positivos) a la distancia exterior de control de 35 cm durante una ventana de observación de 5,0 segundos.*

---

---

# CAPÍTULO II: MARCO TEÓRICO Y REFERENCIAL

## 2.1 Antecedentes nacionales y antecedentes de robótica escolar en Paraguay

La incorporación de sistemas de control electrónico y robótica en el ámbito paraguayo cuenta con antecedentes valiosos que sustentan el enfoque adoptado en esta investigación:

* **Antecedente Técnico Nacional (Burgos Delvalle & Estigarribia Barreto, 2020):** Investigadores de la Universidad Nacional de Caaguazú (UNAE / FACAT) documentaron en su artículo *«Domótica de bajo coste controlada por comandos de voz»* el desarrollo de una vivienda a escala automatizada con Arduino UNO y enlace Bluetooth HC-06. En sus hallazgos, los autores reportaron que el reconocimiento de voz por celular presentaba fallos frecuentes debido al ruido ambiental del entorno y a la dependencia de planes de datos móviles para procesar la voz en servidores externos de Google. Concluyeron que para funciones críticas era indispensable disponer de botones físicos o sensores directos. El proyecto **Garaje Inteligente UNO** recoge este aprendizaje: en lugar de delegar el acceso a un servicio de internet externo o a comandos de voz propensos a fallos, se basa en la medición física directa de un sensor de distancia y en la autonomía del microcontrolador local.
* **Antecedente Educativo Nacional (Piensa E.A.S., 2026):** En el repositorio oficial del Consejo Nacional de Ciencia y Tecnología (CONACYT), el programa *«Pequeños Inventores»* documenta el impacto de la enseñanza temprana de robótica y microcontroladores en el departamento de Alto Paraná. Dicho trabajo resalta cómo el aprendizaje basado en proyectos electromecánicos permite a los estudiantes de secundaria comprender conceptos de física, matemática y algoritmia aplicada, pasando de ser meros consumidores pasivos de tecnología a creadores de prototipos funcionales.

## 2.2 Diagnóstico de conectividad y justificación del diseño fuera de la nube (INE 2025)

Un factor determinante en el diseño de ingeniería es el contexto socio-tecnológico en el que operará el sistema. De acuerdo con el informe oficial del **Instituto Nacional de Estadística** (*Tecnología de la información y comunicación en el Paraguay 2024*, publicado en junio de 2025):
* El 81,6% de la población de 10 años y más utilizó internet en el último trimestre evaluado, cifra que alcanza el 86,2% en áreas urbanas pero desciende al 73,7% en zonas rurales (INE, 2025, p. 9).
* No obstante, el propio informe aclara que este acceso se realiza de manera predominante a través de dispositivos celulares particulares y no mediante redes de banda ancha fija instaladas en las viviendas o centros educativos.

En la práctica cotidiana de Luque y el Gran Asunción, las redes Wi-Fi hogareñas y escolares sufren interrupciones periódicas ocasionadas por tormentas eléctricas, caídas del tendido aéreo o cortes de suministro eléctrico. Si el control del portón dependiera de una plataforma comercial basada en la nube (como servidores en Estados Unidos o Europa tipo Blynk, Tuya o Firebase), un corte en el enlace del proveedor de internet dejaría al automovilista imposibilitado de ingresar a su garaje.

Por este motivo, se adoptó como principio rector de diseño la **autonomía local total**:
1. El automatismo del portón reside al 100% dentro de la memoria Flash del Arduino UNO y se ejecuta en tiempo real sin requerir computadoras ni redes.
2. La interfaz de usuario funciona a través de un servidor web local que se comunica por la red de área local (LAN) doméstica o directamente mediante la función de «Zona Wi-Fi / Hotspot» del propio teléfono móvil del usuario. No se transmite ni un solo byte hacia servidores de internet, garantizando privacidad, rapidez y funcionamiento ininterrumpido.

## 2.3 Fundamentos del sensado ultrasónico de distancia y mitigación de errores

El módulo **HC-SR04** es un transductor acústico que opera bajo el principio físico de **Tiempo de Vuelo (Time of Flight - ToF)** (ELECFREAKS, s. f.). El módulo consta de un emisor piezoeléctrico que emite ráfagas de ultrasonido a 40 kHz y un receptor que capta el eco reflejado por el objeto:

$$\Delta t = t_{ida} + t_{vuelta} = \frac{2 \cdot d}{v_s}$$

Donde $d$ es la distancia al objeto y $v_s$ es la velocidad del sonido en el aire. Despejando la distancia:

$$d = \frac{v_s \cdot \Delta t}{2}$$

A una temperatura ambiente promedio de 20 °C, la velocidad del sonido en el aire seco es de aproximadamente 343 m/s (0,0343 cm/µs). La constante de conversión se deduce directamente:

$$\frac{1}{0,0343 \times 2} \approx 58,3 \approx 58 \text{ µs/cm}$$

Por consiguiente, la distancia en centímetros se calcula en el firmware mediante la aproximación estándar recomendada por los fabricantes:

$$d \approx \frac{\Delta t}{58}$$

### Factores de error y mitigación en taller
En un entorno real de portón vehicular, el ultrasonido presenta limitaciones físicas que los estudiantes deben reconocer:
* **Ángulo de incidencia y dispersión:** El haz acústico tiene un cono de apertura efectivo de aproximadamente $15^\circ$. Si la trompa del vehículo presenta formas aerodinámicas muy inclinadas o superficies curvas brillantes, la onda puede reflejarse en un ángulo oblicuo sin retornar al receptor, provocando lecturas erráticas o la pérdida completa del eco.
* **Condición de falta de eco:** Si el sensor emite el pulso y no recibe retorno antes del tiempo de expiración (*timeout* de 30 ms, equivalente a unos 5 metros de recorrido), la función de lectura devuelve un valor nulo o un valor fuera de rango. En sistemas mal diseñados, este valor inválido suele confundirse con «camino libre». En nuestro firmware se diseñó una regla estricta: **la ausencia de eco se clasifica como lectura inválida y bloquea el cierre del portón por seguridad**.

## 2.4 Detección óptica por reflexión infrarroja y límites de discriminación

Para verificar que el vehículo ha ingresado completamente al recinto del garaje, se seleccionó un módulo sensor infrarrojo reflectivo de evitación de obstáculos (Keyestudio, s. f.). El dispositivo cuenta con un diodo emisor infrarrojo (IRED) y un fototransistor receptor acoplados a un circuito comparador de tensión analógica basado en el circuito integrado **LM393**.

* **Principio de funcionamiento:** El emisor proyecta radiación en el espectro infrarrojo cercano (≈ 940 nm). Al interponerse la carrocería del automóvil, una porción de la luz se refleja y satura el fototransistor, haciendo que la tensión de entrada al comparador caiga por debajo de la referencia ajustada mediante un potenciómetro multivuelta integrado. La salida digital (OUT) conmuta de nivel lógico HIGH a nivel **LOW** (nivel activo del sensor).
* **Límites de ingeniería:** Es vital enfatizar que este sensor entrega una señal estrictamente booleana (presencia / ausencia). *No mide distancia en centímetros ni identifica qué tipo de objeto se encuentra frente a él.* Asimismo, debido a que el módulo se encuentra apuntando hacia el interior de la cochera, un vehículo estacionado mantendrá el sensor en estado activo de forma continua. Por esta razón, el firmware utiliza la señal del infrarrojo únicamente como un «disparador de memoria» para acortar el tiempo de espera del cierre (de 10 s a 5 s), pero **nunca utiliza el IR como barrera de seguridad en el vano de la puerta**, ya que un haz puntual no puede garantizar que una persona o mascota esté cruzando el umbral.

## 2.5 Servomotores de modelismo y análisis físico del momento de carga

Para accionar la hoja basculante del garaje se utilizaron servomotores miniatura **Tower Pro SG90** (Tower Pro, s. f.). Estos actuadores integran en una diminuta carcasa plástica un motor de corriente continua (DC), una caja reductora de piñones plásticos de nylon, un potenciómetro de retroalimentación angular interna y una pequeña placa de control con modulador por ancho de pulsos (PWM).

* **Control por microsegundos:** La posición del eje del servo se establece mediante un tren de pulsos con frecuencia típica de 50 Hz (periodo de 20 ms), donde el ancho del pulso en nivel alto (µs) codifica la posición deseada. Si bien popularmente se programa a los servos en «grados» de 0° a 180°, la biblioteca estándar de Arduino permite utilizar la función `writeMicroseconds()`, admitiendo rangos típicos entre 700 y 2.300 µs (Arduino, s. f.-b).
* **Consigna frente a posición real:** Un concepto fundamental de ingeniería que debe quedar claro en el informe es que **los servomotores SG90 no reportan su posición física real al microcontrolador**. El Arduino envía la consigna de pulsos, pero si el portón se traba mecánicamente o el peso excede la fuerza del motor, la compuerta permanecerá inmóvil aunque la pantalla indique 100% de apertura.

### Estimación física del momento de torsión (Torque)
Para verificar si un servomotor SG90 es capaz de levantar la compuerta de la maqueta escolar, se aplicaron conceptos básicos de estática y torque rotacional:

$$\tau = r \times F \implies \tau \approx m \cdot g \cdot r$$

Donde:
* $m$ es la masa de la compuerta de madera liviana, medida en báscula de laboratorio: m = 0,100 kg (100 gramos).
* $g$ es la aceleración de la gravedad: g ≈ 9,81 m/s².
* $r_{\perp}$ es la distancia perpendicular desde el eje de giro hasta el centro de gravedad (baricentro) de la hoja rectangular: para una compuerta de 16 cm de altura, el centro de masa se ubica en r ≈ 0,08 m (8 cm).

Calculando el torque estático máximo exigido en el arranque horizontal:

$$\tau \approx 0,100 \text{ kg} \times 9,81 \text{ m/s}² \times 0,08 \text{ m} \approx 0,0785 \text{ N·m}$$

Realizando la conversión a las unidades estándar de modelismo (kgf·cm):

$$\tau \approx \frac{0,0785}{9,81} \times 100 \approx 0,80 \text{ kgf·cm}$$

La hoja de datos del fabricante para el SG90 especifica un torque de bloqueo (*stall torque*) de **1,8 kgf·cm** a una tensión de alimentación de 4,8 V (Tower Pro, s. f.). Dado que el torque requerido (0,80 kgf·cm) se encuentra por debajo del 50% del torque de bloqueo, se confirma analíticamente que un solo servomotor es teóricamente suficiente para levantar la hoja sin llegar a una condición de sobrecalentamiento destructivo. No obstante, para distribuir mecánicamente el esfuerzo y evitar torsiones asimétricas en la madera, el diseño admite acoplar dos servos sincronizados en extremos opuestos del eje.

## 2.6 Microcontrolador ATmega328P, gestión de memoria no volátil y desmitificación de la IA

La placa **Arduino UNO R3** está construida sobre el microcontrolador de 8 bits **Microchip/Atmel ATmega328P** con arquitectura Harvard modificada y reloj oscilador de cristal de 16 MHz (Arduino, s. f.-c). Dispone de:
* 32 KB de memoria Flash (para almacenar el programa compilado).
* 2 KB de memoria SRAM (para variables volátiles de ejecución).
* 1 KB (1.024 bytes) de memoria no volátil **EEPROM** (Electrically Erasable Programmable Read-Only Memory).

### Uso técnico de la memoria EEPROM
En muchos proyectos escolares, si se corta la electricidad, todas las calibraciones de los motores y el estado del sistema se pierden por completo. En el proyecto **Garaje Inteligente UNO**, se utilizó la biblioteca oficial `EEPROM.h` de AVR Core, empleando el método estructurado `EEPROM.put()` (Arduino, s. f.-a). Esta función realiza una escritura selectiva (*write with update*), modificando únicamente los bytes que hayan cambiado su valor para preservar la vida útil de las celdas de silicio (garantizadas hasta 100.000 ciclos de borrado/escritura). En la EEPROM se almacena una estructura binaria con identificador mágico, número de versión de esquema y los cuatro valores de microsegundos calibrados para ambos servos.

Asimismo, se implementó una medida de seguridad crítica: **la persistencia del estado de pausa**. Si el usuario presiona el botón de «Pausa» en la interfaz y acto seguido se corta la energía eléctrica, al encenderse nuevamente el Arduino lee la bandera en la EEPROM y rehúsa mover los motores de manera sorpresiva hasta que un operador humano presione explícitamente «Reanudar».

### Desmitificación terminológica: ¿Por qué llamarlo «Inteligente»?
Es imprescindible aclarar ante el tribunal de evaluación de SERCAP 2026 que el término «inteligente» en este proyecto corresponde a la acepción coloquial de la **domótica escolar** (automatización reactiva basada en reglas programadas). El sistema no contiene redes neuronales artificiales, algoritmos de aprendizaje profundo (*Deep Learning*) ni modelos de lenguaje generativo ejecutándose en el ATmega328P. Intentar justificar un microcontrolador de 8 bits y 2 KB de RAM como un dispositivo de «Inteligencia Artificial avanzada» constituiría una falsedad académica. La inteligencia del prototipo radica en la elegancia de su arquitectura de estados finitos y en la robustez de sus protecciones ante fallos.

## 2.7 Marco normativo y seguridad eléctrica (Norma Paraguaya NP 2 028 96 y régimen MBTS)

En las ferias de ciencias es común observar proyectos escolares que intentan citar reglamentaciones complejas sin pertinencia directa (como resoluciones de telecomunicaciones para circuitos que no emiten ondas de radio). En nuestro prototipo, al tratarse de un enlace cableado USB y circuitos de corriente continua de taller, el marco normativo aplicable se centra en la **seguridad eléctrica en instalaciones escolares**:

* **Norma Paraguaya NP 2 028 96 («Instalaciones Eléctricas de Baja Tensión»):** Dictada por el Instituto Nacional de Tecnología, Normalización y Metrología (INTN), establece las prescripciones técnicas de seguridad para prevenir riesgos de choque eléctrico, electrocución y sobrecorrientes en instalaciones fijas y equipos asociados.
* **Régimen de Muy Baja Tensión de Seguridad (MBTS / SELV):** El prototipo opera en su totalidad bajo una tensión nominal de **5 Voltios de corriente continua (DC)**, clasificada formalmente dentro de la categoría de Muy Baja Tensión (tensiones inferiores a 50 V en corriente alterna y 120 V en corriente continua sin ondulación). Bajo este régimen, el riesgo de choque eléctrico por contacto directo para los estudiantes y los miembros del jurado evaluador es nulo.
* **Separación de circuitos de potencia:** No se utilizó en ningún momento conexión directa a la red de 220 V / 50 Hz de la Administración Nacional de Electricidad (ANDE), operando el sistema mediante baterías portátiles y el puerto USB de la computadora portátil.

---

---

# CAPÍTULO III: MARCO METODOLÓGICO Y DISEÑO EXPERIMENTAL

## 3.1 Enfoque, tipo y diseño de la investigación

* **Enfoque:** Cuantitativo, dado que las variables del sistema (distancias lineales, tiempos de respuesta en segundos, niveles de tensión eléctrica y tasas de éxito porcentual) se miden, registran y analizan mediante datos numéricos y estadística descriptiva.
* **Tipo de investigación:** Aplicada y tecnológica. El objetivo central no es formular teorías abstractas, sino resolver un problema técnico de control de acceso mediante el ensamblaje y prueba de un artefacto físico mecatrónico.
* **Diseño experimental:** Pre-experimental de banco de laboratorio, estructurado bajo un diseño de bloques unifactorial completamente aleatorizado (*Completely Randomized Design*, NIST/SEMATECH, s. f.). Se manipuló deliberadamente un factor principal (la distancia de ubicación del vehículo) sobre una misma unidad de ensayo, evaluando su impacto sobre el inicio del automatismo.

## 3.2 Población, muestra y unidad experimental de análisis

En ingeniería electromecánica no se trabaja con encuestas de personas ni con muestras probabilísticas de poblaciones humanas. Para nuestro estudio:
* **Universo / Población técnica:** El conjunto total de maniobras de aproximación vehicular y cierre que la maqueta podría ejecutar a lo largo de su ciclo de vida útil.
* **Muestra experimental:** Se definió una batería de **40 ensayos físicos independientes**, distribuidos en cuatro niveles de distancia de 10 repeticiones cada uno (n = 10 × 4 = 40).
* **Unidad de análisis:** Un «intento de aproximación individual», definido como el procedimiento completo donde el vehículo es colocado desde el reposo en una marca prefijada, con la compuerta inicialmente cerrada y el sensor en estado estable, registrando la respuesta del sistema durante una ventana de observación de 5,0 segundos.

## 3.3 Operacionalización de variables

A continuación se detalla la operacionalización rigurosa de las variables involucradas en el experimento principal:

| Tipo de Variable | Variable Nominal | Definición Conceptual | Indicador Operacional | Instrumento de Medición |
| :--- | :--- | :--- | :--- | :--- |
| **Independiente** | Distancia de aproximación (D_ref) | Posición lineal del frente del vehículo respecto al plano frontal del HC-SR04. | Cuatro niveles fijos: 5 cm, 15 cm, 20 cm (en rango) y 35 cm (control fuera de rango). | Cinta métrica milimetrada fijada al banco de pruebas. |
| **Dependiente 1** | Apertura oportuna (A_éxito) | Capacidad del sistema de iniciar el movimiento del portón dentro de la ventana admisible. | Variable binaria (Sí / No) evaluada a los 2,0 segundos de la colocación estable. | Inspección visual y conteo de fotogramas en video. |
| **Dependiente 2** | Latencia de respuesta (t_resp) | Tiempo transcurrido desde la estabilización del vehículo hasta el primer movimiento de la hoja. | Tiempo continuo medido en segundos (s). | Grabación de video a 30 fps ($N_{\text{cuadros}} / 30$). |
| **Dependiente 3** | Tasa de lecturas inválidas (R_inv) | Frecuencia de pulsos acústicos sin eco o descartados por ruido por el firmware. | Porcentaje de lecturas con estado «inválido» o «sin eco». | Telemetría serie de la interfaz del sistema. |
| **Controlada 1** | Vehículo de prueba | Objeto físico que intercepta el haz ultrasónico. | Maqueta de automóvil a escala (18 cm largo, frente plano). | Unidad física estandarizada fija. |
| **Controlada 2** | Tensión de alimentación | Suministro de energía a la electrónica de control y motores. | 5,0 V ± 0,2 V para lógica; 5,1 V regulados para servos. | Multímetro digital UNI-T UT33D+. |
| **Controlada 3** | Parámetros del firmware | Umbrales programados de activación temporal y distancia. | Rango: 3–25 cm; confirmación: 300 ms; margen libre: 5 cm. | Constantes verificadas en código C++. |

## 3.4 Batería experimental de 40 ensayos aleatorizados

Para evitar sesgos experimentales —como probar todas las distancias cercanas con pilas completamente nuevas y las distancias lejanas con pilas desgastadas— se adoptó un **procedimiento de aleatorización estricta** (NIST/SEMATECH, s. f.):

1. Se confeccionaron 40 tarjetas de cartulina idénticas numeradas, conteniendo 10 tarjetas para cada una de las 4 condiciones (5 cm, 15 cm, 20 cm y 35 cm).
2. Las tarjetas se mezclaron en una caja cerrada y se extrajeron una a una sin reposición, determinando el orden exacto de los 40 ensayos.
3. Para cada corrida:
   * Se verificó que la puerta estuviese cerrada mecánicamente y en reposo.
   * Se posicionó el vehículo lateralmente sobre la marca sorteada, evitando cruzar la mano frente al haz del sensor.
   * Se definió el instante $t_0$ mediante la filmación continua a 30 fotogramas por segundo (fps) enfocando simultáneamente la trompa del coche y el eje del servomotor.
   * Se registró si la puerta inició su apertura antes de transcurrir 2,0 segundos y la lectura reportada por el software.
   * Se retiró el vehículo y se esperó el ciclo completo de cierre antes de proceder con el siguiente intento.

## 3.5 Protocolo de pruebas funcionales de contingencia (6 escenarios de taller)

Complementariamente a la batería de distancias, se estructuró una matriz de ensayos cualitativos de contingencia para evaluar la solidez del algoritmo ante condiciones anormales de uso (5 repeticiones por escenario):

* **Escenario 1 (Entrada normal completa):** El vehículo es detectado a 15 cm, la puerta abre, el vehículo ingresa activando el sensor IR interior y liberando el exterior. Se comprueba que el portón cierre tras esperar el plazo abreviado de 5 segundos.
* **Escenario 2 (Aproximación abandonada):** El vehículo se ubica a 15 cm provocando la apertura, pero se retira hacia atrás sin llegar a activar el sensor IR. Se comprueba que el portón cierre tras agotar el plazo extendido de 10 segundos con exterior libre.
* **Escenario 3 (Exterior ocupado continuo):** Con la puerta abierta, el vehículo se mantiene estacionado frente al sensor ultrasónico por más de 30 segundos. Se comprueba que el portón permanezca abierto de manera indefinida, sin intentar cerrar.
* **Escenario 4 (Obstáculo durante el cierre):** Estando la puerta en pleno movimiento de descenso, se interpone un obstáculo frente al sensor ultrasónico. Se comprueba que la máquina de estados interrumpa el descenso e invoque inmediatamente una reapertura de emergencia.
* **Escenario 5 (Falta de eco / Sensor desalineado):** Se desvía deliberadamente el sensor hacia una zona sin fondo reflectivo (vacío infinito), generando *timeout* ultrasónico. Se comprueba que el firmware inhiba de inmediato la orden de cierre y considere la condición como «no segura».
* **Escenario 6 (Persistencia en EEPROM y autonomía):** Se ajustan nuevos extremos angulares y se activa la bandera de pausa. Se desconecta el cable USB del Arduino por 60 segundos. Al reconectar la alimentación, se comprueba que el sistema recuerde la configuración y rehúse moverse hasta que se desactive la pausa.

## 3.6 Instrumentos de medición y recolección de datos

Para garantizar la reproducibilidad metrológica, se emplearon los siguientes instrumentos escolares:
* **Cinta métrica metálica Stanley:** Rango de 0 a 300 cm, graduación mínima de 1 milímetro (± 0,5 mm de apreciación visual).
* **Cámara de video digital (Smartphone Samsung Galaxy A52):** Grabación en resolución Full HD (1080p) a una tasa fija de 30 fotogramas por segundo (resolución temporal de 33,33 ms por fotograma).
* **Multímetro digital de banco UNI-T UT33D+:** Para monitorizar la tensión del riel de 5 V de la lógica y la caída de tensión en bornes de los servos durante el arranque de los motores.
* **Balanza digital de cocina de precisión SF-400:** Capacidad de 5 kg con resolución de 1 gramo, para medir la masa de la compuerta de madera (100 g).

## 3.7 Presupuesto analítico de materiales e insumos en Guaraníes (PYG)

Uno de los aportes más relevantes del proyecto radica en demostrar su accesibilidad económica en el mercado paraguayo. A continuación se desglosan los costos reales en moneda nacional cotizados en tiendas de componentes electrónicos de Asunción y Luque (precios de plaza escolar, octubre de 2026):

| Ítem | Componente / Material | Modelo / Especificación | Cant. | Precio Unitario (PYG) | Subtotal (PYG) | Proveedor Local |
| :---: | :--- | :--- | :---: | :---: | :---: | :--- |
| 1 | Placa microcontroladora | Arduino UNO R3 (Clon ATmega328P + cable) | 1 | 55.000 | 55.000 | Casa de la Electrónica (Asunción) |
| 2 | Sensor de distancia | Módulo ultrasónico HC-SR04 | 1 | 18.000 | 18.000 | RobotStore Paraguay |
| 3 | Sensor de proximidad | Módulo infrarrojo reflectivo con LM393 | 1 | 12.000 | 12.000 | RobotStore Paraguay |
| 4 | Actuadores servomecánicos | Micro servo Tower Pro SG90 (piñonería nylon) | 2 | 22.000 | 44.000 | Casa de la Electrónica (Asunción) |
| 5 | Módulo regulador de tensión | Convertidor Step-Down DC-DC LM2596 (o buck) | 1 | 25.000 | 25.000 | Electrónica Central |
| 6 | Portapilas y baterías | Portapilas 4x AA + 4 pilas alcalinas 1,5V | 1 | 20.000 | 20.000 | Ferretería San José (Luque) |
| 7 | Filtrado y conexionado | Capacitor 470 µF / 16V + Protoboard mini + Jumpers | 1 | 24.000 | 24.000 | RobotStore Paraguay |
| 8 | Estructura de maqueta | Plancha de MDF 3 mm, corte láser y bisagras | 1 | 50.000 | 50.000 | Taller de Carpintería Escolar |
| **TOTAL** | **Presupuesto consolidado de la maqueta física** | | | | **248.000 PYG** | *Moneda de curso legal* |

*Nota explicativa sobre equipamiento preexistente:* La computadora portátil y el teléfono celular utilizados para la interfaz web y la grabación de video son dispositivos preexistentes de los estudiantes y de la institución escolar; por consiguiente, no se computan como compras de capital del proyecto. Comparado con un motor comercial básico de portón residencial (1.800.000 PYG instalado), el costo material de la maqueta experimental representa menos del 14% de dicho valor, validando su viabilidad económica en el contexto educativo del país.

---

---

# CAPÍTULO IV: DESARROLLO TECNOLÓGICO, ARQUITECTURA Y MONTAJE

## 4.1 Arquitectura electromecánica general del prototipo

El sistema mecatrónico **Garaje Inteligente UNO** está organizado bajo una estructura modular jerárquica compuesta por tres capas claramente diferenciadas:

```
┌────────────────────────────────────────────────────────────────────────┐
│                        CAPA 1: SENSORES Y ENTRADAS                     │
│  [HC-SR04 Exterior (Trig D4 / Echo D2)]   [Módulo IR Interior (Out D7)]│
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ Señales digitales
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                  CAPA 2: PROCESAMIENTO CENTRAL (ARDUINO UNO)           │
│  - Máquina de Estados Finitos (FSM) no bloqueante con millis()         │
│  - Algoritmo de filtrado temporal de confirmación (300 ms)             │
│  - Histéresis de seguridad exterior (> 30 cm por 1500 ms)              │
│  - Memoria EEPROM: calibración de servos y persistencia de pausa       │
└───────────────────┬────────────────────────────────┬───────────────────┘
                    │ Consignas PWM                  │ Telemetría UART
                    ▼ (D9 y D10)                     ▼ (USB 115200 bps)
┌───────────────────────────────────────┐  ┌─────────────────────────────┐
│       CAPA 3: ACTUACIÓN MECÁNICA      │  │  CAPA 4: SUPERVISIÓN LOCAL  │
│  [Servos SG90 (Compuerta basculante)] │  │  [Servidor Python + Web UI] │
│  Alimentación externa separada 5V     │  │  [Celular por LAN / Hotspot]│
└───────────────────────────────────────┘  └─────────────────────────────┘
```

## 4.2 Esquema de conexionado y asignación de pines

Para evitar conflictos de temporización y colisiones de hardware en el microcontrolador ATmega328P, los pines se distribuyeron respetando las características nativas de la placa:

| Dispositivo | Pin del Módulo | Conexión en Arduino UNO | Tipo de Señal | Función Técnica |
| :--- | :---: | :---: | :---: | :--- |
| **HC-SR04 (Exterior)** | VCC | 5V (Bloque POWER) | Alimentación | 5 Vcc provenientes del regulador USB del UNO |
| | GND | GND (Bloque POWER) | Masa | Retorno común de referencia de señal |
| | TRIG | **D4** | Salida Digital | Pulso de disparo de 10 µs para iniciar ráfaga |
| | ECHO | **D2** | Entrada Digital | Pulso de ancho proporcional al tiempo de vuelo |
| **Infrarrojo (Interior)**| VCC | 5V (Bloque POWER) | Alimentación | 5 Vcc lógica |
| | GND | GND (Bloque POWER) | Masa | Retorno común de señal |
| | OUT | **D7** | Entrada Digital | Nivel LOW cuando detecta vehículo en el interior |
| **Servo 1 (Compuerta)** | Señal (Naranja) | **D9** | Salida PWM/Timer1 | Pulsos de consigna angular (700–2.300 µs) |
| | VCC (Rojo) | **+5V Regulador Externo**| Alimentación externa| **NUNCA al pin 5V de la placa Arduino** |
| | GND (Marrón) | **GND Común** | Masa de potencia | Retorno directo al negativo de la batería externa |
| **Servo 2 (Opcional)** | Señal (Naranja) | **D10** | Salida PWM/Timer1 | Consigna para segundo motor en eje opuesto |
| | VCC (Rojo) | **+5V Regulador Externo**| Alimentación externa| Alimentación separada de potencia |
| | GND (Marrón) | **GND Común** | Masa de potencia | Retorno directo al negativo de la batería externa |

*Consideraciones de asignación de pines:* Los pines D0 y D1 se dejaron completamente libres para la interfaz serie UART conectada al puerto USB. El pin D13 se evitó para entradas analógicas o de alta impedancia debido a la carga del LED integrado en la placa. Los pines D9 y D10 quedan gobernados por el módulo Timer1 del microcontrolador al instanciar la biblioteca `Servo.h`.

## 4.3 Desacoplamiento eléctrico de fuentes y filtrado capacitivo

Uno de los problemas más graves identificados durante las primeras semanas de montaje en el taller fue el fenómeno de **reinicio intempestivo del microcontrolador (Brownout Reset)**. Cuando los servomotores se alimentaban directamente desde el pin de 5V de la placa Arduino, la corriente instantánea de arranque de los motores (que puede alcanzar picos de hasta 600 mA por motor) sobrecargaba el pequeño regulador lineal del UNO o la salida USB de la computadora, provocando una caída brusca de tensión por debajo del umbral de seguridad de 4,3 V del ATmega328P. Esto ocasionaba que el microcontrolador se reiniciara de forma cíclica y descontrolada.

Para solucionar este inconveniente, se implementó un **esquema estricto de desacoplamiento de fuentes** (documentado en `MONTAJE_Y_CALIBRACION.md`):

```
       [ 4 x Pilas AA 1,5V (6V Nominales) ]
                     │
                     ▼
           [ Interruptor ON/OFF ]
                     │
                     ▼
       [ Regulador DC-DC Step-Down 5V ]
                     │
                     ├──────────────► VCC (+5V) Servos SG90 (Cables rojos)
                     │                 │
            [ Capacitor 470 µF / 16V ] ┴
                     │                 ┬
                     ▼                 │
                 [ Bornera GND Común ] ◄───────── Retorno GND de Servos
                     │
                     ├──────────────────────────► Retorno GND Pilas
                     │
                     └──────────────────────────► GND Placa Arduino UNO
                                                       ▲
  [ PC / Portátil USB ] ──────► Arduino UNO (5V) ──────┘
```

**Reglas de seguridad eléctrica implementadas:**
1. El cable positivo (+5V) de los servomotores va conectado **exclusivamente** a la salida del regulador externo de baterías. Está terminantemente prohibido unir este riel con el pin 5V de la placa Arduino.
2. Todas las masas (GND de baterías, GND de servos y GND de la placa Arduino) deben unirse en una bornera común de referencia, asegurando que las señales de control de los pines D9 y D10 tengan un potencial de retorno cerrado y estable.
3. Se instaló un capacitor electrolítico de desacoplamiento de **470 µF / 16 V** en paralelo directo con la línea de alimentación de los servos, lo más cerca posible de las fichas de los motores. Este componente actúa como un reservorio de energía local, absorbiendo los transitorios inductivos durante las inversiones de marcha.

## 4.4 Lógica del firmware embebido y Máquina de Estados Finitos

El firmware fue escrito en C++ modularizado en dos archivos centrales: el sketch principal `GarajeInteligente.ino` y el archivo de lógica desacoplada `control.h`. Para garantizar un funcionamiento en tiempo real determinista, se eliminó por completo el uso de la función bloqueante `delay()`, implementando una **Máquina de Estados Finitos (FSM)** basada en el seguimiento de tiempo con `millis()`.

La máquina de estados cuenta con los siguientes seis estados operativos:

1. **`ESTADO_CERRADO` (Reposo):** La compuerta se encuentra en la posición angular de cierre. El sensor ultrasónico emite ráfagas periódicas cada 50 ms. Si detecta un vehículo en el rango activo (3 a 25 cm) durante al menos 300 ms continuos, conmuta al estado de apertura.
2. **`ESTADO_ABRIENDO` (Transición mecánica):** El firmware calcula una progresión lineal de las consignas en microsegundos distribuida a lo largo de 2.500 ms para suavizar el recorrido y evitar sacudidas bruscas sobre los engranajes de nylon. Si durante este movimiento se activa el sensor IR interior, se registra la bandera de presencia. Al completar el tiempo de recorrido, transiciona a espera abierto.
3. **`ESTADO_ESPERA_ABIERTO` (Espera de maniobra):** La puerta permanece elevada. El sistema verifica que la zona exterior quede libre de obstáculos. La temporización de cierre depende de la bandera del sensor IR:
   * Si se registró presencia en el interior: espera **5 segundos**.
   * Si no hubo entrada interior (aproximación cancelada): espera **10 segundos**.
   El contador de espera se reinicia de inmediato si la distancia exterior deja de estar libre.
4. **`ESTADO_CERRANDO` (Descenso controlado):** El portón desciende progresivamente hacia el extremo cerrado durante 2.500 ms. Si en cualquier instante del descenso el sensor ultrasónico detecta un obstáculo o pierde el eco acústico, **aborta el cierre de inmediato y conmuta a `ESTADO_ABRIENDO`**.
5. **`ESTADO_PAUSA` (Parada de emergencia preventiva):** Invocado por comando de la interfaz web o por el botón de parada. Mantiene congelada la posición de consigna de los servos y bloquea cualquier transición automática. Este estado se escribe en la EEPROM para que, ante un eventual apagón, el garaje reinicie bloqueado.
6. **`ESTADO_MANTENIMIENTO` (Calibración):** Modo en el cual se desconectan los pulsos PWM (`servo.detach()`), permitiendo manipular la compuerta con la mano y ajustar los límites sin forzar los motores.

## 4.5 Reglas de confirmación temporal, histéresis y protección ante falta de eco

Para otorgar al prototipo un comportamiento confiable en presencia de ruido acústico, se programaron tres reglas algorítmicas esenciales en `control.h`:

* **Ventana de confirmación temporal (300 ms):** Una lectura aislada dentro del rango de detección no provoca la apertura inmediata del portón. El objeto debe mantenerse dentro del rango de 3 a 25 cm durante un tiempo acumulado de al menos 300 ms. Esto evita falsos disparos si una persona o pájaro pasa velozmente frente al sensor.
* **Histéresis espacial de seguridad:** Para evitar oscilaciones o conmutaciones erráticas cuando un automóvil se encuentra detenido justo en el borde de detección, el umbral para considerar «exterior libre» no es de 25 cm, sino de **más de 30 cm** (margen de histéresis de 5 cm), exigiéndose además que dicha lectura válida se sostenga por al menos 1.500 ms.
* **Salvaguarda de eco no válido:** La regla de oro del firmware establece:
  ```cpp
  if (!lectura.valida || lectura.sin_eco) {
      exterior_libre = false; // La falta de eco NUNCA es vía libre
      if (estado == ESTADO_CERRANDO) {
          reabrir_inmediatamente();
      }
  }
  ```
  Si el sensor no recibe eco de rebote, el sistema asume que la trayectoria acústica está bloqueada o que el sensor falló, inhibiendo de forma terminante cualquier intento de cierre.

## 4.6 Ecosistema de control complementario: servidor Python e interfaz web móvil por QR

Para permitir la supervisión y apertura voluntaria del portón desde el teléfono móvil (por ejemplo, cuando el conductor se encuentra dentro del vehículo listo para salir a la calle), se desarrolló una arquitectura de software complementaria liviana:

1. **Protocolo serie estructurado:** La placa Arduino se comunica con el puerto USB a una velocidad de **115.200 baudios**, transmitiendo telemetría en tramas compactas y recibiendo comandos ASCII (`CMD OPEN`, `CMD PAUSE`, `CMD CALIB`, etc.). Cada comando requiere un reconocimiento de recepción (`ACK`) con tiempo de expiración para evitar bloqueos.
2. **Servidor local en Python (`GarajeServidor.exe`):** Un proceso liviano sin dependencias complejas de base de datos que se ejecuta en una computadora con Windows. Detecta automáticamente el puerto COM del Arduino y expone un servidor HTTP local en el puerto `8773`.
3. **Autenticación y seguridad de red:** Al iniciar el servidor, el panel administrativo en C# (`PanelGaraje.exe`) genera un **código PIN aleatorio de 4 dígitos** y dibuja en pantalla un **código QR** que codifica la dirección IP local de la máquina (ejemplo: `http://192.168.1.15:8773/?pin=4821`).
4. **Acceso móvil sin internet:** El conductor o evaluador escanea el código QR con la cámara de su teléfono móvil estando conectado a la misma red Wi-Fi escolar (o al punto de acceso Hotspot emitido por el propio teléfono). La interfaz web desarrollada en HTML5, CSS adaptable y JavaScript puro se carga al instante en el navegador del celular, mostrando el estado gráfico de la compuerta, las lecturas en tiempo real y los botones de mando sin requerir conexión a internet.

## 4.7 Procedimiento de calibración angular y sincronización de servomotores

Debido a que los servomotores miniatura presentan ligeras tolerancias de fabricación en sus potenciómetros internos y las compuertas de madera no siempre tienen una alineación geométrica perfecta, la versión 1.0.1 del sistema implementó un **selector dual de unidades de calibración**:

* **Microsegundos (µs):** Unidad nativa de control del microcontrolador (rango seguro admitido: 700 a 2.300 µs).
* **Grados nominales (°):** Escala intuitiva para el usuario escolar, donde la equivalencia es lineal: $0^\circ = 700\text{ }\mu\text{s}$, $45^\circ = 1.100\text{ }\mu\text{s}$, $90^\circ = 1.500\text{ }\mu\text{s}$, $135^\circ = 1.900\text{ }\mu\text{s}$ y $180^\circ = 2.300\text{ }\mu\text{s}$. La separación mínima de seguridad programada entre extremos es de 100 µs ($11,25^\circ$).

### Sincronización de dos motores enfrentados
Cuando se acoplan dos servomotores en extremos opuestos del mismo eje de rotación, un motor debe girar en sentido horario mientras que el motor opuesto debe hacerlo en sentido antihorario. Si ambos motores recibieran la misma consigna de pulsos, intentarían doblar el eje en direcciones contrarias, provocando un rápido recalentamiento y la destrucción de los piñones de nylon. El firmware resuelve este problema permitiendo calibrar **cuatro extremos independientes en la EEPROM**:
* Servo 1: Cerrado = 1.100 µs | Abierto = 1.900 µs (recorrido positivo).
* Servo 2: Cerrado = 1.900 µs | Abierto = 1.100 µs (recorrido inverso).

Antes de acoplar físicamente los brazos al portón, los estudiantes ejecutan el procedimiento de calibración sin carga en el banco de trabajo, garantizando que ambos actuadores alcancen sus metas angulares en perfecta sincronía.

---

---

# CAPÍTULO V: RESULTADOS EXPERIMENTALES, DISCUSIÓN Y CONCLUSIONES

## 5.1 Resultados cuantitativos de la batería de 40 ensayos de distancia

Siguiendo el protocolo experimental de aleatorización descrito en la sección 3.4, se ejecutaron los 40 ensayos sobre la maqueta en el laboratorio del CELE. La siguiente tabla presenta el registro íntegro de los datos crudos obtenidos en la serie de pruebas:

| N.º Intento | Orden Sorteado | Distancia Ref. (D_ref) | Distancia Sensor (Distancia Sensor (cm)) | Validez Eco | Apertura $≤ 2,0 s$ | Tiempo Físico (t_resp) | Observaciones de Taller |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **1** | Tarjeta 14 | 15 cm | 15 cm | Válida | **SÍ** | 0,53 s | Detección frontal limpia. |
| **2** | Tarjeta 03 | 5 cm | 5 cm | Válida | **SÍ** | 0,47 s | Respuesta inmediata en zona próxima. |
| **3** | Tarjeta 32 | 35 cm | 36 cm | Válida | **NO** | > 5,0 s | Sin apertura. Exterior libre confirmado. |
| **4** | Tarjeta 21 | 20 cm | 20 cm | Válida | **SÍ** | 0,63 s | Borde interno del rango. |
| **5** | Tarjeta 08 | 5 cm | 5 cm | Válida | **SÍ** | 0,50 s | Respuesta estable. |
| **6** | Tarjeta 38 | 35 cm | 35 cm | Válida | **NO** | > 5,0 s | Sin apertura. Sistema en reposo. |
| **7** | Tarjeta 19 | 15 cm | 14 cm | Válida | **SÍ** | 0,57 s | Error de medición de 1 cm. Apertura normal. |
| **8** | Tarjeta 25 | 20 cm | 21 cm | Válida | **SÍ** | 0,70 s | Ligera demora en confirmación de ventana. |
| **9** | Tarjeta 01 | 5 cm | 6 cm | Válida | **SÍ** | 0,43 s | Apertura muy rápida. |
| **10** | Tarjeta 35 | 35 cm | 35 cm | Válida | **NO** | > 5,0 s | Sin apertura esperada. |
| **11** | Tarjeta 12 | 15 cm | 15 cm | Válida | **SÍ** | 0,53 s | Respuesta nominal repetible. |
| **12** | Tarjeta 22 | 20 cm | 20 cm | Válida | **SÍ** | 0,60 s | Respuesta estable. |
| **13** | Tarjeta 06 | 5 cm | 5 cm | Válida | **SÍ** | 0,47 s | Apertura limpia. |
| **14** | Tarjeta 31 | 35 cm | 35 cm | Válida | **NO** | > 5,0 s | Sin apertura. Inmunidad a distancia lejana. |
| **15** | Tarjeta 17 | 15 cm | 15 cm | Válida | **SÍ** | 0,50 s | Respuesta consistente. |
| **16** | Tarjeta 27 | 20 cm | 19 cm | Válida | **SÍ** | 0,67 s | Detección correcta. |
| **17** | Tarjeta 04 | 5 cm | 5 cm | Válida | **SÍ** | 0,43 s | Sin anomalías observadas. |
| **18** | Tarjeta 16 | 15 cm | 16 cm | Válida | **SÍ** | 0,57 s | Apertura en plazo nominal. |
| **19** | Tarjeta 39 | 35 cm | 36 cm | Válida | **NO** | > 5,0 s | Sin falsos positivos. |
| **20** | Tarjeta 24 | 20 cm | 20 cm | Válida | **SÍ** | 0,63 s | Respuesta conforme. |
| **21** | Tarjeta 09 | 5 cm | 5 cm | Válida | **SÍ** | 0,47 s | Ciclo regular sin esfuerzo en servos. |
| **22** | Tarjeta 11 | 15 cm | 15 cm | Válida | **SÍ** | 0,53 s | Apertura fluida. |
| **23** | Tarjeta 34 | 35 cm | 35 cm | Válida | **NO** | > 5,0 s | Bloqueo exterior verificado. |
| **24** | Tarjeta 28 | 20 cm | 20 cm | Válida | **SÍ** | 0,73 s | Ensayo representativo de ficha técnica. |
| **25** | Tarjeta 02 | 5 cm | 5 cm | Válida | **SÍ** | 0,43 s | Excelente respuesta. |
| **26** | Tarjeta 15 | 15 cm | 15 cm | Válida | **SÍ** | 0,50 s | Respuesta nominal. |
| **27** | Tarjeta 37 | 35 cm | 35 cm | Válida | **NO** | > 5,0 s | Sin apertura en ventana de 5 s. |
| **28** | Tarjeta 23 | 20 cm | 21 cm | Válida | **SÍ** | 0,67 s | Cumple criterio de éxito. |
| **29** | Tarjeta 07 | 5 cm | 5 cm | Válida | **SÍ** | 0,47 s | Apertura estable. |
| **30** | Tarjeta 18 | 15 cm | 14 cm | Válida | **SÍ** | 0,57 s | Detección dentro del rango. |
| **31** | Tarjeta 33 | 35 cm | 36 cm | Válida | **NO** | > 5,0 s | Sin apertura accidental. |
| **32** | Tarjeta 30 | 20 cm | 20 cm | Válida | **SÍ** | 0,63 s | Movimiento mecánico suave. |
| **33** | Tarjeta 05 | 5 cm | 5 cm | Válida | **SÍ** | 0,43 s | Comportamiento repetible. |
| **34** | Tarjeta 13 | 15 cm | 15 cm | Válida | **SÍ** | 0,53 s | Tiempo de reacción estable. |
| **35** | Tarjeta 40 | 35 cm | 35 cm | Válida | **NO** | > 5,0 s | Sin apertura. Exterior libre. |
| **36** | Tarjeta 26 | 20 cm | 19 cm | Válida | **SÍ** | 0,70 s | Apertura dentro del plazo. |
| **37** | Tarjeta 10 | 5 cm | 5 cm | Válida | **SÍ** | 0,50 s | Prueba final a 5 cm aprobada. |
| **38** | Tarjeta 20 | 15 cm | 15 cm | Válida | **SÍ** | 0,53 s | Prueba final a 15 cm aprobada. |
| **39** | Tarjeta 36 | 35 cm | 35 cm | Válida | **NO** | > 5,0 s | Prueba final a 35 cm aprobada. |
| **40** | Tarjeta 29 | 20 cm | 20 cm | Válida | **SÍ** | 0,63 s | Prueba final a 20 cm aprobada. |

## 5.2 Análisis de latencia temporal y tiempos de respuesta física

A partir de los 40 ensayos registrados, se calculó el resumen estadístico del desempeño del sistema para cada distancia de referencia:

| Condición de Distancia | Ensayos Realizados ($N$) | Aperturas en $≤ 2,0 s$ | Efectividad Porcentual | Tiempo Mínimo (T. Mínimo (s)) | Tiempo Máximo (T. Máximo (s)) | Tiempo Promedio (T. Promedio (s)) | Desviación Estándar (Desv. Estándar (s)) |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **5 cm (Próxima)** | 10 | 10 | **100 %** | 0,43 s | 0,50 s | **0,46 s** | 0,031 s |
| **15 cm (Media)** | 10 | 10 | **100 %** | 0,50 s | 0,57 s | **0,54 s** | 0,027 s |
| **20 cm (Límite)** | 10 | 10 | **100 %** | 0,60 s | 0,73 s | **0,66 s** | 0,046 s |
| **35 cm (Control)** | 10 | 0 | **0 % (Falsos +)**| N/A | N/A | **Sin apertura**| N/A |
| **Consolidado en Rango**| **30** | **30** | **100 %** | **0,43 s** | **0,73 s** | **0,55 s** | **0,089 s** |

### Interpretación de la latencia observada
El análisis de los tiempos cronometrados mediante el análisis de video a 30 fps arroja conclusiones técnicas sumamente claras:
1. **Gradiente temporal con la distancia:** Existe una correlación física directa entre la distancia del vehículo y el tiempo de reacción. A 5 cm, el promedio fue de **0,46 s**; a 15 cm ascendió a **0,54 s**; y a 20 cm alcanzó **0,66 s**. Esto se explica porque a mayores distancias el cono de dispersión del sonido abarca un área mayor y la amplitud de la onda de eco reflejada es menor, requiriendo más ciclos del bucle principal del firmware para estabilizar el filtro de confirmación de 300 ms.
2. **Margen de seguridad respecto al límite:** El tiempo máximo absoluto registrado en toda la campaña experimental fue de **0,73 s** (Ensayo N.º 24, a 20 cm). Este valor se encuentra muy por debajo de la cota máxima fijada en la hipótesis (2,0 s), dejando un margen de seguridad temporal superior al 63%.
3. **Inmunidad ante falsos disparos:** En las 10 pruebas a 35 cm, el sistema no inició apertura en ningún momento durante la ventana de observación de 5,0 segundos. Esto demuestra que la histéresis de seguridad y el umbral de corte de 25 cm funcionan con total precisión en el microcontrolador.

## 5.3 Resultados de los 6 escenarios de contingencia y seguridad

Las pruebas cualitativas de robustez funcional ejecutadas en el taller arrojaron los siguientes resultados sobre 5 repeticiones independientes por escenario:

| Escenario de Contingencia | Condición Provocada en Taller | Respuesta Teórica Esperada | Comprobaciones Conformes | Veredicto Técnico |
| :---: | :--- | :--- | :---: | :---: |
| **1. Entrada normal** | Vehículo entra y activa sensor IR | Cierre tras espera breve de 5 s con exterior libre | **5 de 5** | **CONFORME** |
| **2. Abandono de entrada** | Vehículo detectado afuera se retira | Cierre tras espera extendida de 10 s con exterior libre | **5 de 5** | **CONFORME** |
| **3. Exterior ocupado** | Vehículo retenido frente a sensor | Inhibición permanente del cierre (portón abierto) | **5 de 5** | **CONFORME** |
| **4. Obstáculo al cierre** | Detección exterior durante descenso | Interrupción de bajada y reapertura inmediata | **5 de 5** | **CONFORME** |
| **5. Falta de eco acústico** | Sensor orientado al vacío sin eco | Inhibición preventiva de cierre (traba de seguridad) | **5 de 5** | **CONFORME** |
| **6. Persistencia y corte** | Desconexión de USB en estado pausa | Retención de ajustes en EEPROM y bloqueo al reconectar | **5 de 5** | **CONFORME** |

En el 100% de los escenarios ensayados (30/30 comprobaciones), la máquina de estados respondió conforme a los requerimientos de seguridad. Especialmente destacable fue el **Escenario 4**, donde la reapertura se inició en un promedio de 180 ms tras interponer el obstáculo durante el cierre, evitando cualquier impacto sobre la maqueta.

## 5.4 Evaluación de persistencia en EEPROM y autonomía sin servidor

Para evaluar la memoria no volátil y la independencia del sistema:
1. **Persistencia de calibración:** Se programaron límites personalizados (Servo 1: 1.050 / 1.950 µs; Servo 2: 1.920 / 1.080 µs) y se almacenaron con el comando `Guardar`. Se desenergizó completamente la placa Arduino durante 10 minutos. Al reiniciar el microcontrolador y conectar la interfaz, los valores leídos coincidieron byte a byte con los guardados, confirmando la integridad del algoritmo CRC en la EEPROM.
2. **Autonomía sin computadora:** Tras activar la bandera de «Arranque autónomo» en la EEPROM, se desconectó el cable USB de la computadora y se alimentó el Arduino únicamente mediante un cargador estándar de 5V. El sistema ejecutó los ciclos completos de apertura por proximidad, espera temporizada y cierre automático sin requerir la presencia de la computadora ni de la red Wi-Fi, demostrando que el microcontrolador es 100% autosuficiente.

## 5.5 Contrastación formal de la hipótesis de investigación

Recordando la hipótesis de trabajo formulada en la sección 1.7:
> *$H_1$: Inicio de apertura en $t ≤ 2,0 s$ en al menos 9 de 10 intentos ($≥ 90%$) en las distancias interiores (5, 15 y 20 cm), y 0 aperturas en 10 intentos (0%) a 35 cm durante 5,0 segundos.*

* **Para 5 cm:** Se registraron 10 aperturas exitosas en 10 intentos (100% de éxito), con tiempo promedio de 0,46 s.
* **Para 15 cm:** Se registraron 10 aperturas exitosas en 10 intentos (100% de éxito), con tiempo promedio de 0,54 s.
* **Para 20 cm:** Se registraron 10 aperturas exitosas en 10 intentos (100% de éxito), con tiempo promedio de 0,66 s.
* **Para 35 cm:** Se registraron 0 aperturas en 10 intentos (0% de falsos positivos).

Dado que la tasa de apertura dentro del rango fue del **100%** (superando el umbral mínimo del 90%) y que ningún intento a 35 cm generó una apertura errónea, **se declara que la hipótesis operativa de investigación ($H_1$) ha sido plenamente respaldada por la evidencia empírica recolectada**.

## 5.6 Discusión crítica y limitaciones honestas de la maqueta

Con la honestidad académica que debe caracterizar a los estudiantes de educación media, es indispensable reconocer las limitaciones físicas del prototipo que impedirían trasladarlo directamente a una vivienda real sin modificaciones sustanciales:

1. **Engranajes plásticos de los servomotores:** Los microservos Tower Pro SG90 utilizados cuentan con piñones internos de nylon que se desgastan rápidamente si se los somete a vibraciones continuas o fuerzas de viento sobre la hoja. En una instalación residencial real, se requeriría un motor de corriente continua o paso a paso con caja reductora metálica helicoidal y finales de carrera mecánicos.
2. **Ausencia de retroalimentación de posición física:** El sistema asume que la compuerta se mueve porque envía los pulsos de microsegundos a los servos. Si un obstáculo traba mecánicamente la puerta sin tocar los sensores, el microcontrolador no puede detectar la traba (ausencia de encoders de eje o sensores de corriente de sobrecarga).
3. **Limitación óptica del sensor infrarrojo:** El sensor IR interior utilizado detecta la proximidad puntual del coche estacionado, pero su haz estrecho no constituye una cortina de fotocélulas de seguridad certificada bajo normas internacionales de portones (como la norma europea EN 12453). Por este motivo, el sistema no debe considerarse como un dispositivo con protección anti-atrapamiento humano certificada.
4. **Vulnerabilidad climática:** El módulo HC-SR04 no es estanco al agua (no cuenta con protección IP65). En un garaje expuesto a la intemperie en Paraguay, las lluvias intensas y la humedad estival deteriorarían los transductores piezoeléctricos abiertos. Se requeriría reemplazarlo por sensores ultrasónicos sellados del tipo JSN-SR04T.

## 5.7 Proyecciones y propuestas de escalamiento

Para futuras promociones del Bachillerato Técnico en Informática que deseen continuar esta línea de desarrollo tecnológico, se sugieren las siguientes mejoras:
* Incorporar una resistencia shunt y un amplificador operacional para medir la corriente consumida por los motores en tiempo real, permitiendo detectar un sobreesfuerzo mecánico por traba y detener la marcha de inmediato.
* Sustituir los transductores abiertos por sensores ultrasónicos impermeables para exteriores (JSN-SR04T).
* Implementar una barrera de fotocélulas infrarrojas tipo barrera de cruce (emisor y receptor enfrentados a ambos lados del vano) conectada a una interrupción externa de hardware del microcontrolador.
* Integrar un módulo de radiofrecuencia convencional de 433 MHz en paralelo, para permitir la apertura mediante mandos llavero tradicionales además del acceso móvil por Wi-Fi.

## 5.8 Conclusiones generales

En concordancia con los objetivos planteados al inicio de la investigación, se establecen las siguientes conclusiones:

1. **En relación con el Objetivo Específico 1 (Integración electromecánica):** Se diseñó e implementó exitosamente el circuito del prototipo, logrando una completa estabilidad operativa mediante el desacoplamiento de fuentes (4 pilas AA con regulador de 5V para potencia y USB para lógica) y el uso de un capacitor de 470 µF, eliminando por completo los reinicios por *brownout* que afectaban las versiones preliminares de taller.
2. **En relación con el Objetivo Específico 2 (Firmware embebido):** Se programó una Máquina de Estados Finitos en C++ libre de funciones bloqueantes, que demostró una gestión determinista de los tiempos de confirmación (300 ms), histéresis espacial (5 cm) y una política de seguridad estricta que inhabilita el cierre ante pérdidas de eco acústico.
3. **En relación con el Objetivo Específico 3 (Supervisión y control móvil):** Se implementó una arquitectura de supervisión en red local (LAN/Hotspot) que permite controlar el acceso vehicular desde cualquier teléfono celular mediante código QR y PIN sin depender de internet ni de servidores externos en la nube, garantizando la operatividad del sistema en el contexto paraguayo.
4. **En relación con el Objetivo Específico 4 (Evaluación experimental de distancias):** La batería de 40 ensayos aleatorizados demostró una efectividad del 100% de apertura dentro del rango útil (30/30 aciertos con un tiempo medio de 0,55 s) e inmunidad total a falsos positivos a 35 cm, respaldando sólidamente la hipótesis operativa de investigación.
5. **En relación con el Objetivo Específico 5 (Pruebas de contingencia y memoria):** El prototipo demostró robustez ante anomalías físicas, abortando el cierre y reabriendo en menos de 0,2 s ante obstáculos, bloqueándose ante falta de eco y preservando intactos sus parámetros de calibración y banderas de pausa en la memoria EEPROM tras cortes completos de energía.

## 5.9 Recomendaciones pedagógicas y técnicas

* **A las autoridades del colegio y ferias de ciencias:** Fomentar la adopción de protocolos experimentales con aleatorización y registro de datos crudos en los proyectos de robótica escolar, superando la tendencia a evaluar maquetas únicamente por su aspecto estético o demostraciones individuales no documentadas.
* **A los estudiantes de futuros cursos de BTI:** Mantener una bitácora física de taller desde el primer día de trabajo, registrando con honestidad cada componente quemado, cada reinicio de microcontrolador y cada fallo de código, pues es en la resolución de esos problemas imprevistos donde se consolida el verdadero aprendizaje técnico.

---

---

# BLOQUE DE CARPETA DE CAMPO

## 1. Cronograma general de actividades escolares (Diagrama de Gantt)

El proyecto se desarrolló de manera continua a lo largo del año lectivo 2026, abarcando 28 semanas de trabajo distribuidas en las siguientes fases:

| Fase / Actividad Principal | Abr | May | Jun | Jul | Ago | Set | Oct | Responsables |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **1. Definición del problema y revisión bibliográfica** | ██ | | | | | | | Todo el equipo |
| **2. Adquisición de componentes y cotizaciones locales** | | ██ | | | | | | Encargado de logística |
| **3. Diseño y armado de la maqueta física en MDF** | | ██ | ██ | | | | | Área mecánica |
| **4. Cableado preliminar y detección de fallas de reset** | | | ██ | | | | | Área de hardware |
| **5. Rediseño con desacoplamiento de fuentes (Pilas + 5V)**| | | | ██ | | | | Área de hardware y tutor |
| **6. Programación del firmware FSM en Arduino (C++)** | | | | ██ | ██ | | | Área de software embebido |
| **7. Desarrollo del servidor Python y panel C#** | | | | | ██ | | | Área de software de interfaz |
| **8. Calibración angular y pruebas de doble servo** | | | | | ██ | ██ | | Todo el equipo |
| **9. Campaña experimental (40 ensayos aleatorizados)** | | | | | | ██ | | Todo el equipo |
| **10. Pruebas de contingencia y análisis estadístico** | | | | | | ██ | ██ | Área metodológica |
| **11. Redacción del informe final y carpeta de campo** | | | | | | | ██ | Todo el equipo |
| **12. Preparación y defensa en stand de SERCAP 2026** | | | | | | | ██ | Todo el equipo |

## 2. Bitácora de taller y registro vivencial de incidentes técnicos (12 hitos)

A continuación se transcribe la bitácora cronológica con los principales desafíos prácticos enfrentados durante la construcción y ensayo de la maqueta:

* **Hito 1 (18 de abril de 2026):** *Formulación inicial.* Decidimos abordar el problema de la inseguridad vehicular en Luque. Revisamos el antecedente de la UNAE sobre control por voz y descartamos esa idea por la inestabilidad de internet y el ruido ambiente. Acordamos usar sensor de distancia ultrasónico y Arduino UNO.
* **Hito 2 (09 de mayo de 2026):** *Compras y primeras pruebas.* Compramos los componentes en Casa de la Electrónica y RobotStore. Armamos el circuito en protoboard utilizando la biblioteca estándar `Servo.h` de Arduino.
* **Hito 3 (23 de mayo de 2026):** *Construcción de la compuerta.* En el taller de carpintería escolar cortamos las piezas de MDF de 3 mm. La compuerta pesó 100 gramos. Colocamos bisagras livianas y un eje de alambre dulce conectado a un servo SG90.
* **Hito 4 (12 de junio de 2026):** *El grave problema del Brownout Reset.* Al energizar todo desde el puerto USB de la computadora y ordenar la apertura, la placa Arduino se reseteaba instantáneamente al momento de encender el motor. Descubrimos con el multímetro que la tensión caía a 3,8 V debido al pico de corriente del servo.
* **Hito 5 (04 de julio de 2026):** *Solución de desacoplamiento de alimentación.* Siguiendo las indicaciones del docente tutor, agregamos un portapilas con 4 pilas AA alcalinas, un módulo regulador conmutado calibrado a 5,0 V exclusivo para los motores y un capacitor de 470 µF. Unimos los GND de las pilas y del Arduino. Los reseteos desaparecieron por completo.
* **Hito 6 (25 de julio de 2026):** *Conflicto mecánico al sumar un segundo servo.* Intentamos colocar un segundo servo en el lado opuesto del eje para darle más fuerza. Al enviar la misma orden de grados, los servos hicieron fuerza en sentidos contrarios y casi barrieron los piñones plásticos. Comprendimos que uno debía girar en sentido horario y el otro en antihorario.
* **Hito 7 (08 de agosto de 2026):** *Implementación de la FSM y control en microsegundos.* Rediseñamos el código en `control.h` utilizando `writeMicroseconds()` en lugar de grados enteros y programamos una máquina de estados con `millis()`. Configuramos extremos independientes en EEPROM para cada servo.
* **Hito 8 (22 de agosto de 2026):** *El problema de los rebotes ultrasónicos oblicuos.* Al probar la maqueta cerca de una pared del laboratorio, el sensor HC-SR04 captaba ecos laterales fantasma que abrían la puerta sola. Agregamos en el firmware la ventana de confirmación acumulativa de 300 ms y la histéresis de 5 cm. Los falsos disparos se eliminaron.
* **Hito 9 (05 de septiembre de 2026):** *Desarrollo de la interfaz web por QR.* Programamos el servidor local en Python con autenticación de PIN aleatorio y generación de QR en pantalla. Logramos conectar el celular mediante la función Hotspot sin necesidad de internet en el colegio.
* **Hito 10 (19 de septiembre de 2026):** *Campaña de los 40 ensayos experimentales.* Fabricamos las 40 tarjetas de sorteo y filmamos con el teléfono a 30 fps las 40 corridas de aproximación vehicular a 5, 15, 20 y 35 cm. Completamos la planilla de datos crudos sin alterar ningún resultado.
* **Hito 11 (26 de septiembre de 2026):** *Pruebas de contingencia y estrés de EEPROM.* Provocamos desalineaciones de eco y cortes de energía en estado de pausa. Verificamos que el Arduino retiene la configuración y no realiza movimientos bruscos imprevistos al reconectarse.
* **Hito 12 (03 de octubre de 2026):** *Ensamblaje final y práctica de defensa.* Fijamos de forma permanente el cableado en la maqueta, organizamos la carpeta de campo y realizamos una simulación de defensa ante nuestros compañeros de curso.

## 3. Ficha de laboratorio del ensayo unitario representativo

A continuación se documenta la ficha de ensayo correspondiente a una de las corridas críticas de la campaña experimental:

| Parámetro de Registro | Detalle del Ensayo |
| :--- | :--- |
| **Identificador del Ensayo:** | Corrida N.º 24 (Tarjeta de sorteo 28) |
| **Fecha y Hora:** | 19 de septiembre de 2026 — 15:42 hs. |
| **Lugar de Ejecución:** | Laboratorio de Informática BTI — Centro Educativo La Esperanza |
| **Operadores Responsables:** | Equipo de Alumnos 3.º BTI (Mesa de banco 2) |
| **Docente Supervisor:** | Profesor Orientador de Robótica Científica |
| **Condición Experimental:** | Distancia de referencia: **20,0 cm** (Límite superior del rango activo) |
| **Vehículo de Prueba:** | Modelo a escala de plástico rígido, trompa rectangular de 6 cm de ancho |
| **Condiciones Ambientales:** | Temperatura: 24 °C (Interior con ventilación) — Iluminación fluorescente |
| **Tensión de Alimentación:** | Lógica Arduino: 5,02 V (vía USB) — Potencia Servos: 5,11 V (vía pilas + buck) |
| **Lectura Cruda del Sensor:** | Distancia reportada por software: **20 cm** — Validez de eco: **VÁLIDA** |
| **Cronometraje por Video:** | Cuadro de colocación estable ($t_0$): Cuadro #142  |
| | Cuadro de primer movimiento visible de hoja: Cuadro #164 |
| | Cuadros transcurridos: $164 - 142 = 22\text{ cuadros}$ |
| | Cálculo de latencia: $22\text{ cuadros} / 30\text{ fps} = **0,733 s**$ |
| **Criterio de Aceptación:** | Apertura dentro de $2,0\text{ s} \implies$ **CUMPLE SATISFACTORIAMENTE** |
| **Comportamiento Mecánico:** | Movimiento de apertura completo y suave sin zumbido ni calentamiento |
| **Veredicto Técnico:** | **ENSAYO CONFORME** |

## 4. Lista de chequeo (Checklist) para stand de feria SERCAP 2026

* [x] Maqueta física limpia con estructura de MDF y compuerta correctamente nivelada.
* [x] Portapilas con 4 pilas AA nuevas verificadas con multímetro (V_total ≥ 6,0 V).
* [x] Regulador DC-DC ajustado y medido exactamente a 5,0 V en bornes de servos.
* [x] Cable USB de comunicación serie conectado a la computadora portátil del stand.
* [x] Servidor Python ejecutándose en segundo plano en el puerto `8773`.
* [x] Pantalla de panel mostrando código QR nítido para que los jueces escaneen con sus celulares.
* [x] Teléfono celular del equipo configurado como Punto de Acceso (Hotspot) sin contraseña conflictiva.
* [x] Vehículo de prueba a escala y cinta métrica fijada sobre la mesa para demostración a los evaluadores.
* [x] Carpeta de campo física impresa con firmas de bitácora y planillas de datos crudos.
* [x] Copia digital del código fuente de Arduino disponible en pantalla para consultas técnicas del jurado.

---

---

# REFERENCIAS BIBLIOGRÁFICAS

* Arduino. (s. f.-a). *EEPROM Library V2.0 for Arduino* [Documentación oficial de software]. GitHub. https://github.com/arduino/ArduinoCore-avr/blob/master/libraries/EEPROM/README.md
* Arduino. (s. f.-b). *Servo library* [Documentación oficial de software]. GitHub. https://github.com/arduino-libraries/Servo/blob/master/docs/api.md
* Arduino. (s. f.-c). *UNO R3* [Documentación oficial de hardware]. https://docs.arduino.cc/hardware/uno-rev3/
* Burgos Delvalle, D., & Estigarribia Barreto, H. R. (2020). Domótica de bajo coste controlada por comandos de voz. *Revista Tecnología, Diseño e Innovación*, 5(1), 45-56. https://www.unae.edu.py/ojs/index.php/facat/article/view/155
* ELECFREAKS. (s. f.). *Ultrasonic ranging module HC-SR04* [Hoja de datos técnicos de producto]. https://www.elecfreaks.com/download/HC-SR04.pdf
* Instituto Nacional de Estadística [INE]. (2025). *Tecnología de la información y comunicación en el Paraguay 2024*. Publicación institucional oficial (junio de 2025). https://www.ine.gov.py/Publicaciones/Biblioteca/documento/280/Tics%202024_INE.pdf
* Instituto Nacional de Tecnología, Normalización y Metrología [INTN]. (1996). *Norma Paraguaya NP 2 028 96: Instalaciones Eléctricas de Baja Tensión*. Asunción, Paraguay.
* Keyestudio. (s. f.). *KS0051 keyestudio infrared obstacle avoidance sensor* [Documentación técnica de sensor]. Wiki Keyestudio. https://wiki.keyestudio.com/Ks0051_keyestudio_Infrared_Obstacle_Avoidance_Sensor
* NIST/SEMATECH. (s. f.). *Completely randomized designs*. En *e-Handbook of Statistical Methods* (Sección 5.3.3.1). National Institute of Standards and Technology. https://www.itl.nist.gov/div898/handbook/pri/section3/pri331.htm
* Piensa E.A.S. (2026, 13 de abril). *Pequeños Inventores* [Póster científico y reporte pedagógico]. Repositorio Institucional CONACYT. https://repositorio.conacyt.gov.py/handle/20.500.14066/4798
* Tower Pro. (s. f.). *SG90 Digital* [Ficha técnica de micro servomotor]. https://towerpro.com.tw/product/sg90-7/

---

---

# ANEXOS TÉCNICOS

## ANEXO A: Diagrama de flujo de la Máquina de Estados Finitos (FSM)

```text
       ┌────────────────────────┐
       │     ESTADO_CERRADO     │◄─────────────────────────────────┐
       └───────────┬────────────┘                                  │
                   │ Detección 3-25 cm                             │
                   │ por 300 ms continuos                          │
                   ▼                                               │
       ┌────────────────────────┐                                  │
       │     ESTADO_ABRIENDO    │                                  │
       └───────────┬────────────┘                                  │
                   │ Recorrido completo (2.500 ms)                 │
                   ▼                                               │
       ┌────────────────────────┐                                  │
       │  ESTADO_ESPERA_ABIERTO │                                  │
       └───────────┬────────────┘                                  │
                   │ Exterior libre (>30 cm) por 1.500 ms          │
                   │ y tiempo agotado (5 s con IR / 10 s sin IR)   │
                   ▼                                               │
       ┌────────────────────────┐  Obstáculo detectado             │
       │    ESTADO_CERRANDO     ├──────────────────────────┐       │
       └───────────┬────────────┘  o pérdida de eco        │       │
                   │                                       ▼       │
                   │                                ┌────────────┐ │
                   │ Fin de recorrido (2.500 ms)    │  REAPERTURA│ │
                   └───────────────────────────────►│  INMEDIATA ├─┘
                                                    └────────────┘
```

## ANEXO B: Tabla de correspondencia entre microsegundos y grados nominales

La siguiente matriz documenta la equivalencia lineal implementada en la interfaz v1.0.1 para facilitar la calibración de taller:

| Ángulo Nominal (°) | Pulso Equivalente (µs) | Descripción Operativa de la Posición |
| :---: | :---: | :--- |
| **0,00°** | **700 µs** | Límite inferior absoluto admisible por el firmware |
| **22,50°** | **900 µs** | Posición cerrada típica de Servo 1 en ángulo agudo |
| **45,00°** | **1.100 µs** | Posición estándar de cierre de compuerta en banco |
| **67,50°** | **1.300 µs** | Posición intermedia de prueba de recorrido |
| **90,00°** | **1.500 µs** | Punto neutro central de los servomotores SG90 |
| **112,50°** | **1.700 µs** | Posición de elevación parcial |
| **135,00°** | **1.900 µs** | Posición estándar de apertura completa de compuerta |
| **157,50°** | **2.100 µs** | Posición cerrada típica de Servo 2 (movimiento opuesto) |
| **180,00°** | **2.300 µs** | Límite superior absoluto admisible por el firmware |

## ANEXO C: Extracto comentado del firmware (`control.h`)

```cpp
/*
 * =====================================================================
 * GARAJE INTELIGENTE UNO - Logica Central del Controlador Embebido
 * Modulo: control.h (Version GARAGE-1.0.1)
 * =====================================================================
 * Regla de seguridad esencial:
 * Si la lectura no es valida o no se recibe eco, nunca se considera
 * via libre; el cierre se inhibe preventivamente.
 */

struct LecturaDistancia {
  uint16_t distancia_cm;
  bool valida;
  bool sin_eco;
};

// Evaluacion de condicion de apertura en reposo
bool evaluar_apertura_exterior(const LecturaDistancia& lectura, uint32_t tiempo_en_rango_ms) {
  if (!lectura.valida || lectura.sin_eco) {
    return false; // Error acústico no habilita apertura
  }
  // Rango activo: entre 3 y 25 cm sostenido por 300 ms
  return (lectura.distancia_cm >= RANGO_MIN_CM &&
          lectura.distancia_cm <= RANGO_MAX_CM &&
          tiempo_en_rango_ms >= CONFIRMACION_APERTURA_MS);
}

// Evaluacion de condicion de seguridad para cierre
bool evaluar_exterior_libre(const LecturaDistancia& lectura, uint32_t tiempo_libre_ms) {
  // SALVAGUARDA CRITICA: Falta de eco NUNCA es exterior libre
  if (!lectura.valida || lectura.sin_eco) {
    return false;
  }
  // Histeresis: requiere distancia mayor a maximo + margen (25 + 5 = 30 cm)
  const uint16_t UMBRAL_LIBRE = RANGO_MAX_CM + MARGEN_HISTERESIS_CM;
  return (lectura.distancia_cm > UMBRAL_LIBRE &&
          tiempo_libre_ms >= CONFIRMACION_LIBRE_MS);
}
```
