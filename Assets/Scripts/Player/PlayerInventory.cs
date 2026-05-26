// ============================================================
// CARAVAN KITCHEN — PlayerInventory.cs
// Script #4 de 14
// PROGRESO: Player/PlayerInventory.cs ✅ completado
// SIGUIENTE: World/ZoneManager.cs
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CaravanKitchen.Core;

namespace CaravanKitchen.Player
{
    /// <summary>
    /// Inventario del jugador en campo (bolsa de expedición).
    /// Tiene un límite de peso/slots. Al volver a la caravana
    /// los items se transfieren a GameData.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        // ── Config ─────────────────────────────────────────────
        [Header("Capacidad de bolsa")]
        [SerializeField] private int maxSlots = 20;

        // ── Datos ──────────────────────────────────────────────
        private Dictionary<string, int> _bag = new Dictionary<string, int>();
        private int _usedSlots = 0;

        // ── Eventos ─────────────────────────────────────────────
        public static event System.Action<Dictionary<string, int>> OnBagChanged;
        public static event System.Action OnBagFull;

        // ── Propiedades ──────────────────────────────────────────
        public int UsedSlots  => _usedSlots;
        public int MaxSlots   => maxSlots;
        public bool IsFull    => _usedSlots >= maxSlots;
        public IReadOnlyDictionary<string, int> Bag => _bag;

        // ── Singleton ──────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Añadir item a la bolsa de campo ──────────────────────────
        public bool AddToBag(string itemId, int amount = 1)
        {
            if (_usedSlots + amount > maxSlots)
            {
                Debug.LogWarning("[Inventory] Bolsa llena!");
                OnBagFull?.Invoke();
                return false;
            }

            if (_bag.ContainsKey(itemId)) _bag[itemId] += amount;
            else                          _bag[itemId]  = amount;

            _usedSlots += amount;
            OnBagChanged?.Invoke(_bag);
            Debug.Log($"[Inventory] Bolsa: +{amount} {itemId} | Slots: {_usedSlots}/{maxSlots}");
            return true;
        }

        // ── Transferir bolsa a GameData al regresar a caravana ────────
        public void TransferToCaravan()
        {
            if (GameData.Instance == null) return;

            foreach (var kvp in _bag)
                GameData.Instance.AddItem(kvp.Key, kvp.Value);

            Debug.Log($"[Inventory] Transferidos {_usedSlots} items a la caravana.");
            ClearBag();
        }

        // ── Limpiar bolsa ─────────────────────────────────────────
        public void ClearBag()
        {
            _bag.Clear();
            _usedSlots = 0;
            OnBagChanged?.Invoke(_bag);
        }

        // ── Upgrades: expandir bolsa ────────────────────────────────
        public void ExpandBag(int extraSlots)
        {
            maxSlots += extraSlots;
            Debug.Log($"[Inventory] Bolsa expandida a {maxSlots} slots.");
        }

        // ── Info ────────────────────────────────────────────────
        public int GetCount(string itemId)
        {
            return _bag.TryGetValue(itemId, out int v) ? v : 0;
        }
    }
}
