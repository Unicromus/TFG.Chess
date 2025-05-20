# ♟️ Ajedrez en Unity 3D (Versión VR)

Esta es la **versión VR** del juego de ajedrez desarrollado en **Unity 3D**, diseñada para ser jugada en realidad virtual. Utiliza el visor **Meta Quest 2** (y otros dispositivos compatibles con OpenXR) y el **XR Interaction Toolkit** para interactuar de manera natural con las piezas de ajedrez, los menús y los controles del reloj.

## 🎮 Características

### ✅ Modos de juego:
- Jugar solo, controlando ambos equipos.
- Jugar contra una **IA aleatoria**, sin estrategia.
- Jugar contra una **IA agresiva**, que realiza movimientos aleatorios, pero ataca si tiene la opción.

### ✅ Opciones del menú Canvas (VR):
- **Seleccionar el modo de juego y el equipo**.
- **Cambiar la mesa de ajedrez** entre tres opciones:
  - Mesa cuadrada de **madera clara**.
  - Mesa redonda de **madera oscura**.
  - Mesa cuadrada de **cristal**.
- **Resetear la partida** para empezar de cero.
- **Reposicionar las piezas** en su lugar original sin reiniciar (útil si las físicas las mueven).
- **Guardar y cargar el estado del tablero** con notación **FEN** (Forsyth-Edwards Notation).

### ✅ Reloj digital de ajedrez:
- Dos cronómetros que cuentan el tiempo de cada jugador desde **10 minutos** hasta **0**.
- Si el tiempo restante es menor a **30 segundos**, cada movimiento **añade 3 segundos** automáticamente.
- Botones interactivos:
  - Subir y bajar el tiempo de juego.
  - Pausar la partida.
  - Iniciar o reanudar el reloj.
  - Resetear el reloj.
  - Dos botones superiores para **pasar el turno** al otro jugador.

### ✅ Gráficos y físicas:
- **Modelos 3D** del tablero, mesa, piezas y reloj diseñados en **Blender**.
- Aplicación de **texturas y materiales realistas**.
- **Iluminación HDRI** y **Reflection Probe** para reflejos precisos.
- **Sombras dinámicas** y físicas avanzadas con **Rigidbodies y Colliders**.

### ✅ Nuevas características para VR:

#### **1. Simulador de VR:**
- Se ha añadido un **Simulador de VR** como prefab para probar el juego en PC sin necesidad de un visor físico.
- **Desactivarlo** antes de hacer el build para dispositivos VR.

#### **2. Cámara y movimiento:**
- **XR Origin (XR Rig)** ha sido implementado para la compatibilidad con VR, eliminando el uso de la cámara estándar.
- Se ha configurado un **sistema de locomoción continua** (mover al personaje mediante el controlador izquierdo) y un **sistema de giro continuo** (girar el personaje mediante el controlador derecho).

#### **3. Modelos de manos:**
- Se han añadido modelos de **manos con animaciones** para una experiencia de inmersión total.

#### **4. Interacción con las piezas:**
- Los **Raycasts** de la versión base han sido reemplazados por dos **Raycasts verticales** desde cada controlador de mano. Esto permite detectar las baldosas y piezas al pasar las manos por encima de ellas.
- Las piezas ahora se agarran y mueven utilizando los controladores VR gracias al **XR Grab Interactable** y el **Direct Interactor**.

#### **5. Interacción con el reloj:**
- Se han implementado botones interactivos para el reloj utilizando **XR Ray Interactor** y **XR Simple Interactable**.
- Los botones ahora responden mediante **Event Select Entered** en lugar de usar **OnMouseUpAsButton()**.

#### **6. VR Menú:**
- El **Canvas** del menú ha sido reestructurado para utilizar **World Space**.
- Se ha mejorado el **tamaño de los botones y el texto** para que sean más legibles y fáciles de interactuar con los controladores.
- El menú es ahora interactuable con los controladores de mano mediante el **VRGameMenuManager**.
  
#### **7. Teclado espacial:**
- Se ha añadido un **Teclado Espacial** (Spatial Keyboard - MRTK) para poder introducir la notación **FEN** en el juego en VR.

