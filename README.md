# 🌙 Moonfield

> **Trabajo de Fin de Grado** — Prototipo experimental de videojuego 2D para comparar el impacto en la experiencia de usuario de dos paradigmas de interacción con NPCs: **árboles de diálogo clásicos** frente a **conversación libre impulsada por LLM**.

![Unity](https://img.shields.io/badge/Unity-6000.3.10f1-black?logo=unity)
![Language](https://img.shields.io/badge/Lenguaje-C%23-239120?logo=csharp)
![API](https://img.shields.io/badge/API-OpenAI%20GPT--4o--mini-412991?logo=openai)
![Platform](https://img.shields.io/badge/Plataforma-Windows-0078D4?logo=windows)

---

## 📋 Tabla de Contenidos

- [Sobre el Proyecto](#-sobre-el-proyecto)
- [Características del Juego](#-características-del-juego)
- [Modos de Juego](#-modos-de-juego)
- [Stack Tecnológico](#-stack-tecnológico)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Configuración y Puesta en Marcha](#-configuración-y-puesta-en-marcha)
- [Sistema de Métricas](#-sistema-de-métricas)
- [Contexto Académico](#-contexto-académico)
- [Autora](#-autora)

---

## 🎯 Sobre el Proyecto

**Moonfield** es un videojuego 2D *top-down* desarrollado en **Unity 6** como artefacto principal de un Trabajo de Fin de Grado. El prototipo permite comparar, bajo las mismas condiciones de juego, dos formas de interacción con personajes no jugadores (NPCs):

- **Modo Árbol Clásico** — el jugador elige entre opciones predefinidas en un árbol de diálogo.
- **Modo IA** — el jugador escribe texto libre que se procesa en tiempo real mediante la API de OpenAI.

El jugador explora un entorno 2D, conversa con la NPC **Pam** para conseguir el objeto clave del juego, y resuelve el enigma del cofre final. Dependiendo del modo activo, la experiencia de esa conversación es radicalmente diferente — esa diferencia es precisamente el objeto de estudio del TFG.

---

## ✨ Características del Juego

| Característica | Descripción |
|---|---|
| 🗺️ **Exploración 2D** | Movimiento top-down en 8 direcciones con física `Rigidbody2D` |
| 💬 **Sistema de diálogo dual** | Árbol de opciones clásico **o** chat libre con LLM, intercambiable desde el menú principal |
| 🕐 **Ciclo día/noche** | Reloj interno con gradiente de color que simula amanecer, día, atardecer y noche |
| 🎒 **Inventario visual** | Sistema de slots con iconos para recoger y gestionar objetos del mundo |
| 🔐 **Cofre final con contraseña** | Mecanismo de victoria con validación de código y sistema de pistas progresivas |
| 📊 **Registro de métricas** | Generación automática de reportes `.txt` por sesión, persistidos en disco |
| 🎵 **Música continua** | Singleton de `AudioSource` que persiste entre cambios de escena sin interrupciones |
| ⏸️ **Menú de pausa** | Pausa del juego accesible en cualquier momento durante la partida |

---

## 🎮 Modos de Juego

El modo de juego se selecciona en el **menú principal** y se persiste en `PlayerPrefs`. Al cargar la escena de juego, `GestorModoJuego` lee esa preferencia y la propaga a todos los `NPCController` presentes en la escena.

### 🌲 Modo Árbol Clásico

La conversación con Pam discurre por **nodos predefinidos** (`NodoDialogo`). El jugador elige entre las opciones disponibles mediante botones. Cada elección suma o resta puntos de **amistad** (`amistadPam`); al llegar al nodo terminal, el sistema evalúa si el total es positivo para determinar si Pam entrega el objeto clave o lo deja inaccesible en el mapa.

### 🤖 Modo IA (LLM)

La conversación con Pam es completamente abierta. El jugador escribe texto libre en un `InputField`, que se envía a la API de OpenAI (`gpt-4o-mini`). El LLM responde en rol, ajustándose a la personalidad definida en `npcLore`, e incluye **etiquetas ocultas** (`etiquetaExito` / `etiquetaFracaso`) que el sistema detecta para determinar el desenlace sin exponerlas al jugador. El jugador dispone de **3 intentos** antes de que el juego fuerce un fracaso por agotamiento.

---

## 🛠️ Stack Tecnológico

| Tecnología | Uso |
|---|---|
| **Unity 6** `6000.3.10f1` | Motor de juego y editor |
| **C#** | Lenguaje de scripting |
| **TextMeshPro** | Renderizado de texto en UI |
| **Unity UI (Canvas)** | Sistema de interfaz: paneles, botones, slots de inventario |
| **Rigidbody2D / Collider2D** | Física y detección de colisiones en 2D |
| **OpenAI API** · `gpt-4o-mini` | Generación de diálogo libre en Modo IA |
| **UnityWebRequest** | Peticiones HTTP a la API de OpenAI |
| **JsonUtility** | Serialización/deserialización de peticiones y respuestas JSON |
| **Unity Legacy Input System** | Lectura de input de teclado y ratón |
| **PlayerPrefs** | Persistencia de preferencias de modo entre sesiones |
| **System.IO.File** | Escritura de reportes de métricas en disco |

---

## 📁 Estructura del Proyecto

```
Assets/
└── Scripts/
    ├── Dialogos/
    │   ├── ActivadorDialogoPorPasos.cs   # Desencadena diálogos en secuencia condicional
    │   ├── ControladorDialogo.cs         # Flujo del árbol clásico y puntuación de amistad
    │   ├── NodoDialogo.cs                # Datos de un nodo: texto, opciones, cambios de amistad
    │   └── CofreVictoria/
    │       └── CofreVictoria.cs          # Lógica del cofre final: contraseña, éxito y pistas
    ├── Horario/
    │   └── RelojJuego.cs                 # Reloj in-game con ciclo día/noche por gradiente
    ├── Interfaz/
    │   ├── CambiarCursor.cs              # Cursor personalizado según contexto
