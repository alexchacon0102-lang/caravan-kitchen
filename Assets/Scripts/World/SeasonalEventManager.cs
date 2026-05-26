using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// SeasonalEventManager — Gestiona eventos temporales, festivales y criaturas especiales de temporada.
/// Los eventos tienen duración real en días de juego y ofrecen recetas/cosméticos únicos.
/// </summary>
[System.Serializable]
public class SeasonalEvent
{
    public string eventId;
    public string displayName;
    [TextArea] public string description;
    public int durationInGameDays;
    public int startDayOfYear;      // 1-365
    public bool isActive;
    public bool isCompleted;
    public string[] exclusiveCreatureIds;
    public string[] exclusiveRecipeIds;
    public string[] rewardCosmeticIds;
    public int bonusXPMultiplier;   // 1 = normal, 2 = doble XP
    public Color eventSkyTint;
}

public class SeasonalEventManager : MonoBehaviour
{
    public static SeasonalEventManager Instance { get; private set; }

    public List<SeasonalEvent> events = new List<SeasonalEvent>();
    public SeasonalEvent activeEvent { get; private set; }

    public event Action<SeasonalEvent> OnEventStarted;
    public event Action<SeasonalEvent> OnEventEnded;

    private int _currentGameDay = 1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitEvents();
        LoadDay();
    }

    void InitEvents()
    {
        events = new List<SeasonalEvent>
        {
            new SeasonalEvent
            {
                eventId = "festival_niebla",
                displayName = "Festival de la Niebla",
                description = "La niebla se espesa y las criaturas raras emergen. Ofrece platillos a los espíritus.",
                durationInGameDays = 7, startDayOfYear = 30,
                exclusiveCreatureIds = new[]{ "espiritu_niebla", "gran_puffshroom" },
                exclusiveRecipeIds   = new[]{ "ofrenda_de_bruma", "sopa_espectral" },
                rewardCosmeticIds    = new[]{ "skin_caravana_neblinosa" },
                bonusXPMultiplier = 2,
                eventSkyTint = new Color(0.6f, 0.7f, 0.9f)
            },
            new SeasonalEvent
            {
                eventId = "temporada_miel",
                displayName = "Temporada de la Gran Miel",
                description = "Los Abejarines producen miel legendaria. Los árboles de vapor florecen.",
                durationInGameDays = 5, startDayOfYear = 80,
                exclusiveCreatureIds = new[]{ "abejarin_dorado", "oso_miel_brumoso" },
                exclusiveRecipeIds   = new[]{ "hidromiel_legendario", "pastel_de_miel_dorada" },
                rewardCosmeticIds    = new[]{ "skin_nimbus_dorado" },
                bonusXPMultiplier = 1,
                eventSkyTint = new Color(1f, 0.9f, 0.5f)
            },
            new SeasonalEvent
            {
                eventId = "tormenta_arrecife",
                displayName = "Tormenta del Arrecife",
                description = "Una tormenta celés impulsa peces raros a la superficie. Pesca todo lo que puedas.",
                durationInGameDays = 4, startDayOfYear = 160,
                exclusiveCreatureIds = new[]{ "nubipez_tormenta", "calamar_relampago" },
                exclusiveRecipeIds   = new[]{ "caldo_de_tormenta", "sashimi_del_rayo" },
                rewardCosmeticIds    = new[]{ "skin_herramienta_tormenta" },
                bonusXPMultiplier = 2,
                eventSkyTint = new Color(0.4f, 0.5f, 0.8f)
            },
            new SeasonalEvent
            {
                eventId = "festival_caldero",
                displayName = "Festival del Gran Caldero",
                description = "La roca volcánica se enfría y revela especias milenarias. El aroma llena el mundo.",
                durationInGameDays = 6, startDayOfYear = 250,
                exclusiveCreatureIds = new[]{ "salamandra_legendaria", "topo_cristal" },
                exclusiveRecipeIds   = new[]{ "guiso_milenario", "esencia_de_caldero" },
                rewardCosmeticIds    = new[]{ "skin_caravana_volcanica" },
                bonusXPMultiplier = 1,
                eventSkyTint = new Color(0.9f, 0.5f, 0.3f)
            },
            new SeasonalEvent
            {
                eventId = "noche_abisal",
                displayName = "Noche Abisal",
                description = "Las mareas se retiran por completo. Lo que emerge de las profundidades no tiene nombre.",
                durationInGameDays = 3, startDayOfYear = 340,
                exclusiveCreatureIds = new[]{ "medusa_abismal", "ballena_sal_legendaria" },
                exclusiveRecipeIds   = new[]{ "sopa_del_abismo", "cristal_de_caldo_profundo" },
                rewardCosmeticIds    = new[]{ "skin_nimbus_abisal", "titulo_maestro_abisal" },
                bonusXPMultiplier = 2,
                eventSkyTint = new Color(0.1f, 0.1f, 0.3f)
            }
        };
    }

    // ─── AVANCE DE DÍA ────────────────────────────────────────────────────────
    /// <summary>Llamar desde DayNightCycle al completar un día.</summary>
    public void AdvanceDay()
    {
        _currentGameDay++;
        if (_currentGameDay > 365) _currentGameDay = 1;
        CheckEventTriggers();
        SaveDay();
    }

    void CheckEventTriggers()
    {
        foreach (var ev in events)
        {
            if (_currentGameDay == ev.startDayOfYear && !ev.isCompleted)
            {
                StartEvent(ev);
                return;
            }
        }
        if (activeEvent != null)
        {
            int elapsed = _currentGameDay - activeEvent.startDayOfYear;
            if (elapsed >= activeEvent.durationInGameDays)
                EndEvent(activeEvent);
        }
    }

    void StartEvent(SeasonalEvent ev)
    {
        if (activeEvent != null) EndEvent(activeEvent);
        activeEvent = ev;
        ev.isActive = true;
        OnEventStarted?.Invoke(ev);
        AudioManager.Instance?.PlaySFX("event_start");
        Debug.Log($"[SeasonalEvent] Iniciado: {ev.displayName}");
    }

    void EndEvent(SeasonalEvent ev)
    {
        ev.isActive = false;
        ev.isCompleted = true;
        OnEventEnded?.Invoke(ev);
        activeEvent = null;
        Debug.Log($"[SeasonalEvent] Terminado: {ev.displayName}");
    }

    public bool IsEventActive(string eventId) => activeEvent?.eventId == eventId;
    public int GetXPMultiplier() => activeEvent?.bonusXPMultiplier ?? 1;

    void SaveDay() => PlayerPrefs.SetInt("GameDay", _currentGameDay);
    void LoadDay() => _currentGameDay = PlayerPrefs.GetInt("GameDay", 1);
}
