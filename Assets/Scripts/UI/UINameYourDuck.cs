// ============================================================
// CARAVAN KITCHEN — UINameYourDuck.cs
// Panel de bautizo del Pato al iniciar el juego por primera vez.
// Se muestra después del tutorial step 0 (presentación del pato).
// Compatible con Unity 6.3 LTS
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaravanKitchen.UI
{
    public class UINameYourDuck : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject      panel;          // El panel completo
        [SerializeField] private TMP_InputField  nameInput;      // Campo de texto
        [SerializeField] private Button          confirmButton;  // Botón "¡Ese es!"
        [SerializeField] private Button          randomButton;   // Botón "Nombre aleatorio"
        [SerializeField] private TextMeshProUGUI previewText;    // Preview: "Tu pato se llamará..."
        [SerializeField] private TextMeshProUGUI errorText;      // "El nombre no puede estar vacío"

        // Nombres aleatorios sugeridos (todos con vibra cozy)
        private static readonly string[] RandomNames =
        {
            "Quackie", "Tofú", "Patillas", "Plumas", "Cuachin",
            "Burbuja", "Mantequilla", "Mochi", "Gordo", "Pompón",
            "Arroz", "Fideos", "Canela", "Turrón", "Malvavisco",
            "Páprika", "Mostaza", "Azafrán", "Nuez", "Trufa"
        };

        private void Awake()
        {
            confirmButton?.onClick.AddListener(OnConfirm);
            randomButton?.onClick.AddListener(OnRandom);
            nameInput?.onValueChanged.AddListener(OnInputChanged);
            if (errorText) errorText.gameObject.SetActive(false);
        }

        private void Start()
        {
            // Solo mostrar si el pato todavía tiene nombre default
            bool alreadyNamed = SaveSystem.Instance != null &&
                SaveSystem.Instance.GetPatoName("") != "";
            panel?.SetActive(!alreadyNamed);
        }

        // ─── EVENTOS UI ───────────────────────────────────────────────────
        private void OnInputChanged(string value)
        {
            if (previewText)
                previewText.text = string.IsNullOrWhiteSpace(value)
                    ? "Tu pato se llamará..."
                    : $"Tu pato se llamará \"<b>{value.Trim()}</b>\"";
            if (errorText) errorText.gameObject.SetActive(false);
        }

        private void OnConfirm()
        {
            string nombre = nameInput != null ? nameInput.text.Trim() : "";
            if (string.IsNullOrEmpty(nombre))
            {
                if (errorText)
                {
                    errorText.text = "¡Ponle un nombre a tu pato!";
                    errorText.gameObject.SetActive(true);
                }
                return;
            }

            ApplyName(nombre);
        }

        private void OnRandom()
        {
            string nombre = RandomNames[Random.Range(0, RandomNames.Length)];
            if (nameInput) nameInput.text = nombre;
            OnInputChanged(nombre);
        }

        private void ApplyName(string nombre)
        {
            CaravanKitchen.Companion.PatoController.Instance?.SetPatoName(nombre);
            panel?.SetActive(false);
            // Avanza el tutorial al siguiente paso (presentó al pato)
            Core.TutorialManager.NotifyAction("duck_named");
        }
    }
}
