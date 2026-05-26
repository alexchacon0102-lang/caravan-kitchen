# 💎 CARAVAN KITCHEN — PROGRESS TRACKER v4.0

> **Motor:** Unity 6.3 LTS (6000.3.16f1)
> **Plataforma:** Android (Google Play) + iOS futuro
> **Última sesión:** Mayo 26, 2026
> **Estado:** Fase 4 COMPLETA — 33/33 scripts

---

## ✅ CORRECCIONES UNITY 6.3 LTS APLICADAS

| Script | Problema | Corrección |
|---|---|---|
| HUDController.cs | `FindObjectOfType<T>()` obsoleto en Unity 6 | → `FindFirstObjectByType<T>()` ✅ |
| ReputationManager.cs | Mismo problema en `AddReputation()` | → `FindFirstObjectByType<T>()` ✅ |
| SeasonalEventManager.cs | Mismo problema en `StartEvent()` / `EndEvent()` | → `FindFirstObjectByType<T>()` ✅ |
| UIUpgradeShop.cs | Mismo problema en `BuySelected()` | → `FindFirstObjectByType<T>()` ✅ |

> Todos los demás 29 scripts ya eran compatibles con Unity 6.3 LTS sin cambios.
> `linearVelocity` (PlayerController, CreatureAI) ya era la API correcta.
> Input System ya estaba importado con el namespace correcto.

---

## 📊 PROGRESO TOTAL — 33/33 Scripts

### Carpetas y scripts completos

| Carpeta | Scripts | Estado |
|---|---|---|
| Assets/Scripts/Core/ | GameManager, GameData, SaveSystem, XPManager, AchievementManager, RankManager, TutorialManager | 7/7 ✅ |
| Assets/Scripts/Player/ | PlayerController, PlayerInventory, CaptureToolController, CameraFollow | 4/4 ✅ |
| Assets/Scripts/World/ | ZoneManager, ResourceNode, CaravanUpgradeManager, DayNightCycle, MapExpansionManager, ReputationManager, SeasonalEventManager | 7/7 ✅ |
| Assets/Scripts/Creatures/ | CreatureBase, CreatureAI | 2/2 ✅ |
| Assets/Scripts/Cooking/ | CookingStation, RecipeDatabase, DishQualitySystem | 3/3 ✅ |
| Assets/Scripts/Economy/ | OrderManager, CurrencyManager | 2/2 ✅ |
| Assets/Scripts/UI/ | HUDController, MainMenuUI, UIOrderCard, UIRecipeBook, UIAchievementsPanel, UIUpgradeShop, UIRankDisplay | 7/7 ✅ |
| Assets/Scripts/Audio/ | AudioManager | 1/1 ✅ |
| Assets/Scripts/Companion/ | NimbusController | 1/1 ✅ |

---

## 🚀 FASE 4 — Scripts nuevos (8 scripts)

| # | Script | Carpeta | Qué hace |
|---|---|---|---|
| 26 | NimbusController.cs | Companion/ | Compañero Nimbus: 8 emociones, colores reactivos, frases aleatorias, detección de rareza, sistema de afinidad |
| 27 | MapExpansionManager.cs | World/ | Regiones desbloqueables con costo, prerequisitos, nivel requerido y evento de primera visita |
| 28 | ReputationManager.cs | World/ | Reputación por región (5 niveles), modifica precios, activa criaturas raras |
| 29 | SeasonalEventManager.cs | World/ | Festivales y eventos de temporada cada N días, con ingredientes/criaturas/recetas exclusivas |
| 30 | UIAchievementsPanel.cs | UI/ | Panel de logros con filtro por categoría, tarjetas con barra de progreso y estado bloqueado/desbloqueado |
| 31 | UIUpgradeShop.cs | UI/ | Tienda de mejoras de caravana con lista, detalle y botón de compra |
| 32 | UIRankDisplay.cs | UI/ | Pantalla de progreso de rango con tabla de los 8 rangos, barra XP y próximo desbloqueo |
| 33 | TutorialManager.cs | Core/ | Tutorial guiado por Nimbus, pasos con overlay, acciones del jugador como disparadores, skip disponible |

---

## 🔗 DEPENDENCIAS ENTRE SCRIPTS

```
GameManager
  └── GameData, SaveSystem, XPManager, AchievementManager

PlayerController
  └── PlayerInventory, CaptureToolController, CameraFollow

CookingStation
  └── RecipeDatabase, DishQualitySystem, PlayerInventory

OrderManager
  └── CurrencyManager, DishQualitySystem, XPManager, AchievementManager

HUDController
  └── XPManager, CurrencyManager, DayNightCycle, OrderManager

DayNightCycle
  └── AudioManager, ZoneManager, SeasonalEventManager (via evento onDawn)

NimbusController
  └── CreatureBase (detección de rareza)

MapExpansionManager
  └── XPManager, CurrencyManager, SaveSystem, NimbusController

ReputationManager
  └── HUDController (floating text), SaveSystem

SeasonalEventManager
  └── DayNightCycle (onDawn), HUDController, NimbusController

UIAchievementsPanel
  └── AchievementManager

UIUpgradeShop
  └── CaravanUpgradeManager, HUDController

UIRankDisplay
  └── XPManager

TutorialManager
  └── SaveSystem, NimbusController

CaravanUpgradeManager
  └── CurrencyManager, SaveSystem

AudioManager — independiente (todos lo consumen)
RecipeDatabase — independiente (almacena datos)
ZoneManager   — independiente al inicio
```

