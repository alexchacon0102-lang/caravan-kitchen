// ============================================================
// CARAVAN KITCHEN — ZoneManager.cs
// Script #5 de 14
// PROGRESO: World/ZoneManager.cs ✅ completado
// SIGUIENTE: World/ResourceNode.cs
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CaravanKitchen.Core;

namespace CaravanKitchen.World
{
    [System.Serializable]
    public class ZoneData
    {
        public string zoneId;
        public string zoneName;
        public string description;
        public int    unlockCost;    // en monedas
        public int    unlockFame;    // en fama culinaria
        public Sprite zoneIcon;
        public bool   isStartZone;
        [TextArea] public string lore;
    }

    /// <summary>
    /// Gestiona las zonas de exploración: carga, bloqueo/desbloqueo
    /// y spawn de recursos en el mapa activo.
    /// </summary>
    public class ZoneManager : MonoBehaviour
    {
        public static ZoneManager Instance { get; private set; }

        [Header("Zonas del juego")]
        [SerializeField] private List<ZoneData> allZones = new List<ZoneData>();

        [Header("Prefabs de recursos")]
        [SerializeField] private GameObject[] resourcePrefabs;
        [SerializeField] private Transform   resourceParent;

        // ── Estado ─────────────────────────────────────────────
        private ZoneData _currentZone;
        private List<GameObject> _spawnedResources = new List<GameObject>();

        // ── Eventos ────────────────────────────────────────────
        public static event System.Action<ZoneData> OnZoneLoaded;
        public static event System.Action<ZoneData> OnZoneUnlocked;

        // ── Singleton ──────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Cargar zona ──────────────────────────────────────────
        public void LoadZone(string zoneId)
        {
            ZoneData zone = GetZone(zoneId);
            if (zone == null)
            {
                Debug.LogError($"[ZoneManager] Zona no encontrada: {zoneId}");
                return;
            }

            if (!IsUnlocked(zoneId) && !zone.isStartZone)
            {
                Debug.LogWarning($"[ZoneManager] Zona bloqueada: {zoneId}");
                return;
            }

            ClearSpawnedResources();
            _currentZone = zone;
            SpawnResources();
            OnZoneLoaded?.Invoke(zone);
            Debug.Log($"[ZoneManager] Zona cargada: {zone.zoneName}");
        }

        // ── Desbloquear zona ──────────────────────────────────────
        public bool TryUnlockZone(string zoneId)
        {
            ZoneData zone = GetZone(zoneId);
            if (zone == null || IsUnlocked(zoneId)) return false;

            var data = GameData.Instance;
            if (data.coins < zone.unlockCost || data.fameCookery < zone.unlockFame)
            {
                Debug.LogWarning($"[ZoneManager] No hay suficientes recursos para desbloquear {zone.zoneName}");
                return false;
            }

            data.SpendCoins(zone.unlockCost);
            data.fameCookery -= zone.unlockFame;
            data.unlockedZones.Add(zoneId);
            OnZoneUnlocked?.Invoke(zone);
            Debug.Log($"[ZoneManager] Zona desbloqueada: {zone.zoneName}");
            return true;
        }

        // ── Spawn de recursos en la zona ──────────────────────────
        private void SpawnResources()
        {
            if (resourcePrefabs == null || resourcePrefabs.Length == 0) return;
            if (resourceParent == null) return;

            int count = Random.Range(5, 12);
            for (int i = 0; i < count; i++)
            {
                int   idx    = Random.Range(0, resourcePrefabs.Length);
                float x      = Random.Range(-8f, 8f);
                float y      = Random.Range(-4f, 4f);
                var   go     = Instantiate(resourcePrefabs[idx],
                               new Vector3(x, y, 0f), Quaternion.identity, resourceParent);
                _spawnedResources.Add(go);
            }
            Debug.Log($"[ZoneManager] Spawneados {count} recursos en {_currentZone.zoneName}");
        }

        private void ClearSpawnedResources()
        {
            foreach (var go in _spawnedResources)
                if (go != null) Destroy(go);
            _spawnedResources.Clear();
        }

        // ── Helpers ───────────────────────────────────────────────
        public bool IsUnlocked(string zoneId)
        {
            return GameData.Instance != null &&
                   GameData.Instance.unlockedZones.Contains(zoneId);
        }

        public ZoneData GetZone(string zoneId)
        {
            return allZones.Find(z => z.zoneId == zoneId);
        }

        public ZoneData CurrentZone => _currentZone;

        public List<ZoneData> GetAllZones() => allZones;
    }
}
