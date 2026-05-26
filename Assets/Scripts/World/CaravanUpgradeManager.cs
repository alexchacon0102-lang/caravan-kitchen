// ============================================================
// CARAVAN KITCHEN — CaravanUpgradeManager.cs
// Fase 3 — Script 3 de 8
// PROGRESO: World/CaravanUpgradeManager.cs ✅
// SIGUIENTE: World/DayNightCycle.cs
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CaravanKitchen.Core;

namespace CaravanKitchen.World
{
    [System.Serializable]
    public class UpgradeDefinition
    {
        public string  upgradeId;
        public string  displayName;
        [TextArea]
        public string  description;
        public int     maxLevel        = 3;
        public int[]   costPerLevel;        // costo en monedas por nivel
        public int[]   famePerLevel;        // costo en fama por nivel
        // Objetos visuales que aparecen en la caravana al subir de nivel
        public GameObject[] visualByLevel;
    }

    /// <summary>
    /// Gestiona las mejoras de la caravana.
    /// Cada mejora tiene múltiples niveles, costo y un objeto
    /// visual que aparece físicamente en la caravana al comprar.
    /// </summary>
    public class CaravanUpgradeManager : MonoBehaviour
    {
        public static CaravanUpgradeManager Instance { get; private set; }

        [Header("Mejoras disponibles")]
        [SerializeField] private List<UpgradeDefinition> upgrades;

        // ── Eventos ────────────────────────────────────────────
        public static event System.Action<UpgradeDefinition, int> OnUpgradePurchased;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            RefreshAllVisuals();
        }

        // ── Comprar mejora ─────────────────────────────────────────
        public bool TryPurchaseUpgrade(string upgradeId)
        {
            var def  = upgrades.Find(u => u.upgradeId == upgradeId);
            if (def == null)
            {
                Debug.LogWarning($"[Upgrade] No existe: {upgradeId}");
                return false;
            }

            var data = GameData.Instance;
            int current = data.GetUpgradeLevel(upgradeId);

            if (current >= def.maxLevel)
            {
                Debug.Log($"[Upgrade] {def.displayName} ya está al máximo.");
                return false;
            }

            int costIdx   = Mathf.Clamp(current, 0, def.costPerLevel.Length - 1);
            int coinCost  = def.costPerLevel[costIdx];
            int fameCost  = def.famePerLevel != null && def.famePerLevel.Length > costIdx
                            ? def.famePerLevel[costIdx] : 0;

            if (data.coins < coinCost || data.fameCookery < fameCost)
            {
                Debug.LogWarning($"[Upgrade] Fondos insuficientes para {def.displayName}.");
                return false;
            }

            data.SpendCoins(coinCost);
            data.fameCookery -= fameCost;
            int newLevel = current + 1;
            data.SetUpgradeLevel(upgradeId, newLevel);

            ApplyVisual(def, newLevel);
            OnUpgradePurchased?.Invoke(def, newLevel);

            // Auto-guardar tras mejora
            SaveSystem.Save();

            Debug.Log($"[Upgrade] {def.displayName} subido a nivel {newLevel}.");
            return true;
        }

        // ── Visuales ──────────────────────────────────────────────
        private void ApplyVisual(UpgradeDefinition def, int level)
        {
            if (def.visualByLevel == null) return;
            for (int i = 0; i < def.visualByLevel.Length; i++)
            {
                if (def.visualByLevel[i] != null)
                    def.visualByLevel[i].SetActive(i < level);
            }
        }

        private void RefreshAllVisuals()
        {
            var data = GameData.Instance;
            if (data == null) return;
            foreach (var def in upgrades)
                ApplyVisual(def, data.GetUpgradeLevel(def.upgradeId));
        }

        // ── Info ────────────────────────────────────────────────
        public int GetCurrentLevel(string upgradeId)
        {
            return GameData.Instance?.GetUpgradeLevel(upgradeId) ?? 0;
        }

        public bool IsMaxLevel(string upgradeId)
        {
            var def = upgrades.Find(u => u.upgradeId == upgradeId);
            if (def == null) return true;
            return GetCurrentLevel(upgradeId) >= def.maxLevel;
        }

        public int GetNextCost(string upgradeId)
        {
            var def = upgrades.Find(u => u.upgradeId == upgradeId);
            if (def == null) return -1;
            int lvl = GetCurrentLevel(upgradeId);
            int idx = Mathf.Clamp(lvl, 0, def.costPerLevel.Length - 1);
            return def.costPerLevel[idx];
        }

        public List<UpgradeDefinition> AllUpgrades => upgrades;
    }
}
