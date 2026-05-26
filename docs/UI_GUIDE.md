# 🎮 Guía de UI — Caravan Kitchen

> Instrucciones **no técnicas** para crear los menús, botones,
> paneles y la pantalla de regreso de cacería en Unity.
> Versión: 1.0 | Mayo 26, 2026

---

## Antes de empezar: concepto visual

Todo el juego tiene estética **cozy fantasy** — cálido, redondo,
sin bordes agresivos. Los colores base son:

| Elemento | Color principal | Color secundario |
|---|---|---|
| Fondos de panel | Marrón miel oscuro `#3D2B1F` | Borde crema `#E8D5B0` |
| Botones activos | Turquesa `#4ECDC4` | Texto crema `#FFF8E7` |
| Botones secundarios | Naranja ámbar `#F4A261` | Texto oscuro `#3D2B1F` |
| Texto principal | Crema `#FFF8E7` | — |
| Texto de valor/moneda | Dorado `#FFD700` | — |
| Fondos de overlay | Negro semitransparente `rgba(0,0,0,0.55)` | — |

**Fuente sugerida:** `Lato Bold` o `Nunito Bold` para títulos,
`Lato Regular` para cuerpo. Ambas gratuitas en Google Fonts.

---

## 📋 PANTALLA 1 — Menú Principal

### Qué tiene esta pantalla

```
┌─────────────────────────────────────┐
│          [Logo del juego]           │
│        Caravan Kitchen 🍳           │
│                                     │
│         [ ▶ JUGAR ]                 │
│         [ ⚙ OPCIONES ]              │
│         [ 🏆 LOGROS ]               │
│                                     │
│    Versión 0.1    [Créditos]        │
└─────────────────────────────────────┘
```

### Cómo crearlo en Unity paso a paso

1. **Crear escena** `MainMenu.unity` en `Assets/Scenes/`
2. Click derecho en Hierarchy → **UI → Canvas**
   - Render Mode: `Screen Space — Overlay`
   - UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1080 x 1920` (vertical móvil)
3. Dentro del Canvas, crea un **Image** como fondo
   - Asígnale el sprite de fondo o usa color sólido `#3D2B1F`
   - Anchor: stretch completo (todo el canvas)
4. Agrega el **logo** como Image centrado arriba (Y = +400)
5. Crea los **3 botones** uno debajo del otro (spacing ~120px)
   - Cada botón: Image con sprite redondeado, TextMeshPro adentro
   - Botón JUGAR: color turquesa, texto grande
   - Botones secundarios: naranja ámbar, texto mediano
6. Al botón JUGAR asígnale en el `On Click()` → `MainMenuUI.OnPlayPressed()`
7. Al botón OPCIONES → `MainMenuUI.OnOptionsPressed()`

---

## 🦆 PANTALLA 2 — Bautizo del Pato (primera vez)

### Qué hace
Aparece solo la primera vez que el jugador entra al juego,
después de la animación de intro. El pato aparece en pantalla
y el jugador le pone nombre.

### Estructura del panel
```
┌─────────────────────────────────────┐
│  🦆  (sprite del pato animado)      │
│                                     │
│  "¡Hola! Soy tu compañero.          │
│   ¿Cómo me vas a llamar?"           │
│                                     │
│  ┌─────────────────────────────┐   │
│  │  Escribe un nombre...       │   │
│  └─────────────────────────────┘   │
│                                     │
│  Tu pato se llamará "Quackie"       │
│                                     │
│  [ 🎲 Nombre aleatorio ]           │
│  [ ✅ ¡Ese es! ]                   │
│                                     │
│  ⚠ El nombre no puede estar vacío  │
└─────────────────────────────────────┘
```

### Cómo crearlo en Unity

1. En la escena GameScene, crea un **Panel** hijo del Canvas principal
   - Nombre: `Panel_NameYourDuck`
   - Fondo: imagen redondeada color miel oscuro con borde crema
   - Tamaño: 800x900px centrado en pantalla
2. Agrega el **sprite del pato** arriba del panel (Image, 200x200px)
3. Agrega un **TextMeshPro** con el texto de bienvenida
4. Agrega un **TMP_InputField** (campo de texto)
   - Placeholder text: `"Escribe un nombre..."`
   - Character Limit: 12
5. Agrega TextMeshPro para el preview `"Tu pato se llamará..."`
6. Crea **2 botones**: `Btn_Random` y `Btn_Confirm`
7. Crea TextMeshPro rojo para `"¡Ponle un nombre a tu pato!"` (desactivado por default)
8. Al objeto raíz del panel agrega el script **UINameYourDuck.cs**
9. Arrastra cada elemento al campo correspondiente en el Inspector

---

