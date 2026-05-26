// ============================================================
// CARAVAN KITCHEN — CurrencyManager.cs
// Script #12 de 14
// PROGRESO: Economy/CurrencyManager.cs ✅ completado
// SIGUIENTE: UI/HUDController.cs
// ============================================================
using UnityEngine;
using CaravanKitchen.Core;

namespace CaravanKitchen.Economy
{
    /// <summary>
    /// Capa de abstracción sobre las monedas de GameData.
    /// Expone eventos para que la UI se actualice automáticamente.
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        // ── Eventos ────────────────────────────────────────────
        public static event System.Action<int, int, int> OnCurrencyChanged;
        // (coins, fame, gems)

        // ── Singleton ──────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Propiedades de lectura ─────────────────────────────────
        public int Coins       => GameData.Instance?.coins       ?? 0;
        public int FameCookery => GameData.Instance?.fameCookery ?? 0;
        public int Gems        => GameData.Instance?.gems        ?? 0;

        // ── Operaciones ─────────────────────────────────────────
        public void AddCoins(int amount)
        {
            GameData.Instance?.AddCoins(amount);
            NotifyChange();
        }

        public bool SpendCoins(int amount)
        {
            bool ok = GameData.Instance?.SpendCoins(amount) ?? false;
            if (ok) NotifyChange();
            return ok;
        }

        public void AddFame(int amount)
        {
            if (GameData.Instance == null) return;
            GameData.Instance.fameCookery += amount;
            NotifyChange();
        }

        public void AddGems(int amount)
        {
            if (GameData.Instance == null) return;
            GameData.Instance.gems += amount;
            NotifyChange();
        }

        public bool SpendGems(int amount)
        {
            if (GameData.Instance == null || GameData.Instance.gems < amount) return false;
            GameData.Instance.gems -= amount;
            NotifyChange();
            return true;
        }

        private void NotifyChange()
        {
            OnCurrencyChanged?.Invoke(Coins, FameCookery, Gems);
        }
    }
}
