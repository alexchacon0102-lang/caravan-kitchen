using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NimbusController — Compañero Nimbus
/// Controla movimiento orbital, colores de estado, reacciones emocionales,
/// sistema de afinidad y detección de rareza en expedición.
/// Fase 4 — Caravan Kitchen
/// </summary>
public class NimbusController : MonoBehaviour
{
    public static NimbusController Instance { get; private set; }

    // ─── Config ───────────────────────────────────────────
    [Header("Movimiento orbital")]
    public Transform followTarget;          // Kael
    public float orbitRadius   = 1.4f;
    public float orbitSpeed    = 1.8f;
    public float floatAmplitude = 0.18f;
    public float floatSpeed    = 2.2f;
    public float followSmoothness = 6f;

    [Header("Colores de estado")]
    public Color colorNeutral  = new Color(0.90f, 0.95f, 1.00f);
    public Color colorCurious  = new Color(0.30f, 0.70f, 1.00f);  // azul
    public Color colorHappy    = new Color(1.00f, 0.90f, 0.20f);  // amarillo
    public Color colorDanger   = new Color(1.00f, 0.25f, 0.20f);  // rojo
    public Color colorMystery  = new Color(0.65f, 0.30f, 1.00f);  // morado
    public Color colorApproval = new Color(0.30f, 1.00f, 0.50f);  // verde
    public float colorTransitionSpeed = 3f;

    [Header("Detección")]
    public float rareDetectRadius  = 5f;
    public float itemDetectRadius  = 3.5f;
    public LayerMask creatureLayer;
    public LayerMask resourceLayer;

    [Header("Afinidad")]
    public int affinityPoints = 0;          // persiste vía SaveSystem
    // Niveles: 0-19 Neutral | 20-49 Amigable | 50-99 Cercano | 100+ Leal

    [Header("UI")]
    public SpriteRenderer bodyRenderer;
    public ParticleSystem emotionParticles;
    public GameObject speechBubble;
    public Text speechText;
    public float speechDuration = 2.5f;

    // ─── Estado ───────────────────────────────────────────
    public enum NimbusState { Neutral, Curious, Happy, Danger, Mystery, Approval }
    private NimbusState _currentState = NimbusState.Neutral;
    private NimbusState _previousState = NimbusState.Neutral;

    private float _orbitAngle = 0f;
    private Vector3 _basePos;
    private float _floatTimer = 0f;
    private Coroutine _speechCo;
    private Coroutine _pulseReactionCo;

    // Frases por nivel de afinidad
    private readonly Dictionary<string, string[]> _phrases = new()
    {
        ["rare_neutral"]   = new[]{ "!", "?!", "...!" },
        ["rare_friendly"]  = new[]{ "¡Algo raro por ahí!", "¡Brilla mucho!", "¡Mira eso!" },
        ["rare_close"]     = new[]{ "¡Es brillante, ve rápido!", "¡No lo dejes escapar!" },
        ["rare_loyal"]     = new[]{ "¡RARO ÉPICO! ¡Corre!", "¡Ese ingrediente vale oro!" },
        ["recipe_new"]     = new[]{ "¡Receta nueva!", "¡Eso huele increíble!", "¡Genial!" },
        ["level_up"]       = new[]{ "¡Subiste de nivel!", "¡Sigo contigo!", "¡Imparable!" },
        ["night_start"]    = new[]{ "...La noche despierta criaturas raras.", "Cuidado en la oscuridad." },
        ["morning"]        = new[]{ "¡Buen día!", "¡A explorar!", "El desayuno puede esperar." },
        ["affinity_up"]    = new[]{ "Me alegra estar aquí.", "Gracias por incluirme.", "Juntos llegamos lejos." }
    };

