// ============================================================
// CARAVAN KITCHEN — ReputationManager.cs
// Script #28 — Fase 4
// Reputación por región: afecta precios, criaturas disponibles,
// diálogos de NPCs y misiones desbloqueables.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CaravanKitchen.World
{
    public class ReputationManager : MonoBehaviour
    {
        public static ReputationManager Instance { get; private set; }

        // ─── NIVELES DE REPUTACIÓN ───────────────────────────────────────
        public enum RepLevel
        {
            Desconocido = 0,  // nunca ha cocinado para esta región
            Conocido    = 1,  // 1–25 puntos
            Apreciado   = 2,  // 26–60 puntos
            Respetado   = 3,  // 61–90 puntos
            Legendario  = 4   // 91–100 puntos
        }

        // ─── DATOS DE REPUTACIÓN POR REGIÓN ─────────────────────────────
        [System.Serializable]
        public class RegionRep
        {
            public string   regionID;
            public string   displayName;
            [Range(0, 100)]
            public int      points;
            public RepLevel Level => points switch
            {
                0       => RepLevel.Desconocido,
                <= 25   => RepLevel.Conocido,
                <= 60   => RepLevel.Apreciado,
                <= 90   => RepLevel.Respetado,
                _       => RepLevel.Legendario
            };
            // Modificadores de precio por nivel (índice = RepLevel)
            public static readonly float[] PriceMultiplier = { 1.5f, 1.0f, 0.9f, 0.8f, 0.7f };
        }

        // ─── INSPECTOR ───────────────────────────────────────────────────
        [Header("Regiones con reputación")]
        [SerializeField] private List<RegionRep> regionReps = new List<RegionRep>();

        [Header("Eventos")]
        public UnityEvent<RegionRep> onRepLevelUp;
        public UnityEvent<RegionRep> onRepLevelDown;

        private Dictionary<string, RegionRep> _repMap;

        // ─── INIT ─────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            _repMap  = new Dictionary<string, RegionRep>();
            foreach (var r in regionReps) _repMap[r.regionID] = r;
        }

        // ─── AÑADIR REPUTACIÓN ────────────────────────────────────────────
        /// <summary>Suma puntos de reputación en una región. Dispara evento si sube de nivel.</summary>
        public void AddReputation(string regionID, int amount)
        {
            if (!_repMap.TryGetValue(regionID, out var rep)) return;
            var prevLevel = rep.Level;
            rep.points    = Mathf.Clamp(rep.points + amount, 0, 100);

            if (rep.Level > prevLevel)
            {
                onRepLevelUp?.Invoke(rep);
                HUDController hud = FindFirstObjectByType<HUDController>();
                hud?.ShowFloatingText($"⭐ Reputación: {rep.Level}", Color.yellow);
                Debug.Log($"[Reputation] ⬆️ {rep.displayName} → {rep.Level}");
            }
            SaveSystem.Instance?.Save();
        }

        /// <summary>Resta puntos de reputación.</summary>
        public void ReduceReputation(string regionID, int amount)
        {
            if (!_repMap.TryGetValue(regionID, out var rep)) return;
            var prevLevel = rep.Level;
            rep.points    = Mathf.Clamp(rep.points - amount, 0, 100);
            if (rep.Level < prevLevel) onRepLevelDown?.Invoke(rep);
            SaveSystem.Instance?.Save();
        }

        // ─── CONSULTAS ───────────────────────────────────────────────────
        public RepLevel GetLevel(string regionID)
            => _repMap.TryGetValue(regionID, out var r) ? r.Level : RepLevel.Desconocido;

        public int GetPoints(string regionID)
            => _repMap.TryGetValue(regionID, out var r) ? r.points : 0;

        /// <summary>Devuelve el multiplicador de precio de ingredientes en esta región.</summary>
        public float GetPriceMultiplier(string regionID)
        {
            var lvl = (int)GetLevel(regionID);
            return RegionRep.PriceMultiplier[Mathf.Clamp(lvl, 0, RegionRep.PriceMultiplier.Length - 1)];
        }

        /// <summary>¿Las criaturas raras aparecen? Solo si reputación >= Apreciado.</summary>
        public bool RareCreaturesAvailable(string regionID)
            => GetLevel(regionID) >= RepLevel.Apreciado;

        public List<RegionRep> GetAllReps() => regionReps;
    }
}