## 🗺️ PANTALLA 3 — Selección de Zona (Mapa)

### Estructura
```
┌─────────────────────────────────────┐
│  ←  MAPA DE REGIONES                │
│                                     │
│  [🌿 Pradera de Bruma    ★☆☆☆☆]   │
│  [🌲 Bosque de Vapor     ★★☆☆☆]   │
│  [☁️ Arrecife de Nubes   🔒]       │
│  [🌋 Barranco del Caldero 🔒]      │
│                                     │
│  [← Volver a Caravana]             │
└─────────────────────────────────────┘
```

### Cómo crearlo

1. Crea un **Panel** `Panel_Map` con scroll vertical si hay muchas zonas
2. Cada zona es una **tarjeta** (Image redondeada + nombre + estrellas + botón)
   - Zonas bloqueadas: overlay semitransparente + ícono candado
   - Zonas desbloqueadas: botón activo
3. Agrega el script existente `MapExpansionManager.cs` para controlar
   qué zonas están activas (ya está escrito)
4. El botón de cada zona llama a `ZoneManager.LoadZone(zoneID)`

---

## 🏕️ PANTALLA 4 — HUD de Exploración (durante cacería)

### Qué muestra mientras el jugador explora
```
┌─────────────────────────────────────┐
│ 🪙 350   ⭐ 1240 XP    🌤 Pradera  │  ← Barra superior
│                                     │
│              [JUEGO]                │
│                                     │
│ 🦆 Pato: "Algo hay aquí..."        │  ← Burbuja del pato
│                                     │
│ [🎒 Mochila]  [🗺️ Mapa]  [🏠 Casa]│  ← Barra inferior
└─────────────────────────────────────┘
```

### Cómo crearlo

1. En Canvas de GameScene crea **3 zonas del HUD**:

   **Barra superior** (Height: 120px, arriba del canvas)
   - Image fondo oscuro semitransparente
   - TextMeshPro monedas (con ícono de moneda)
   - TextMeshPro XP (con ícono de estrella)
   - TextMeshPro nombre de zona actual

   **Burbuja del Pato** (esquina inferior izquierda)
   - Image burbuja de diálogo (sprite con cola apuntando al pato)
   - TextMeshPro para el texto
   - Este es el `dialogBubble` que se asigna en `PatoController`

   **Barra inferior** (Height: 150px, abajo)
   - 3 botones en fila: Mochila, Mapa, Caravana
   - Cada botón: Image ícono + TextMeshPro label pequeño

2. Al Canvas HUD asígna el script `HUDController.cs`
3. Vincula los campos en el Inspector según la tabla del PROGRESS.md

---

## 🏁 PANTALLA 5 — Pantalla de Regreso de Cacería

> Esta es la pantalla que aparece cuando el jugador vuelve a la caravana
> después de una expedición. Resume lo que encontró.

### Diseño del panel
```
┌─────────────────────────────────────┐
│  🦆  "¡Qué expedición, Kael! CUAC" │
│                                     │
│  📦 BOTÍN DE HOY                   │
│  ─────────────────────────────────  │
│  🍄 Puffshroom      x3             │
│  🌿 Baya de Bruma   x5             │
│  ✨ Puffshroom Dorado x1  ← RARO!  │
│                                     │
│  ──────────────────────────────     │
│  🏆 +120 XP        🪙 +0 monedas   │
│                                     │
│        [ ✅ IR A COCINAR ]          │
└─────────────────────────────────────┘
```

### Cómo crearlo paso a paso

#### Paso 1 — Crear el Panel en Unity
1. En la escena `GameScene`, crea un Panel hijo del Canvas
   - Nombre: `Panel_HuntResults`
   - Fondo: imagen redondeada grande, color miel oscuro, borde dorado
   - Tamaño: 900 x 1100px, centrado
   - **Desactivado por default** (SetActive false)

#### Paso 2 — Contenido del panel (de arriba a abajo)

| Elemento | Tipo Unity | Notas |
|---|---|---|
| Imagen del Pato | Image (Sprite) | 160x160px, arriba del panel |
| Frase del Pato | TextMeshPro | Texto dinámico, cursiva, color crema |
| Título "BOTÍN DE HOY" | TextMeshPro | Negrita, letras grandes |
| Línea divisora | Image (1px height, color dorado) | — |
| Lista de items | ScrollView → Vertical Layout Group | Máx 5 visibles, scroll si hay más |
| Cada fila de item | Prefab `HuntResultRow` (ver abajo) | Instanciado dinámicamente |
| Línea divisora 2 | Image | — |
| Texto XP ganado | TextMeshPro | Ícono estrella + número |
| Texto monedas | TextMeshPro | Ícono moneda + número |
| Botón IR A COCINAR | Button grande turquesa | Cierra panel, cambia escena |

