// ============================================================
// CARAVAN KITCHEN — SeasonalEventManager.cs
// Script #29 — Fase 4
// Festivales y eventos de temporada: modifican disponibilidad
// de ingredientes, criaturas, recetas y recompensas.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CaravanKitchen.World
{
    public class SeasonalEventManager : MonoBehaviour
    {
        public static SeasonalEventManager Instance { get; private set; }

        // ─── MODELO DE EVENTO ────────────────────────────────────────────
        [System.Serializable]
        public class SeasonalEvent
        {
            public string   eventID;
            public string   displayName;
            [TextArea]
            public string   description;
            public string   emoji;
            public float    durationDays;       // En días de juego
            public bool     isActive;
            public bool     hasBeenTriggered;

            [Header("Modificadores")]
            public float    ingredientRarityBonus;  // +X% a rareza de drops
            public float    coinRewardMultiplier;   // Multiplicador de monedas
            public List<string> exclusiveIngredients;  // IDs de ingredientes solo en este evento
            public List<string> exclusiveCreatures;    // IDs de criaturas solo en este evento
            public List<string> exclusiveRecipes;      // IDs de recetas solo en este evento
        }

        // ─── INSPECTOR ───────────────────────────────────────────────────
        [Header("Eventos de Temporada")]
        [SerializeField] private List<SeasonalEvent> events = new List<SeasonalEvent>();

        [Header("Días para ciclo de festival (días de juego)")]
        [SerializeField] private int festivalCycleDays = 28;

        [Header("Eventos Unity")]
        public UnityEvent<SeasonalEvent> onEventStarted;
        public UnityEvent<SeasonalEvent> onEventEnded;

        private SeasonalEvent _activeEvent;
        private int           _dayCounter;
        private int           _eventDayCounter;

        // ─── INIT ─────────────────────────────────────────────────────────
        private void Awake() => Instance = this;

        private void Start()
        {
            if (DayNightCycle.Instance != null)
                DayNightCycle.Instance.onDawn.AddListener(OnNewDay);
        }

        // ─── NUEVO DÍA ────────────────────────────────────────────────────
        private void OnNewDay()
        {
            _dayCounter++;
            if (_activeEvent != null)
            {
                _eventDayCounter++;
                if (_eventDayCounter >= _activeEvent.durationDays)
                    EndEvent(_activeEvent);
            }
            else if (_dayCounter % festivalCycleDays == 0)
            {
                TriggerNextEvent();
            }
        }

        // ─── INICIAR EVENTO ───────────────────────────────────────────────
        private void TriggerNextEvent()
        {
            var available = events.FindAll(e => !e.isActive);
            if (available.Count == 0)
            {
                // Reiniciar ciclo
                foreach (var e in events) e.hasBeenTriggered = false;
                available = events;
            }
            if (available.Count == 0) return;

            var next = available[Random.Range(0, available.Count)];
            StartEvent(next);
        }

        public void StartEvent(SeasonalEvent ev)
        {
            if (ev == null || ev.isActive) return;
            _activeEvent     = ev;
            ev.isActive      = true;
            ev.hasBeenTriggered = true;
            _eventDayCounter = 0;
            onEventStarted?.Invoke(ev);

            // Notificar al jugador
            FindFirstObjectByType<HUDController>()
                ?.ShowFloatingText($"{ev.emoji} ¡{ev.displayName} ha comenzado!", Color.yellow);
            CaravanKitchen.Companion.NimbusController.Instance
                ?.SayPhrase($"¡Es el {ev.displayName}!");

            Debug.Log($"[SeasonalEvent] 🎉 Evento iniciado: {ev.displayName}");
        }

        private void EndEvent(SeasonalEvent ev)
        {
            ev.isActive  = false;
            _activeEvent = null;
            onEventEnded?.Invoke(ev);
            FindFirstObjectByType<HUDController>()
                ?.ShowFloatingText($"{ev.emoji} {ev.displayName} ha terminado.", Color.gray);
            Debug.Log($"[SeasonalEvent] Evento terminado: {ev.displayName}");
        }

        // ─── CONSULTAS ────────────────────────────────────────────────────
        public bool             HasActiveEvent()              => _activeEvent != null;
        public SeasonalEvent    GetActiveEvent()              => _activeEvent;
        public float            GetCoinMultiplier()           => _activeEvent?.coinRewardMultiplier ?? 1f;
        public float            GetRarityBonus()              => _activeEvent?.ingredientRarityBonus ?? 0f;
        public bool             IsIngredientExclusive(string id)
            => _activeEvent != null && _activeEvent.exclusiveIngredients.Contains(id);
        public bool             IsCreatureExclusive(string id)
            => _activeEvent != null && _activeEvent.exclusiveCreatures.Contains(id);
    }
}
