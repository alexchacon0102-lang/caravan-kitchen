// ============================================================
// CARAVAN KITCHEN — PatoController.cs
// Script #26 — Fase 4
// Compañero: PATO — pato blanco regordete, ojitos negros,
// cachetes rosados. El jugador le pone su propio nombre.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections;
using UnityEngine;
using TMPro;

namespace CaravanKitchen.Companion
{
    public class PatoController : MonoBehaviour
    {
        public static PatoController Instance { get; private set; }

        // ─── EMOCIONES ───────────────────────────────────────────────────
        public enum PatoEmotion
        {
            Neutral,    // Blanco puro       — waddle tranquilo
            Curious,    // Tint azul suave   — detecta algo
            Happy,      // Tint amarillo     — hallazgo / receta nueva
            Excited,    // Tint naranja      — criatura brillante
            Alert,      // Tint rojo         — rareza muy alta
            Mysterious, // Tint morado       — zona nueva / secreto
            Approving,  // Tint verde        — receta cocinada con éxito
            Tired       // Tint gris         — energía baja
        }

        // ─── INSPECTOR ───────────────────────────────────────────────────
        [Header("Sprite del Pato")]
        [SerializeField] private SpriteRenderer bodyRenderer;   // Sprite blanco base (recibe tint)
        [SerializeField] private SpriteRenderer glowRenderer;   // Aura detrás (recibe tint)
        [SerializeField] private SpriteRenderer cheeksRenderer; // Cachetes rosados (fijos)

        [Header("Animación Waddle")]
        [SerializeField] private float waddleAmplitude = 0.10f;
        [SerializeField] private float waddleSpeed     = 2.5f;
        [SerializeField] private float bobAmplitude    = 0.08f;
        [SerializeField] private float colorTransSpeed = 3f;

        [Header("Detección de Criaturas")]
        [SerializeField] private float     rareDetectRadius = 4f;
        [SerializeField] private LayerMask creatureLayer;

        [Header("UI de Diálogo")]
        [SerializeField] private GameObject      dialogBubble;
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private float           dialogDuration = 3f;

        [Header("Nombre del Pato")]
        [Tooltip("Nombre por defecto antes de que el jugador lo cambie")]
        [SerializeField] private string defaultName = "Pato";
        [SerializeField] private TextMeshProUGUI nameTagText; // Label flotante sobre el pato (opcional)

        [Header("Afinidad")]
        [SerializeField] private int maxAffinity = 100;

        // ─── TINTS (sobre sprite blanco) ─────────────────────────────────
        private static readonly Color TintNeutral    = Color.white;
        private static readonly Color TintCurious    = new Color(0.70f, 0.88f, 1.00f);
        private static readonly Color TintHappy      = new Color(1.00f, 0.96f, 0.60f);
        private static readonly Color TintExcited    = new Color(1.00f, 0.75f, 0.40f);
        private static readonly Color TintAlert      = new Color(1.00f, 0.55f, 0.55f);
        private static readonly Color TintMysterious = new Color(0.85f, 0.70f, 1.00f);
        private static readonly Color TintApproving  = new Color(0.70f, 1.00f, 0.75f);
        private static readonly Color TintTired      = new Color(0.75f, 0.75f, 0.80f);
        private static readonly Color CheeksColor    = new Color(1.00f, 0.75f, 0.80f, 1f);

        // ─── ESTADO ──────────────────────────────────────────────────────
        private PatoEmotion _currentEmotion = PatoEmotion.Neutral;
        private Color       _targetTint     = Color.white;
        private float       _waddleOffset;
        private int         _affinity       = 0;
        private Vector3     _baseLocalPos;
        private Coroutine   _dialogCoroutine;
        private bool        _isSitting      = false;
        private string      _patoName;           // Nombre actual (guardado en SaveSystem)

        // ─── FRASES (usan {name} como placeholder) ────────────────────────
        // Se reemplazan en tiempo real con el nombre real del pato.
        private static readonly string[] PhrasesHappy      = { "¡CUAC! ¡Mira eso!", "¡Cuac cuac!", "¡Qué hallazgo!", "¡Lo sabía, cuac!" };
        private static readonly string[] PhrasesExcited    = { "¡¡CUAC CUAC CUAC!!", "¡CRIATURA BRILLANTE!", "¡No la dejes ir, cuac!", "¡¡{name} ALETEA DE EMOCIÓN!!" };
        private static readonly string[] PhrasesApproving  = { "Mmm... ¡cuac delicioso!", "¡Excelente, chef!", "Huele increíble. Cuac.", "¡El mejor platillo!" };
        private static readonly string[] PhrasesCurious    = { "...cuac?", "Algo hay aquí.", "Espera... ¿lo notas?", "{name} lo siente." };
        private static readonly string[] PhrasesTired      = { "...cuac...", "Descansemos...", "{name} cansado.", "Volvamos, cuac." };
        private static readonly string[] PhrasesMysterious = { "...este lugar es raro.", "Cuac... mágico.", "Kael, cuidado.", "Jamás vi algo así. Cuac." };

        // ─── UNITY ───────────────────────────────────────────────────────
        private void Awake()
        {
            Instance      = this;
            _targetTint   = TintNeutral;
            _baseLocalPos = transform.localPosition;

            if (cheeksRenderer != null)
                cheeksRenderer.color = CheeksColor;

            // Cargar nombre guardado o usar default
            _patoName = SaveSystem.Instance != null
                ? SaveSystem.Instance.GetPatoName(defaultName)
                : defaultName;

            UpdateNameTag();
            if (dialogBubble) dialogBubble.SetActive(false);
        }

        private void Start() => StartCoroutine(DetectionLoop());

