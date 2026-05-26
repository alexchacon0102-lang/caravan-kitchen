# 💎 CARAVAN KITCHEN — PROGRESS TRACKER v3.0

> **Motor:** Unity 6.3 LTS (6000.3.16f1) — Actualizado desde Unity 2022.3 LTS
> **Plataforma:** Android (Google Play) + iOS futuro
> **Última sesión:** Mayo 26, 2026
> Actualizar este archivo al inicio y al final de cada sesión de trabajo.

---

## ⚠️ NOTA DE VERSIÓN — UNITY 6.3 LTS

Este proyecto usa **Unity 6.3 LTS** (versión `6000.3.16f1`, mayo 2026).
Es la versión LTS más estable disponible, con soporte garantizado hasta **diciembre 2027**.

### Diferencias clave respecto a Unity 2022.3 LTS
| Aspecto | Unity 2022.3 | Unity 6.3 LTS |
|---|---|---|
| Input System | Package separado | Incluido por defecto |
| UI Toolkit | Experimental | Estable y recomendado |
| Universal Render Pipeline | URP 14 | URP 17 (mejorado) |
| 2D Lights | Light2D básico | Light2D con sombras dinámicas mejoradas |
| Scripting | Mono/.NET | Inicia migración a CoreCLR (retrocompatible) |
| Package Manager | Asset Store clásico | UPM con paquetes firmados |
| Android Build | Gradle manual | Gradle automático mejorado |

> Todos los scripts C# del proyecto son compatibles con Unity 6.3 LTS sin cambios.
> Si Unity muestra advertencias de API obsoleta, son advertencias menores, no errores.

---

## ✅ ESTADO ACTUAL — Fase 3 Completa (Mayo 26, 2026)

### Documentación `/docs/`
| Archivo | Estado | Descripción |
|---|---|---|
| README.md | ✅ Completo | Presentación del proyecto |
| docs/GDD.md | ✅ v2.0 | Game Design Document completo |
| docs/CREATURES.md | ✅ Completo | Bestiario completo por región |
| docs/RECIPES.md | ✅ Completo | Sistema de recetas y estaciones |
| docs/ECONOMY.md | ✅ Completo | Economía, monedas y pedidos |
| docs/ROADMAP.md | ✅ Completo | Fases de desarrollo |

### Scripts Unity C# — 25/25 Completados

#### Fase 1 y 2 — Scripts Base (14 scripts)
| # | Archivo `.cs` | Carpeta Unity | Descripción |
|---|---|---|---|
| 1 | GameManager.cs | Assets/Scripts/Core/ | Controlador principal del juego |
| 2 | GameData.cs | Assets/Scripts/Core/ | Estructura de datos globales |
| 3 | PlayerController.cs | Assets/Scripts/Player/ | Movimiento y acciones del jugador |
| 4 | PlayerInventory.cs | Assets/Scripts/Player/ | Inventario de ingredientes y criaturas |
| 5 | ZoneManager.cs | Assets/Scripts/World/ | Control de regiones y zonas |
| 6 | ResourceNode.cs | Assets/Scripts/World/ | Nodos de recursos recolectables |
| 7 | CreatureBase.cs | Assets/Scripts/Creatures/ | Clase base de todas las criaturas |
| 8 | CreatureAI.cs | Assets/Scripts/Creatures/ | Comportamiento IA de criaturas |
| 9 | CookingStation.cs | Assets/Scripts/Cooking/ | Estaciones de cocina |
| 10 | RecipeDatabase.cs | Assets/Scripts/Cooking/ | Base de datos de recetas |
| 11 | OrderManager.cs | Assets/Scripts/Economy/ | Sistema de pedidos |
| 12 | CurrencyManager.cs | Assets/Scripts/Economy/ | Gestión de monedas |
| 13 | HUDController.cs | Assets/Scripts/UI/ | Interfaz del jugador v2.0 |
| 14 | MainMenuUI.cs | Assets/Scripts/UI/ | Menú principal |

