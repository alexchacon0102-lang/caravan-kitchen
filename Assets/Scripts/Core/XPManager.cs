using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Caravan Kitchen — XPManager.cs
/// Sistema de experiencia y rangos del jugador.
/// XP se gana con: capturas, recetas cocinadas, pedidos entregados, logros.
/// Cada rango desbloquea mejoras, zonas y cosméticos.
/// </summary>
public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    // ─── RANGOS ───────────────────────────────────────────────────────────
    public enum ChefRank
    {
        Aprendiz,       // Nv 1–3     Rango inicial
        AyudanteDeCocina, // Nv 4–6  Primera mejora visible
        CocinaDeRuta,   // Nv 7–9     Desbloquea zona 2
        ChefViajero,    // Nv 10–14   Desbloquea Nimbus upgrade
        MaestroDeSabor, // Nv 15–19   Desbloquea zona 4
        ChefLegendario, // Nv 20–24   Desbloquea Mercado Suspendido
        NomadaCelestial,// Nv 25–29   Desbloquea recetas legendarias
        ElGranChef      // Nv 30+     Rango máximo
    }

    [System.Serializable]
    public class RankData
    {
        public ChefRank rank;
        public string displayName;
        public string emoji;
        public int requiredLevel;
        public string unlockDescription;
        public Color rankColor;
    }

    // ─── TABLA DE RANGOS ─────────────────────────────────────────────────
    public RankData[] rankTable = new RankData[]
    {
        new RankData { rank=ChefRank.Aprendiz,          displayName="Aprendiz",           emoji="🌱", requiredLevel=1,  unlockDescription="Bienvenido a la caravana.",                       rankColor=new Color(0.6f,0.9f,0.6f) },
        new RankData { rank=ChefRank.AyudanteDeCocina,  displayName="Ayudante de Cocina", emoji="🥂", requiredLevel=4,  unlockDescription="+1 slot pedido activo. Caldero Nivel 2.",          rankColor=new Color(0.8f,0.9f,0.5f) },
        new RankData { rank=ChefRank.CocinaDeRuta,      displayName="Cocina de Ruta",     emoji="🚜", requiredLevel=7,  unlockDescription="Desbloquea: Bosque de Vapor Dulce.",               rankColor=new Color(0.5f,0.85f,0.85f) },
        new RankData { rank=ChefRank.ChefViajero,       displayName="Chef Viajero",       emoji="🧃", requiredLevel=10, unlockDescription="Nimbus con radar de rareza. +1 herramienta.",      rankColor=new Color(0.4f,0.7f,1.0f) },
        new RankData { rank=ChefRank.MaestroDeSabor,    displayName="Maestro de Sabor",   emoji="⭐", requiredLevel=15, unlockDescription="Desbloquea: Arrecife de Nubes + Barranco.",          rankColor=new Color(1.0f,0.8f,0.2f) },
        new RankData { rank=ChefRank.ChefLegendario,    displayName="Chef Legendario",    emoji="🔥", requiredLevel=20, unlockDescription="Desbloquea: Mercado Suspendido.",                 rankColor=new Color(1.0f,0.5f,0.2f) },
        new RankData { rank=ChefRank.NomadaCelestial,   displayName="Nómada Celestial",   emoji="🌌", requiredLevel=25, unlockDescription="Recetas legendarias disponibles. Zona 6.",       rankColor=new Color(0.7f,0.4f,1.0f) },
        new RankData { rank=ChefRank.ElGranChef,        displayName="El Gran Chef",        emoji="👑", requiredLevel=30, unlockDescription="Rango máximo. Acceso a la Cumbre del Gran Velo.", rankColor=new Color(1.0f,0.9f,0.1f) },
    };

    // ─── XP POR ACCIÓN ────────────────────────────────────────────────────
    public static readonly int XP_CAPTURE_COMMON   = 10;
    public static readonly int XP_CAPTURE_UNCOMMON = 20;
    public static readonly int XP_CAPTURE_RARE     = 40;
    public static readonly int XP_CAPTURE_EPIC     = 80;
    public static readonly int XP_CAPTURE_SHINY    = 150;
    public static readonly int XP_RECIPE_COOK      = 15;
    public static readonly int XP_RECIPE_NEW       = 50;
    public static readonly int XP_ORDER_NORMAL     = 20;
    public static readonly int XP_ORDER_PREMIUM    = 40;
    public static readonly int XP_ORDER_LEGENDARY  = 100;
    public static readonly int XP_ACHIEVEMENT      = 75;
    public static readonly int XP_ZONE_UNLOCK      = 200;

    // ─── ESTADO ──────────────────────────────────────────────────────────
    public int CurrentXP      { get; private set; }
    public int CurrentLevel   { get; private set; } = 1;
    public ChefRank CurrentRank { get; private set; } = ChefRank.Aprendiz;
    public int XPForNextLevel => LevelToXP(CurrentLevel + 1);
    public float XPProgress   => (float)(CurrentXP - LevelToXP(CurrentLevel)) / (LevelToXP(CurrentLevel + 1) - LevelToXP(CurrentLevel));

    public UnityEvent<int, int> onLevelUp;   // (oldLevel, newLevel)
    public UnityEvent<ChefRank> onRankUp;

    private HUDController hud;

    // ─── INICIALIZACIÓN ───────────────────────────────────────────────────
    void Awake()
    {
        Instance = this;
        hud = FindObjectOfType<HUDController>();
    }

    // ─── AÑADIR XP ────────────────────────────────────────────────────────
    public void AddXP(int amount)
    {
        CurrentXP += amount;
        hud?.ShowFloatingText($"+{amount} XP", new Color(0.4f, 0.9f, 1f));
        CheckLevelUp();
        GameManager.Instance?.SaveGame();
    }

    private void CheckLevelUp()
    {
        while (CurrentXP >= LevelToXP(CurrentLevel + 1))
        {
            int oldLevel = CurrentLevel;
            CurrentLevel++;
            onLevelUp?.Invoke(oldLevel, CurrentLevel);
            hud?.ShowLevelUpPopup(CurrentLevel);
            Debug.Log($"[XP] ¡Nivel {CurrentLevel}!");
            CheckRankUp();
        }
        hud?.UpdateXPBar(XPProgress, CurrentLevel);
    }

    private void CheckRankUp()
    {
        for (int i = rankTable.Length - 1; i >= 0; i--)
        {
            if (CurrentLevel >= rankTable[i].requiredLevel && CurrentRank != rankTable[i].rank)
            {
                CurrentRank = rankTable[i].rank;
                onRankUp?.Invoke(CurrentRank);
                hud?.ShowRankUpPopup(rankTable[i]);
                Debug.Log($"[Rank] Nuevo rango: {rankTable[i].displayName}");
                break;
            }
        }
    }

    // ─── FÓRMULA DE NIVEL ─────────────────────────────────────────────────
    // XP total requerida para alcanzar un nivel: 100 * (nivel^1.5)
    public static int LevelToXP(int level) => Mathf.RoundToInt(100f * Mathf.Pow(level, 1.5f));

    // ─── CARGAR DESDE SAVE ───────────────────────────────────────────────
    public void LoadFromSave(int savedXP, int savedLevel)
    {
        CurrentXP    = savedXP;
        CurrentLevel = savedLevel;
        for (int i = rankTable.Length - 1; i >= 0; i--)
            if (CurrentLevel >= rankTable[i].requiredLevel) { CurrentRank = rankTable[i].rank; break; }
        hud?.UpdateXPBar(XPProgress, CurrentLevel);
    }
}
