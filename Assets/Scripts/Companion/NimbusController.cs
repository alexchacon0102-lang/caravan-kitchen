using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NimbusController — Compañero vivo con personalidad, afinidad y reacciones.
/// Detecta rarezas, comenta recetas, guía al jugador y evoluciona con el tiempo.
/// </summary>
public class NimbusController : MonoBehaviour
{
    public static NimbusController Instance { get; private set; }

    // ─── ESTADO DE AFINIDAD ───────────────────────────────────────────────────
    [Header("Afinidad")]
    [Range(0, 100)] public float affinityLevel = 0f;
    public int affinityTier => affinityLevel < 25 ? 0 : affinityLevel < 50 ? 1 : affinityLevel < 75 ? 2 : 3;
    // Tier 0 = Desconocido | 1 = Amigable | 2 = Cercano | 3 = Inseparable

    [Header("Reacciones")] 
    public float reactionCooldown = 3f;
    private float _lastReactionTime;

    // ─── COMPONENTES ─────────────────────────────────────────────────────────
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Light2D pointLight;
    public Animator animator;
    public Transform floatingTextAnchor;

    [Header("Movimiento")]
    public Transform playerTransform;
    public float followDistance = 1.4f;
    public float followSpeed = 3f;
    public float bobAmplitude = 0.12f;
    public float bobFrequency = 1.5f;

    // ─── COLORES POR ESTADO ───────────────────────────────────────────────────
    private static readonly Color ColorNeutral  = new Color(0.85f, 0.92f, 1f);
    private static readonly Color ColorCurious  = new Color(0.3f,  0.6f,  1f);
    private static readonly Color ColorHappy    = new Color(1f,    0.85f, 0.2f);
    private static readonly Color ColorDanger   = new Color(1f,    0.25f, 0.2f);
    private static readonly Color ColorMystery  = new Color(0.7f,  0.3f,  1f);
    private static readonly Color ColorApproval = new Color(0.3f,  1f,    0.5f);

    // ─── FRASES POR CONTEXTO ──────────────────────────────────────────────────
    private readonly Dictionary<string, string[]> _phrases = new Dictionary<string, string[]>
    {
        { "rare_found",   new[]{ "¡Ahí está!", "¡Eso es raro!", "¡No te lo pierdas!" } },
        { "recipe_new",   new[]{ "¡Receta nueva!", "¡Huele increíble!", "¡Esto es arte!" } },
        { "recipe_fail",  new[]{ "Hmm... algo faltó", "¿Quizás más calor?", "Prueba otra vez" } },
        { "level_up",     new[]{ "¡Lo lograste!", "¡Eres increíble!", "¡Sube más!" } },
        { "night_start",  new[]{ "Se hace de noche...", "Ten cuidado", "Las criaturas cambian" } },
        { "zone_new",     new[]{ "¡Zona nueva!", "¡Qué lugar tan raro!", "¡Exploremos!" } },
        { "idle_long",    new[]{ "¿Todo bien?", "¡Vamos a cocinar!", "Tengo hambre... bueno" } },
        { "affinity_up",  new[]{ "¡Me alegra estar contigo!", "Somos un buen equipo", "No te dejaré solo" } }
    };

    private Vector3 _bobOrigin;
    private bool _isReacting;

    // ─── UNITY ───────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _bobOrigin = transform.localPosition;
        LoadAffinity();
        SetColor(ColorNeutral);
    }

    void Update()
    {
        FollowPlayer();
        Bob();
        DetectNearbyRarities();
    }

    // ─── MOVIMIENTO ───────────────────────────────────────────────────────────
    void FollowPlayer()
    {
        if (playerTransform == null) return;
        Vector3 target = playerTransform.position + Vector3.left * followDistance + Vector3.up * 0.8f;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSpeed);
    }

    void Bob()
    {
        float y = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.localPosition = new Vector3(transform.localPosition.x, _bobOrigin.y + y, transform.localPosition.z);
    }

    // ─── DETECCIÓN DE RAREZA ──────────────────────────────────────────────────
    void DetectNearbyRarities()
    {
        if (Time.time - _lastReactionTime < reactionCooldown) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);
        foreach (var hit in hits)
        {
            CreatureBase c = hit.GetComponent<CreatureBase>();
            if (c != null && (int)c.rarity >= (int)ItemRarity.Rare)
            {
                React("rare_found", ColorCurious);
                return;
            }
        }
    }

    // ─── REACCIONES PÚBLICAS ──────────────────────────────────────────────────
    public void OnRecipeSuccess()   => React("recipe_new",  ColorApproval);
    public void OnRecipeFail()      => React("recipe_fail", ColorDanger);
    public void OnLevelUp()         => React("level_up",    ColorHappy);
    public void OnNightStart()      => React("night_start", ColorMystery);
    public void OnNewZone()         => React("zone_new",    ColorCurious);

    public void React(string context, Color lightColor)
    {
        if (_isReacting) return;
        _lastReactionTime = Time.time;
        StartCoroutine(DoReact(context, lightColor));
        GainAffinity(0.5f);
    }

    IEnumerator DoReact(string context, Color lightColor)
    {
        _isReacting = true;
        SetColor(lightColor);
        ShowPhrase(context);
        if (animator) animator.SetTrigger("React");
        yield return new WaitForSeconds(2f);
        SetColor(ColorNeutral);
        _isReacting = false;
    }

    // ─── COLOR DE LUZ ─────────────────────────────────────────────────────────
    void SetColor(Color c)
    {
        if (pointLight) { pointLight.color = c; pointLight.intensity = 1.2f; }
        if (spriteRenderer) spriteRenderer.color = c;
    }

    // ─── FRASE FLOTANTE ───────────────────────────────────────────────────────
    void ShowPhrase(string context)
    {
        if (!_phrases.ContainsKey(context)) return;
        string[] options = _phrases[context];
        string phrase = options[Random.Range(0, options.Length)];
        // Instanciar FloatingText si existe en el pool global
        if (FloatingTextPool.Instance != null)
            FloatingTextPool.Instance.Spawn(phrase, floatingTextAnchor ? floatingTextAnchor.position : transform.position, Color.white);
    }

    // ─── AFINIDAD ─────────────────────────────────────────────────────────────
    public void GainAffinity(float amount)
    {
        float prev = affinityLevel;
        affinityLevel = Mathf.Clamp(affinityLevel + amount, 0, 100);
        if ((int)(prev / 25) < affinityTier)
        {
            React("affinity_up", ColorHappy);
            GameManager.Instance?.AchievementManager?.UnlockAchievement("nimbus_bond_" + affinityTier);
        }
        SaveAffinity();
    }

    // ─── PERSISTENCIA ────────────────────────────────────────────────────────
    void SaveAffinity()  => PlayerPrefs.SetFloat("NimbusAffinity", affinityLevel);
    void LoadAffinity()  => affinityLevel = PlayerPrefs.GetFloat("NimbusAffinity", 0f);

    // ─── IDLE LARGO ──────────────────────────────────────────────────────────
    private float _idleTimer;
    void OnEnable() => InvokeRepeating(nameof(CheckIdle), 60f, 60f);
    void CheckIdle()
    {
        if (!playerTransform) return;
        if (playerTransform.GetComponent<Rigidbody2D>()?.velocity.magnitude < 0.1f)
        {
            _idleTimer += 60f;
            if (_idleTimer >= 180f) { React("idle_long", ColorHappy); _idleTimer = 0; }
        }
        else _idleTimer = 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
