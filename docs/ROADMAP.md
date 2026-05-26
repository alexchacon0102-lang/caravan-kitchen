# 🗺️ Roadmap de Desarrollo — Caravan Kitchen
**Versión:** 2.0 | Mayo 26, 2026

---

## 🧭 Filosofía de diseño del juego

> **Caravan Kitchen no es un juego que se termina. Es un mundo que crece.**

El jugador debe sentir en todo momento que:
- Siempre hay **una criatura que aún no ha visto**
- Siempre hay **una receta que aún no ha descubierto**
- Siempre hay **una zona que aún no ha desbloqueado**
- Siempre hay **un evento especial próximo que vale esperar**

Esto se logra con **cuatro pilares de expansión perpetua:**

| Pilar | Qué hace | Frecuencia |
|---|---|---|
| 🌍 **Zonas nuevas** | Regiones completamente nuevas con criaturas, mecánicas y biomas distintos | Cada 2–3 meses |
| 🐾 **Criaturas nuevas** | Nuevas criaturas en zonas existentes, eventos o temporadas | Cada 3–4 semanas |
| 🍽️ **Recetas nuevas** | Recetas ligadas a criaturas nuevas o eventos de temporada | Cada 2–3 semanas |
| 🎉 **Eventos de temporada** | Festivales limitados con contenido exclusivo que vuelve cada año | Cada 4–6 semanas |

---

## 📊 Estado actual del proyecto

- [x] GDD v0.3 completo
- [x] Repositorio GitHub estructurado
- [x] 33 scripts C# base generados (Fases 1–4)
- [x] Compañero Pato con nombre personalizable
- [x] Bestiario 16 criaturas (4 zonas)
- [x] Recetario 20 recetas MVP + 4 legendarias
- [ ] Primer prototipo jugable
- [ ] Build Android interno

---

## 🔨 FASES DE DESARROLLO MVP (Semanas 1–14)

### Fase 0 — Fundación (Semanas 1–2)
**Objetivo:** Kael se mueve en una escena vacía.
- [ ] Proyecto Unity 6.3 LTS configurado (2D URP, Android Build Support)
- [ ] Estructura de carpetas y control de versiones
- [ ] Kael: sprite placeholder + Idle, Walk, Jump
- [ ] Cámara con CameraFollow.cs
- [ ] GameManager + SaveSystem funcionando

### Fase 1 — Exploración Básica (Semanas 3–4)
**Objetivo:** Kael explora, recolecta y ve criaturas moverse.
- [ ] Pradera de Bruma: tilemap + fondo paralaje
- [ ] Ingredientes I01–I04 como prefabs interactuables
- [ ] Inventario básico + UI simple
- [ ] Puffshroom con IA: IDLE → ALERT → FLEE
- [ ] DayNightCycle funcional (24 min = 1 día)

### Fase 2 — Sistema de Captura (Semanas 5–6)
**Objetivo:** Jugador captura 3 criaturas con 2 herramientas.
- [ ] Red básica + Cebo de miel funcionales
- [ ] Mielín: ATTRACTED → CAPTURED
- [ ] Raicita: EMERGE → FLEE
- [ ] Pato: sprite + detección de rareza (tint)
- [ ] Puffshroom Dorado: spawn solo al amanecer
- [ ] Panel de bautizo del Pato (UINameYourDuck)

### Fase 3 — Caravana y Cocina (Semanas 7–9)
**Objetivo:** Loop completo explorar → cocinar.
- [ ] Escena Caravana con Caldero funcional
- [ ] Drag & drop de ingredientes + timer cocción
- [ ] RecipeDatabase con 5 recetas base
- [ ] Recetario visual (UIRecipeBook)

### Fase 4 — Pedidos y Economía (Semanas 10–11)
**Objetivo:** Loop completo explorar → cocinar → vender → cobrar.
- [ ] OrderManager: 3 pedidos activos con timer
- [ ] CurrencyManager: monedas + fama
- [ ] NPCs Brulo y Velta con diálogo básico
- [ ] Sistema de reputación básico
- [ ] Save/Load completo

### Fase 5 — Progresión y UI (Semana 12)
**Objetivo:** El juego se siente como un producto.
- [ ] APIs públicas verificadas entre scripts (ver PROGRESS.md)
- [ ] Escenas MainMenu, GameScene y Caravana creadas
- [ ] HUD completo con barra superior + burbuja del Pato + barra inferior
- [ ] Panel de regreso de cacería (UIHuntResults)
- [ ] Panel de bautizo del Pato funcional
- [ ] Sistema de mejoras de caravana (UIUpgradeShop)
- [ ] Sistema de logros (UIAchievementsPanel)
- [ ] Sistema de rangos (UIRankDisplay)
- [ ] Tutorial guiado por el Pato (TutorialManager)