#### Paso 3 — Prefab de fila de item (`HuntResultRow.prefab`)
```
HuntResultRow
  ├── Image_ItemIcon    (32x32, ícono del ingrediente/criatura)
  ├── Text_ItemName     (TextMeshPro, nombre del item)
  ├── Text_Quantity     (TextMeshPro, "x3")
  └── Image_RareBadge  (Image con texto "✨ RARO", desactivado por default)
```
- Guardar en `Assets/Prefabs/UI/HuntResultRow.prefab`
- El badge "RARO" se activa solo si el item es rareza ≥ 3

#### Paso 4 — Script a crear en Fase 5: `UIHuntResults.cs`
Este script se encarga de:
- Recibir la lista de items recolectados al terminar la expedición
- Instanciar una fila `HuntResultRow` por cada item
- Mostrar el XP y monedas ganadas
- Hacer que el Pato diga una frase según el resultado
- Animar la aparición del panel (fade in + slide up)

**Frases del Pato según resultado:**
| Resultado | Frase |
|---|---|---|
| Mochila llena | `"¡{name} está orgulloso! ¡CUAC!"` |
| Criatura rara capturada | `"¡¡CUAC!! ¿Eso era real?"` |
| Regresó sin nada | `"...cuac. Mejor la próxima vez."` |
| Solo ingredientes comunes | `"Algo es algo, cuac."` |

---

## 🍳 PANTALLA 6 — HUD de Caravana (cocina)

### Qué muestra
```
┌─────────────────────────────────────┐
│ 🪙 350   ⭐ Rank: Fogonero   [📋 3]│  ← Pedidos activos
│                                     │
│   [🧪 Caldero] [🔥 Parrilla]       │
│   [🍮 Mesa Dulce] [💨 Secador]     │
│                                     │
│  [📖 Recetario] [⬆️ Mejoras] [🗺️] │
└─────────────────────────────────────┘
```

### Botones de las estaciones
1. Crea **4 botones grandes** en grid 2x2 en el centro
2. Cada botón tiene: ícono de la estación + nombre + indicador de estado
   - Verde: disponible
   - Naranja: cocinando (con timer visible)
   - Gris: bloqueada (nivel caravana insuficiente)
3. Al hacer tap en una estación abierta → abre panel de recetas de esa estación

---

## 🎨 REGLAS GENERALES DE DISEÑO UI

### Botones
- **Siempre redondeados** — border-radius visual alto
- **Tamaño mínimo:** 120x70px (para dedos en móvil)
- **Estado pressed:** reduce escala a 0.93 (Spring animation)
- **Estado disabled:** opacidad 40%, no interactuable

### Textos
- Nunca texto blanco puro sobre fondo blanco
- Siempre usar `TMP_Text` (TextMeshPro), nunca el Text legacy
- Tamaños: título=48, subtítulo=32, cuerpo=24, pequeño=18

### Paneles
- Siempre con fondo oscuro semitransparente detrás al abrirse
- Animación de apertura: `scale from 0.8 → 1.0` en 0.2 seg + fade in
- Animación de cierre: `scale 1.0 → 0.8` en 0.15 seg + fade out
- Botón de cerrar [X] siempre en esquina superior derecha

### Consistencia de íconos
- Todos los íconos: 48x48px, mismo estilo pixel art
- Moneda: ícono dorado
- XP/Estrella: ícono amarillo
- Rareza: ⚪ común, 🟢 poco común, 🟡 raro, 🔴 muy raro

---

## 📱 CONFIGURACIÓN DE CANVAS PARA MÓVIL

1. Selecciona el Canvas en Unity
2. En el componente **Canvas Scaler**:
   - UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1080 x 1920`
   - Screen Match Mode: `Match Width Or Height`
   - Match: `0.5` (50% width, 50% height)
3. Esto hace que la UI se vea bien en pantallas de 720p a 4K
4. Para pruebas en el Editor: usa el botón **Game** → resolución `1080x1920`
   o `Portrait` desde el menú de aspecto ratio

---

## 🔢 ORDEN DE CREACIÓN RECOMENDADO (Fase 5)

1. Canvas base con configuración móvil ✓
2. Panel_NameYourDuck + UINameYourDuck.cs
3. HUD exploración (barras superior e inferior)
4. Burbuja de diálogo del Pato
5. Panel_HuntResults + HuntResultRow.prefab
6. Pantalla de caravana con estaciones
7. Panel de recetario (UIRecipeBook ya existe)
8. Panel de pedidos (UIOrderCard ya existe)
9. Panel de mejoras (UIUpgradeShop ya existe)
10. Panel de logros (UIAchievementsPanel ya existe)
11. Menú principal (MainMenu.unity)