#### Fase 3 — Scripts Nuevos (11 scripts)
| # | Archivo `.cs` | Carpeta Unity | Descripción |
|---|---|---|---|
| 15 | SaveSystem.cs | Assets/Scripts/Core/ | Guardar/cargar partida en JSON local |
| 16 | XPManager.cs | Assets/Scripts/Core/ | XP, niveles y 8 rangos de chef |
| 17 | AchievementManager.cs | Assets/Scripts/Core/ | 22 logros con recompensas |
| 18 | CaptureToolController.cs | Assets/Scripts/Player/ | 6 herramientas con cooldown y mejora |
| 19 | CameraFollow.cs | Assets/Scripts/Player/ | Cámara suave con zoom y shake |
| 20 | CaravanUpgradeManager.cs | Assets/Scripts/World/ | 10 mejoras visuales de caravana |
| 21 | DayNightCycle.cs | Assets/Scripts/World/ | Ciclo día/noche con iluminación |
| 22 | DishQualitySystem.cs | Assets/Scripts/Cooking/ | 5 niveles de calidad de platillos |
| 23 | UIOrderCard.cs | Assets/Scripts/UI/ | Tarjeta de pedido con timer visual |
| 24 | UIRecipeBook.cs | Assets/Scripts/UI/ | Recetario visual con filtros |
| 25 | AudioManager.cs | Assets/Scripts/Audio/ | Música adaptativa y SFX |

---

## 📐 DEPENDENCIAS ENTRE SCRIPTS

Antes de implementar en Unity, respeta este orden. Un script necesita que el otro ya exista en la escena.

```
GameManager
  └── GameData           (GameManager lo lee al iniciar)
  └── SaveSystem         (GameManager llama a SaveSystem)
  └── XPManager          (GameManager referencia XPManager)
  └── AchievementManager (AchievementManager escucha eventos de XPManager)

PlayerController
  └── PlayerInventory    (PlayerController llama a PlayerInventory al recolectar)
  └── CaptureToolController (PlayerController activa herramientas)
  └── CameraFollow       (CameraFollow necesita la Transform del PlayerController)

CookingStation
  └── RecipeDatabase     (CookingStation consulta RecipeDatabase para validar)
  └── DishQualitySystem  (CookingStation usa DishQualitySystem al terminar)
  └── PlayerInventory    (CookingStation consume ingredientes del inventario)

OrderManager
  └── CurrencyManager    (OrderManager paga con CurrencyManager al entregar)
  └── DishQualitySystem  (OrderManager evalúa calidad del platillo)
  └── XPManager          (OrderManager da XP al completar pedido)
  └── AchievementManager (OrderManager dispara logros de pedidos)

HUDController
  └── XPManager          (HUD muestra barra de XP)
  └── CurrencyManager    (HUD muestra monedas)
  └── DayNightCycle      (HUD muestra hora actual)
  └── OrderManager       (HUD muestra pedidos activos)

DayNightCycle
  └── AudioManager       (DayNightCycle cambia música según hora)
  └── ZoneManager        (DayNightCycle activa criaturas nocturnas)

CaravanUpgradeManager
  └── CurrencyManager    (verifica que hay monedas para mejora)
  └── SaveSystem         (guarda el estado de mejoras)

AudioManager             (no depende de otros, pero todos lo usan)
ZoneManager              (no depende de otros al inicio)
RecipeDatabase           (no depende de otros, solo almacena datos)
```

---

## 🎬 ESCENAS NECESARIAS EN UNITY

El proyecto requiere **3 escenas** en total. Aquí están sus nombres exactos, qué contienen y cómo crearlas.

---

### ESCENA 1 — `MainMenu`
**Ruta en Unity:** `Assets/Scenes/MainMenu.unity`

**Para qué sirve:** Pantalla de inicio del juego. Muestra el logo, botón Jugar, botón Opciones y botón Créditos.

**Objetos que debe tener esta escena:**

| Nombre del objeto | Tipo | Script que se le asigna |
|---|---|---|
| `Canvas_MainMenu` | Canvas (UI) | MainMenuUI.cs |
| `AudioSource_Music` | GameObject vacío | AudioManager.cs |
| `EventSystem` | EventSystem | (automático al crear Canvas) |
| `Camera_Main` | Camera | (sin script, solo cámara) |
| `Background_Image` | Image (hijo del Canvas) | (sin script, solo sprite) |

