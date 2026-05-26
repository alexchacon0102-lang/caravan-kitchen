// ============================================================
// CARAVAN KITCHEN — ResourceNode.cs
// Script #6 de 14
// PROGRESO: World/ResourceNode.cs ✅ completado
// SIGUIENTE: Creatures/CreatureBase.cs
// ============================================================
using System.Collections;
using UnityEngine;
using CaravanKitchen.Player;

namespace CaravanKitchen.World
{
    /// <summary>
    /// Nodo de recurso recolectable en el mundo (bayas, hongos,
    /// minerales, plantas, etc.). El jugador interactúa con él
    /// para agregar items a su bolsa de campo.
    /// Implementa IInteractable.
    /// </summary>
    public class ResourceNode : MonoBehaviour, IInteractable
    {
        // ── Config ─────────────────────────────────────────────
        [Header("Item que da")]
        [SerializeField] private string itemId     = "baya_de_bruma";
        [SerializeField] private string itemName   = "Baya de Bruma";
        [SerializeField] private int    minAmount  = 1;
        [SerializeField] private int    maxAmount  = 3;

        [Header("Rareza")]
        [Range(0f, 1f)]
        [SerializeField] private float rareDropChance  = 0.1f;  // 10% drop raro
        [SerializeField] private string rareItemId     = "";

        [Header("Recolección")]
        [SerializeField] private float harvestTime     = 1.2f;  // segundos animación
        [SerializeField] private float respawnTime     = 30f;   // segundos hasta reaparecer

        [Header("Visual")]
        [SerializeField] private GameObject visualActive;
        [SerializeField] private GameObject visualDepleted;
        [SerializeField] private ParticleSystem harvestParticles;

        // ── Estado ─────────────────────────────────────────────
        private bool _isDepleted    = false;
        private bool _isHarvesting  = false;

        // ── IInteractable ────────────────────────────────────────
        public void Interact(PlayerController player)
        {
            if (_isDepleted || _isHarvesting) return;
            StartCoroutine(HarvestRoutine(player));
        }

        // ── Rutina de recolección ──────────────────────────────────
        private IEnumerator HarvestRoutine(PlayerController player)
        {
            _isHarvesting = true;
            player.SetCanMove(false);

            // Esperar animación de cosecha
            yield return new WaitForSeconds(harvestTime);

            // Calcular cantidad
            int amount = Random.Range(minAmount, maxAmount + 1);

            // Intentar añadir a la bolsa
            var inv = PlayerInventory.Instance;
            if (inv != null)
            {
                bool added = inv.AddToBag(itemId, amount);

                // Drop raro
                if (added && !string.IsNullOrEmpty(rareItemId) &&
                    Random.value <= rareDropChance)
                {
                    inv.AddToBag(rareItemId, 1);
                    Debug.Log($"[ResourceNode] ✨ Drop raro: {rareItemId}!");
                }
            }

            // Efectos visuales
            if (harvestParticles != null) harvestParticles.Play();
            SetDepleted(true);

            player.SetCanMove(true);
            _isHarvesting = false;

            // Respawn
            if (respawnTime > 0)
                StartCoroutine(RespawnRoutine());
        }

        // ── Respawn ──────────────────────────────────────────────
        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnTime);
            SetDepleted(false);
            Debug.Log($"[ResourceNode] Respawn: {itemName}");
        }

        private void SetDepleted(bool depleted)
        {
            _isDepleted = depleted;
            if (visualActive   != null) visualActive.SetActive(!depleted);
            if (visualDepleted != null) visualDepleted.SetActive(depleted);

            // Deshabilitar collider cuando está agotado
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = !depleted;
        }

        // ── Gizmos ───────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _isDepleted ? Color.gray : Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}
