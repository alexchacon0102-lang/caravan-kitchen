using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SeasonalEventManager — Eventos temporales y festivales
/// Maneja eventos diarios, semanales y de temporada con
/// ingredientes especiales, criaturas raras y recompensas únicas.
/// Fase 4 — Caravan Kitchen
/// </summary>
public class SeasonalEventManager : MonoBehaviour
{
    public static SeasonalEventManager Instance { get; private set; }

    // ─── Tipos ────────────────────────────────────────────
    public enum EventType { Daily, Weekly, Seasonal }
    public enum EventStatus { Inactive, Active, Completed, Missed }

    [Serializable]
    public class GameEvent
    {
        public string    id;
        public string    displayName;
        [TextArea(2,3)]
        public string    description;
        public EventType type;
        public EventStatus status = EventStatus.Inactive;

        [Header("Tiempo")]
        public float durationHours;       // duración real en horas
        public float timeRemainingSeconds;

        [Header("Contenido")]
        public string   featuredIngredient;   // id del ingrediente especial
        public string   featuredCreature;     // id de la criatura especial
        public string   featuredRecipeId;     // receta exclusiva del evento
        public float    rarityMultiplier = 1.5f;
        public float    rewardMultiplier = 1.3f;

        [Header("Recompensas al completar")]
        public int      completionXP;
        public int      completionCoins;
        public string   cosmeticReward;       // id de cosmético
        public bool     isCosmeticPremium;    // si el cosmético es de pago

        // Runtime
        public bool IsActive => status == EventStatus.Active;
        public string TimeRemainingFormatted => TimeSpan.FromSeconds(timeRemainingSeconds).ToString(@"hh\:mm\:ss");
    }

    // ─── Inspector ────────────────────────────────────────
    [Header("Eventos configurables")]
    public List<GameEvent> allEvents = new();

    [Header("Referencias")]
    public XPManager       xpManager;
    public CurrencyManager currencyManager;

    // ─── Eventos C# ─────────────────────────────────────
    public event Action<GameEvent>  OnEventStarted;
    public event Action<GameEvent>  OnEventEnded;
    public event Action<GameEvent>  OnEventCompleted;

