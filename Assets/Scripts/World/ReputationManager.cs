using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ReputationManager — Sistema de reputación por región
/// Afecta precios, comportamiento de criaturas, diálogos de NPCs
/// y desbloqueo de contenido exclusivo.
/// Fase 4 — Caravan Kitchen
/// </summary>
public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance { get; private set; }

    // ─── Niveles de reputación ────────────────────────────
    public enum RepLevel { Desconocido = 0, Conocido = 1, Apreciado = 2, Respetado = 3, Legendario = 4 }

    [Serializable]
    public class RegionReputation
    {
        public string regionId;
        public string regionName;
        public int    points;           // 0 – 1000
        public RepLevel level;          // calculado automáticamente

        // Efecto en economía (calculado por nivel)
        public float  priceModifier    => level switch   // precio de venta
        {
            RepLevel.Conocido   => 1.05f,
            RepLevel.Apreciado  => 1.12f,
            RepLevel.Respetado  => 1.22f,
            RepLevel.Legendario => 1.40f,
            _                   => 1.00f
        };
        public float  buyModifier      => level switch   // precio de compra
        {
            RepLevel.Conocido   => 0.97f,
            RepLevel.Apreciado  => 0.92f,
            RepLevel.Respetado  => 0.85f,
            RepLevel.Legendario => 0.75f,
            _                   => 1.00f
        };
        // Rareza de criaturas
        public float  rarityBonus      => level switch
        {
            RepLevel.Apreciado  => 0.05f,
            RepLevel.Respetado  => 0.10f,
            RepLevel.Legendario => 0.18f,
            _                   => 0f
        };
    }

    // ─── Inspector ────────────────────────────────────────
    [Header("Reputaciones")]
    public List<RegionReputation> reputations = new();

    [Header("Thresholds (puntos para cada nivel)")]
    public int thresholdKnown     = 100;
    public int thresholdAppreciated = 300;
    public int thresholdRespected   = 600;
    public int thresholdLegendary   = 1000;

    // ─── Eventos ─────────────────────────────────────────
    public event Action<string, RepLevel> OnRepLevelUp;
    public event Action<string, int>      OnRepGained;

    // ─── Unity ────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        InitializeRegions();
        LoadAll();
    }

    void InitializeRegions()
    {
        if (reputations.Count > 0) return;
        var ids = new[]
        {
            ("pradera_bruma",     "Pradera de Bruma"),
            ("bosque_vapor",      "Bosque de Vapor Dulce"),
            ("arrecife_nubes",    "Arrecife de Nubes"),
            ("barranco_caldero",  "Barranco del Caldero"),
            ("mercado_suspendido","Mercado Suspendido"),
            ("jungla_canela",     "Jungla de Canela Viva"),
            ("mareas_eternas",    "Las Mareas Eternas"),
            ("cumbre_gran_velo",  "Cumbre del Gran Velo")
        };
        foreach (var (id, name) in ids)
            reputations.Add(new RegionReputation { regionId = id, regionName = name });
    }

    // ─── API pública ──────────────────────────────────────
    public void AddReputation(string regionId, int amount)
    {
        var rep = GetRep(regionId);
        if (rep == null) return;

        rep.points = Mathf.Clamp(rep.points + amount, 0, 1000);
        RepLevel newLevel = CalculateLevel(rep.points);

        if (newLevel > rep.level)
        {
            rep.level = newLevel;
            OnRepLevelUp?.Invoke(regionId, newLevel);
            Debug.Log($"[Reputation] {rep.regionName} subió a {newLevel}");
        }

        OnRepGained?.Invoke(regionId, amount);
        SaveAll();
    }

    public RepLevel GetLevel(string regionId)
        => GetRep(regionId)?.level ?? RepLevel.Desconocido;

    public float GetSellMultiplier(string regionId)
        => GetRep(regionId)?.priceModifier ?? 1f;

    public float GetBuyMultiplier(string regionId)
        => GetRep(regionId)?.buyModifier ?? 1f;

    public float GetRarityBonus(string regionId)
        => GetRep(regionId)?.rarityBonus ?? 0f;

    public int GetPoints(string regionId)
        => GetRep(regionId)?.points ?? 0;

    public int GetPointsToNextLevel(string regionId)
    {
        var rep = GetRep(regionId);
        if (rep == null) return 0;
        return rep.level switch
        {
            RepLevel.Desconocido => thresholdKnown      - rep.points,
            RepLevel.Conocido    => thresholdAppreciated - rep.points,
            RepLevel.Apreciado   => thresholdRespected   - rep.points,
            RepLevel.Respetado   => thresholdLegendary   - rep.points,
            _                    => 0
        };
    }

    /// <summary>Reputación ganada al entregar un pedido de esta región.</summary>
    public void OnOrderDelivered(string regionId, bool isSpecialOrder)
    {
        int gain = isSpecialOrder ? 25 : 10;
        AddReputation(regionId, gain);
    }

    /// <summary>Reputación ganada al descubrir la receta legendaria de la región.</summary>
    public void OnLegendaryRecipeDelivered(string regionId)
        => AddReputation(regionId, 100);

    // ─── Privados ─────────────────────────────────────────
    RegionReputation GetRep(string id)
        => reputations.Find(r => r.regionId == id);

    RepLevel CalculateLevel(int pts)
    {
        if (pts >= thresholdLegendary)  return RepLevel.Legendario;
        if (pts >= thresholdRespected)  return RepLevel.Respetado;
        if (pts >= thresholdAppreciated)return RepLevel.Apreciado;
        if (pts >= thresholdKnown)      return RepLevel.Conocido;
        return RepLevel.Desconocido;
    }

    // ─── Persistencia ─────────────────────────────────────
    void SaveAll()
    {
        foreach (var r in reputations)
        {
            PlayerPrefs.SetInt($"rep_{r.regionId}_pts",   r.points);
            PlayerPrefs.SetInt($"rep_{r.regionId}_level", (int)r.level);
        }
        PlayerPrefs.Save();
    }

    void LoadAll()
    {
        foreach (var r in reputations)
        {
            r.points = PlayerPrefs.GetInt($"rep_{r.regionId}_pts",   0);
            r.level  = (RepLevel)PlayerPrefs.GetInt($"rep_{r.regionId}_level", 0);
        }
    }
}
