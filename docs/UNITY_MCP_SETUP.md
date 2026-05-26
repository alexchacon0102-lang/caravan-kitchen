# ⚙️ Conectar Claude Code con Unity via MCP

> Guía para configurar el MCP de Unity con Claude Code y trabajar
> directamente sobre el proyecto Caravan Kitchen desde Claude.
> Versión: 1.0 | Mayo 26, 2026

---

## ¿Qué es esto y para qué sirve?

**MCP (Model Context Protocol)** es una conexión entre Claude Code y
herramientas externas como Unity. Con esto puedes:

- Pedirle a Claude que cree o modifique scripts C# directamente en Unity
- Ver el estado de la consola de Unity desde Claude
- Ejecutar comandos del Editor (crear escenas, prefabs, etc.) con palabras
- Compilar el proyecto y ver errores sin abrir Unity manualmente

---

## Opción A — Unity MCP Server (paquete oficial experimental)

### Requisitos
- Unity 6.3 LTS instalado
- Node.js 18+ instalado ([nodejs.org](https://nodejs.org))
- Claude Desktop o Claude Code con soporte MCP
- Git instalado

### Pasos de instalación

#### 1. Instalar el paquete MCP en Unity

Abre Unity, ve a **Window → Package Manager**, haz clic en el botón **+**
y selecciona **"Add package by name"**. Escribe:

```
com.unity.mcp
```

Si no aparece, usa **"Add package from git URL"** con:
```
https://github.com/Unity-Technologies/mcp-unity.git
```

#### 2. Iniciar el servidor MCP desde Unity

Una vez instalado el paquete, en Unity ve a:
**Tools → MCP → Start Server**

Deja Unity abierto. El servidor corre en `localhost:6400` por defecto.

#### 3. Configurar Claude Desktop

Abre el archivo de configuración de Claude Desktop:

**Mac/Linux:**
```
~/.config/claude/claude_desktop_config.json
```

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

Agrega esta configuración:

```json
{
  "mcpServers": {
    "unity": {
      "command": "node",
      "args": ["/ruta/al/mcp-unity/build/index.js"],
      "env": {
        "UNITY_PORT": "6400"
      }
    }
  }
}
```

> Reemplaza `/ruta/al/mcp-unity/` con la ruta real donde está instalado.
> En Windows usa barras invertidas o escapa con doble barra: `C:\\Users\\...`

#### 4. Reiniciar Claude Desktop

Cierra y vuelve a abrir Claude Desktop. Si la conexión fue exitosa
verás el ícono de Unity en el panel de herramientas de Claude.

---

## Opción B — Prompt directo para Claude Code (sin instalación MCP)

Si no quieres instalar el servidor MCP ahora, usa este prompt al
iniciar cada sesión con Claude Code para darle todo el contexto:

```
Eres un asistente de desarrollo de videojuegos Unity especializado en el
proyecto Caravan Kitchen. El proyecto está en:
https://github.com/alexchacon0102-lang/caravan-kitchen

Tecnologías:
- Motor: Unity 6.3 LTS (6000.3.16f1)
- Lenguaje: C# con namespaces CaravanKitchen.*
- Plataforma objetivo: Android (Google Play)
- Input System: Unity Input System incluido
- UI: TextMeshPro + Unity UI
- 2D con URP (Universal Render Pipeline)
- Rigidbody2D usa linearVelocity (no velocity — obsoleto en Unity 6)
- FindObjectOfType obsoleto → usar FindFirstObjectByType

Estructura de carpetas:
Assets/Scripts/Core/       → GameManager, SaveSystem, XPManager, AchievementManager, RankManager, TutorialManager
Assets/Scripts/Player/     → PlayerController, PlayerInventory, CaptureToolController, CameraFollow  
Assets/Scripts/World/      → ZoneManager, ResourceNode, CaravanUpgradeManager, DayNightCycle, MapExpansionManager, ReputationManager, SeasonalEventManager
Assets/Scripts/Creatures/  → CreatureBase, CreatureAI
Assets/Scripts/Cooking/    → CookingStation, RecipeDatabase, DishQualitySystem
Assets/Scripts/Economy/    → OrderManager, CurrencyManager
Assets/Scripts/UI/         → HUDController, MainMenuUI, UIOrderCard, UIRecipeBook, UIAchievementsPanel, UIUpgradeShop, UIRankDisplay
Assets/Scripts/Audio/      → AudioManager
Assets/Scripts/Companion/  → PatoController (compañero pato blanco regordete)

Compañero: Pato blanco regordete, ojitos negros, cachetes rosados.
Script: PatoController.cs (NimbusController.cs es alias obsoleto).

Contexto de la siguiente tarea: [DESCRIBE AQUÍ LO QUE NECESITAS]
```

---

## Opción C — Claude Code con contexto via CLAUDE.md

Crea un archivo `CLAUDE.md` en la raíz del proyecto con el contexto
de arriba. Claude Code lo lee automáticamente al abrir el proyecto:

### Instrucciones:
1. Descarga el proyecto de GitHub:
   ```bash
   git clone https://github.com/alexchacon0102-lang/caravan-kitchen.git
   ```
2. Abre la carpeta en Claude Code (File → Open Folder)
3. Claude Code leerá el `CLAUDE.md` automáticamente
4. Cada vez que pidas código, Claude tendrá el contexto completo

---

## Comandos útiles para pedirle a Claude con MCP Unity activo

```
"Crea una escena nueva llamada GameScene con los managers necesarios"
"Agrega el script PatoController al GameObject llamado Pato en la escena"
"Muéstrame los errores de compilación actuales"
"Ejecuta el juego en modo Play y dime qué errores hay en consola"
"Crea un Prefab del Pato en Assets/Prefabs/Companion/"
"Cambia el valor de moveSpeed en PlayerController a 5.5"
```

---

## Verificar que la conexión MCP funciona

Con Unity abierto y el servidor MCP corriendo, escríbele a Claude:

```
¿Puedes ver el proyecto Unity que tengo abierto?
Dime qué escenas tiene y qué scripts están en Assets/Scripts/
```

Si Claude responde con la lista real de archivos de tu proyecto,
la conexión está funcionando correctamente.

---

## Notas importantes

- Unity debe estar **abierto y con el proyecto cargado** para que MCP funcione
- Si Unity pierde foco o entra en modo Play, el servidor puede pausarse
- Guarda la escena en Unity antes de pedirle cambios a Claude
- Los cambios de Claude via MCP se aplican en tiempo real al Editor
- Si hay errores de compilación, Claude los verá y los corregirá solo
