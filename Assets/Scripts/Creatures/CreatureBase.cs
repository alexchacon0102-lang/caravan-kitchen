// ============================================================
// CARAVAN KITCHEN — CreatureBase.cs
// Script #7 de 14
// PROGRESO: Creatures/CreatureBase.cs ✅ completado
// SIGUIENTE: Creatures/CreatureAI.cs
// ============================================================
using UnityEngine;

namespace CaravanKitchen.Creatures
{
    public enum CreatureRarity { Common, Uncommon, Rare, Legendary }
    public enum CaptureMethod  { Net, Bait, Trap, Bell, Potion }

    [System.Serializable]
    public class CreatureDropData
    {
        public string itemId;
        public string itemName;
        [Range(0f, 1f)] public float dropChance = 1f;
        public int minAmount = 1;
        public int maxAmount = 2;
    }

    /// <summary>
    /// Clase base de datos para cualquier criatura captuable
    /// del mundo de Caravan Kitchen.
    /// Contiene estadísticas, drops y comportamiento base.
    /// </summary>
    public class CreatureBase : MonoBehaviour
    {
        // ── Identidad ────────────────────────────────────────────
        [Header("Identidad")]
        public string creatureId   = "puffshroom";
        public string creatureName = "Puffshroom";
        [TextArea] public string description;
        public CreatureRarity rarity = CreatureRarity.Common;
        public Sprite portrait;

        // ── Captura ────────────────────────────────────────────
        [Header("Captura")]
        public CaptureMethod captureMethod  = CaptureMethod.Net;
        [Range(0f, 1f)] public float baseCaptureChance = 0.6f;
        public bool onlyAtNight     = false;
        public bool fleesOnSight    = false;
        public bool requiresTired   = false;  // Debe estar "cansada" antes

        // ── Comportamiento ───────────────────────────────────────
        [Header("Movimiento")]
        public float moveSpeed      = 2f;
        public float detectionRange = 3f;
        public float fleeRange      = 2f;
        public float roamRadius     = 5f;

        // ── Drops al capturar ──────────────────────────────────────
        [Header("Drops")]
        public CreatureDropData[] drops;

        // ── Valor de fama ─────────────────────────────────────────
        [Header("Recompensa")]
        public int fameOnCapture    = 5;
        public int coinsOnCapture   = 10;

        // ── Estado runtime ───────────────────────────────────────
        public bool IsTired     { get; private set; } = false;
        public bool IsCaptured  { get; private set; } = false;

        // ── Evento ─────────────────────────────────────────────
        public static event System.Action<CreatureBase> OnCreatureCaptured;

        // ── Intentar captura ──────────────────────────────────────
        public bool TryCapture(CaptureMethod method)
        {
            if (IsCaptured) return false;

            // Penalizar si el método no coincide
            float chance = baseCaptureChance;
            if (method != captureMethod) chance *= 0.4f;
            if (requiresTired && !IsTired) chance *= 0.5f;

            bool success = Random.value <= chance;
            if (success)
            {
                IsCaptured = true;
                GiveDrops();
                OnCreatureCaptured?.Invoke(this);
                Debug.Log($"[Creature] Capturada: {creatureName}!");
                Destroy(gameObject, 0.5f);
            }
            else
            {
                Debug.Log($"[Creature] Falló la captura de {creatureName}.");
            }
            return success;
        }

        // ── Dar drops al inventario ─────────────────────────────────
        private void GiveDrops()
        {
            var inv = Player.PlayerInventory.Instance;
            if (inv == null || drops == null) return;

            foreach (var drop in drops)
            {
                if (Random.value <= drop.dropChance)
                {
                    int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                    inv.AddToBag(drop.itemId, amount);
                }
            }
        }

        // ── Efecto "cansancio" (usado por trampa o poón) ──────────────
        public void ApplyTired(float duration)
        {
            if (!IsTired) StartCoroutine(TiredRoutine(duration));
        }

        private System.Collections.IEnumerator TiredRoutine(float duration)
        {
            IsTired = true;
            Debug.Log($"[Creature] {creatureName} está cansada.");
            yield return new UnityEngine.WaitForSeconds(duration);
            IsTired = false;
        }

        // ── Gizmos ───────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, fleeRange);
        }
    }
}
