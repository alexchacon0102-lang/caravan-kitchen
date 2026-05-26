# 🗺️ Roadmap de Desarrollo — Caravan Kitchen

---

## Estado actual

- [x] GDD v0.2 completo
- [x] Repositorio GitHub estructurado
- [x] Scripts C# base generados
- [ ] Primer prototipo jugable
- [ ] Build Android interno

---

## Fase 0 — Fundación (Semanas 1–2)

### Objetivos
- [ ] Proyecto Unity creado (2022.3 LTS, 2D URP, Android Build Support)
- [ ] Estructura de carpetas siguiendo `/Assets/_Scripts/`, `/_Data/`, `/Art/`, `/Scenes/`
- [ ] Control de versiones Git configurado
- [ ] Personaje Kiri: sprite placeholder + animaciones Idle, Walk, Jump
- [ ] Cámara 2D con seguimiento Cinemachine
- [ ] Movimiento horizontal + salto funcional
- [ ] GameManager singleton básico

### Entregable
Kiri se mueve en una escena vacía.

---

## Fase 1 — Exploración Básica (Semanas 3–4)

### Objetivos
- [ ] Mapa Pradera de Bruma: tilemap + plataformas + fondo paralaje 3 capas
- [ ] Sistema de recolección: ingredientes I01–I04 como prefabs interactuables
- [ ] Inventario básico (ScriptableObject por ingrediente)
- [ ] UI inventario simple (burbuja en pantalla)
- [ ] 1 criatura IA: Puffshroom (estados IDLE → ALERT → FLEE)
- [ ] Ciclo día/noche in-game (24 min = 1 día)

### Entregable
Kiri camina por Pradera, recolecta ingredientes y ve al Puffshroom huir.

---

## Fase 2 — Sistema de Captura (Semanas 5–6)

### Objetivos
- [ ] Red básica: animación lanzar + colisión + resultado captura
- [ ] Cebo de miel: colocar prefab + timer + atracción de Mielín
- [ ] Criatura Mielín: IDLE → ATTRACTED → CAPTURED
- [ ] Criatura Raicita: emerge del suelo + FLEE
- [ ] Feedback visual captura: VFX partículas + sonido
- [ ] Compañero Fumi: sprite + brillo al detectar rareza
- [ ] Puffshroom Dorado: spawn solo en amanecer

### Entregable
Jugador puede capturar 3 criaturas con 2 herramientas distintas.

---

## Fase 3 — Caravana y Cocina (Semanas 7–9)

### Objetivos
- [ ] Escena Caravana: arte placeholder + navegación desde mapa
- [ ] Caldero funcional: UI drag & drop ingredientes + timer cocción
- [ ] ScriptableObject RecipeData: ingredientes, tiempo, resultado, calidad
- [ ] RecipeBook: lista de recetas descubiertas/bloqueadas
- [ ] Inventario de platillos cocinados
- [ ] 5 recetas funcionales: R01, R02, R03, R04, R05
- [ ] Animación cocinando en caldero

### Entregable
Jugador captura, va a caravana y cocina 5 recetas distintas.

---

## Fase 4 — Pedidos y Economía (Semanas 10–11)

### Objetivos
- [ ] Sistema de pedidos: 3 activos, timer visual, recompensa
- [ ] CurrencyManager: monedas, obtener, mostrar, gastar
- [ ] 2 NPCs básicos: Brulo y Velta con sprite + diálogo básico
- [ ] UI de venta rápida con botón y animación de monedas
- [ ] Sistema de fama culinaria básico
- [ ] Save/Load con JSON básico

### Entregable
Jugador puede completar el loop completo: explorar → cocinar → vender → cobrar.

---

## Fase 5 — Mejoras y Progresión (Semana 12)

### Objetivos
- [ ] UpgradeManager: 4 mejoras de herramienta funcionales
- [ ] UI de árbol de mejoras
- [ ] Caravana nivel 2 desbloqueable
- [ ] Parrilla funcional + 2 recetas nuevas
- [ ] Tutorial básico guiado por Fumi (pantallas de ayuda)

### Entregable
Jugador puede mejorar equipo y desbloquear segundo nivel de caravana.

---

## Fase 6 — Pulido MVP (Semanas 13–14)

### Objetivos
- [ ] SFX básicos: captura, cocina, venta, recolección
- [ ] Música de fondo en loop para Pradera y Caravana
- [ ] UI limpia y legible con paleta de colores correcta
- [ ] Tutorial completo con Fumi
- [ ] 1 Rewarded Ad opcional (duplicar venta) — AdMob
- [ ] Build Android → firma APK
- [ ] Prueba interna Google Play Console

### Entregable
Build jugable en Android, subido a track interno de Google Play.

---

## Post-MVP — Fase 7 en adelante

| Fase | Contenido |
|---|---|
| 7 | Bosque de Vapor Dulce: 4 criaturas + recetas nuevas |
| 8 | Arrecife de Nubes: peces-nube, campana, verticalidad |
| 9 | Mercado Suspendido: NPCs, pedidos premium |
| 10 | Bestiario y recetario visuales completos |
| 11 | Sistema de eventos de temporada |
| 12 | Barranco del Caldero: zona final, criaturas raras |
| 13 | Decoración de caravana y skins |
| 14 | Pase de temporada culinaria |
| 15 | Lanzamiento en Google Play (Open Beta) |

---

## Estimado total

| Etapa | Semanas |
|---|---|
| MVP funcional | 14 semanas |
| Contenido post-MVP (3 zonas) | +12 semanas |
| Lanzamiento Open Beta | ~26 semanas desde inicio |
