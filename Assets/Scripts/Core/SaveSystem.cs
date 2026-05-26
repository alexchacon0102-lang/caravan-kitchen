// ============================================================
// CARAVAN KITCHEN — SaveSystem.cs
// Fase 3 — Script 1 de 8
// PROGRESO: Core/SaveSystem.cs ✅
// SIGUIENTE: Player/CaptureToolController.cs
// ============================================================
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using CaravanKitchen.Core;

namespace CaravanKitchen.Core
{
    /// <summary>
    /// Guarda y carga el estado de GameData en JSON.
    /// Soporta múltiples slots de guardado.
    /// </summary>
    public static class SaveSystem
    {
        // ── Estructura serializable del save ─────────────────────────
        [System.Serializable]
        public class SaveData
        {
            public int     coins;
            public int     fameCookery;
            public int     gems;
            public int     totalOrdersCompleted;
            public int     totalDishesCooked;
            public int     totalCreaturesCaught;
            public List<string>           unlockedZones    = new List<string>();
            public List<string>           unlockedRecipes  = new List<string>();
            public List<string>           discoveredItems  = new List<string>();
            public List<InventoryEntry>   inventory        = new List<InventoryEntry>();
            public List<UpgradeEntry>     upgrades         = new List<UpgradeEntry>();
            public string                 saveDate;
            public int                    saveSlot;
        }

        [System.Serializable]
        public class InventoryEntry { public string id; public int count; }

        [System.Serializable]
        public class UpgradeEntry   { public string id; public int level; }

        // ── Rutas ────────────────────────────────────────────────
        private static string SavePath(int slot) =>
            Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");

        private const string META_KEY = "CK_LastSaveSlot";

        // ── Guardar ──────────────────────────────────────────────
        public static void Save(int slot = 0)
        {
            var d = GameData.Instance;
            if (d == null) { Debug.LogError("[Save] GameData no encontrado."); return; }

            var save = new SaveData
            {
                coins                = d.coins,
                fameCookery          = d.fameCookery,
                gems                 = d.gems,
                totalOrdersCompleted = d.totalOrdersCompleted,
                totalDishesCooked    = d.totalDishesCooked,
                totalCreaturesCaught = d.totalCreaturesCaught,
                saveDate             = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                saveSlot             = slot
            };

            foreach (var z in d.unlockedZones)    save.unlockedZones.Add(z);
            foreach (var r in d.unlockedRecipes)  save.unlockedRecipes.Add(r);
            foreach (var i in d.discoveredItems)  save.discoveredItems.Add(i);
            foreach (var kvp in d.inventory)      save.inventory.Add(new InventoryEntry { id = kvp.Key, count = kvp.Value });
            foreach (var kvp in d.upgradeLevel)   save.upgrades.Add(new UpgradeEntry   { id = kvp.Key, level = kvp.Value });

            string json = JsonUtility.ToJson(save, true);
            File.WriteAllText(SavePath(slot), json);
            PlayerPrefs.SetInt(META_KEY, slot);
            PlayerPrefs.Save();
            Debug.Log($"[Save] Guardado en slot {slot}: {SavePath(slot)}");
        }

        // ── Cargar ──────────────────────────────────────────────
        public static bool Load(int slot = 0)
        {
            string path = SavePath(slot);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[Save] No existe save en slot {slot}.");
                return false;
            }

            var d = GameData.Instance;
            if (d == null) return false;

            string   json = File.ReadAllText(path);
            SaveData save = JsonUtility.FromJson<SaveData>(json);

            d.coins                = save.coins;
            d.fameCookery          = save.fameCookery;
            d.gems                 = save.gems;
            d.totalOrdersCompleted = save.totalOrdersCompleted;
            d.totalDishesCooked    = save.totalDishesCooked;
            d.totalCreaturesCaught = save.totalCreaturesCaught;

            d.unlockedZones.Clear();
            d.unlockedRecipes.Clear();
            d.discoveredItems.Clear();
            d.inventory.Clear();
            d.upgradeLevel.Clear();

            foreach (var z in save.unlockedZones)   d.unlockedZones.Add(z);
            foreach (var r in save.unlockedRecipes) d.unlockedRecipes.Add(r);
            foreach (var i in save.discoveredItems) d.discoveredItems.Add(i);
            foreach (var e in save.inventory)       d.inventory[e.id]      = e.count;
            foreach (var e in save.upgrades)        d.upgradeLevel[e.id]   = e.level;

            Debug.Log($"[Save] Cargado slot {slot} del {save.saveDate}");
            return true;
        }

        // ── Verificar existencia ─────────────────────────────────────
        public static bool SlotExists(int slot) => File.Exists(SavePath(slot));

        public static int  LastSlot() => PlayerPrefs.GetInt(META_KEY, 0);

        public static void DeleteSlot(int slot)
        {
            string path = SavePath(slot);
            if (File.Exists(path)) File.Delete(path);
            Debug.Log($"[Save] Slot {slot} eliminado.");
        }

        /// <summary>Metadato rápido sin cargar toda la partida.</summary>
        public static SaveData PeekSlot(int slot)
        {
            string path = SavePath(slot);
            if (!File.Exists(path)) return null;
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }
    }
}
