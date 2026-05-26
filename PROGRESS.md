# 💎 CARAVAN KITCHEN — PROGRESS TRACKER

> Útil para retomar si hay error de conexión con GitHub.
> Actualizar este archivo en cada sesión de trabajo.

---

## ✅ ESTADO ACTUAL (Mayo 26, 2026)

### Documentación
| Archivo | Estado |
|---|---|
| README.md | ✅ Completo |
| docs/GDD.md | ✅ Completo |
| docs/CREATURES.md | ✅ Completo |
| docs/RECIPES.md | ✅ Completo |
| docs/ECONOMY.md | ✅ Completo |
| docs/ROADMAP.md | ✅ Completo |

### Scripts Unity C# (14/14)
| # | Script | Carpeta | Estado |
|---|---|---|---|
| 1 | GameManager.cs | Assets/Scripts/Core/ | ✅ Completo |
| 2 | GameData.cs | Assets/Scripts/Core/ | ✅ Completo |
| 3 | PlayerController.cs | Assets/Scripts/Player/ | ✅ Completo |
| 4 | PlayerInventory.cs | Assets/Scripts/Player/ | ✅ Completo |
| 5 | ZoneManager.cs | Assets/Scripts/World/ | ✅ Completo |
| 6 | ResourceNode.cs | Assets/Scripts/World/ | ✅ Completo |
| 7 | CreatureBase.cs | Assets/Scripts/Creatures/ | ✅ Completo |
| 8 | CreatureAI.cs | Assets/Scripts/Creatures/ | ✅ Completo |
| 9 | CookingStation.cs | Assets/Scripts/Cooking/ | ✅ Completo |
| 10 | RecipeDatabase.cs | Assets/Scripts/Cooking/ | ✅ Completo |
| 11 | OrderManager.cs | Assets/Scripts/Economy/ | ✅ Completo |
| 12 | CurrencyManager.cs | Assets/Scripts/Economy/ | ✅ Completo |
| 13 | HUDController.cs | Assets/Scripts/UI/ | ✅ Completo |
| 14 | MainMenuUI.cs | Assets/Scripts/UI/ | ✅ Completo |

---

## 🔜 SIGUIENTE PASO

### Fase 2 — Configurar Unity (hacer en el editor)
1. Crear proyecto Unity 2D (URP o Built-in)
2. Instalar paquetes: Input System, TextMeshPro, 2D Tilemap
3. Crear escenas: `MainMenu` y `GameScene`
4. Agregar los scripts a GameObjects en la escena
5. Configurar Input Actions (Move, Sprint, Interact)
6. Asignar referencias en Inspector

### Fase 3 — Scripts pendientes (próxima sesión)
- [ ] SaveSystem.cs — guardar/cargar partida (JSON)
- [ ] CaptureToolController.cs — herramienta de captura del jugador
- [ ] CaravanUpgradeManager.cs — mejoras de la caravana
- [ ] DayNightCycle.cs — ciclo día/noche para criaturas nocturnas
- [ ] UIOrderCard.cs — tarjeta de pedido en HUD
- [ ] UIRecipeBook.cs — recetario visual
- [ ] AudioManager.cs — música y sfx
- [ ] CameraFollow.cs — cámara que sigue al jugador

---

## ⚠️ NOTAS DE RETOMA

Si hay error de GitHub, leer este archivo y continuar desde:
**`Último commit exitoso`** = Scripts 11-14 (Economy + UI) guardados.

Orden de scripts aún pendientes para Fase 3:
1. SaveSystem.cs
2. CaptureToolController.cs
3. CaravanUpgradeManager.cs
4. DayNightCycle.cs
5. UIOrderCard.cs
6. UIRecipeBook.cs
7. AudioManager.cs
8. CameraFollow.cs
