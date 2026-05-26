using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// MapExpansionManager — Controla el desbloqueo de regiones del mundo.
/// Cada región tiene requisitos de monedas, fama, nivel y zonas previas.
/// </summary>
[System.Serializable]
public class RegionData
{
    public string regionId;
    public string displayName;
    [TextArea] public string description;
    public int requiredLevel;
    public int requiredFame;
    public int unlockCost;          // Monedas de Sabor
    public string[] prerequisiteRegionIds;
    public bool isUnlocked;
    public bool isDiscovered;       // Visto pero no desbloqueado
    public int difficulty;          // 1-5
    public string[] exclusiveCreatures;
    public string[] exclusiveRecipes;
    public DayNightRequirement timeRestriction;

    public enum DayNightRequirement { Any, DayOnly, NightOnly }
}

public class MapExpansionManager : MonoBehaviour
{
    public static MapExpansionManager Instance { get; private set; }

    [Header("Regiones del mundo")]
    public List<RegionData> regions = new List<RegionData>();

    [Header("Eventos")]
    public event Action<RegionData> OnRegionUnlocked;
    public event Action<RegionData> OnRegionDiscovered;

    // ─── DATOS DE REGIONES ───────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitializeRegions();
        LoadProgress();
    }

    void InitializeRegions()
    {
        regions = new List<RegionData>
        {
            new RegionData
            {
                regionId = "pradera_bruma", displayName = "Pradera de Bruma",
                description = "El primer paso fuera de Isla Umbral. Niebla suave, criaturas tranquilas.",
                requiredLevel = 1, requiredFame = 0, unlockCost = 0,
                prerequisiteRegionIds = new string[0],
                isUnlocked = true, isDiscovered = true, difficulty = 1,
                exclusiveCreatures = new[]{ "puffshroom", "mielin", "caracol_canela" },
                exclusiveRecipes   = new[]{ "sopa_brumosa", "te_de_miel", "tostada_basica" }
            },
            new RegionData
            {
                regionId = "bosque_vapor", displayName = "Bosque de Vapor Dulce",
                description = "Árboles carmesí que sudan néctar. Insectos musicales y especias ardientes.",
                requiredLevel = 4, requiredFame = 100, unlockCost = 500,
                prerequisiteRegionIds = new[]{ "pradera_bruma" },
                isUnlocked = false, isDiscovered = false, difficulty = 2,
                exclusiveCreatures = new[]{ "abejarin", "mariposa_miel", "lagartico_carmesi" },
                exclusiveRecipes   = new[]{ "néctar_tibio", "infusion_carmesi", "jalea_de_abejas" }
            },
            new RegionData
            {
                regionId = "arrecife_nubes", displayName = "Arrecife de Nubes",
                description = "Coral blanco flotante. Los peces nadan entre nubes como si el cielo fuera mar.",
                requiredLevel = 8, requiredFame = 300, unlockCost = 1200,
                prerequisiteRegionIds = new[]{ "bosque_vapor" },
                isUnlocked = false, isDiscovered = false, difficulty = 3,
                exclusiveCreatures = new[]{ "nubipez", "esponjacoral", "calamar_celeste" },
                exclusiveRecipes   = new[]{ "ceviche_de_nube", "sopa_celeste", "sal_de_nube_curada" },
                timeRestriction = RegionData.DayNightRequirement.DayOnly
            },
            new RegionData
            {
                regionId = "barranco_caldero", displayName = "Barranco del Caldero",
                description = "Roca volcánica y caldos minerales. Caliente. El aroma es imposible de describir.",
                requiredLevel = 14, requiredFame = 700, unlockCost = 2500,
                prerequisiteRegionIds = new[]{ "arrecife_nubes" },
                isUnlocked = false, isDiscovered = false, difficulty = 4,
                exclusiveCreatures = new[]{ "salamandra_azafran", "escorpion_pimienta", "larva_cobre" },
                exclusiveRecipes   = new[]{ "guiso_volcanico", "pan_de_roca", "esencia_mineral" }
            },
            new RegionData
            {
                regionId = "mercado_suspendido", displayName = "Mercado Suspendido",
                description = "Ciudad flotante sobre redes. Comerciantes de todas las regiones. Concursos culinarios.",
                requiredLevel = 10, requiredFame = 800, unlockCost = 0,
                prerequisiteRegionIds = new[]{ "bosque_vapor" },
                isUnlocked = false, isDiscovered = false, difficulty = 1,
                exclusiveCreatures = new string[0],
                exclusiveRecipes   = new[]{ "receta_legendaria_mercado" }
            },
            new RegionData
            {
                regionId = "jungla_canela", displayName = "Jungla de Canela Viva",
                description = "Lluvia de vainilla. Árboles que huelen a especias. Flora que canta.",
                requiredLevel = 18, requiredFame = 1200, unlockCost = 4000,
                prerequisiteRegionIds = new[]{ "barranco_caldero", "mercado_suspendido" },
                isUnlocked = false, isDiscovered = false, difficulty = 4,
                exclusiveCreatures = new[]{ "serpiente_canela", "tucan_cardamomo", "rana_vainilla" },
                exclusiveRecipes   = new[]{ "curry_de_jungla", "pastel_de_canela_viva", "extracto_aromatico" }
            },
            new RegionData
            {
                regionId = "mareas_eternas", displayName = "Las Mareas Eternas",
                description = "El mar sube y baja cada 10 minutos revelando zonas únicas. Criaturas abisales.",
                requiredLevel = 24, requiredFame = 2000, unlockCost = 6000,
                prerequisiteRegionIds = new[]{ "jungla_canela" },
                isUnlocked = false, isDiscovered = false, difficulty = 5,
                exclusiveCreatures = new[]{ "pulpo_tormenta", "medusa_luz", "cangrejo_runa" },
                exclusiveRecipes   = new[]{ "sopa_abisal", "perla_de_caldo", "marinado_de_marea" },
                timeRestriction = RegionData.DayNightRequirement.NightOnly
            },
            new RegionData
            {
                regionId = "cumbre_velo", displayName = "Cumbre del Gran Velo",
                description = "El origen de todo. La receta maestra está aquí. La bruma puede terminar.",
                requiredLevel = 30, requiredFame = 5000, unlockCost = 0,
                prerequisiteRegionIds = new[]{ "mareas_eternas" },
                isUnlocked = false, isDiscovered = false, difficulty = 5,
                exclusiveCreatures = new string[0],
                exclusiveRecipes   = new[]{ "receta_maestra" }
            }
        };
    }

    // ─── LÓGICA DE DESBLOQUEO ────────────────────────────────────────────────
    public bool CanUnlock(string regionId)
    {
        RegionData r = GetRegion(regionId);
        if (r == null || r.isUnlocked) return false;

        int playerLevel = XPManager.Instance ? XPManager.Instance.currentLevel : 1;
        int playerFame  = CurrencyManager.Instance ? CurrencyManager.Instance.Fame : 0;
        int playerGold  = CurrencyManager.Instance ? CurrencyManager.Instance.Coins : 0;

        if (playerLevel < r.requiredLevel) return false;
        if (playerFame  < r.requiredFame)  return false;
        if (playerGold  < r.unlockCost)    return false;

        foreach (string prereq in r.prerequisiteRegionIds)
        {
            RegionData pre = GetRegion(prereq);
            if (pre == null || !pre.isUnlocked) return false;
        }
        return true;
    }

    public bool TryUnlockRegion(string regionId)
    {
        if (!CanUnlock(regionId)) return false;
        RegionData r = GetRegion(regionId);
        if (r.unlockCost > 0)
            CurrencyManager.Instance?.SpendCoins(r.unlockCost);
        r.isUnlocked = true;
        r.isDiscovered = true;
        OnRegionUnlocked?.Invoke(r);
        NimbusController.Instance?.OnNewZone();
        SaveProgress();
        GameManager.Instance?.AchievementManager?.UnlockAchievement("region_" + regionId);
        return true;
    }

    public void DiscoverRegion(string regionId)
    {
        RegionData r = GetRegion(regionId);
        if (r != null && !r.isDiscovered)
        {
            r.isDiscovered = true;
            OnRegionDiscovered?.Invoke(r);
            SaveProgress();
        }
    }

    public RegionData GetRegion(string id) => regions.Find(r => r.regionId == id);
    public List<RegionData> GetUnlockedRegions() => regions.FindAll(r => r.isUnlocked);
    public List<RegionData> GetAvailableRegions()
    {
        // Disponibles = desbloqueadas + las que cumplen prereqs pero no costo
        return regions.FindAll(r => r.isUnlocked || HasPrereqsMet(r));
    }

    bool HasPrereqsMet(RegionData r)
    {
        foreach (string p in r.prerequisiteRegionIds)
        { if (!(GetRegion(p)?.isUnlocked ?? false)) return false; }
        return true;
    }

    // ─── PERSISTENCIA ────────────────────────────────────────────────────────
    void SaveProgress()
    {
        for (int i = 0; i < regions.Count; i++)
        {
            PlayerPrefs.SetInt("Region_" + regions[i].regionId + "_unlocked", regions[i].isUnlocked ? 1 : 0);
            PlayerPrefs.SetInt("Region_" + regions[i].regionId + "_discovered", regions[i].isDiscovered ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        foreach (var r in regions)
        {
            r.isUnlocked   = PlayerPrefs.GetInt("Region_" + r.regionId + "_unlocked",   r.isUnlocked ? 1 : 0) == 1;
            r.isDiscovered = PlayerPrefs.GetInt("Region_" + r.regionId + "_discovered", r.isDiscovered ? 1 : 0) == 1;
        }
    }
}
