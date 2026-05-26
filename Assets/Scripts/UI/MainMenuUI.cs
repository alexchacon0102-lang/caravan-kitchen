// ============================================================
// CARAVAN KITCHEN — MainMenuUI.cs
// Script #14 de 14
// PROGRESO: UI/MainMenuUI.cs ✅ completado
// ✅✅✅ TODOS LOS SCRIPTS MVP COMPLETADOS ✅✅✅
// SIGUIENTE PASO: Configurar escenas en Unity y asignar refs
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaravanKitchen.Core;

namespace CaravanKitchen.UI
{
    /// <summary>
    /// UI del Menú Principal.
    /// Maneja pantallas: Título, Nueva Partida, Créditos.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Pantallas")]
        [SerializeField] private GameObject panelTitle;
        [SerializeField] private GameObject panelCredits;

        [Header("Botones principal")]
        [SerializeField] private Button btnNewGame;
        [SerializeField] private Button btnContinue;
        [SerializeField] private Button btnCredits;
        [SerializeField] private Button btnQuit;

        [Header("Creditos")]
        [SerializeField] private Button btnBackFromCredits;

        [Header("Animación de título")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private float           titleBobSpeed    = 1.2f;
        [SerializeField] private float           titleBobAmount   = 8f;
        private Vector3 _titleBasePos;

        // ── Init ───────────────────────────────────────────────
        private void Start()
        {
            ShowPanel(panelTitle);

            btnNewGame?.onClick.AddListener(OnNewGame);
            btnContinue?.onClick.AddListener(OnContinue);
            btnCredits?.onClick.AddListener(OnShowCredits);
            btnQuit?.onClick.AddListener(OnQuit);
            btnBackFromCredits?.onClick.AddListener(() => ShowPanel(panelTitle));

            if (titleText != null)
                _titleBasePos = titleText.rectTransform.anchoredPosition;
        }

        private void Update()
        {
            // Animación flotante del título
            if (titleText != null)
            {
                float y = _titleBasePos.y + Mathf.Sin(Time.time * titleBobSpeed) * titleBobAmount;
                titleText.rectTransform.anchoredPosition = new Vector2(_titleBasePos.x, y);
            }
        }

        // ── Acciones ────────────────────────────────────────────
        private void OnNewGame()
        {
            Debug.Log("[MainMenu] Nueva partida");
            GameManager.Instance?.StartNewGame();
        }

        private void OnContinue()
        {
            // En MVP no hay save: directo a nueva
            Debug.Log("[MainMenu] Continuar (redirige a nueva partida en MVP)");
            GameManager.Instance?.StartNewGame();
        }

        private void OnShowCredits()
        {
            ShowPanel(panelCredits);
        }

        private void OnQuit()
        {
            Debug.Log("[MainMenu] Salir");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        // ── Helper ───────────────────────────────────────────────
        private void ShowPanel(GameObject target)
        {
            if (panelTitle   != null) panelTitle.SetActive(panelTitle   == target);
            if (panelCredits != null) panelCredits.SetActive(panelCredits == target);
        }
    }
}