---

## 🎬 ESCENAS NECESARIAS EN UNITY

### ESCENA 1 — `MainMenu`
**Ruta:** `Assets/Scenes/MainMenu.unity`

| Objeto | Script asignado |
|---|---|
| Canvas_MainMenu | MainMenuUI.cs |
| AudioSource_Music | AudioManager.cs |
| Camera_Main | (sin script) |
| EventSystem | (automático) |

### ESCENA 2 — `GameScene`
**Ruta:** `Assets/Scenes/GameScene.unity`

| Objeto | Scripts asignados |
|---|---|
| GameManager | GameManager.cs + GameData.cs + SaveSystem.cs |
| XPManager | XPManager.cs |
| AchievementManager | AchievementManager.cs |
| AudioManager | AudioManager.cs |
| DayNightCycle | DayNightCycle.cs |
| ZoneManager | ZoneManager.cs |
| OrderManager | OrderManager.cs |
| CurrencyManager | CurrencyManager.cs |
| MapExpansionManager | MapExpansionManager.cs |
| ReputationManager | ReputationManager.cs |
| SeasonalEventManager | SeasonalEventManager.cs |
| TutorialManager | TutorialManager.cs |
| Player | PlayerController.cs + PlayerInventory.cs + CaptureToolController.cs |
| Nimbus | NimbusController.cs |
| Camera_Main | CameraFollow.cs |
| Canvas_HUD | HUDController.cs + UIAchievementsPanel.cs + UIRankDisplay.cs |
| Light_Global | Global Light 2D (sin script) |
| Tilemap_Ground | (sin script) |
| Tilemap_Objects | (sin script) |

### ESCENA 3 — `Caravana`
**Ruta:** `Assets/Scenes/Caravana.unity`

| Objeto | Scripts asignados |
|---|---|
| GameManager | GameManager.cs + SaveSystem.cs |
| CurrencyManager | CurrencyManager.cs |
| OrderManager | OrderManager.cs |
| CaravanUpgradeManager | CaravanUpgradeManager.cs |
| AudioManager | AudioManager.cs |
| RecipeDatabase | RecipeDatabase.cs |
| DishQualitySystem | DishQualitySystem.cs |
| Station_Caldero | CookingStation.cs |
| Station_Parrilla | CookingStation.cs |
| Canvas_CaravanaUI | UIOrderCard.cs + UIRecipeBook.cs + UIUpgradeShop.cs |
| Light_Global | Global Light 2D (sin script) |

---

## 🔗 CAMPOS A VINCULAR EN INSPECTOR

| Script | Campo en Inspector | Qué arrastrar |
|---|---|---|
| CameraFollow.cs | Target | Objeto `Player` |
| HUDController.cs | XP Manager | Objeto `XPManager` |
| HUDController.cs | Currency Manager | Objeto `CurrencyManager` |
| HUDController.cs | Order Manager | Objeto `OrderManager` |
| HUDController.cs | Day Night Cycle | Objeto `DayNightCycle` |
| OrderManager.cs | Currency Manager | Objeto `CurrencyManager` |
| OrderManager.cs | XP Manager | Objeto `XPManager` |
| CookingStation.cs | Recipe Database | Objeto `RecipeDatabase` |
| CookingStation.cs | Player Inventory | Objeto `Player` |
| CaravanUpgradeManager.cs | Currency Manager | Objeto `CurrencyManager` |
| DayNightCycle.cs | Global Light | Objeto `Light_Global` |
| NimbusController.cs | Dialog Bubble | Panel hijo de Nimbus con TextMeshPro |
| TutorialManager.cs | Tutorial Panel | Panel UI del tutorial |
| TutorialManager.cs | Overlay Canvas Group | CanvasGroup del overlay oscuro |

---

## 📦 PREFABS NECESARIOS

| Prefab | Ruta | Uso |
|---|---|---|
| FloatingText.prefab | Assets/Prefabs/UI/ | Texto animado flotante (+XP, +monedas) |
| OrderCard.prefab | Assets/Prefabs/UI/ | Tarjeta de pedido activo |
| AchievementCard.prefab | Assets/Prefabs/UI/ | Tarjeta de logro en el panel |
| UpgradeCard.prefab | Assets/Prefabs/UI/ | Tarjeta de mejora en tienda |
| RankRow.prefab | Assets/Prefabs/UI/ | Fila de la tabla de rangos |
| ResourceNode.prefab | Assets/Prefabs/World/ | Nodo de ingrediente recolectable |
| CreatureBase.prefab | Assets/Prefabs/Creatures/ | Base de criaturas |
| CookingStation.prefab | Assets/Prefabs/Cooking/ | Estación de cocina |

