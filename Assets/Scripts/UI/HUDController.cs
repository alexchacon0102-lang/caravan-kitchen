// ============================================================
// CARAVAN KITCHEN — HUDController.cs
// Script #13 de 14
// PROGRESO: UI/HUDController.cs ✅ completado
// SIGUIENTE: UI/MainMenuUI.cs
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;   // TextMeshPro requerido
using CaravanKitchen.Core;
using CaravanKitchen.Economy;
using CaravanKitchen.Player;

namespace CaravanKitchen.UI
{
    /// <summary>
    /// Controlador del HUD principal (en escena de juego).
    /// Muestra monedas, fama, slots de bolsa, boton de interacción
    /// y boton de regresar a caravana.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Monedas y Fama")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI fameText;
        [SerializeField] private TextMeshProUGUI gemText;

        [Header("Bolsa")]
        [SerializeField] private TextMeshProUGUI bagSlotsText;
        [SerializeField] private Slider          bagSlider;

        [Header("Interacción")]
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private TextMeshProUGUI interactLabel;

        [Header("Botones")]
        [SerializeField] private Button returnToCaravanBtn;
        [SerializeField] private Button pauseBtn;

        [Header("Notificaciones")]
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float           notifDuration = 2.5f;
        private Coroutine _notifRoutine;

        // ── Init ───────────────────────────────────────────────
        private void OnEnable()
        {
            CurrencyManager.OnCurrencyChanged += UpdateCurrencyUI;
            PlayerInventory.OnBagChanged      += UpdateBagUI;
            PlayerInventory.OnBagFull         += OnBagFull;
            PlayerController.OnInteractableFound += ShowInteractPrompt;
            PlayerController.OnInteractableLost  += HideInteractPrompt;
        }

        private void OnDisable()
        {
            CurrencyManager.OnCurrencyChanged -= UpdateCurrencyUI;
            PlayerInventory.OnBagChanged      -= UpdateBagUI;
            PlayerInventory.OnBagFull         -= OnBagFull;
            PlayerController.OnInteractableFound -= ShowInteractPrompt;
            PlayerController.OnInteractableLost  -= HideInteractPrompt;
        }

        private void Start()
        {
            // Valores iniciales
            var data = GameData.Instance;
            if (data != null)
                UpdateCurrencyUI(data.coins, data.fameCookery, data.gems);

            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (notificationText != null) notificationText.gameObject.SetActive(false);

            // Botones
            returnToCaravanBtn?.onClick.AddListener(OnReturnToCaravan);
            pauseBtn?.onClick.AddListener(OnPause);
        }

        // ── Callbacks de eventos ───────────────────────────────────
        private void UpdateCurrencyUI(int coins, int fame, int gems)
        {
            if (coinText != null) coinText.text = $"🪙 {coins}";
            if (fameText != null) fameText.text = $"⭐ {fame}";
            if (gemText  != null) gemText.text  = $"💎 {gems}";
        }

        private void UpdateBagUI(System.Collections.Generic.Dictionary<string, int> bag)
        {
            var inv = PlayerInventory.Instance;
            if (inv == null) return;
            if (bagSlotsText != null)
                bagSlotsText.text = $"{inv.UsedSlots}/{inv.MaxSlots}";
            if (bagSlider != null)
            {
                bagSlider.maxValue = inv.MaxSlots;
                bagSlider.value    = inv.UsedSlots;
            }
        }

        private void OnBagFull()
        {
            ShowNotification("⚠️ ¡Bolsa llena! Regresa a la caravana.");
        }

        private void ShowInteractPrompt(GameObject target)
        {
            if (interactPrompt == null) return;
            interactPrompt.SetActive(true);
            if (interactLabel != null)
                interactLabel.text = "Presiona [E] para interactuar";
        }

        private void HideInteractPrompt()
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }

        // ── Botones ──────────────────────────────────────────────
        private void OnReturnToCaravan()
        {
            PlayerInventory.Instance?.TransferToCaravan();
            GameManager.Instance?.ReturnToCaravan();
        }

        private void OnPause()
        {
            GameManager.Instance?.PauseGame();
        }

        // ── Notificaciones ────────────────────────────────────────
        public void ShowNotification(string msg)
        {
            if (notificationText == null) return;
            if (_notifRoutine != null) StopCoroutine(_notifRoutine);
            _notifRoutine = StartCoroutine(NotifRoutine(msg));
        }

        private System.Collections.IEnumerator NotifRoutine(string msg)
        {
            notificationText.text = msg;
            notificationText.gameObject.SetActive(true);
            yield return new UnityEngine.WaitForSeconds(notifDuration);
            notificationText.gameObject.SetActive(false);
        }
    }
}