    // ─── Unity ────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        InitializeDefaultEvents();
        LoadEventStates();
        StartDailyEvents();
    }

    void Update()
    {
        TickActiveEvents();
    }

    // ─── Tick ────────────────────────────────────────────
    void TickActiveEvents()
    {
        foreach (var ev in allEvents)
        {
            if (!ev.IsActive) continue;
            ev.timeRemainingSeconds -= Time.deltaTime;
            if (ev.timeRemainingSeconds <= 0f)
            {
                ev.status = EventStatus.Completed;
                OnEventEnded?.Invoke(ev);
                Debug.Log($"[SeasonalEvent] Evento terminado: {ev.displayName}");
            }
        }
    }

    // ─── Activación ───────────────────────────────────────
    public void StartEvent(string eventId)
    {
        var ev = allEvents.Find(e => e.id == eventId);
        if (ev == null || ev.IsActive) return;

        ev.status               = EventStatus.Active;
        ev.timeRemainingSeconds = ev.durationHours * 3600f;
        OnEventStarted?.Invoke(ev);
        Debug.Log($"[SeasonalEvent] Evento iniciado: {ev.displayName} — {ev.durationHours}h");
        SaveEventStates();
    }

    public void CompleteEvent(string eventId)
    {
        var ev = allEvents.Find(e => e.id == eventId);
        if (ev == null || !ev.IsActive) return;

        ev.status = EventStatus.Completed;
        if (xpManager       != null) xpManager.AddXP(ev.completionXP);
        if (currencyManager != null) currencyManager.AddCoins(ev.completionCoins);

        OnEventCompleted?.Invoke(ev);
        SaveEventStates();
    }

    void StartDailyEvents()
    {
        // Activa todos los eventos diarios que no hayan sido completados hoy
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        foreach (var ev in allEvents)
        {
            if (ev.type != EventType.Daily) continue;
            string lastDay = PlayerPrefs.GetString($"event_{ev.id}_lastday", "");
            if (lastDay != today)
            {
                ev.status = EventStatus.Inactive;
                PlayerPrefs.SetString($"event_{ev.id}_lastday", today);
                StartEvent(ev.id);
            }
        }
    }

    // ─── Getters ─────────────────────────────────────────
    public List<GameEvent> GetActiveEvents()
        => allEvents.FindAll(e => e.IsActive);

    public GameEvent GetEvent(string id)
        => allEvents.Find(e => e.id == id);

    public bool IsEventActive(string id)
        => allEvents.Exists(e => e.id == id && e.IsActive);

    public float GetActiveRarityMultiplier()
    {
        float mult = 1f;
        foreach (var ev in GetActiveEvents())
            mult = Mathf.Max(mult, ev.rarityMultiplier);
        return mult;
    }

    public float GetActiveRewardMultiplier()
    {
        float mult = 1f;
        foreach (var ev in GetActiveEvents())
            mult = Mathf.Max(mult, ev.rewardMultiplier);
        return mult;
    }

    // ─── Datos por defecto ───────────────────────────────
    void InitializeDefaultEvents()
    {
        if (allEvents.Count > 0) return;
        allEvents = new List<GameEvent>
        {
            new() { id="daily_rare_creature",
                    displayName="Criatura Brillante del Día",
                    description="Una criatura de rareza elevada ha aparecido en una zona aleatoria.",
                    type=EventType.Daily,      durationHours=24f,
                    rarityMultiplier=2.0f,     completionXP=50,  completionCoins=30 },

            new() { id="daily_special_order",
                    displayName="Pedido Especial del Día",
                    description="Un cliente raro pide un platillo específico con recompensa doble.",
                    type=EventType.Daily,      durationHours=24f,
                    rewardMultiplier=2.0f,     completionXP=80,  completionCoins=60 },

            new() { id="weekly_cooking_contest",
                    displayName="Concurso de Sabores",
                    description="El Mercado Suspendido celebra el concurso semanal de cocina.",
                    type=EventType.Weekly,     durationHours=168f,
                    featuredRecipeId="receta_concurso",
                    rewardMultiplier=1.5f,     completionXP=300, completionCoins=500,
                    cosmeticReward="banner_ganador_semana" },

            new() { id="weekly_rare_route",
                    displayName="Ruta Temporal Secreta",
                    description="Una ruta oculta aparece durante 7 días con criaturas épicas.",
                    type=EventType.Weekly,     durationHours=168f,
                    rarityMultiplier=2.5f,     completionXP=200, completionCoins=300 },

            new() { id="season_festival_bruma",
                    displayName="Festival de la Bruma",
                    description="Festival mensual de la Pradera. Receta ceremonial exclusiva.",
                    type=EventType.Seasonal,   durationHours=720f,
                    featuredIngredient="baya_festival",
                    featuredRecipeId="sopa_festival_bruma",
                    rarityMultiplier=1.8f,     rewardMultiplier=1.6f,
                    completionXP=800,          completionCoins=1000,
                    cosmeticReward="skin_caravana_festival", isCosmeticPremium=true },

            new() { id="season_night_creatures",
                    displayName="La Noche Viva",
                    description="Durante este evento, las criaturas nocturnas aparecen todo el día.",
                    type=EventType.Seasonal,   durationHours=336f,
                    featuredCreature="mariposa_brumosa",
                    rarityMultiplier=2.0f,     completionXP=600, completionCoins=800,
                    cosmeticReward="skin_nimbus_nocturno" }
        };
    }

    // ─── Persistencia ───────────────────────────────────
    void SaveEventStates()
    {
        foreach (var ev in allEvents)
        {
            PlayerPrefs.SetInt(   $"event_{ev.id}_status",    (int)ev.status);
            PlayerPrefs.SetFloat( $"event_{ev.id}_remaining", ev.timeRemainingSeconds);
        }
        PlayerPrefs.Save();
    }

    void LoadEventStates()
    {
        foreach (var ev in allEvents)
        {
            ev.status               = (EventStatus)PlayerPrefs.GetInt(   $"event_{ev.id}_status",    0);
            ev.timeRemainingSeconds =              PlayerPrefs.GetFloat( $"event_{ev.id}_remaining", 0f);
        }
    }
}
