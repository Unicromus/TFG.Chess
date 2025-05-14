# ♟️ Ajedrez en Unity 3D (Versión Base)

Esta es la versión estándar del juego de ajedrez desarrollada en **Unity 3D**, sin compatibilidad con realidad virtual. Utiliza **teclado y ratón** para interactuar con el entorno. Cuenta con físicas semi-realistas y modelos 3D creados en **Blender**.

## 🎮 Características

### ✅ Modos de juego:
- Jugar solo, controlando ambos equipos.
- Jugar contra una **IA aleatoria**, sin estrategia.
- Jugar contra una **IA agresiva**, que realiza movimientos aleatorios, pero ataca si tiene la opción.

### ✅ Opciones del menú Canvas:
- Elegir el **modo de juego** y el **equipo**.
- **Mover la cámara** a vista aérea para observar el tablero desde arriba.
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

## 📂 Descarga

Esta rama corresponde a la **versión base sin VR**. Puedes clonar o descargar directamente desde:

- [Versión Base (base-feature)](https://github.com/Unicromus/P.MR.Chess/tree/base-feature)

También puedes descargar una versión compilada en la sección de [Releases](https://github.com/Unicromus/P.MR.Chess/releases).

## 🕹️ Instrucciones para jugar

### Si descargaste la versión compilada:

1. Ve a la sección de [Releases](https://github.com/Unicromus/P.MR.Chess/releases) y descarga el archivo `.zip` correspondiente a la versión base.
2. Extrae el contenido en una carpeta de tu PC.
3. Ejecuta `Ajedrez.Base.exe` para jugar.

### Controles del juego:

#### 🖱️ Ratón (con raycast):

* Interactúa con:

  * El **menú Canvas** (selección de modo de juego, equipo, mesa, cámara, etc.).
  * Las **piezas de ajedrez** (clic para seleccionar y mover).
  * Los **botones del reloj** (iniciar, pausar, resetear, pasar turno).

#### ⌨️ Teclado:

* Escribe en el **campo FEN** para cargar un estado personalizado del tablero.

### Flujo básico de juego:

1. **Selecciona el modo de juego y el equipo** desde el menú principal.
2. **Inicia el reloj** para comenzar la partida.
3. A partir de ese momento puedes **mover las piezas** con el ratón.
4. **Pasa el turno** entre jugadores haciendo clic en los botones superiores del reloj.
5. Pulsa el botón **"Opciones"** del menú Canvas para acceder a:

   * **Guardar o cargar** el estado del tablero (mediante notación FEN).
   * **Resetear la partida** para empezar de cero.
   * **Reposicionar las piezas** a su lugar original (útil si se descolocaron por físicas).
   * **Cambiar la mesa** entre tres estilos disponibles.
   * **Cambiar la cámara** a vista aérea.
   * **Volver al menú principal**.

## 🛠️ Requisitos

- **Unity 2022.3.14f1 LTS** (solo si quieres abrir el proyecto en el editor).
- Compatible con **Windows** (interacción con teclado y ratón).

## 📜 Licencia

Este proyecto está bajo la licencia **MIT**.

## 📧 Contacto

Si tienes dudas o sugerencias, puedes abrir un [issue](https://github.com/Unicromus/P.MR.Chess/issues). Probablemente tarde en responder.
