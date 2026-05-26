using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Caravan Kitchen — DayNightCycle.cs
/// Ciclo día/noche que afecta: criaturas disponibles, luz ambiental,
/// música y eventos especiales nocturnos.
/// Un día completo = dayDuration segundos en tiempo real.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    // ─── ENUMS ────────────────────────────────────────────────────────────
    public enum TimeOfDay { Dawn, Morning, Afternoon, Dusk, Night, Midnight }

    // ─── INSPECTOR ────────────────────────────────────────────────────────
    [Header("Configuración")]
    [Range(60f, 1200f)] public float dayDuration = 300f;  // 5 min = 1 día
    [Range(0f, 1f)]     public float currentTime = 0.25f; // 0=medianoche, 0.5=mediodía

    [Header("Iluminación")]
    public Light2D globalLight;
    public Gradient lightColorOverDay;
    public AnimationCurve lightIntensityOverDay;

    [Header("Eventos")]
    public UnityEvent onDawn;
    public UnityEvent onNight;
    public UnityEvent onMidnight;

    // ─── ESTADO ───────────────────────────────────────────────────────────
    public TimeOfDay CurrentTimeOfDay { get; private set; }
    public int CurrentDay { get; private set; } = 1;
    public bool IsNight => CurrentTimeOfDay == TimeOfDay.Night || CurrentTimeOfDay == TimeOfDay.Midnight;
    public float NormalizedTime => currentTime;

    private TimeOfDay lastTimeOfDay;
    private bool paused = false;

    // ─── INICIALIZACIÓN ────────────────────────────────────────────────────
    void Awake() => Instance = this;

    void Start()
    {
        UpdateTimeOfDay();
        lastTimeOfDay = CurrentTimeOfDay;
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────
    void Update()
    {
        if (paused) return;

        currentTime += Time.deltaTime / dayDuration;

        if (currentTime >= 1f)
        {
            currentTime -= 1f;
            CurrentDay++;
            Debug.Log($"[DayNight] Nuevo día: {CurrentDay}");
        }

        UpdateTimeOfDay();
        UpdateLighting();
        FireTransitionEvents();
    }

    // ─── CLASIFICAR HORA ───────────────────────────────────────────────────
    private void UpdateTimeOfDay()
    {
        if      (currentTime < 0.10f) CurrentTimeOfDay = TimeOfDay.Midnight;
        else if (currentTime < 0.25f) CurrentTimeOfDay = TimeOfDay.Dawn;
        else if (currentTime < 0.50f) CurrentTimeOfDay = TimeOfDay.Morning;
        else if (currentTime < 0.70f) CurrentTimeOfDay = TimeOfDay.Afternoon;
        else if (currentTime < 0.80f) CurrentTimeOfDay = TimeOfDay.Dusk;
        else                           CurrentTimeOfDay = TimeOfDay.Night;
    }

    // ─── ILUMINACIÓN DINÁMICA ──────────────────────────────────────────────
    private void UpdateLighting()
    {
        if (globalLight == null) return;
        globalLight.color     = lightColorOverDay.Evaluate(currentTime);
        globalLight.intensity = lightIntensityOverDay.Evaluate(currentTime);
    }

    // ─── EVENTOS DE TRANSICIÓN ─────────────────────────────────────────────
    private void FireTransitionEvents()
    {
        if (lastTimeOfDay == CurrentTimeOfDay) return;

        switch (CurrentTimeOfDay)
        {
            case TimeOfDay.Dawn:     onDawn?.Invoke();     break;
            case TimeOfDay.Night:    onNight?.Invoke();    break;
            case TimeOfDay.Midnight: onMidnight?.Invoke(); break;
        }
        lastTimeOfDay = CurrentTimeOfDay;
    }

    // ─── PAUSA ────────────────────────────────────────────────────────────
    public void Pause()  => paused = true;
    public void Resume() => paused = false;

    // ─── HORA COMO STRING ─────────────────────────────────────────────────
    public string GetTimeString()
    {
        int totalMinutes = Mathf.FloorToInt(currentTime * 24f * 60f);
        int hours   = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return $"{hours:00}:{minutes:00}";
    }
}
