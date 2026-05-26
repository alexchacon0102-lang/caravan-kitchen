# 🦆 Diseño Visual del Pato — Caravan Kitchen

> Documento de referencia para arte, animación y UI del compañero Pato.
> Versión: 1.0 | Mayo 26, 2026

---

## Descripción General

El Pato es el compañero inseparable del chef Kael. Es un pato blanco regordete
con ojitos negros brillantes, cachetes rosados permanentes, pico naranja rechoncho
y alitas cortas. Su personalidad es curiosa, leal y entrañable. Habla con "cuac".

---

## 🎨 Prompt de Imagen de Referencia

### Prompt principal (idle / neutral)
```
2D pixel art companion character, chubby white duck, small round black shiny
eyes, permanent rosy pink chubby cheeks, small orange beak, tiny short wings,
orange stubby feet, round plump body, cozy fantasy game style, front-facing
idle pose with gentle side-to-side waddle implied, clean black outlines,
transparent background, soft warm palette, mobile game sprite, 64x64px
```

### Prompt hoja de emociones (sprite sheet)
```
2D pixel art sprite sheet, chubby white duck companion, 8 emotion states in
a grid 4x2: neutral (white, gentle waddle), curious (light blue tint, head
tilted), happy (yellow tint, spinning), excited (orange tint, wings flapping),
alert (red tint, trembling), mysterious (lavender tint, wide eyes), approving
(mint green tint, puffed up satisfied), tired (gray tint, sitting down with
half-closed eyes). Rosy cheeks visible in all states. Clean outlines, cozy
fantasy mobile style, transparent background
```

### Prompt cachetes y expresiones de cerca
```
2D pixel art duck face closeup reference sheet, chubby white duck, multiple
expressions: happy (eyes curved upward, cheeks bright pink), excited (eyes
wide open with sparkles), tired (eyes half-closed, cheeks pale), curious
(one eye bigger, head tilted), approving (eyes squinted happy, puffed cheeks).
Round black eyes, prominent rosy cheeks, small orange beak. Clean art style.
```

### Prompt animación waddle (frame sheet)
```
2D pixel art animation frames, chubby white duck walking waddle cycle,
6 frames side view: lean left, center up, lean right, center down, repeat.
Body rocks side to side, feet alternate, wings slightly out for balance.
Cozy game animation style, clean outlines, transparent background, 48x48px each frame
```

---

## 🖼️ Assets de Sprite a Crear

| Asset | Tamaño | Descripción |
|---|---|---|
| pato_body.png | 64x64 | Sprite base blanco (recibe tint de color) |
| pato_cheeks.png | 64x64 | Solo los cachetes rosados (capa separada) |
| pato_glow.png | 80x80 | Aura/glow detrás del pato (recibe tint) |
| pato_beak.png | 64x64 | Pico naranja (capa separada, no recibe tint) |
| pato_feet.png | 64x64 | Patitas naranjas (capa separada) |
| pato_shadow.png | 64x32 | Sombra suave debajo del pato |

> **IMPORTANTE:** El sprite `pato_body.png` debe ser completamente **blanco puro (#FFFFFF)**
> para que el sistema de tint de color funcione correctamente en Unity.
> Los cachetes, pico y patitas son capas separadas con sus colores fijos.

---

## 🎬 Animaciones a Crear (Animator Controller)

| Estado | Frames | FPS | Loop | Disparador |
|---|---|---|---|---|
| Waddle_Idle | 4 | 8 | Sí | Por defecto |
| Waddle_Walk | 6 | 12 | Sí | `isMoving = true` |
| Flap_Excited | 6 | 16 | No | Trigger `Excited` |
| Sit_Tired | 3 | 6 | Sí (último frame) | Trigger `Tired` |
| Head_Tilt | 3 | 8 | No | Trigger `Curious` |
| Spin_Happy | 8 | 16 | No | Trigger `Happy` |
| Puff_Approving | 4 | 10 | No | Trigger `Approving` |
| Shiver_Alert | 4 | 20 | Sí | Trigger `Alert` |
| Quack_Talk | 3 | 10 | No | Trigger `Talk` (al mostrar diálogo) |

---

## ⚙️ Configuración en Unity Inspector

El Pato en Unity tiene **4 capas de SpriteRenderer** apiladas en Z:

```
GameObject: Pato
  ├── Sprite: pato_glow      (Order: 0)  ← Aura detrás, recibe tint
  ├── Sprite: pato_shadow    (Order: 1)  ← Sombra debajo
  ├── Sprite: pato_body      (Order: 2)  ← Cuerpo blanco, recibe tint  ← ASIGNAR en bodyRenderer
  ├── Sprite: pato_feet      (Order: 3)  ← Patitas naranjas fijas
  ├── Sprite: pato_beak      (Order: 4)  ← Pico naranja fijo
  └── Sprite: pato_cheeks    (Order: 5)  ← Cachetes rosados fijos     ← ASIGNAR en cheeksRenderer

Componentes en el objeto raíz:
  ├── PatoController.cs
  ├── Animator (usa PatoAnimator.controller)
  ├── CircleCollider2D (trigger, radio = 0.4)
  └── AudioSource (para cuacs)

Canvas hijo (para el diálogo):
  └── DialogBubble (GameObject)
        └── Text_Dialog (TextMeshProUGUI)  ← ASIGNAR en dialogText
```

---

## 🔗 Cómo el Pato se conecta con otros scripts

| Quién llama | Método | Cuándo |
|---|---|---|
| OrderManager.cs | `PatoController.Instance.ReactToLegendaryDish()` | Pedido legendario entregado |
| CookingStation.cs | `PatoController.Instance.ReactToNewRecipe()` | Receta nueva completada |
| MapExpansionManager.cs | `PatoController.Instance.ReactToNewZone()` | Primera visita a zona |
| XPManager.cs | `PatoController.Instance.AddAffinity(5)` | Al subir de nivel |
| TutorialManager.cs | `PatoController.Instance.SayPhrase(step.nimbusPhrase)` | Paso de tutorial |
| GameManager.cs | `PatoController.Instance.ReactToLowEnergy()` | Energía < 20% |

> **Nota:** TutorialManager usa el campo `nimbusPhrase` que internamente
> ahora es la frase del pato. El nombre del campo se puede renombrar a
> `patoPhrase` en Fase 5 sin romper funcionalidad.

---

## 🔊 Sonidos del Pato

| Sonido | Archivo sugerido | Cuándo |
|---|---|---|
| Cuac normal | `sfx_pato_quack.wav` | Al mostrar diálogo |
| Cuac emocionado | `sfx_pato_excited.wav` | Emoción Excited |
| Cuac suave | `sfx_pato_soft.wav` | Emoción Curious |
| Aleteo | `sfx_pato_flap.wav` | Flap_Excited animation |
| Cuac cansado | `sfx_pato_tired.wav` | Emoción Tired |

Todos en formato `.ogg` para Android. Guardar en `Assets/Audio/SFX/Companion/`