    // ─── Unity ────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (bodyRenderer == null) bodyRenderer = GetComponent<SpriteRenderer>();
        if (speechBubble  != null) speechBubble.SetActive(false);
        _basePos = transform.position;
        InvokeRepeating(nameof(ScanEnvironment), 1f, 1.5f);
    }

    void Update()
    {
        HandleOrbitalMovement();
        HandleColorTransition();
    }

    // ─── Movimiento ───────────────────────────────────────
    void HandleOrbitalMovement()
    {
        if (followTarget == null) return;

        _orbitAngle  += orbitSpeed * Time.deltaTime;
        _floatTimer  += floatSpeed * Time.deltaTime;

        float x = followTarget.position.x + Mathf.Cos(_orbitAngle) * orbitRadius;
        float y = followTarget.position.y + 1.2f + Mathf.Sin(_floatTimer) * floatAmplitude;
        float z = followTarget.position.z;

        transform.position = Vector3.Lerp(transform.position,
            new Vector3(x, y, z), followSmoothness * Time.deltaTime);
    }

    // ─── Color ────────────────────────────────────────────
    void HandleColorTransition()
    {
        if (bodyRenderer == null) return;
        Color target = StateToColor(_currentState);
        bodyRenderer.color = Color.Lerp(bodyRenderer.color, target,
            colorTransitionSpeed * Time.deltaTime);
    }

    Color StateToColor(NimbusState state) => state switch
    {
        NimbusState.Curious  => colorCurious,
        NimbusState.Happy    => colorHappy,
        NimbusState.Danger   => colorDanger,
        NimbusState.Mystery  => colorMystery,
        NimbusState.Approval => colorApproval,
        _                    => colorNeutral
    };

    // ─── Detección ambiental ──────────────────────────────
    void ScanEnvironment()
    {
        // Criaturas raras
        Collider2D[] creatures = Physics2D.OverlapCircleAll(
            transform.position, rareDetectRadius, creatureLayer);
        foreach (var c in creatures)
        {
            var cb = c.GetComponent<CreatureBase>();
            if (cb != null && (int)cb.rarity >= 3)  // Raro, Épico, Brillante
            {
                ReactToRarity(cb.rarity);
                return;
            }
        }

        // Recursos ocultos
        Collider2D[] resources = Physics2D.OverlapCircleAll(
            transform.position, itemDetectRadius, resourceLayer);
        foreach (var r in resources)
        {
            var rn = r.GetComponent<ResourceNode>();
            if (rn != null && rn.isHidden)
            {
                SetState(NimbusState.Curious);
                Speak("item_hint", "¡Hay algo escondido cerca!");
                return;
            }
        }

        // Nada especial
        if (_currentState != NimbusState.Neutral)
            SetState(NimbusState.Neutral);
    }

    void ReactToRarity(CreatureRarity rarity)
    {
        switch (rarity)
        {
            case CreatureRarity.Raro:
                SetState(NimbusState.Curious);
                SpeakFromPool("rare_friendly");
                break;
            case CreatureRarity.Epico:
                SetState(NimbusState.Danger);
                SpeakFromPool("rare_close");
                PulseReaction(colorDanger);
                break;
            case CreatureRarity.Brillante:
                SetState(NimbusState.Mystery);
                SpeakFromPool("rare_loyal");
                PulseReaction(colorMystery);
                break;
        }
    }

    // ─── API pública ──────────────────────────────────────
    public void OnNewRecipeCooked()     { SetState(NimbusState.Happy);    SpeakFromPool("recipe_new");  GainAffinity(3); }
    public void OnPlayerLevelUp()       { SetState(NimbusState.Happy);    SpeakFromPool("level_up");    GainAffinity(5); }
    public void OnNightBegins()         { SetState(NimbusState.Mystery);  SpeakFromPool("night_start"); }
    public void OnMorning()             { SetState(NimbusState.Happy);    SpeakFromPool("morning");     }
    public void OnCookingSuccess()      { SetState(NimbusState.Approval); TriggerEmotionParticle();     GainAffinity(1); }

    public void SetState(NimbusState state)
    {
        _previousState = _currentState;
        _currentState  = state;
    }

    public NimbusState GetState() => _currentState;

    public int GetAffinityLevel()
    {
        if (affinityPoints >= 100) return 3;  // Leal
        if (affinityPoints >= 50)  return 2;  // Cercano
        if (affinityPoints >= 20)  return 1;  // Amigable
        return 0;                              // Neutral
    }

    public string GetAffinityLevelName() => GetAffinityLevel() switch
    {
        3 => "Leal",
        2 => "Cercano",
        1 => "Amigable",
        _ => "Neutral"
    };

    // ─── Afinidad ─────────────────────────────────────────
    void GainAffinity(int amount)
    {
        int prev = GetAffinityLevel();
        affinityPoints = Mathf.Min(affinityPoints + amount, 200);
        if (GetAffinityLevel() > prev)
        {
            SpeakFromPool("affinity_up");
            PulseReaction(colorApproval);
        }
    }

    // ─── Speech ───────────────────────────────────────────
    void SpeakFromPool(string key)
    {
        if (!_phrases.TryGetValue(key, out string[] pool)) return;
        string msg = pool[Random.Range(0, pool.Length)];
        Speak(key, msg);
    }

    void Speak(string _, string message)
    {
        if (speechBubble == null) return;
        if (_speechCo != null) StopCoroutine(_speechCo);
        _speechCo = StartCoroutine(ShowSpeech(message));
    }

    IEnumerator ShowSpeech(string message)
    {
        speechBubble.SetActive(true);
        if (speechText != null) speechText.text = message;
        yield return new WaitForSeconds(speechDuration);
        speechBubble.SetActive(false);
    }

    // ─── Partículas ───────────────────────────────────────
    void TriggerEmotionParticle()
    {
        if (emotionParticles != null) emotionParticles.Play();
    }

    void PulseReaction(Color pulseColor)
    {
        if (_pulseReactionCo != null) StopCoroutine(_pulseReactionCo);
        _pulseReactionCo = StartCoroutine(PulseRoutine(pulseColor));
    }

    IEnumerator PulseRoutine(Color pulseColor)
    {
        if (bodyRenderer == null) yield break;
        for (int i = 0; i < 3; i++)
        {
            bodyRenderer.color = pulseColor * 1.5f;
            yield return new WaitForSeconds(0.12f);
            bodyRenderer.color = pulseColor;
            yield return new WaitForSeconds(0.12f);
        }
    }

    // ─── Gizmos ───────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rareDetectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemDetectRadius);
    }
}