#### **8. Optimización del proyecto:**
- Se han realizado algunas optimizaciones para mejorar el rendimiento, especialmente con las sombras.

## 📂 Descarga

Esta rama corresponde a la **versión VR**. Puedes clonar o descargar directamente desde:

- [Versión VR (vr-feature)](https://github.com/Unicromus/P.MR.Chess/tree/vr-feature)

También puedes descargar una versión compilada en la sección de [Releases](https://github.com/Unicromus/P.MR.Chess/releases).

## 🕹️ Instrucciones para jugar

### Si descargaste la versión compilada:

1. Ve a la sección de [Releases](https://github.com/Unicromus/P.MR.Chess/releases) y descarga el archivo `.zip` correspondiente a la versión VR.
2. Extrae el contenido en una carpeta de tu PC.
3. Ejecuta `Ajedrez.VR.exe` para jugar.

### Controles del juego:

#### 🖱️ Controladores VR (con raycast):

* **Interacción con el menú Canvas** (selección de modo de juego, equipo, mesa, etc.).
* **Mover las piezas de ajedrez** (con las manos usando raycast y el XR Grab Interactable).
* **Botones del reloj** (iniciar, pausar, resetear, pasar turno, etc.).
* **Moverse por el entorno** (mediante la locomoción, controlador izquierdo movimiento y derecho giro).
* **Mover y ocultar el menú** (boton menú de los controladores izquierdo y derecho respectivamente).

#### 🖐️ Manos:

* **Agarrar y soltar piezas** con las manos usando el sistema de **XR Grab Interactable**.

#### ⌨️ Teclado (para FEN):

* **Escribir en el campo FEN** para cargar un estado personalizado del tablero (usando el teclado espacial VR).

### Flujo básico de juego:

1. **Selecciona el modo de juego y el equipo** desde el menú principal.
2. **Inicia el reloj** para comenzar la partida.
3. A partir de ese momento puedes **mover las piezas** con los controladores.
4. **Pasa el turno** entre jugadores haciendo clic en los botones superiores del reloj.
5. Pulsa el botón **"Opciones"** del menú Canvas para acceder a:

   * **Guardar o cargar** el estado del tablero (mediante notación FEN).
   * **Resetear la partida** para empezar de cero.
   * **Reposicionar las piezas** a su lugar original (útil si se descolocaron por físicas).
   * **Cambiar la mesa** entre tres estilos disponibles.
   * **Volver al menú principal**.

---

## 🧪 Builds disponibles

A continuación, se listan las distintas versiones compiladas del proyecto para diferentes plataformas y modos de uso:

### 🖥️ Desktop Build (With Simulator)

Versión para PC que permite jugar **sin visor de realidad virtual**.  
Incluye el **simulador XR**, ideal para probar el juego con teclado y ratón.  
✅ **Recomendado si no tienes visor VR y quieres probar el proyecto.**

### 🖥️ Desktop Build

Versión para PC sin simulador.  
Pensada para jugar con un visor VR compatible con **OpenXR**, conectado al ordenador (Meta Quest 2 con Link/AirLink).  
✅ **Requiere visor VR.**

### 🤖 Android Build

Versión compilada para instalarse directamente en visores autónomos como **Meta Quest 2**.  
✅ No requiere conexión a un PC.

---

## 🛠️ Requisitos

- **Unity 2022.3.60f1 LTS** o superior (solo si quieres abrir el proyecto en el editor).
- **Meta Quest 2** o cualquier otro dispositivo compatible con OpenXR.
- Compatible con **Windows**.

## Módulos Utilizados

- **Android Build Support** (Oculus/Meta Quest 2).
  - OpenJDK
  - Android SDK & NDK Tools

## Paquetes Utilizados

- **XR Plugin Management**
- **Open XR Plugin** (Estándar VR)
- **XR Interaction Toolkit**
  - Starter Assets (interacciones y XR Rig)
  - XR Device Simulator (para probar en PC sin visor VR)

## 📜 Licencia

Este proyecto está bajo la licencia **MIT**.

## 📧 Contacto

Si tienes dudas o sugerencias, puedes abrir un [issue](https://github.com/Unicromus/P.MR.Chess/issues). Probablemente tarde en responder.
