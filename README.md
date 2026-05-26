# 🍲 Caravan Kitchen

> *Explora la bruma, captura ingredientes vivos, cocina recetas mágicas y expande tu cocina viajera.*

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20iOS-green?logo=android)
![Status](https://img.shields.io/badge/Status-Pre--Production%20MVP-orange)
![License](https://img.shields.io/badge/License-MIT-blue)

---

## 🎮 ¿Qué es Caravan Kitchen?

**Caravan Kitchen** es un juego móvil 2D cozy de exploración, captura y cocina. El jugador controla a **Kiri**, una chef-exploradora nómada que viaja por regiones flotantes cubiertas de bruma para capturar criaturas culinarias mágicas y recolectar ingredientes raros. Todo lo recolectado se transforma en platillos dentro de una cocina-caravana ambulante que el jugador vende, mejora y expande para desbloquear nuevas rutas y recetas.

### Loop principal

```
Explorar región → Capturar criaturas → Recolectar ingredientes
        ↓
Volver a la Caravana → Cocinar recetas → Vender platillos
        ↓
Mejorar herramientas → Desbloquear nueva región → Repetir
```

---

## 📁 Estructura del proyecto

```
caravan-kitchen/
├── docs/                    ← GDD, arte conceptual, referencias
│   ├── GDD.md               ← Game Design Document completo
│   ├── CREATURES.md         ← Bestiario completo
│   ├── RECIPES.md           ← Recetario completo
│   ├── ECONOMY.md           ← Sistema económico
│   └── ROADMAP.md           ← Roadmap de desarrollo
├── Assets/                  ← Carpeta raíz de Unity
│   ├── _Scripts/            ← Todos los scripts C#
│   ├── _Data/               ← ScriptableObjects
│   ├── Art/                 ← Sprites, UI, VFX
│   ├── Audio/               ← Música y SFX
│   ├── Prefabs/             ← Prefabs del juego
│   └── Scenes/              ← Escenas Unity
├── ProjectSettings/         ← Configuración Unity (no editar manual)
└── README.md
```

---

## 🌍 Regiones del juego

| Región | Tema | Estado |
|---|---|---|
| 🌾 Pradera de Bruma | Zona inicial, suave y tutorial | MVP |
| 🌿 Bosque de Vapor Dulce | Vegetación cálida, néctares | Post-MVP |
| ☁️ Arrecife de Nubes | Zona aérea, peces-nube | Post-MVP |
| 🔥 Barranco del Caldero | Zona media exigente, especias | Futuro |
| 🏮 Mercado Suspendido | Hub social, pedidos especiales | Futuro |

---

## 🍳 Sistemas principales

- **Exploración 2D** — Side-scrolling con plataformas, paralaje y ciclo de tiempo
- **Captura de criaturas** — Cebos, redes, campanas y trampas por tipo
- **Recolección de ingredientes** — Plantas, hongos, néctares, minerales
- **Cocina por recetas** — Caldero, parrilla, mesa dulce, secador, molino
- **Sistema de pedidos** — Clientes normales, especiales, raros y de evento
- **Mejora de caravana** — Visual y funcional, 5 niveles de evolución
- **Bestiario y recetario** — Colección progresiva
- **Monetización opcional** — Rewarded ads + IAP cosmético

---

## 🛠️ Stack tecnológico

| Herramienta | Uso |
|---|---|
| Unity 2022.3 LTS | Motor de juego |
| C# | Lenguaje de scripting |
| Unity 2D URP | Pipeline de renderizado |
| ScriptableObjects | Data del juego |
| Addressables | Carga de escenas por región |
| AdMob SDK | Rewarded ads opcionales |
| Android Build Support | Exportación a Google Play |

---

## 👤 Personajes

### Kiri — Chef Exploradora
Protagonista principal. Silueta redonda y amigable, sombrero de cocina con goggles, mochila-caldero, bufanda larga, red plegable al costado.

### Fumi — Compañero Nubecita
Nubecita con ojos brillantes. Brilla al detectar criaturas raras, señala ingredientes ocultos y guía tutoriales.

---

## 📊 Estado del MVP

- [x] GDD completo
- [x] Estructura de repositorio
- [x] Scripts base (ScriptableObjects)
- [x] PlayerController
- [x] Sistema de inventario
- [x] Sistema de criaturas
- [x] Sistema de cocina
- [x] Sistema de pedidos
- [x] Sistema económico
- [ ] Arte de personajes
- [ ] Primer mapa jugable
- [ ] Build Android

---

## 📖 Documentación

- [📋 GDD Completo](docs/GDD.md)
- [🐾 Bestiario](docs/CREATURES.md)
- [🍽️ Recetario](docs/RECIPES.md)
- [💰 Economía](docs/ECONOMY.md)
- [🗺️ Roadmap](docs/ROADMAP.md)

---

## 📄 Licencia

MIT License — Ver [LICENSE](LICENSE)

---

*Caravan Kitchen — Desarrollado con Unity para Android • 2026*
