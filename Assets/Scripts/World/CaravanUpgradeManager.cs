using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Caravan Kitchen — CaravanUpgradeManager.cs
/// Gestiona todas las mejoras de la caravana:
/// Cocina, Almacenamiento, Exploración, Clientela, Decoración, Velocidad.
/// Cada mejora tiene costo en Monedas y Fama, nivel máximo y efecto.
/// </summary>
public class CaravanUpgradeManager : MonoBehaviour
{
    public static CaravanUpgradeManager Instance { get; private set; }

    // ─── CATEGORÍAS DE MEJORA ──────────────────────────────────────────────
    public enum UpgradeCategory { Kitchen, Storage, Exploration, Clientele, Decoration, TravelSpeed }

    [System.Serializable]
    public class Upgrade
    {
        public string id;
        public string displayName;
        public string description;
        public UpgradeCategory category;
        public int currentLevel;
        public int maxLevel;
        public int[] coinCosts;    // costo por nivel
        public int[] famaCosts;    // costo en fama por nivel
        public float[] effectValues; // valor del efecto por nivel
        public bool isUnlocked;
    }

    // ─── CATÁLOGO DE MEJORAS ───────────────────────────────────────────────
    [Header("Mejoras")]
    public List<Upgrade> upgrades = new List<Upgrade>
    {
        // ── COCINA ──
        new Upgrade { id="kitchen_speed",    displayName="Velocidad de Cocina",   description="Reduce el tiempo de cocción.",            category=UpgradeCategory.Kitchen,     currentLevel=0, maxLevel=3, coinCosts=new[]{200,500,1200},  famaCosts=new[]{0,50,150},  effectValues=new[]{0.80f,0.60f,0.40f}, isUnlocked=true },
        new Upgrade { id="kitchen_quality",  displayName="Calidad de Ingredientes",description="Aumenta la calidad base de platillos.",   category=UpgradeCategory.Kitchen,     currentLevel=0, maxLevel=3, coinCosts=new[]{300,700,1500},  famaCosts=new[]{20,80,200}, effectValues=new[]{1.1f,1.25f,1.5f},   isUnlocked=true },
        new Upgrade { id="kitchen_stations", displayName="Estaciones Extra",       description="Desbloquea estaciones de cocina nuevas.",  category=UpgradeCategory.Kitchen,     currentLevel=0, maxLevel=3, coinCosts=new[]{400,900,2000},  famaCosts=new[]{30,100,250},effectValues=new[]{2f,3f,4f},           isUnlocked=true },
        // ── ALMACENAMIENTO ──
        new Upgrade { id="storage_slots",    displayName="Slots de Inventario",   description="Más espacio para ingredientes.",          category=UpgradeCategory.Storage,     currentLevel=0, maxLevel=3, coinCosts=new[]{150,400,900},   famaCosts=new[]{0,30,100},  effectValues=new[]{20f,30f,50f},        isUnlocked=true },
        new Upgrade { id="storage_preserve", displayName="Conservación",          description="Los ingredientes duran más tiempo.",      category=UpgradeCategory.Storage,     currentLevel=0, maxLevel=2, coinCosts=new[]{250,600},        famaCosts=new[]{10,60},     effectValues=new[]{1.5f,3f},           isUnlocked=true },
        // ── EXPLORACIÓN ──
        new Upgrade { id="explore_energy",   displayName="Energía de Viaje",      description="Más expediciones por día.",               category=UpgradeCategory.Exploration, currentLevel=0, maxLevel=3, coinCosts=new[]{200,500,1000},  famaCosts=new[]{0,40,120},  effectValues=new[]{4f,5f,6f},          isUnlocked=true },
        new Upgrade { id="explore_radar",    displayName="Radar de Rareza",       description="Nimbus detecta criaturas desde más lejos.",category=UpgradeCategory.Exploration, currentLevel=0, maxLevel=3, coinCosts=new[]{350,800,1800},  famaCosts=new[]{25,90,220}, effectValues=new[]{5f,8f,12f},         isUnlocked=true },
        // ── CLIENTELA ──
        new Upgrade { id="client_capacity",  displayName="Capacidad de Clientes", description="Más clientes por ciclo de venta.",        category=UpgradeCategory.Clientele,   currentLevel=0, maxLevel=3, coinCosts=new[]{300,600,1400},  famaCosts=new[]{15,70,180}, effectValues=new[]{3f,4f,6f},          isUnlocked=true },
        new Upgrade { id="client_premium",   displayName="Clientes Premium",      description="Aparecen clientes con pedidos mejor pagados.",category=UpgradeCategory.Clientele, currentLevel=0, maxLevel=2, coinCosts=new[]{500,1200},       famaCosts=new[]{50,150},    effectValues=new[]{0.15f,0.30f},       isUnlocked=false },
        // ── VELOCIDAD ──
        new Upgrade { id="travel_speed",     displayName="Motor de Bruma",        description="Llega más rápido a cada región.",         category=UpgradeCategory.TravelSpeed, currentLevel=0, maxLevel=3, coinCosts=new[]{250,600,1300},  famaCosts=new[]{0,50,130},  effectValues=new[]{0.85f,0.70f,0.55f}, isUnlocked=true },
    };