### Fase 6 — Pulido MVP (Semanas 13–14)
**Objetivo:** Build jugable en Android.
- [ ] SFX: captura, cocina, venta, cuacs del Pato
- [ ] Música Pradera + Caravana (loop)
- [ ] 1 Rewarded Ad (duplicar venta) — AdMob
- [ ] Tutorial completo con el Pato
- [ ] Build Android → APK firmado
- [ ] Track interno Google Play Console

---

## 🚀 LANZAMIENTO

### v1.0 — Early Access (Semana 15–16)
- Pradera de Bruma completa (3 criaturas + 5 recetas)
- Caravana Nv1 y Nv2
- 4 NPCs básicos
- Sistema de pedidos funcional
- Pato con nombre personalizable
- Google Play Open Beta

---

## 🌱 SISTEMA DE CONTENIDO VIVO (Post-lanzamiento)

> A partir del lanzamiento, el juego **nunca deja de crecer**.
> Cada actualización responde a la pregunta del jugador:
> *"¿Qué hay nuevo hoy en Caravan Kitchen?"*

---

### 🌍 EXPANSIONES DE ZONA (cada 2–3 meses)

Cada zona nueva es una actualización mayor con trailer, teaser de criaturas
y mecánica nunca vista antes en el juego.

| Actualización | Zona | Criaturas | Recetas | Mecánica nueva |
|---|---|---|---|---|
| v1.1 | Bosque de Vapor Dulce | +4 (C05–C08) | +4 | Cebo aromático, trampa de calor |
| v1.2 | Arrecife de Nubes | +4 (C09–C12) | +4 | Campana de bruma, movimiento vertical |
| v1.3 | Barranco del Caldero | +4 (C13–C16) | +4 | Frasco térmico, trampa de vapor |
| v2.0 | Mercado Suspendido | NPCs nuevos | +6 | Pedidos premium, recetas legendarias |
| v2.1 | **[ZONA 5 — sin revelar]** | +4–6 | +4–6 | ??? |
| v2.2 | **[ZONA 6 — sin revelar]** | +4–6 | +4–6 | ??? |
| v3.0 | **[ZONA 7 — Gran Expansión]** | +8 | +8 | Mecánica completamente nueva |

> 🔒 Las zonas 5–7 se diseñan pero **no se anuncian** hasta que la zona anterior
> lleva al menos 4 semanas activa. El misterio es parte del juego.

---

### 🐾 CRIATURAS NUEVAS (cada 3–4 semanas)

Criaturas que no están ligadas a una zona nueva sino a **condiciones especiales:**

| Tipo | Ejemplo | Cómo aparece |
|---|---|---|
| **Criatura de evento** | "Espantapatos de Maíz" en Festival de Cosecha | Solo durante el evento |
| **Criatura de clima** | "Cristalín de Tormenta" | Solo en clima de tormenta |
| **Criatura nocturna nueva** | Variante de Caracol de Canela azul | Cambio de temporada |
| **Criatura de aniversario** | Pato Dorado (homenaje al compañero) | Solo en el aniversario del juego |
| **Criatura secreta** | Aparece sin anuncio previo en zona ya existente | Descubrimiento orgánico |

> Las criaturas secretas son el motor del **boca a boca** del juego.
> Un jugador las descubre y lo comparte. El juego nunca las anuncia oficialmente.

---

### 🎉 EVENTOS DE TEMPORADA (cada 4–6 semanas)

Festivales limitados con contenido exclusivo. Vuelven cada año pero con
**1 criatura o receta nueva** añadida para que los jugadores veteranos
tengan razón de volver.

| Evento | Temporada | Exclusivos |
|---|---|---|
| 🌸 Festival de la Bruma Rosa | Primavera | Criatura: Puffshroom Rosa, Receta: Sopa de Pétalos |
| ☀️ Gran Caravana de Verano | Verano | Criatura: Mielín Solar, Receta: Helado de Néctar |
| 🍂 Festival de la Cosecha | Otoño | Criatura: Raicita Dorada, Receta: Guiso de Temporada |
| ❄️ Noche del Caldero Helado | Invierno | Criatura: Puffshroom de Nieve, Receta: Sopa Glacial |
| 🦆 Día del Pato | Aniversario del juego | Skin del Pato + receta exclusiva por 48h |

> Los eventos tienen **timer visible** para crear urgencia sana.
> El contenido exclusivo de evento **no regresa** hasta el siguiente año,
> pero las recetas quedan en el recetario como "obtenida en Festival X".

