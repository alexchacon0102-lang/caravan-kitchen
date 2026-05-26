using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// ReputationManager — Reputación por región que afecta precios, criaturas disponibles
/// y diálogos de NPCs. Cada región tiene su propia barra (0-1000).
/// </summary>
[System.Serializable]
public class RegionReputation
{
    public string regionId;
    public string displayName;
    public float value;           // 0 – 1000
    public ReputationTier tier;

    public enum ReputationTier
    {
        Desconocido = 0,   // 0–99
        Conocido    = 1,   // 100–299
        Respetado   = 2,   // 300–599
        Admirado    = 3,   // 600–899
        Legendario  = 4    // 900–1000
    }

    public ReputationTier CalculateTier()
    {
        if (value < 100)  return ReputationTier.Desconocido;
        if (value < 300)  return ReputationTier.Conocido;
        if (value < 600)  return ReputationTier.Respetado;
        if (value < 900)  return ReputationTier.Admirado;
        return ReputationTier.Legendario;
    }

    // Multiplicador de precio de venta basado en reputación (0.8x – 1.5x)
    public float SellPriceMultiplier => 0.8f + (value / 1000f) * 0.7f;

    // Criaturas raras aparecen más seguido con mejor reputación
    public float RarityBonus => (value / 1000f) * 0.15f;  // hasta +15% rareza
}

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance { get; private set; }

    public List<RegionReputation> reputations = new List<RegionReputation>();
    public event Action<RegionReputation, ReputationReputation.ReputationTier> OnTierUp;

    // ─── INICIALIZACIÓN ───────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitReputations();
        LoadAll();
    }

    void InitReputations()
    {
        string[] ids   = { "pradera_bruma","bosque_vapor","arrecife_nubes","barranco_caldero","mercado_suspendido","jungla_canela","mareas_eternas","cumbre_velo" };
        string[] names = { "Pradera de Bruma","Bosque de Vapor","Arrecife de Nubes","Barranco del Caldero","Mercado Suspendido","Jungla de Canela","Mareas Eternas","Cumbre del Velo" };
        reputations.Clear();
        for (int i = 0; i < ids.Length; i++)
            reputations.Add(new RegionReputation { regionId = ids[i], displayName = names[i], value = 0 });
    }

    // ─── MODIFICAR REPUTACIÓN ─────────────────────────────────────────────────
    /// <summary>Sumar reputación en una región. Fuente: entregar pedido, platillo legendario, misión.</summary>
    public void AddReputation(string regionId, float amount)
    {
        RegionReputation rep = GetReputation(regionId);
        if (rep == null) return;
        var prevTier = rep.CalculateTier();
        rep.value = Mathf.Clamp(rep.value + amount, 0, 1000);
        rep.tier  = rep.CalculateTier();
        if (rep.tier > prevTier)
        {
            OnTierUp?.Invoke(rep, rep.tier);
            HandleTierUpReward(rep);
        }
        SaveAll();
    }

    void HandleTierUpReward(RegionReputation rep)
    {
        // Dar monedas y fama al subir tier
        int[] coinRewards = { 0, 200, 500, 1000, 2500 };
        int[] fameRewards = { 0,  50,  150,  300,  750 };
        int tier = (int)rep.tier;
        CurrencyManager.Instance?.AddCoins(coinRewards[tier]);
        CurrencyManager.Instance?.AddFame(fameRewards[tier]);
        // Logro si llega a Legendario
        if (rep.tier == RegionReputation.ReputationTier.Legendario)
            GameManager.Instance?.AchievementManager?.UnlockAchievement("rep_legend_" + rep.regionId);
    }

    // ─── GETTERS ─────────────────────────────────────────────────────────────
    public RegionReputation GetReputation(string regionId) => reputations.Find(r => r.regionId == regionId);

    /// <summary>Multiplicador de precio de venta para una región específica.</summary>
    public float GetSellMultiplier(string regionId)
    {
        var r = GetReputation(regionId);
        return r != null ? r.SellPriceMultiplier : 0.8f;
    }

    /// <summary>Bonus de rareza activo en la región actual.</summary>
    public float GetRarityBonus(string regionId)
    {
        var r = GetReputation(regionId);
        return r != null ? r.RarityBonus : 0f;
    }

    // ─── PERSISTENCIA ────────────────────────────────────────────────────────
    void SaveAll()
    {
        foreach (var r in reputations)
            PlayerPrefs.SetFloat("Rep_" + r.regionId, r.value);
        PlayerPrefs.Save();
    }

    void LoadAll()
    {
        foreach (var r in reputations)
        {
            r.value = PlayerPrefs.GetFloat("Rep_" + r.regionId, 0f);
            r.tier  = r.CalculateTier();
        }
    }

    // ─── DEBUG ────────────────────────────────────────────────────────────────
    [ContextMenu("Debug — Max All Reputations")]
    void DebugMaxAll() { foreach (var r in reputations) AddReputation(r.regionId, 1000); }
}