    // ─── REFERENCIAS ──────────────────────────────────────────────────────
    private CurrencyManager currency;
    private HUDController hud;

    void Awake()
    {
        Instance = this;
        currency = FindObjectOfType<CurrencyManager>();
        hud = FindObjectOfType<HUDController>();
    }

    // ─── COMPRAR MEJORA ────────────────────────────────────────────────────
    public bool PurchaseUpgrade(string upgradeId)
    {
        Upgrade upg = upgrades.Find(u => u.id == upgradeId);
        if (upg == null) { Debug.LogWarning($"[Upgrade] ID no encontrado: {upgradeId}"); return false; }
        if (!upg.isUnlocked) { hud?.ShowFloatingText("Mejora bloqueada", Color.red); return false; }
        if (upg.currentLevel >= upg.maxLevel) { hud?.ShowFloatingText("¡Nivel máximo!", Color.yellow); return false; }

        int lvl = upg.currentLevel;
        int coinCost = upg.coinCosts[lvl];
        int famaCost = upg.famaCosts[lvl];

        if (!currency.CanAfford(coinCost, famaCost))
        {
            hud?.ShowFloatingText("Monedas o Fama insuficientes", Color.red);
            return false;
        }

        currency.SpendCoins(coinCost);
        currency.SpendFama(famaCost);
        upg.currentLevel++;

        hud?.ShowFloatingText($"{upg.displayName} → Nv{upg.currentLevel}", Color.cyan);
        OnUpgradeApplied(upg);
        GameManager.Instance.SaveGame();
        return true;
    }

    // ─── APLICAR EFECTOS ───────────────────────────────────────────────────
    private void OnUpgradeApplied(Upgrade upg)
    {
        float val = upg.effectValues[upg.currentLevel - 1];
        switch (upg.id)
        {
            case "kitchen_speed":    FindObjectOfType<CookingStation>()?.SetCookingSpeedMultiplier(val); break;
            case "storage_slots":    FindObjectOfType<PlayerInventory>()?.SetMaxSlots((int)val); break;
            case "explore_energy":   GameManager.Instance?.SetMaxEnergy((int)val); break;
            case "client_capacity":  FindObjectOfType<OrderManager>()?.SetMaxOrders((int)val); break;
            case "travel_speed":     FindObjectOfType<ZoneManager>()?.SetTravelSpeedMultiplier(val); break;
        }
        Debug.Log($"[Upgrade] {upg.displayName} Nv{upg.currentLevel} aplicada. Valor: {val}");
    }

    // ─── OBTENER VALOR ACTUAL ──────────────────────────────────────────────
    public float GetUpgradeValue(string upgradeId)
    {
        Upgrade upg = upgrades.Find(u => u.id == upgradeId);
        if (upg == null || upg.currentLevel == 0) return 1f;
        return upg.effectValues[upg.currentLevel - 1];
    }

    // ─── DESBLOQUEAR POR FAMA ──────────────────────────────────────────────
    public void UnlockByFama(int currentFama)
    {
        foreach (var upg in upgrades)
            if (!upg.isUnlocked && currentFama >= upg.famaCosts[0])
                upg.isUnlocked = true;
    }
}