**Cómo crear esta escena paso a paso (sin tecnicismos):**
1. En Unity, en el menú superior ve a **File → New Scene**
2. Elige la plantilla **"Basic 2D"**
3. Ve a **File → Save As** y guárdala como `MainMenu` dentro de `Assets/Scenes/`
4. En la ventana Hierarchy (izquierda), haz clic derecho → **UI → Canvas** — esto crea el Canvas y el EventSystem automáticamente
5. Con el Canvas seleccionado, en el Inspector (derecha) busca el componente **Canvas** y cambia **Render Mode** a `Screen Space - Overlay`
6. Arrastra el script `MainMenuUI.cs` desde el Project (abajo) al objeto `Canvas_MainMenu` en la Hierarchy
7. Crea un `GameObject` vacío (clic derecho en Hierarchy → Create Empty), nómbralo `AudioSource_Music`, y arrástrale el script `AudioManager.cs`

---

### ESCENA 2 — `GameScene`
**Ruta en Unity:** `Assets/Scenes/GameScene.unity`

**Para qué sirve:** La escena principal de exploración donde Kael camina, captura criaturas y recolecta ingredientes.

**Objetos que debe tener esta escena:**

| Nombre del objeto | Tipo | Script que se le asigna |
|---|---|---|
| `GameManager` | GameObject vacío | GameManager.cs + GameData.cs + SaveSystem.cs |
| `XPManager` | GameObject vacío | XPManager.cs |
| `AchievementManager` | GameObject vacío | AchievementManager.cs |
| `AudioManager` | GameObject vacío | AudioManager.cs |
| `DayNightCycle` | GameObject vacío | DayNightCycle.cs |
| `ZoneManager` | GameObject vacío | ZoneManager.cs |
| `OrderManager` | GameObject vacío | OrderManager.cs |
| `CurrencyManager` | GameObject vacío | CurrencyManager.cs |
| `Player` | Sprite/GameObject | PlayerController.cs + PlayerInventory.cs + CaptureToolController.cs |
| `Camera_Main` | Camera | CameraFollow.cs |
| `Tilemap_Ground` | Tilemap | (sin script) |
| `Tilemap_Objects` | Tilemap | (sin script) |
| `Canvas_HUD` | Canvas | HUDController.cs |
| `Light_Global` | Global Light 2D | (sin script, ajustar intensidad) |
| `EventSystem` | EventSystem | (automático) |

**Cómo crear esta escena paso a paso:**
1. **File → New Scene → Basic 2D** → guárdala como `GameScene` en `Assets/Scenes/`
2. **Crear los Managers:** Haz clic derecho en la Hierarchy → **Create Empty** por cada manager de la tabla de arriba. Nómbralos exactamente como aparecen en la columna "Nombre del objeto"
3. **Asignar scripts a los managers:** Selecciona cada objeto vacío, ve al Inspector, haz clic en **"Add Component"** (botón abajo del todo), escribe el nombre del script y selecciónalo
4. **Crear al jugador:** Clic derecho en Hierarchy → **2D Object → Sprites → Square** — nómbralo `Player`. Luego asígnale los 3 scripts del jugador con "Add Component"
5. **Asignar CameraFollow:** Selecciona la `Main Camera`, dale "Add Component" → `CameraFollow`. En el Inspector verás un campo "Target" — arrastra el objeto `Player` a ese campo
6. **Crear el Tilemap:** Clic derecho en Hierarchy → **2D Object → Tilemap → Rectangular**. Esto crea un objeto `Grid` con un `Tilemap` hijo. Renombra el Tilemap a `Tilemap_Ground`. Duplica el Tilemap (Ctrl+D) y nómbralo `Tilemap_Objects`
7. **Crear el HUD:** Clic derecho → **UI → Canvas**, nómbralo `Canvas_HUD`. Asígnale `HUDController.cs`
8. **Crear la luz global:** Clic derecho → **Light → Global Light 2D**, nómbralo `Light_Global`. Pon su intensidad en `0.8` al inicio

---

### ESCENA 3 — `Caravana`
**Ruta en Unity:** `Assets/Scenes/Caravana.unity`

