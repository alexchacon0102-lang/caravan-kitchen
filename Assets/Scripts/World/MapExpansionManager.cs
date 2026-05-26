// ============================================================
// CARAVAN KITCHEN — MapExpansionManager.cs
// Script #27 — Fase 4
// Gestión de regiones desbloqueables del mundo de Aetherea.
// Controla estado de zonas, costo de desbloqueo y eventos
// de primera visita.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CaravanKitchen.World
{
    public class MapExpansionManager : MonoBehaviour
    {
        public static MapExpansionManager Instance { get; private set; }

        // ─── MODELO DE REGIÓN ────────────────────────────────────────────
        [System.Serializable]
        public class RegionData
        {
            public string  regionID;           // ID interno ej: "pradera_bruma"
            public string  displayName;        // Nombre mostrado en UI
            public string  description;        // Descripción breve
            public int     unlockCostCoins;    // Costo en Monedas de Sabor
            public int     unlockCostFama;     // Costo en Fama Culinaria
            public int     requiredPlayerLevel;// Nivel mínimo del jugador
            public string  prerequisiteRegion; // RegionID que debe estar desbloqueada primero ("" = ninguna)
            public bool    isUnlocked;         // Se serializa para guardado
            public bool    hasBeenVisited;     // Primera visita
            public Sprite  mapIcon;            // Ícono en el mapa
            public Color   regionColor;        // Color del marcador de mapa
            [TextArea] public string firstVisitDialog; // Frase de Nimbus al visitar por primera vez
        }

        // ─── INSPECTOR ───────────────────────────────────────────────────
        [Header("Regiones del Mundo")]
        [SerializeField] private List<RegionData> regions = new List<RegionData>();

        [Header("Eventos")]
        public UnityEvent<RegionData> onRegionUnlocked;
        public UnityEvent<RegionData> onRegionFirstVisit;

        // ─── RUNTIME ─────────────────────────────────────────────────────
        private Dictionary<string, RegionData> _regionMap;

        // ─── INIT ─────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            _regionMap = new Dictionary<string, RegionData>();
            foreach (var r in regions)
                _regionMap[r.regionID] = r;

            // La primera región siempre desbloqueada
            if (regions.Count > 0)
                regions[0].isUnlocked = true;
        }

        // ─── DESBLOQUEO ───────────────────────────────────────────────────
        /// <summary>Intenta desbloquear una región. Devuelve true si tuvo éxito.</summary>
        public bool TryUnlockRegion(string regionID)
        {
            if (!_regionMap.TryGetValue(regionID, out var region))
            {
                Debug.LogWarning($"[MapExpansion] Región no encontrada: {regionID}");
                return false;
            }

            if (region.isUnlocked) return true;

            // Verificar prerequisito
            if (!string.IsNullOrEmpty(region.prerequisiteRegion))
            {
                if (!_regionMap.TryGetValue(region.prerequisiteRegion, out var prereq) || !prereq.isUnlocked)
                {
                    Debug.Log($"[MapExpansion] Prerequisito no cumplido: {region.prerequisiteRegion}");
                    return false;
                }
            }

            // Verificar nivel
            var xpMgr = XPManager.Instance;
            if (xpMgr != null && xpMgr.CurrentLevel < region.requiredPlayerLevel)
            {
                Debug.Log($"[MapExpansion] Nivel insuficiente para {regionID}");
                return false;
            }

            // Verificar monedas y fama
            var currency = CurrencyManager.Instance;
            if (currency == null) return false;

            if (!currency.TrySpend(region.unlockCostCoins, region.unlockCostFama))
            {
                Debug.Log($"[MapExpansion] Recursos insuficientes para {regionID}");
                return false;
            }

            // Desbloquear
            region.isUnlocked = true;
            onRegionUnlocked?.Invoke(region);
            SaveSystem.Instance?.Save();
            Debug.Log($"[MapExpansion] ✅ Región desbloqueada: {region.displayName}");
            return true;
        }

        // ─── PRIMERA VISITA ───────────────────────────────────────────────
        /// <summary>Registra la primera visita a una región y dispara eventos.</summary>
        public void RegisterVisit(string regionID)
        {
            if (!_regionMap.TryGetValue(regionID, out var region)) return;
            if (region.hasBeenVisited) return;

            region.hasBeenVisited = true;
            onRegionFirstVisit?.Invoke(region);

            // Nimbus reacciona
            if (!string.IsNullOrEmpty(region.firstVisitDialog))
                CaravanKitchen.Companion.NimbusController.Instance?.SayPhrase(region.firstVisitDialog);
            else
                CaravanKitchen.Companion.NimbusController.Instance?.ReactToNewZone();

            SaveSystem.Instance?.Save();
        }

        // ─── CONSULTAS ────────────────────────────────────────────────────
        public RegionData     GetRegion(string id)   => _regionMap.TryGetValue(id, out var r) ? r : null;
        public List<RegionData> GetAllRegions()       => regions;
        public List<RegionData> GetUnlockedRegions()  => regions.FindAll(r => r.isUnlocked);
        public bool IsUnlocked(string id)             => _regionMap.TryGetValue(id, out var r) && r.isUnlocked;
        public bool CanUnlock(string id)
        {
            if (!_regionMap.TryGetValue(id, out var r)) return false;
            if (r.isUnlocked) return false;
            if (!string.IsNullOrEmpty(r.prerequisiteRegion) && !IsUnlocked(r.prerequisiteRegion)) return false;
            var xp = XPManager.Instance;
            return xp == null || xp.CurrentLevel >= r.requiredPlayerLevel;
        }
    }
}
