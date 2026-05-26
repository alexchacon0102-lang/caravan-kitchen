# 📋 Game Design Document — Caravan Kitchen
**Versión:** 0.2 | **Motor:** Unity 2022.3 LTS | **Plataforma:** Android (Google Play)

---

## 1. Visión del Juego

**Caravan Kitchen** es un juego móvil 2D cozy de exploración culinaria donde un chef nómada viaja por regiones flotantes de bruma capturando criaturas culinarias e ingredientes raros para cocinar, vender y expandir una cocina-caravana mágica.

### Propuesta de valor única
> *"El loop de captura→proceso→venta→mejora de Zombie Catchers, transformado en una fantasía gastronómica cozy con identidad propia."*

### Pilares de diseño

| Pilar | Descripción |
|---|---|
| Explorar tiene propósito | Cada salida responde "¿qué me falta para cocinar lo siguiente?" |
| Cocinar es el corazón | La cocina conecta la captura con la venta de forma satisfactoria |
| La caravana crece contigo | El hub evoluciona visualmente con cada mejora |
| Loop corto, satisfacción grande | 3–8 min por sesión, siempre con al menos 1 recompensa sentida |
| Sin presión, con motivación | Monetización opcional, sin paywall que bloquee progreso central |

---

## 2. Core Loop Detallado

### Loop de sesión (3–8 minutos)
```
[CARAVANA] Revisar pedidos activos
     ↓
[MAPA] Elegir región según necesidad
     ↓
[EXPLORACIÓN] Recolectar ingredientes + capturar criaturas
     ↓
[CARAVANA] Cocinar recetas con recursos
     ↓
[VENTA] Entregar pedidos + venta libre
     ↓
[MEJORA] Invertir en upgrades o desbloqueos
     ↓
→ Nueva motivación generada (criatura nueva / receta nueva / zona nueva)
```

### Loop diario
- 3 misiones del día
- 1 pedido especial rotativo
- 1 ingrediente destacado del día
- 1 criatura brillante (rareza aumentada)

### Loop semanal
- 1 receta semanal nueva
- 1 ruta limitada temporal
- 1 evento de temporada activo

---

## 3. Jugador y Fantasía

El jugador debe sentir que es un **chef aventurero** con un negocio vivo y en crecimiento.

Emociones objetivo por fase del loop:
- **Exploración:** curiosidad, descubrimiento
- **Captura:** pequeña tensión, satisfacción
- **Cocina:** placer creativo, anticipación
- **Venta:** orgullo, recompensa
- **Mejora:** motivación, anticipación del siguiente ciclo

---

## 4. Personajes

### Kiri — Protagonista
- Chef exploradora nómada
- Sombrero de cocina con goggles
- Mochila-caldero, bufanda larga
- Red plegable, guantes y botas de campo
- Paleta: crema, turquesa, ámbar miel

**Animaciones Unity (Animator Controller):**
- Idle, Walk, Run, Jump, Land
- Collect (agacharse)
- ThrowNet, PlaceBait
- Cook (revolver caldero)
- Celebrate, Tired

### Fumi — Compañero
- Nubecita pequeña con ojos expresivos
- Brilla en azul al detectar criatura rara
- Brilla en amarillo al detectar ingrediente especial
- Reacciona con emojis flotantes al cocinar recetas nuevas
- Guía tutoriales con flechas y burbujas

### NPCs de Mercado
| NPC | Función |
|---|---|
| Marchante Brulo | Pedidos normales diarios |
| Señora Velta | Pedidos especiales de recetas raras |
| El Anciano Nomo | Recetas legendarias y pistas de zonas |
| Kofi el Trotamundos | Intercambio de ingredientes por platillos |

---

## 5. Mundo y Regiones

Ver [CREATURES.md](CREATURES.md) para fauna completa por región.

### Caravana Celeste (Hub)
Base de operaciones. Crece visualmente de nivel 1 a 5:
- **Nv1:** Caldero básico, toldo pequeño
- **Nv2:** Parrilla añadida, toldo grande
- **Nv3:** Mesa dulce, farolitos, más espacio
- **Nv4:** Secador, molino, decoraciones
- **Nv5:** Estación completa, aspecto restaurante viajero

### Regiones desbloqueables
| Región | Dificultad | Desbloqueo | Mecánica nueva |
|---|---|---|---|
| Pradera de Bruma | ★☆☆☆☆ | Inicio | Recolección, red, cebo básico |
| Bosque de Vapor Dulce | ★★☆☆☆ | 500m + Red Nv2 | Cebo aromático, trampa calor |
| Arrecife de Nubes | ★★★☆☆ | 1200m + Mochila Nv2 | Campana, movimiento vertical |
| Barranco del Caldero | ★★★★☆ | 2500m + Herramienta Nv3 | Frasco térmico, trampa vapor |
| Mercado Suspendido | Social | 800 fama + Zona 2 | Pedidos premium, recetas legendarias |

---

## 6. Sistemas de Juego

### Sistema de captura
No es combate. Es observación + herramienta correcta:
- Detectar criatura (Fumi brilla)
- Seleccionar herramienta adecuada
- Colocar/lanzar en posición correcta
- Animación de captura → ingresa inventario

**Comportamientos de IA de criaturas:**
- IDLE → deambula en zona
- ALERT → jugador detectado
- FLEE → huye
- ATTRACTED → va al cebo
- CAPTURED → animación y recolección

### Sistema de cocina
- Arrastrar ingredientes a la estación correcta
- Timer de cocción (30 seg – 3 min)
- Calidad determinada por ingredientes usados
- Resultado: platillo con nombre, ilustración, valor

### Sistema de pedidos
- 3 pedidos activos simultáneos (más con mejoras)
- Tipos: normal, especial, raro, evento
- Clientes espontáneos con 10–30% bonus
- Timer visible pero no agresivo

---

## 7. Monetización

### Rewarded Ads (opt-in, máx 5/sesión)
| Activador | Recompensa |
|---|---|
| Botón en venta | Duplicar ganancia del platillo |
| Botón en cocina | Terminar cocción al instante |
| Botón en expedición | Aparecer criatura rara 30 seg |
| Botón en mochila llena | +2 espacios temporales |
| Botón al volver vacío | Reintentar con cebo premium gratis |

### IAP
| Producto | Precio |
|---|---|
| Quitar anuncios | $1.99 |
| Pack Inicial del Chef | $2.99 |
| Skin Caravana Nocturna | $1.99 |
| Skin Caravana Festiva | $1.99 |
| Cristales 100 | $0.99 |
| Pase de Temporada | $4.99 |

---

## 8. Progresión de largo plazo

| Capa | Tiempo | Meta |
|---|---|---|
| Inmediata | Segundos/min | Capturar, cocinar, vender |
| Diaria | Por sesión | Misiones del día, pedido especial |
| Colección | Días/semanas | Recetario y bestiario completos |
| Estructural | Semanas/meses | Caravana max, todas las zonas |
