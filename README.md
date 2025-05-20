# ♟️ Proyecto Ajedrez 3D en Unity

Este es un proyecto de ajedrez 3D desarrollado en Unity, con dos versiones completamente funcionales:

1. 🎮 **Versión clásica (base)**: jugable con teclado y ratón.
2. 🥽 **Versión en realidad virtual (VR)**: jugable con visores como Meta Quest 2 y controladores VR.

Las instrucciones detalladas y requisitos específicos están en los README de cada rama:

---

## 🧭 Acerca del repositorio

La rama `main` contiene el **estado más reciente y actualizado del proyecto**, que actualmente corresponde a la versión VR.

El desarrollo se organiza en ramas separadas para mantener claridad y facilidad de uso:

| Rama | Descripción |
|------|-------------|
| [`base-feature`](https://github.com/Unicromus/P.MR.Chess/tree/base-feature) | Versión clásica sin realidad virtual. |
| [`vr-feature`](https://github.com/Unicromus/P.MR.Chess/tree/vr-feature) | Versión con soporte completo para realidad virtual. |
| `main` | Última versión del proyecto, actualmente sincronizada con `vr-feature`. |

---

## 📂 Builds disponibles

Puedes encontrar compilaciones listas para usar en la sección de [Releases](https://github.com/Unicromus/P.MR.Chess/releases), con las siguientes versiones:

### 🖱️ Desktop Build (Versión Base)
- Ejecuta la **versión clásica del juego**, controlado con **teclado y ratón**.
- No requiere visor de realidad virtual.
- Incluye todas las funcionalidades: IA, reloj digital, guardado/carga FEN, reposicionar piezas, cámara aérea, selección de mesa, etc.

### 🖥️ Desktop Build (With Simulator)
- Juega sin visor VR, usando el **simulador XR** incluido.
- Ideal para pruebas rápidas en PC.

### 🖥️ Desktop Build (Requiere visor)
- Ejecuta el proyecto en PC con un visor VR conectado (por ejemplo, Meta Quest 2 + AirLink/Link).

### 🤖 Android Build
- Instala directamente el proyecto en visores autónomos como **Meta Quest 2**.
- No requiere conexión a un PC.

---

## 🔀 Comparativa de versiones

| Característica | Versión clásica | Versión VR |
|----------------|------------------|-------------|
| Jugar contra IA | ✅ | ✅ |
| Guardar/cargar FEN | ✅ | ✅ |
| Reloj interactivo | ✅ | ✅ (VR compatible) |
| Cambiar cámara desde el menú | ✅ | ❌ (XR Rig reemplaza la cámara) |
| Interfaz VR | ❌ | ✅ |
| Manos y movimiento VR | ❌ | ✅ |

---

## 🎓 Requisitos de desarrollo

- **Unity** (versión recomendada o superior a: `2022.3.62f1`)
- Para VR:
  - Módulos Android (OpenJDK, SDK, NDK)
  - OpenXR, XR Plugin Management
  - XR Interaction Toolkit

---

## 📜 Licencia

Este proyecto está distribuido bajo la licencia **MIT**.

---

## 📧 Contacto

Para dudas o sugerencias, puedes abrir un [issue](https://github.com/Unicromus/P.MR.Chess/issues).
