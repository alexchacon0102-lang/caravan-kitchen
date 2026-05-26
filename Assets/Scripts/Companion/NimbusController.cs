// ============================================================
// CARAVAN KITCHEN — NimbusController.cs
// Script #26 — Fase 4
// Compañero Nimbus: afinidad, colores reactivos, detección
// de rareza, comentarios de recetas y diálogos por zona.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaravanKitchen.Companion
{
    public class NimbusController : MonoBehaviour
    {
        public static NimbusController Instance { get; private set; }

        // ─── ESTADOS DE NIMBUS ───────────────────────────────────────────
        public enum NimbusEmotion
        {
            Neutral,    // Blanco-gris — estado base
            Curious,    // Azul        — detecta algo
            Happy,      // Amarillo    — hallazgo / receta nueva
            Excited,    // Naranja     — criatura brillante
            Alert,      // Rojo        — peligro / rareza alta
            Mysterious, // Morado      — zona nueva / secreto
            Approving,  // Verde       — receta cocinada con éxito
            Tired       // Gris oscuro — energía baja
        }

        // ─── INSPECTOR ──────────────────────────────────────────────────
        [Header("Visual")]
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private SpriteRenderer glowRenderer;
        [SerializeField] private float floatAmplitude  = 0.15f;
        [SerializeField] private float floatSpeed      = 1.8f;
        [SerializeField] private float colorTransSpeed = 3f;

        [Header("Detección")]
        [SerializeField] private float rareDetectRadius = 4f;
        [SerializeField] private LayerMask creatureLayer;

        [Header("UI de diálogo")]
        [SerializeField] private GameObject  dialogBubble;
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private float dialogDuration = 3f;

        [Header("Afinidad")]
        [SerializeField] private int maxAffinity = 100;

        // ─── PALETA DE COLORES ───────────────────────────────────────────
        private static readonly Color ColorNeutral    = new Color(0.90f, 0.92f, 0.95f);
        private static readonly Color ColorCurious    = new Color(0.30f, 0.60f, 1.00f);
        private static readonly Color ColorHappy      = new Color(1.00f, 0.90f, 0.20f);
        private static readonly Color ColorExcited    = new Color(1.00f, 0.55f, 0.10f);
        private static readonly Color ColorAlert      = new Color(0.95f, 0.20f, 0.20f);
        private static readonly Color ColorMysterious = new Color(0.65f, 0.25f, 0.95f);
        private static readonly Color ColorApproving  = new Color(0.30f, 0.85f, 0.40f);
        private static readonly Color ColorTired      = new Color(0.45f, 0.45f, 0.50f);

        // ─── ESTADO INTERNO ──────────────────────────────────────────────
        private NimbusEmotion _currentEmotion = NimbusEmotion.Neutral;
        private Color         _targetColor;
        private float         _floatOffset;
        private int           _affinity = 0;
        private Vector3       _baseLocalPos;
        private Coroutine     _dialogCoroutine;
        private Coroutine     _detectCoroutine;

        // ─── FRASES POR EMOCIÓN ──────────────────────────────────────────
        private static readonly string[] PhrasesHappy      = { "¡Mira eso!", "¡Qué descubrimiento!", "¡Lo sabía!", "¡Brillante!" };
        private static readonly string[] PhrasesExcited    = { "¡¡CRIATURA BRILLANTE!!", "¡Es rarísima!", "¡No la dejes escapar!" };
        private static readonly string[] PhrasesApproving  = { "Mmm... ¡delicioso!", "¡Excelente receta!", "Huele increíble.", "¡El mejor platillo!" };
        private static readonly string[] PhrasesCurious    = { "Algo hay por aquí...", "Siento una presencia.", "Espera... ¿lo notas?", "Olé, hay algo oculto." };
        private static readonly string[] PhrasesTired      = { "Descansemos un poco...", "Casi sin energía.", "Volvamos a la caravana." };
        private static readonly string[] PhrasesMysterious = { "Este lugar es antiguo.", "Algo mágico aquí...", "Kael, presta atención.", "Jamás vi algo así." };

        // ─── UNITY ───────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            _targetColor  = ColorNeutral;
            _baseLocalPos = transform.localPosition;
            if (dialogBubble) dialogBubble.SetActive(false);
        }

        private void Start()
        {
            _detectCoroutine = StartCoroutine(DetectionLoop());
        }

        private void Update()
        {
            FloatAnimation();
            LerpColor();
        }

        // ─── FLOTACIÓN ───────────────────────────────────────────────────
        private void FloatAnimation()
        {
            _floatOffset += Time.deltaTime * floatSpeed;
            float y = Mathf.Sin(_floatOffset) * floatAmplitude;
            transform.localPosition = _baseLocalPos + new Vector3(0f, y, 0f);
        }

        // ─── INTERPOLACIÓN DE COLOR ──────────────────────────────────────
        private void LerpColor()
        {
            if (bodyRenderer != null)
                bodyRenderer.color = Color.Lerp(bodyRenderer.color, _targetColor, Time.deltaTime * colorTransSpeed);
            if (glowRenderer != null)
                glowRenderer.color = Color.Lerp(glowRenderer.color,
                    new Color(_targetColor.r, _targetColor.g, _targetColor.b, 0.35f),
                    Time.deltaTime * colorTransSpeed);
        }

        // ─── LOOP DE DETECCIÓN ───────────────────────────────────────────
        private IEnumerator DetectionLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                DetectNearbyRarity();
            }
        }

        private void DetectNearbyRarity()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, rareDetectRadius, creatureLayer);

            float highestRarity = 0f;
            foreach (var h in hits)
            {
                var cb = h.GetComponent<CaravanKitchen.Creatures.CreatureBase>();
                if (cb != null) highestRarity = Mathf.Max(highestRarity, (float)cb.rarity);
            }

            if      (highestRarity >= 4f) SetEmotion(NimbusEmotion.Excited,    true);
            else if (highestRarity >= 2f) SetEmotion(NimbusEmotion.Curious,    true);
            else if (_currentEmotion == NimbusEmotion.Curious ||
                     _currentEmotion == NimbusEmotion.Excited)
                SetEmotion(NimbusEmotion.Neutral, false);
        }

        // ─── API PÚBLICA ─────────────────────────────────────────────────
        /// <summary>Cambia la emoción de Nimbus. Si showDialog=true muestra frase automática.</summary>
        public void SetEmotion(NimbusEmotion emotion, bool showDialog = false)
        {
            if (_currentEmotion == emotion) return;
            _currentEmotion = emotion;

            _targetColor = emotion switch
            {
                NimbusEmotion.Curious    => ColorCurious,
                NimbusEmotion.Happy      => ColorHappy,
                NimbusEmotion.Excited    => ColorExcited,
                NimbusEmotion.Alert      => ColorAlert,
                NimbusEmotion.Mysterious => ColorMysterious,
                NimbusEmotion.Approving  => ColorApproving,
                NimbusEmotion.Tired      => ColorTired,
                _                        => ColorNeutral
            };

            if (showDialog) ShowAutoPhrase(emotion);
        }

        /// <summary>Muestra una frase personalizada en la burbuja de diálogo.</summary>
        public void SayPhrase(string phrase)
        {
            if (_dialogCoroutine != null) StopCoroutine(_dialogCoroutine);
            _dialogCoroutine = StartCoroutine(ShowDialog(phrase));
        }

        /// <summary>Llama esto cuando el jugador cocina una receta nueva.</summary>
        public void ReactToNewRecipe()    => SetEmotion(NimbusEmotion.Happy,      true);

        /// <summary>Llama esto cuando la energía de viaje es baja.</summary>
        public void ReactToLowEnergy()    => SetEmotion(NimbusEmotion.Tired,      true);

        /// <summary>Llama esto al entrar a una zona nueva por primera vez.</summary>
        public void ReactToNewZone()      => SetEmotion(NimbusEmotion.Mysterious, true);

        /// <summary>Llama esto al completar un pedido legendario.</summary>
        public void ReactToLegendaryDish() => SetEmotion(NimbusEmotion.Approving, true);

        // ─── AFINIDAD ────────────────────────────────────────────────────
        /// <summary>Suma puntos de afinidad (máx maxAffinity). Desbloquea frases nuevas.</summary>
        public void AddAffinity(int amount)
        {
            _affinity = Mathf.Clamp(_affinity + amount, 0, maxAffinity);
            if (_affinity >= 50 && _affinity - amount < 50)
                SayPhrase("¡Ya confío en ti, Kael!");
            else if (_affinity >= maxAffinity)
                SayPhrase("Eres el mejor chef que he conocido.");
        }

        public int  GetAffinity()   => _affinity;
        public bool IsHighAffinity  => _affinity >= 75;

        // ─── INTERNAS ────────────────────────────────────────────────────
        private void ShowAutoPhrase(NimbusEmotion emotion)
        {
            string[] pool = emotion switch
            {
                NimbusEmotion.Happy      => PhrasesHappy,
                NimbusEmotion.Excited    => PhrasesExcited,
                NimbusEmotion.Approving  => PhrasesApproving,
                NimbusEmotion.Curious    => PhrasesCurious,
                NimbusEmotion.Tired      => PhrasesTired,
                NimbusEmotion.Mysterious => PhrasesMysterious,
                _ => null
            };
            if (pool != null) SayPhrase(pool[Random.Range(0, pool.Length)]);
        }

        private IEnumerator ShowDialog(string phrase)
        {
            if (dialogBubble == null || dialogText == null) yield break;
            dialogText.text = phrase;
            dialogBubble.SetActive(true);
            yield return new WaitForSeconds(dialogDuration);
            dialogBubble.SetActive(false);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, rareDetectRadius);
        }
    }
}
