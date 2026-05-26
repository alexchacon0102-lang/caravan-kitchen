// ============================================================
// CARAVAN KITCHEN — PatoController.cs
// Script #26 — Fase 4 (renombrado desde NimbusController)
// Compañero: PATO — pato blanco de ojitos negros y cachetes
// regordetes. Waddles al caminar, alas pequeñas, pico naranja.
// Reacciona con colores en el cuerpo (overlay/tint) y frases.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaravanKitchen.Companion
{
    /// <summary>
    /// Controlador del compañero Pato.
    /// El Pato es un pato blanco regordete con ojitos negros brillantes
    /// y cachetes rosados. Reacciona al mundo con tints de color sobre
    /// su sprite base blanco, waddle animation y frases en burbuja.
    /// </summary>
    public class PatoController : MonoBehaviour
    {
        public static PatoController Instance { get; private set; }

        // ─── EMOCIONES DEL PATO ──────────────────────────────────────────
        public enum PatoEmotion
        {
            Neutral,    // Blanco puro       — waddle tranquilo
            Curious,    // Tint azul suave   — detecta algo
            Happy,      // Tint amarillo     — hallazgo / receta nueva
            Excited,    // Tint naranja      — criatura brillante (aletea fuerte)
            Alert,      // Tint rojo         — peligro / rareza alta
            Mysterious, // Tint morado       — zona nueva / secreto
            Approving,  // Tint verde        — receta cocinada con éxito
            Tired       // Tint gris         — energía baja (se sienta)
        }

        // ─── INSPECTOR ──────────────────────────────────────────────────
        [Header("Sprite del Pato")]
        [SerializeField] private SpriteRenderer bodyRenderer;   // Sprite blanco base del pato
        [SerializeField] private SpriteRenderer glowRenderer;   // Aura/glow detrás del pato
        [SerializeField] private SpriteRenderer cheeksRenderer; // Cachetes rosados (siempre visibles)

        [Header("Animación Waddle")]
        [SerializeField] private float waddleAmplitude  = 0.10f;  // Balanceo lateral (pato)
        [SerializeField] private float waddleSpeed      = 2.5f;   // Más rápido que la nube
        [SerializeField] private float bobAmplitude     = 0.08f;  // Bob vertical suave
        [SerializeField] private float colorTransSpeed  = 3f;

        [Header("Detección de Criaturas")]
        [SerializeField] private float rareDetectRadius = 4f;
        [SerializeField] private LayerMask creatureLayer;

        [Header("UI de Diálogo")]
        [SerializeField] private GameObject      dialogBubble;
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private float           dialogDuration = 3f;

        [Header("Afinidad")]
        [SerializeField] private int maxAffinity = 100;

        // ─── PALETA DE TINTS (sobre sprite blanco) ───────────────────────
        // El sprite base del pato es BLANCO para que los tints funcionen correctamente.
        // Los cachetes siempre son rosa: Color(1f, 0.75f, 0.80f)
        private static readonly Color TintNeutral    = Color.white;                          // Blanco puro
        private static readonly Color TintCurious    = new Color(0.70f, 0.88f, 1.00f);      // Azul cielo suave
        private static readonly Color TintHappy      = new Color(1.00f, 0.96f, 0.60f);      // Amarillo suave
        private static readonly Color TintExcited    = new Color(1.00f, 0.75f, 0.40f);      // Naranja cálido
        private static readonly Color TintAlert      = new Color(1.00f, 0.55f, 0.55f);      // Rojo suave
        private static readonly Color TintMysterious = new Color(0.85f, 0.70f, 1.00f);      // Lavanda
        private static readonly Color TintApproving  = new Color(0.70f, 1.00f, 0.75f);      // Verde menta
        private static readonly Color TintTired      = new Color(0.75f, 0.75f, 0.80f);      // Gris azulado
        private static readonly Color CheeksColor    = new Color(1.00f, 0.75f, 0.80f, 1f);  // Rosa cachetes

        // ─── ESTADO INTERNO ──────────────────────────────────────────────
        private PatoEmotion _currentEmotion = PatoEmotion.Neutral;
        private Color       _targetTint      = Color.white;
        private float       _waddleOffset;
        private int         _affinity = 0;
        private Vector3     _baseLocalPos;
        private Coroutine   _dialogCoroutine;
        private Coroutine   _detectCoroutine;
        private bool        _isSitting = false;

        // ─── FRASES DEL PATO (personalidad curiosa y entrañable) ─────────
        private static readonly string[] PhrasesHappy      = { "¡CUAC! ¡Mira eso!", "¡Cuac cuac!", "¡Qué hallazgo!", "¡Lo sabía, cuac!" };
        private static readonly string[] PhrasesExcited    = { "¡¡CUAC CUAC CUAC!!", "¡CRIATURA BRILLANTE!", "¡No la dejes ir, cuac!", "¡¡ALETEA DE EMOCIÓN!!" };
        private static readonly string[] PhrasesApproving  = { "Mmm... ¡cuac delicioso!", "¡Excelente, chef!", "Huele increíble. Cuac.", "¡El mejor platillo!" };
        private static readonly string[] PhrasesCurious    = { "...cuac?", "Algo hay aquí.", "Espera... ¿lo notas?", "El pato lo siente." };
        private static readonly string[] PhrasesTired      = { "...cuac...", "Descansemos...", "Pato cansado.", "Volvamos, cuac." };
        private static readonly string[] PhrasesMysterious = { "...este lugar es raro.", "Cuac... mágico.", "Kael, cuidado.", "Jamás vi algo así. Cuac." };

        // ─── UNITY ───────────────────────────────────────────────────────
        private void Awake()
        {
            Instance     = this;
            _targetTint  = TintNeutral;
            _baseLocalPos = transform.localPosition;

            // Cachetes siempre en rosa
            if (cheeksRenderer != null)
                cheeksRenderer.color = CheeksColor;

            if (dialogBubble) dialogBubble.SetActive(false);
        }

        private void Start()
        {
            _detectCoroutine = StartCoroutine(DetectionLoop());
        }

        private void Update()
        {
            WaddleAnimation();
            LerpTint();
        }

        // ─── ANIMACIÓN WADDLE ────────────────────────────────────────────
        /// <summary>
        /// El pato se balancea lateralmente (waddle) y hace un pequeño bob vertical.
        /// Cuando está Tired se sienta (sin animación de waddle).
        /// </summary>
        private void WaddleAnimation()
        {
            if (_isSitting) return;

            _waddleOffset += Time.deltaTime * waddleSpeed;

            float x = Mathf.Sin(_waddleOffset)         * waddleAmplitude;
            float y = Mathf.Abs(Mathf.Sin(_waddleOffset)) * bobAmplitude; // bob sube en cada paso

            transform.localPosition = _baseLocalPos + new Vector3(x, y, 0f);

            // Rotar levemente el cuerpo para simular el balanceo
            float tilt = Mathf.Sin(_waddleOffset) * 8f; // ±8 grados
            transform.localRotation = Quaternion.Euler(0f, 0f, -tilt);
        }

        // ─── INTERPOLACIÓN DE TINT ───────────────────────────────────────
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
                if (cb != null)
                    highestRarity = Mathf.Max(highestRarity, (float)cb.rarity);
            }

            if      (highestRarity >= 4f) SetEmotion(PatoEmotion.Excited, true);
            else if (highestRarity >= 2f) SetEmotion(PatoEmotion.Curious, true);
            else if (_currentEmotion == PatoEmotion.Curious ||
                     _currentEmotion == PatoEmotion.Excited)
                SetEmotion(PatoEmotion.Neutral, false);
        }

        // ─── API PÚBLICA ─────────────────────────────────────────────────

        /// <summary>Cambia la emoción del Pato. Si showDialog=true muestra frase automática.</summary>
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

            // Cuando está cansado se sienta (detiene waddle)
            _isSitting = emotion == PatoEmotion.Tired;
            if (_isSitting)
            {
                transform.localPosition = _baseLocalPos + new Vector3(0f, -0.05f, 0f);
                transform.localRotation = Quaternion.identity;
            }

            if (showDialog) ShowAutoPhrase(emotion);
        }

        /// <summary>Muestra una frase personalizada en la burbuja de diálogo.</summary>
        public void SayPhrase(string phrase)
        {
            if (_dialogCoroutine != null) StopCoroutine(_dialogCoroutine);
            _dialogCoroutine = StartCoroutine(ShowDialog(phrase));
        }

        // Atajos semánticos para llamar desde otros scripts
        public void ReactToNewRecipe()     => SetEmotion(PatoEmotion.Happy,      true);
        public void ReactToLowEnergy()     => SetEmotion(PatoEmotion.Tired,      true);
        public void ReactToNewZone()       => SetEmotion(PatoEmotion.Mysterious, true);
        public void ReactToLegendaryDish() => SetEmotion(PatoEmotion.Approving,  true);

        // ─── AFINIDAD ────────────────────────────────────────────────────
        public void AddAffinity(int amount)
        {
            _affinity = Mathf.Clamp(_affinity + amount, 0, maxAffinity);
            if (_affinity >= 50 && _affinity - amount < 50)
                SayPhrase("¡Ya confío en ti, cuac!");
            else if (_affinity >= maxAffinity)
                SayPhrase("Eres el mejor chef. ¡CUAC!");
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
            Gizmos.color = new Color(1f, 0.8f, 0f); // amarillo pato
            Gizmos.DrawWireSphere(transform.position, rareDetectRadius);
        }
    }
}