---

### 🍽️ RECETAS NUEVAS INDEPENDIENTES (cada 2–3 semanas)

Recetas que no requieren una actualización de zona, solo un parche menor:

- Recetas de **fusión**: combinan ingredientes de 2 zonas distintas
- Recetas de **temporada**: usan ingrediente de evento actual
- Recetas **legendarias desbloqueables**: requieren completar un desafío especial
- Recetas **del día**: rotación diaria con bonus de precio x1.5

---

### 🏕️ EXPANSIONES DE CARAVANA (cada 1–2 meses)

La caravana como negocio también crece visualmente y funcionalmente:

| Actualización | Qué añade |
|---|---|
| Caravana Nv3 | Mesa Dulce + farolitos, decoraciones |
| Caravana Nv4 | Secador de vapor + molino |
| Caravana Nv5 | Estación completa, aspecto restaurante |
| **Skins de caravana** | Caravana Nocturna, Festiva, Primaveral (IAP o Pase) |
| **Decoraciones** | Macetas, carteles, luces — compradas con monedas o fama |

---

### 🎨 CONTENIDO COSMÉTICO (continuo)

Para jugadores que ya tienen todo el contenido de juego:

- **Skins de Kael:** Traje de invierno, Chef Formal, Explorador Nocturno
- **Skins del Pato:** Pato con sombrero de chef, Pato de Navidad, Pato Pirata
- **Efectos de captura:** Net trail de colores, partículas especiales
- **Fuentes de UI alternativas:** Opción de accesibilidad

---

### 📊 BESTIARIO Y RECETARIO VIVOS

El Bestiario y el Recetario nunca se "completan" — siempre hay
una entrada con `???` que el jugador no ha descubierto aún.

**Principio de diseño:**
- Cuando se añade una criatura nueva → el Bestiario muestra su silueta en negro antes de que el jugador la vea, con el texto: *"Algo nuevo se mueve en [zona]..."*
- Cuando se añade una receta nueva → aparece en el Recetario como `Receta desconocida — pista: necesita ingrediente de zona X`
- El Pato reacciona cuando hay contenido nuevo disponible (tint `Mysterious` al abrir la app)

---

## 📅 CALENDARIO DE CONTENIDO SUGERIDO (Año 1 post-lanzamiento)

| Mes | Actualización | Tipo |
|---|---|---|
| 0 | v1.0 Early Access | Lanzamiento |
| 1 | Parche de balance + 2 recetas nuevas | Menor |
| 2 | v1.1 — Bosque de Vapor Dulce | Mayor (zona) |
| 3 | Festival de la Bruma Rosa (primavera) | Evento |
| 4 | 2 criaturas secretas en Pradera y Bosque | Contenido vivo |
| 5 | v1.2 — Arrecife de Nubes | Mayor (zona) |
| 6 | Gran Caravana de Verano + Skin Kael | Evento + cosmético |
| 7 | Caravana Nv3 desbloqueada | Mejora hub |
| 8 | v1.3 — Barranco del Caldero | Mayor (zona) |
| 9 | Festival de la Cosecha | Evento |
| 10 | 4 recetas legendarias desbloqueables | Contenido vivo |
| 11 | Noche del Caldero Helado + Pato Navidad skin | Evento + cosmético |
| 12 | v2.0 — Mercado Suspendido + **Día del Pato** | Mayor (zona) + aniversario |

---

## 🔔 SISTEMA DE NOTIFICACIONES DE CONTENIDO NUEVO

Para que el jugador **siempre sepa que hay algo nuevo:**

| Tipo | Cuándo aparece | Mensaje |
|---|---|---|
| Badge en ícono del juego | Evento nuevo activo | Punto rojo en el ícono del app |
| Notificación push | 1h antes de que termine un evento | *"¡El Festival acaba en 1 hora! Kael y [nombre del pato] te esperan"* |
| Pantalla de novedades | Al abrir el juego tras actualización | Animación del Pato presentando lo nuevo |
| Frase especial del Pato | Al abrir en evento activo | *"¡{name} detecta algo diferente hoy! ¡CUAC!"* |
| Silueta en Bestiario | Criatura nueva disponible | Silueta negra + texto misterioso |

---

## 📋 LOG DE VERSIONES DEL ROADMAP

| Versión | Fecha | Cambios |
|---|---|---|
| v1.0 | Mayo 2026 | ROADMAP inicial, fases 0–6 + post-MVP básico |
| v2.0 | Mayo 26, 2026 | Filosofía de expansión perpetua, sistema de contenido vivo, calendario año 1, criaturas secretas, Bestiario vivo, notificaciones |