**Para qué sirve:** El hub central del jugador. Aquí está la cocina, el tablón de pedidos, las mejoras y la vista visual de la caravana.

**Objetos que debe tener esta escena:**

| Nombre del objeto | Tipo | Script que se le asigna |
|---|---|---|
| `GameManager` | GameObject vacío | GameManager.cs + SaveSystem.cs |
| `CurrencyManager` | GameObject vacío | CurrencyManager.cs |
| `OrderManager` | GameObject vacío | OrderManager.cs |
| `CaravanUpgradeManager` | GameObject vacío | CaravanUpgradeManager.cs |
| `AudioManager` | GameObject vacío | AudioManager.cs |
| `RecipeDatabase` | GameObject vacío | RecipeDatabase.cs |
| `DishQualitySystem` | GameObject vacío | DishQualitySystem.cs |
| `Station_Caldero` | Sprite/GameObject | CookingStation.cs |
| `Station_Parrilla` | Sprite/GameObject | CookingStation.cs |
| `Canvas_CaravanaUI` | Canvas | UIOrderCard.cs + UIRecipeBook.cs |
| `Camera_Main` | Camera | (sin script, cámara estática aquí) |
| `Light_Global` | Global Light 2D | (sin script) |
| `EventSystem` | EventSystem | (automático) |

**Cómo crear esta escena paso a paso:**
1. **File → New Scene → Basic 2D** → guárdala como `Caravana` en `Assets/Scenes/`
2. Repite el proceso de crear GameObjects vacíos para cada manager (igual que en GameScene)
3. **Crear las estaciones de cocina:** Clic derecho → **2D Object → Sprites → Square**. Nómbralo `Station_Caldero`. Asígnale `CookingStation.cs`. Repite para `Station_Parrilla`
4. **Canvas de UI:** Crea un Canvas llamado `Canvas_CaravanaUI`. Dentro de ese Canvas crea dos GameObjects hijos vacíos: uno llamado `OrderCard_Panel` con `UIOrderCard.cs`, y otro `RecipeBook_Panel` con `UIRecipeBook.cs`
5. **Configurar RecipeDatabase:** Selecciona el objeto `RecipeDatabase`, en el Inspector verás una lista de recetas. Aquí agregarás las recetas manualmente más adelante

---

## 🔗 CÓMO VINCULAR LOS SCRIPTS ENTRE ESCENAS

En Unity, los scripts de escenas diferentes no se hablan directamente. Usa estas técnicas:

**Para datos que persisten entre escenas** (monedas, inventario, XP):
- El objeto `GameManager` debe tener activado **"Don't Destroy On Load"** — esto ya está en el código de `GameManager.cs`
- Significa que cuando cambias de `GameScene` a `Caravana`, el GameManager no se destruye y conserva todos los datos

**Para referenciar objetos dentro de la misma escena:**
- Selecciona el script que necesita la referencia
- En el Inspector verás campos con nombre (por ejemplo, `CameraFollow` tiene el campo "Target Player")
- Arrastra el objeto correspondiente desde la Hierarchy a ese campo — eso es "vincular"

**Campos que necesitas vincular manualmente en el Inspector:**

| Script | Campo visible en Inspector | Qué arrastrar ahí |
|---|---|---|
| CameraFollow.cs | Target | El objeto `Player` |
| HUDController.cs | XP Manager | El objeto `XPManager` |
| HUDController.cs | Currency Manager | El objeto `CurrencyManager` |
| HUDController.cs | Order Manager | El objeto `OrderManager` |
| HUDController.cs | Day Night Cycle | El objeto `DayNightCycle` |
| OrderManager.cs | Currency Manager | El objeto `CurrencyManager` |
| OrderManager.cs | XP Manager | El objeto `XPManager` |
| CookingStation.cs | Recipe Database | El objeto `RecipeDatabase` |
| CookingStation.cs | Player Inventory | El objeto `Player` |
| CaravanUpgradeManager.cs | Currency Manager | El objeto `CurrencyManager` |
| DayNightCycle.cs | Audio Manager | El objeto `AudioManager` |
| AchievementManager.cs | XP Manager | El objeto `XPManager` |

---

## 📦 PREFABS NECESARIOS

