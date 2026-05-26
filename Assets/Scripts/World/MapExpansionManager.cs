using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MapExpansionManager — Mapa de regiones desbloqueables
/// Maneja el estado de cada región, condiciones de desbloqueo,
/// recompensas de descubrimiento y tránsito entre zonas.
/// Fase 4 — Caravan Kitchen
/// </summary>
public class MapExpansionManager : MonoBehaviour
{
    public static MapExpansionManager Instance { get; private set; }

    // ─── Datos de Región ─────────────────────────────────
    [Serializable]
    public class RegionData
    {
        public string id;               // e.g. "pradera_bruma"
        public string displayName;
        [TextArea(2,4)]
        public string description;
        public string sceneToLoad;      // nombre de escena Unity
        public Sprite mapIcon;
        public Vector2 mapPosition;     // posición en el mapa UI

        [Header("Desbloqueo")]
        public bool  unlockedByDefault;
        public int   requiredPlayerLevel;
        public int   requiredFame;
        public string requiredRegionId; // región previa necesaria
        public int   travelEnergyCost;

        [Header("Dificultad")]
        [Range(1,5)] public int difficultyStars = 1;

        [Header("Recompensa al descubrir")]
        public int   discoveryXP    = 150;
        public int   discoveryCoins = 100;
        public string unlockRecipeId;   // receta especial desbloqueada al llegar

        [Header("Estado runtime (no tocar)")]
        public bool isUnlocked;
        public bool isDiscovered;       // jugador ya entró al menos una vez
        public int  timesVisited;
    }

    // ─── Inspector ────────────────────────────────────────
    [Header("Regiones del mundo")]
    public List<RegionData> regions = new();

    [Header("Referencias")]
    public XPManager      xpManager;
    public CurrencyManager currencyManager;
    public AudioManager   audioManager;

    // ─── Eventos ─────────────────────────────────────────
    public event Action<RegionData> OnRegionUnlocked;
    public event Action<RegionData> OnRegionDiscovered;
    public event Action<RegionData> OnRegionEntered;

    private RegionData _currentRegion;