---

## ⚙️ CONFIGURACIÓN DE INPUT ACTIONS (Unity 6.3)

Archivo: `Assets/Settings/PlayerInputActions.inputactions`

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

---

## 🐛 BUGS CONOCIDOS Y PENDIENTES TÉCNICOS

| # | Descripción | Archivo relacionado | Estado |
|---|---|---|---|
| 1 | Escenas sin crear aún | Todas las escenas | ⏳ Pendiente Fase 5 |
| 2 | RecipeDatabase sin recetas cargadas | RecipeDatabase.cs | ⏳ Pendiente Fase 5 |
| 3 | AudioManager sin clips asignados | AudioManager.cs | ⏳ Pendiente Fase 5 |
| 4 | Input Actions sin configurar | PlayerController.cs | ⏳ Pendiente Fase 5 |
| 5 | SaveSystem necesita exponer GetTutorialCompleted() y SetTutorialCompleted() | SaveSystem.cs | ⏳ Añadir en Fase 5 |
| 6 | CurrencyManager necesita exponer TrySpend(coins, fama) | CurrencyManager.cs | ⏳ Añadir en Fase 5 |
| 7 | CaravanUpgradeManager necesita exponer GetAllUpgrades() y TryPurchaseUpgrade() | CaravanUpgradeManager.cs | ⏳ Añadir en Fase 5 |
| 8 | AchievementManager necesita exponer GetAllAchievements() con campo isUnlocked / currentProgress / maxProgress | AchievementManager.cs | ⏳ Añadir en Fase 5 |
| 9 | XPManager necesita exponer NextRankData, CurrentRankIndex, TotalRanks, GetAllRanks(), GetXPProgressToNextLevel(), XPForNextLevel | XPManager.cs | ⏳ Verificar en Fase 5 |

---

## 🔄 FASE 5 — PRÓXIMA SESIÓN

### Ajustes de compatibilidad entre scripts (verificar APIs públicas)
- [ ] SaveSystem.cs — añadir `GetTutorialCompleted()` + `SetTutorialCompleted(bool)`
- [ ] CurrencyManager.cs — añadir `TrySpend(int coins, int fama)`
- [ ] CaravanUpgradeManager.cs — verificar `GetAllUpgrades()` + `TryPurchaseUpgrade(id)`
- [ ] AchievementManager.cs — verificar que AchievementData tenga `isUnlocked`, `currentProgress`, `maxProgress`, `category`
- [ ] XPManager.cs — verificar `NextRankData`, `CurrentRankIndex`, `TotalRanks`, `GetAllRanks()`, `GetXPProgressToNextLevel()`, `XPForNextLevel`

### Creación de escenas en Unity
- [ ] Crear `MainMenu.unity`, `GameScene.unity`, `Caravana.unity`
- [ ] Crear los 8 prefabs de UI y mundo
- [ ] Configurar Input Actions
- [ ] Vincular campos del Inspector según tabla de arriba
- [ ] Pintar Tilemap zona 1 (Pradera de Bruma)

### Assets pendientes
- [ ] Sprite de Kael (4 direcciones mínimo)
- [ ] Sprite de Nimbus (nube pequeña con glow)
- [ ] Sprites de Puffshroom, Mielín, Caracol de Canela
- [ ] Tiles Pradera de Bruma
- [ ] Sprite Caravana nivel 1
- [ ] Íconos de 6 ingredientes base
- [ ] Música exploración + caravana (.ogg o .wav)

---

## 📝 LOG DE VERSIONES

| Versión | Fecha | Cambios |
|---|---|---|
| v1.0 | Mayo 26, 2026 | Repositorio creado, Fase 1 y 2 (14 scripts) |
| v2.0 | Mayo 26, 2026 | Fase 3 completa (25 scripts), HUDController v2.0 |
| v3.0 | Mayo 26, 2026 | PROGRESS: guía no técnica completa, escenas, dependencias, Unity 6.3 LTS |
| v4.0 | Mayo 26, 2026 | FIX Unity 6.3 (FindFirstObjectByType), Fase 4 completa (33 scripts), PROGRESS actualizado |

---

## 🔄 CÓMO RETOMAR DESDE CERO

1. Lee este PROGRESS.md
2. Verifica los 33 scripts en `Assets/Scripts/`
3. Lee `docs/GDD.md` para el contexto del juego
4. Lee `docs/ROADMAP.md` para las fases
5. Continúa desde **Fase 5 — ajustes de APIs públicas** arriba

**Último commit exitoso:** Fase 4 completa (33 scripts) + correcciones Unity 6.3 LTS