Un Prefab es un objeto reutilizable. Necesitas crear estos 5 prefabs:

| Nombre del Prefab | Ruta donde guardarlo | Para qué se usa |
|---|---|---|
| `FloatingText.prefab` | Assets/Prefabs/UI/ | Texto animado que flota (+10 XP, +monedas) |
| `OrderCard.prefab` | Assets/Prefabs/UI/ | Tarjeta visual de cada pedido activo |
| `ResourceNode.prefab` | Assets/Prefabs/World/ | Nodo de ingrediente recolectable en el mapa |
| `CreatureBase.prefab` | Assets/Prefabs/Creatures/ | Base para todas las criaturas |
| `CookingStation.prefab` | Assets/Prefabs/Cooking/ | Estación de cocina reutilizable |

**Cómo crear un Prefab (ejemplo con FloatingText):**
1. Crea un objeto en la escena (clic derecho Hierarchy → UI → Text - TextMeshPro)
2. Nómbralo `FloatingText`
3. En el panel Project (abajo), navega a `Assets/Prefabs/UI/` (créa la carpeta si no existe)
4. Arrastra el objeto desde la Hierarchy a esa carpeta en el Project
5. Listo — el objeto ahora es un Prefab reutilizable (aparece en azul en la Hierarchy)
6. Borra el objeto de la escena; el Prefab ya está guardado

---

## ⚙️ CONFIGURACIÓN DE INPUT ACTIONS (Unity 6.3)

En Unity 6.3 el Input System ya viene incluido. Sigue estos pasos:

1. En Project, clic derecho → **Create → Input Actions**
2. Nómbralo `PlayerInputActions`
3. Guárdalo en `Assets/Settings/`
4. Ábrelo con doble clic y crea estas acciones:

| Action Map | Acción | Tipo | Binding |
|---|---|---|---|
| Player | Move | Value / Vector2 | WASD + Left Stick |
| Player | Sprint | Button | Left Shift + South Button |
| Player | Interact | Button | E + East Button |
| Player | UseToolPressed | Button | Clic Izquierdo + West Button |
| Player | OpenRecipeBook | Button | R + Select Button |
| UI | Navigate | Value / Vector2 | Flechas + D-Pad |
| UI | Submit | Button | Enter + South Button |
| UI | Cancel | Button | Escape + East Button |

5. Haz clic en **"Save Asset"** y luego en **"Generate C# Class"** — esto crea el archivo de código automáticamente
6. Guarda el C# generado en `Assets/Scripts/Core/`

---

## 📊 SISTEMAS IMPLEMENTADOS — FASE 3

### Progreso del jugador
- [x] Sistema XP con fórmula escalable `(100 * nivel^1.5)`
- [x] 8 rangos: Aprendiz → Sous Chef → Chef → Chef de Cuisine → Ejecutivo → Maestro → Gran Maestro → El Gran Chef
- [x] Cada rango desbloquea contenido concreto
- [x] 22 logros en 6 categorías con recompensas

### Herramientas y mejoras
- [x] 6 herramientas de captura con cooldown y durabilidad
- [x] Herramientas mejorables a nivel 3 con monedas
- [x] 10 mejoras de caravana en 5 categorías con cambio visual

### Calidad de platillos
- [x] 5 niveles: Básico / Decente / Bueno / Excelente / Legendario
- [x] Precio escala: x1.0 → x3.5 según calidad
- [x] Panel de mejora (Enhance) integrado en pedidos

### UI
- [x] HUD v2.0 con barra XP, rango, monedas y hora
- [x] Popups animados: LevelUp, RankUp, Achievement
- [x] Floating text animado
- [x] Tarjeta de pedido con timer visual (verde/amarillo/rojo)
- [x] Recetario con filtros por categoría

### Mundo y ambiente
- [x] Ciclo día/noche en 5 fases (Amanecer, Mañana, Tarde, Noche, Medianoche)
- [x] Iluminación dinámica por hora (Light2D con Unity 6.3)
- [x] Criaturas nocturnas activadas solo de noche

### Audio
- [x] Música adaptativa por zona y hora del día
- [x] Crossfade suave entre tracks
- [x] SFX categorizados por acción

