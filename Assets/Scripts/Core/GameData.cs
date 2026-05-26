// ============================================================
// CARAVAN KITCHEN — GameData.cs
// Script #2 de 14
// PROGRESO: Core/GameData.cs ✅ completado
// SIGUIENTE: Player/PlayerController.cs
// ============================================================
using System.Collections.Generic;
using UnityEngine;

namespace CaravanKitchen.Core
{
    /// <summary>
    /// Almacén central de datos de partida (inventario, monedas,
    /// recetas desbloqueadas, zonas descubiertas, mejoras).
    /// Singleton persistente. No es ScriptableObject para permitir
    /// modificación en runtime y reset sencillo.
    /// </summary>
    public class GameData : MonoBehaviour
    {
        public static GameData Instance { get; private set; }

        // ── Monedas ──────────────────────────────────────────────
        [Header("Economía")]
        public int coins       = 0;
        public int fameCookery = 0;   // Fama culinaria
        public int gems        = 0;   // Moneda premium

        // ── Inventario: clave = itemId, valor = cantidad ─────────
        [Header("Inventario")]
        public Dictionary<string, int> inventory = new Dictionary<string, int>();

        // ── Recetas desbloqueadas ────────────────────────────────
        [Header("Recetas")]
        public HashSet<string> unlockedRecipes  = new HashSet<string>();
        public HashSet<string> discoveredItems  = new HashSet<string>();

        // ── Zonas desbloqueadas ──────────────────────────────────
        [Header("Zonas")]
        public HashSet<string> unlockedZones    = new HashSet<string>();

        // ── Mejoras de caravana ──────────────────────────────────
        [Header("Caravana")]
        public Dictionary<string, int> upgradeLevel = new Dictionary<string, int>();

        // ── Estadísticas ─────────────────────────────────────────
        [Header("Stats")]
        public int totalOrdersCompleted = 0;
        public int totalDishesCooked    = 0;
        public int totalCreaturesCaught = 0;

        // ── Singleton setup ──────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDefaults();
        }

        private void InitDefaults()
        {
            coins       = 50;   // Monedas de inicio
            fameCookery = 0;
            gems        = 0;

            // Zona inicial desbloqueada
            unlockedZones.Add("zona_pradera_bruma");

            // Receta básica disponible desde el inicio
            unlockedRecipes.Add("sopa_baya_simple");

            // Mejoras iniciales en nivel 0
            upgradeLevel["caldero"]      = 1;
            upgradeLevel["almacenaje"]   = 1;
            upgradeLevel["velocidad"]    = 1;
        }

        // ── Inventario ───────────────────────────────────────────
        public void AddItem(string itemId, int amount = 1)
        {
            if (inventory.ContainsKey(itemId))
                inventory[itemId] += amount;
            else
                inventory[itemId] = amount;

            discoveredItems.Add(itemId);
            Debug.Log($"[GameData] +{amount} {itemId} | Total: {inventory[itemId]}");
        }

        public bool RemoveItem(string itemId, int amount = 1)
        {
            if (!inventory.ContainsKey(itemId) || inventory[itemId] < amount)
            {
                Debug.LogWarning($"[GameData] No hay suficiente {itemId}");
                return false;
            }
            inventory[itemId] -= amount;
            if (inventory[itemId] <= 0) inventory.Remove(itemId);
            return true;
        }

        public int GetItemCount(string itemId)
        {
            return inventory.TryGetValue(itemId, out int count) ? count : 0;
        }

        public bool HasItem(string itemId, int amount = 1)
        {
            return GetItemCount(itemId) >= amount;
        }

        // ── Monedas ──────────────────────────────────────────────
        public void AddCoins(int amount)
        {
            coins += amount;
            Debug.Log($"[GameData] +{amount} monedas | Total: {coins}");
        }

        public bool SpendCoins(int amount)
        {
            if (coins < amount) return false;
            coins -= amount;
            return true;
        }

        // ── Mejoras ──────────────────────────────────────────────
        public int GetUpgradeLevel(string upgradeId)
        {
            return upgradeLevel.TryGetValue(upgradeId, out int lvl) ? lvl : 0;
        }

        public void SetUpgradeLevel(string upgradeId, int level)
        {
            upgradeLevel[upgradeId] = level;
        }

        // ── Reset ────────────────────────────────────────────────
        public void ResetData()
        {
            inventory.Clear();
            unlockedRecipes.Clear();
            discoveredItems.Clear();
            unlockedZones.Clear();
            upgradeLevel.Clear();
            totalOrdersCompleted = 0;
            totalDishesCooked    = 0;
            totalCreaturesCaught = 0;
            InitDefaults();
            Debug.Log("[GameData] Datos reiniciados.");
        }
    }
}