        private void Update()
        {
            WaddleAnimation();
            LerpTint();
        }

        // ─── NOMBRE PERSONALIZABLE ────────────────────────────────────────

        /// <summary>Nombre actual del pato (el que el jugador eligió).</summary>
        public string PatoName => _patoName;

        /// <summary>
        /// Cambia el nombre del pato y lo guarda en SaveSystem.
        /// Llamar desde el panel de bautizo al inicio del juego.
        /// </summary>
        public void SetPatoName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) return;
            _patoName = newName.Trim();
            SaveSystem.Instance?.SetPatoName(_patoName);
            UpdateNameTag();
            SayPhrase($"¡{_patoName}! ¡Ese es mi nombre! ¡CUAC!");
        }

        private void UpdateNameTag()
        {
            if (nameTagText != null)
                nameTagText.text = _patoName;
        }

        // Reemplaza {name} en frases con el nombre real del pato
        private string ReplaceName(string phrase) =>
            phrase.Replace("{name}", _patoName);

        // ─── WADDLE ──────────────────────────────────────────────────────
        private void WaddleAnimation()
        {
            if (_isSitting) return;
            _waddleOffset += Time.deltaTime * waddleSpeed;
            float x    = Mathf.Sin(_waddleOffset)            * waddleAmplitude;
            float y    = Mathf.Abs(Mathf.Sin(_waddleOffset)) * bobAmplitude;
            transform.localPosition = _baseLocalPos + new Vector3(x, y, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, -Mathf.Sin(_waddleOffset) * 8f);
        }

        // ─── TINT ────────────────────────────────────────────────────────
        private void LerpTint()
        {
            if (bodyRenderer != null)
                bodyRenderer.color = Color.Lerp(bodyRenderer.color, _targetTint,
                    Time.deltaTime * colorTransSpeed);
            if (glowRenderer != null)
                glowRenderer.color = Color.Lerp(glowRenderer.color,
                    new Color(_targetTint.r, _targetTint.g, _targetTint.b, 0.30f),
                    Time.deltaTime * colorTransSpeed);
        }

        // ─── DETECCIÓN ───────────────────────────────────────────────────
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
            float high = 0f;
            foreach (var h in hits)
            {
                var cb = h.GetComponent<CaravanKitchen.Creatures.CreatureBase>();
                if (cb != null) high = Mathf.Max(high, (float)cb.rarity);
            }
            if      (high >= 4f) SetEmotion(PatoEmotion.Excited, true);
            else if (high >= 2f) SetEmotion(PatoEmotion.Curious,  true);
            else if (_currentEmotion == PatoEmotion.Curious ||
                     _currentEmotion == PatoEmotion.Excited)
                SetEmotion(PatoEmotion.Neutral, false);
        }

        // ─── API PÚBLICA ─────────────────────────────────────────────────
        public void SetEmotion(PatoEmotion emotion, bool showDialog = false)
        {
            if (_currentEmotion == emotion) return;
            _currentEmotion = emotion;
            _targetTint = emotion switch
            {
                PatoEmotion.Curious    => TintCurious,
                PatoEmotion.Happy      => TintHappy,
                PatoEmotion.Excited    => TintExcited,
                PatoEmotion.Alert      => TintAlert,
                PatoEmotion.Mysterious => TintMysterious,
                PatoEmotion.Approving  => TintApproving,
                PatoEmotion.Tired      => TintTired,
                _                      => TintNeutral
            };
            _isSitting = emotion == PatoEmotion.Tired;
            if (_isSitting)
            {
                transform.localPosition = _baseLocalPos + new Vector3(0f, -0.05f, 0f);
                transform.localRotation = Quaternion.identity;
            }
            if (showDialog) ShowAutoPhrase(emotion);
        }

        public void SayPhrase(string phrase)
        {
            if (_dialogCoroutine != null) StopCoroutine(_dialogCoroutine);
            _dialogCoroutine = StartCoroutine(ShowDialog(ReplaceName(phrase)));
        }

        public void ReactToNewRecipe()     => SetEmotion(PatoEmotion.Happy,      true);
        public void ReactToLowEnergy()     => SetEmotion(PatoEmotion.Tired,      true);
        public void ReactToNewZone()       => SetEmotion(PatoEmotion.Mysterious, true);
        public void ReactToLegendaryDish() => SetEmotion(PatoEmotion.Approving,  true);

        // ─── AFINIDAD ────────────────────────────────────────────────────
        public void AddAffinity(int amount)
        {
            _affinity = Mathf.Clamp(_affinity + amount, 0, maxAffinity);
            if (_affinity >= 50 && _affinity - amount < 50)
                SayPhrase($"¡Ya confío en ti, Kael! Soy {_patoName}. ¡Cuac!");
            else if (_affinity >= maxAffinity)
                SayPhrase($"Eres el mejor chef. ¡CUAC! — {_patoName} para siempre.");
        }

        public int  GetAffinity()  => _affinity;
        public bool IsHighAffinity => _affinity >= 75;

        // ─── INTERNAS ────────────────────────────────────────────────────
        private void ShowAutoPhrase(PatoEmotion emotion)
        {
            string[] pool = emotion switch
            {
                PatoEmotion.Happy      => PhrasesHappy,
                PatoEmotion.Excited    => PhrasesExcited,
                PatoEmotion.Approving  => PhrasesApproving,
                PatoEmotion.Curious    => PhrasesCurious,
                PatoEmotion.Tired      => PhrasesTired,
                PatoEmotion.Mysterious => PhrasesMysterious,
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
            Gizmos.color = new Color(1f, 0.8f, 0f);
            Gizmos.DrawWireSphere(transform.position, rareDetectRadius);
        }
    }
}