---

## 🔜 FASE 4 — PRÓXIMA SESIÓN

### Scripts pendientes (8 scripts)
| # | Script | Carpeta | Descripción |
|---|---|---|---|
| 26 | NimbusController.cs | Assets/Scripts/Companion/ | Compañero con afinidad, colores y reacciones |
| 27 | MapExpansionManager.cs | Assets/Scripts/World/ | Mapa grande con regiones desbloqueables |
| 28 | ReputationManager.cs | Assets/Scripts/World/ | Reputación por región |
| 29 | SeasonalEventManager.cs | Assets/Scripts/World/ | Eventos temporales y festivales |
| 30 | UIAchievementsPanel.cs | Assets/Scripts/UI/ | Panel visual de logros |
| 31 | UIUpgradeShop.cs | Assets/Scripts/UI/ | Tienda visual de mejoras de caravana |
| 32 | UIRankDisplay.cs | Assets/Scripts/UI/ | Pantalla de progreso de rango |
| 33 | TutorialManager.cs | Assets/Scripts/Core/ | Tutorial guiado por Nimbus |

### Configuración Unity pendiente
- [ ] Crear las 3 escenas: `MainMenu`, `GameScene`, `Caravana`
- [ ] Crear los 5 prefabs: FloatingText, OrderCard, ResourceNode, CreatureBase, CookingStation
- [ ] Configurar Input Actions con las 8 acciones de la tabla de arriba
- [ ] Crear el Animator Controller de Kael con estados: Idle, Walk, Run, Collect, Cook, Celebrate
- [ ] Pintar el Tilemap de Pradera de Bruma (zona 1)
- [ ] Asignar Global Light 2D y configurar DayNightCycle
- [ ] Vincular todos los campos del Inspector según la tabla de vinculación

### Assets pendientes de crear o conseguir
- [ ] Sprite de Kael (personaje principal) — mínimo 4 direcciones
- [ ] Sprite de Nimbus (compañero nube)
- [ ] Sprites de las 3 criaturas del MVP (Puffshroom, Mielín, Caracol de Canela)
- [ ] Tiles para Pradera de Bruma (suelo, hierba, objetos)
- [ ] Sprite de la Caravana (al menos nivel 1)
- [ ] Íconos de ingredientes (mínimo 6)
- [ ] Música de exploración y caravana (formato .ogg o .wav)

---

## 🪲 BUGS CONOCIDOS Y PENDIENTES TÉCNICOS

| # | Descripción | Archivo relacionado | Estado |
|---|---|---|---|
| 1 | Sin escenas creadas aún — todos los scripts están sin escena asignada | Todas las escenas | ⏳ Pendiente Fase 4 |
| 2 | RecipeDatabase sin recetas cargadas todavía | RecipeDatabase.cs | ⏳ Pendiente Fase 4 |
| 3 | AudioManager sin clips de audio asignados | AudioManager.cs | ⏳ Pendiente Fase 4 |
| 4 | Input Actions no configuradas aún | PlayerController.cs | ⏳ Pendiente Fase 4 |

---

## 📝 LOG DE VERSIONES

| Versión | Fecha | Cambios |
|---|---|---|
| v1.0 | Mayo 26, 2026 | Repositorio creado, scripts Fase 1 y 2 (14 scripts) |
| v2.0 | Mayo 26, 2026 | Fase 3 completa, 11 scripts nuevos, HUDController v2.0 |
| v3.0 | Mayo 26, 2026 | PROGRESS actualizado: Unity 6.3 LTS, guía de escenas, dependencias, prefabs, input actions, tabla de vinculación |

---

## 🔄 CÓMO RETOMAR DESDE CERO SI HAY ERROR

Si pierdes el contexto o hay error de GitHub, sigue este orden:

1. Lee este archivo PROGRESS.md
2. Verifica los 25 scripts en `Assets/Scripts/`
3. Lee `docs/GDD.md` para el contexto del juego
4. Lee `docs/ROADMAP.md` para saber qué fase sigue
5. Continúa desde **Fase 4 — 8 scripts pendientes** arriba

**Último commit exitoso:** Fase 3 completa (25 scripts) + PROGRESS v3.0
