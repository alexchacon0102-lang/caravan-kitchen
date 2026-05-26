// ============================================================
// CARAVAN KITCHEN — TutorialManager.cs
// Script #33 — Fase 4
// Tutorial guiado por Nimbus en pasos secuenciales.
// No interrumpe el gameplay: usa overlays suaves y flechas.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaravanKitchen.Core
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        // ─── PASO DE TUTORIAL ────────────────────────────────────────────
        [System.Serializable]
        public class TutorialStep
        {
            public string  stepID;
            public string  title;
            [TextArea]
            public string  message;
            public string  nimbusPhrase;       // Frase de Nimbus al mostrar este paso
            public string  highlightObjectTag; // Tag del objeto a resaltar (opcional)
            public bool    waitForAction;      // Si true, espera que el jugador haga algo
            public string  actionEventID;      // ID del evento que completa el paso
            public bool    isCompleted;
        }

        // ─── INSPECTOR ───────────────────────────────────────────────────
        [Header("Pasos del Tutorial")]
        [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

        [Header("UI del Tutorial")]
        [SerializeField] private GameObject  tutorialPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button      nextButton;
        [SerializeField] private Button      skipButton;
        [SerializeField] private GameObject  highlightArrow;

        [Header("Overlay")]
        [SerializeField] private CanvasGroup overlayCanvasGroup;
        [SerializeField] private float       overlayAlpha = 0.55f;

        // ─── ESTADO ──────────────────────────────────────────────────────
        private int  _currentStepIndex = -1;
        private bool _tutorialActive   = false;
        private bool _tutorialCompleted = false;

        // ─── EVENTS ──────────────────────────────────────────────────────
        // Otros sistemas llaman a NotifyAction(actionID) para avanzar pasos
        public static System.Action<string> OnActionNotified;

        // ─── UNITY ───────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            tutorialPanel?.SetActive(false);
            if (overlayCanvasGroup) overlayCanvasGroup.alpha = 0f;

            nextButton?.onClick.AddListener(NextStep);
            skipButton?.onClick.AddListener(SkipTutorial);

            OnActionNotified += HandleActionNotified;
        }

        private void OnDestroy() => OnActionNotified -= HandleActionNotified;

        private void Start()
        {
            // Cargar estado guardado
            if (SaveSystem.Instance != null)
                _tutorialCompleted = SaveSystem.Instance.GetTutorialCompleted();

            if (!_tutorialCompleted && steps.Count > 0)
                StartCoroutine(StartTutorialDelayed(1.5f));
        }

        // ─── INICIAR ──────────────────────────────────────────────────────
        private IEnumerator StartTutorialDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartTutorial();
        }

        public void StartTutorial()
        {
            if (_tutorialCompleted) return;
            _tutorialActive    = true;
            _currentStepIndex  = -1;
            NextStep();
        }

        // ─── AVANZAR PASO ─────────────────────────────────────────────────
        public void NextStep()
        {
            _currentStepIndex++;

            if (_currentStepIndex >= steps.Count)
            {
                CompleteTutorial();
                return;
            }

            ShowStep(steps[_currentStepIndex]);
        }

        // ─── MOSTRAR PASO ─────────────────────────────────────────────────
        private void ShowStep(TutorialStep step)
        {
            tutorialPanel?.SetActive(true);
            if (titleText)   titleText.text   = step.title;
            if (messageText) messageText.text = step.message;

            // Overlay suave
            if (overlayCanvasGroup)
                StartCoroutine(FadeOverlay(0f, overlayAlpha, 0.4f));

            // Nimbus dice su frase
            if (!string.IsNullOrEmpty(step.nimbusPhrase))
                CaravanKitchen.Companion.NimbusController.Instance?.SayPhrase(step.nimbusPhrase);

            // Ocultar botón Next si el paso espera acción del jugador
            if (nextButton) nextButton.gameObject.SetActive(!step.waitForAction);

            // Flecha de highlight
            if (highlightArrow)
            {
                bool hasTarget = !string.IsNullOrEmpty(step.highlightObjectTag);
                highlightArrow.SetActive(hasTarget);
                if (hasTarget)
                {
                    var target = GameObject.FindGameObjectWithTag(step.highlightObjectTag);
                    if (target) highlightArrow.transform.position = target.transform.position + Vector3.up * 1.2f;
                }
            }
        }

        // ─── COMPLETAR PASO POR ACCIÓN ────────────────────────────────────
        private void HandleActionNotified(string actionID)
        {
            if (!_tutorialActive || _currentStepIndex < 0 || _currentStepIndex >= steps.Count) return;
            var current = steps[_currentStepIndex];
            if (current.waitForAction && current.actionEventID == actionID)
            {
                current.isCompleted = true;
                StartCoroutine(WaitAndNext(0.8f));
            }
        }

        private IEnumerator WaitAndNext(float delay)
        {
            yield return new WaitForSeconds(delay);
            NextStep();
        }

        // ─── SKIP ────────────────────────────────────────────────────────
        public void SkipTutorial()
        {
            foreach (var step in steps) step.isCompleted = true;
            CompleteTutorial();
        }

        // ─── COMPLETAR ───────────────────────────────────────────────────
        private void CompleteTutorial()
        {
            _tutorialActive    = false;
            _tutorialCompleted = true;
            tutorialPanel?.SetActive(false);
            if (overlayCanvasGroup)
                StartCoroutine(FadeOverlay(overlayAlpha, 0f, 0.4f));
            SaveSystem.Instance?.SetTutorialCompleted(true);
            CaravanKitchen.Companion.NimbusController.Instance
                ?.SayPhrase("¡Ya sabes todo lo que necesitas, Kael!");
            Debug.Log("[Tutorial] ✅ Tutorial completado.");
        }

        // ─── FADE OVERLAY ────────────────────────────────────────────────
        private IEnumerator FadeOverlay(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (overlayCanvasGroup) overlayCanvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            if (overlayCanvasGroup) overlayCanvasGroup.alpha = to;
        }

        // ─── API PÚBLICA ─────────────────────────────────────────────────
        /// <summary>
        /// Otros sistemas llaman a esto para notificar que el jugador completó una acción.
        /// Ejemplo: PlayerInventory.cs llama TutorialManager.NotifyAction("first_collect");
        /// </summary>
        public static void NotifyAction(string actionID) => OnActionNotified?.Invoke(actionID);

        public bool IsTutorialActive => _tutorialActive;
    }
}