    // ─── Unity ────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        InitializeDefaultRegions();
        LoadRegionStates();
        EvaluateUnlocks();
    }

    // ─── Inicialización ───────────────────────────────────
    void InitializeDefaultRegions()
    {
        if (regions.Count > 0) return; // ya configuradas por inspector

        regions = new List<RegionData>
        {
            new() { id="pradera_bruma",    displayName="Pradera de Bruma",
                    description="Campo suave cubierto de niebla de amanecer. Zona inicial.",
                    sceneToLoad="Zone_PraderaBruma",   mapPosition=new Vector2(0,0),
                    unlockedByDefault=true,            difficultyStars=1,
                    travelEnergyCost=10,               discoveryXP=0, discoveryCoins=0 },

            new() { id="bosque_vapor",     displayName="Bosque de Vapor Dulce",
                    description="Árboles de corteza carmesí sudan néctar.",
                    sceneToLoad="Zone_BosqueVapor",    mapPosition=new Vector2(220, 80),
                    requiredPlayerLevel=5,             requiredFame=100,
                    requiredRegionId="pradera_bruma",  difficultyStars=2,
                    travelEnergyCost=20,               discoveryXP=150, discoveryCoins=80,
                    unlockRecipeId="te_vapor_dulce" },

            new() { id="arrecife_nubes",   displayName="Arrecife de Nubes",
                    description="Plataformas de coral blanco flotando sobre un mar de nubes.",
                    sceneToLoad="Zone_ArrecifeNubes",  mapPosition=new Vector2(400, -40),
                    requiredPlayerLevel=10,            requiredFame=300,
                    requiredRegionId="bosque_vapor",   difficultyStars=3,
                    travelEnergyCost=35,               discoveryXP=250, discoveryCoins=150,
                    unlockRecipeId="ceviche_nubipez" },

            new() { id="barranco_caldero", displayName="Barranco del Caldero",
                    description="Paredes de roca volcánica. Temperatura sofocante.",
                    sceneToLoad="Zone_BarrancoCaldero",mapPosition=new Vector2(320, -220),
                    requiredPlayerLevel=16,            requiredFame=600,
                    requiredRegionId="arrecife_nubes", difficultyStars=4,
                    travelEnergyCost=50,               discoveryXP=400, discoveryCoins=250,
                    unlockRecipeId="guiso_salamandra" },

            new() { id="mercado_suspendido",displayName="Mercado Suspendido",
                    description="Ciudad flotante. Hub de comercio avanzado.",
                    sceneToLoad="Zone_MercadoSuspendido",mapPosition=new Vector2(500,120),
                    requiredPlayerLevel=12,            requiredFame=800,
                    requiredRegionId="arrecife_nubes", difficultyStars=2,
                    travelEnergyCost=40,               discoveryXP=300, discoveryCoins=500,
                    unlockRecipeId="platillo_mercado" },

            new() { id="jungla_canela",    displayName="Jungla de Canela Viva",
                    description="Los árboles huelen a especias. La lluvia sabe a vainilla.",
                    sceneToLoad="Zone_JunglaCanela",   mapPosition=new Vector2(600, 60),
                    requiredPlayerLevel=22,            requiredFame=1200,
                    requiredRegionId="mercado_suspendido",difficultyStars=4,
                    travelEnergyCost=60,               discoveryXP=500, discoveryCoins=350,
                    unlockRecipeId="te_canela_viva" },

            new() { id="mareas_eternas",   displayName="Las Mareas Eternas",
                    description="El mar sube y baja en ciclos de 10 min.",
                    sceneToLoad="Zone_MareaEterna",    mapPosition=new Vector2(700,-150),
                    requiredPlayerLevel=28,            requiredFame=2000,
                    requiredRegionId="jungla_canela",  difficultyStars=5,
                    travelEnergyCost=80,               discoveryXP=700, discoveryCoins=600,
                    unlockRecipeId="sopa_mareas" },

            new() { id="cumbre_gran_velo", displayName="Cumbre del Gran Velo",
                    description="Aquí comenzó todo. La receta maestra espera.",
                    sceneToLoad="Zone_CumbreGranVelo", mapPosition=new Vector2(850,-80),
                    requiredPlayerLevel=35,            requiredFame=5000,
                    requiredRegionId="mareas_eternas", difficultyStars=5,
                    travelEnergyCost=100,              discoveryXP=2000, discoveryCoins=2000,
                    unlockRecipeId="receta_maestra" }
        };
    }

    // ─── Evaluación de desbloqueos ────────────────────────
    public void EvaluateUnlocks()
    {
        int playerLevel = xpManager != null ? xpManager.CurrentLevel : 1;
        int fame        = currencyManager != null ? currencyManager.GetFame() : 0;

        foreach (var r in regions)
        {
            if (r.isUnlocked) continue;
            if (r.unlockedByDefault) { r.isUnlocked = true; continue; }

            bool levelOK  = playerLevel >= r.requiredPlayerLevel;
            bool fameOK   = fame        >= r.requiredFame;
            bool regionOK = string.IsNullOrEmpty(r.requiredRegionId)
                            || IsDiscovered(r.requiredRegionId);

            if (levelOK && fameOK && regionOK)
            {
                r.isUnlocked = true;
                OnRegionUnlocked?.Invoke(r);
                Debug.Log($"[MapExpansion] Región desbloqueada: {r.displayName}");
            }
        }
    }

    // ─── Viaje ────────────────────────────────────────────
    public bool CanTravelTo(string regionId)
    {
        var r = GetRegion(regionId);
        if (r == null || !r.isUnlocked) return false;
        // Aquí se validaría energía (PlayerController o similar)
        return true;
    }

    public void TravelTo(string regionId)
    {
        var r = GetRegion(regionId);
        if (r == null || !r.isUnlocked)
        {
            Debug.LogWarning($"[MapExpansion] Región no disponible: {regionId}");
            return;
        }

        bool firstVisit = !r.isDiscovered;
        r.isDiscovered  = true;
        r.timesVisited++;
        _currentRegion  = r;

        if (firstVisit)
        {
            OnRegionDiscovered?.Invoke(r);
            if (xpManager       != null) xpManager.AddXP(r.discoveryXP);
            if (currencyManager != null) currencyManager.AddCoins(r.discoveryCoins);
            Debug.Log($"[MapExpansion] Primera visita a: {r.displayName} — XP+{r.discoveryXP}");
        }

        OnRegionEntered?.Invoke(r);
        SaveRegionStates();

        // Cargar escena
        if (!string.IsNullOrEmpty(r.sceneToLoad))
            UnityEngine.SceneManagement.SceneManager.LoadScene(r.sceneToLoad);
    }

    // ─── Utilidades ───────────────────────────────────────
    public RegionData GetRegion(string id)
        => regions.Find(r => r.id == id);

    public bool IsDiscovered(string id)
    {
        var r = GetRegion(id);
        return r != null && r.isDiscovered;
    }

    public RegionData GetCurrentRegion() => _currentRegion;

    public List<RegionData> GetUnlockedRegions()
        => regions.FindAll(r => r.isUnlocked);

    public List<RegionData> GetLockedRegions()
        => regions.FindAll(r => !r.isUnlocked);

    // ─── Persistencia ─────────────────────────────────────
    void SaveRegionStates()
    {
        for (int i = 0; i < regions.Count; i++)
        {
            PlayerPrefs.SetInt($"region_{regions[i].id}_unlocked",   regions[i].isUnlocked  ? 1 : 0);
            PlayerPrefs.SetInt($"region_{regions[i].id}_discovered", regions[i].isDiscovered? 1 : 0);
            PlayerPrefs.SetInt($"region_{regions[i].id}_visits",     regions[i].timesVisited);
        }
        PlayerPrefs.Save();
    }

    void LoadRegionStates()
    {
        foreach (var r in regions)
        {
            r.isUnlocked   = PlayerPrefs.GetInt($"region_{r.id}_unlocked",   r.unlockedByDefault ? 1 : 0) == 1;
            r.isDiscovered = PlayerPrefs.GetInt($"region_{r.id}_discovered", 0) == 1;
            r.timesVisited = PlayerPrefs.GetInt($"region_{r.id}_visits",     0);
        }
    }
}
