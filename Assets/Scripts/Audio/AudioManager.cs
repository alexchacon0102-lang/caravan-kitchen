using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Caravan Kitchen — AudioManager.cs
/// Gestor de audio centralizado:
/// - Música amb dental dinámica por zona y hora del día
/// - SFX categorizados: captura, cocina, UI, ambiente
/// - Crossfade suave entre tracks
/// - Volumen global y por categoría
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // ─── MODELOS ──────────────────────────────────────────────────────────
    [System.Serializable]
    public class SoundEntry
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.8f, 1.2f)] public float pitchVariance = 1f;
        public bool loop = false;
    }

    [System.Serializable]
    public class MusicTrack
    {
        public string id;           // ej: "prairie_day", "forest_night"
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.6f;
    }

    // ─── INSPECTOR ────────────────────────────────────────────────────────
    [Header("Sources")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    [Header("Catalogos")]
    public List<SoundEntry>  sfxCatalog     = new List<SoundEntry>();
    public List<MusicTrack>  musicCatalog   = new List<MusicTrack>();

    [Header("Volumen global")]
    [Range(0f,1f)] public float masterVolume = 1f;
    [Range(0f,1f)] public float musicVolume  = 0.6f;
    [Range(0f,1f)] public float sfxVolume    = 0.8f;

    // ─── ESTADO ──────────────────────────────────────────────────────────
    private bool usingSourceA = true;
    private string currentTrackID = "";
    private Dictionary<string, SoundEntry>  sfxDict   = new Dictionary<string, SoundEntry>();
    private Dictionary<string, MusicTrack>  musicDict = new Dictionary<string, MusicTrack>();

    // ─── INICIALIZACIÓN ───────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var s in sfxCatalog)   sfxDict[s.id]   = s;
        foreach (var m in musicCatalog) musicDict[m.id] = m;
    }

    // ─── REPRODUCIR SFX ─────────────────────────────────────────────────
    public void PlaySFX(string id)
    {
        if (!sfxDict.TryGetValue(id, out var entry)) { Debug.LogWarning($"[Audio] SFX '{id}' no encontrado."); return; }
        sfxSource.pitch  = entry.pitchVariance + Random.Range(-0.05f, 0.05f);
        sfxSource.volume = entry.volume * sfxVolume * masterVolume;
        sfxSource.PlayOneShot(entry.clip);
    }

    // ─── CAMBIAR MÚSICA CON CROSSFADE ──────────────────────────────────────
    public void PlayMusic(string id, float fadeDuration = 1.5f)
    {
        if (id == currentTrackID) return;
        if (!musicDict.TryGetValue(id, out var track)) { Debug.LogWarning($"[Audio] Track '{id}' no encontrado."); return; }

        currentTrackID = id;
        StartCoroutine(CrossfadeMusic(track, fadeDuration));
    }

    private IEnumerator CrossfadeMusic(MusicTrack newTrack, float duration)
    {
        AudioSource outSrc = usingSourceA ? musicSourceA : musicSourceB;
        AudioSource inSrc  = usingSourceA ? musicSourceB : musicSourceA;
        usingSourceA = !usingSourceA;

        inSrc.clip   = newTrack.clip;
        inSrc.volume = 0f;
        inSrc.loop   = true;
        inSrc.Play();

        float targetVol = newTrack.volume * musicVolume * masterVolume;
        float t = 0f;
        float startVol = outSrc.volume;

        while (t < duration)
        {
            t += Time.deltaTime;
            float pct = t / duration;
            outSrc.volume = Mathf.Lerp(startVol, 0f, pct);
            inSrc.volume  = Mathf.Lerp(0f, targetVol, pct);
            yield return null;
        }

        outSrc.Stop();
    }

    // ─── MÚSICA DINÁMICA POR ZONA + HORA ──────────────────────────────────
    public void UpdateMusicForZoneAndTime(string zoneID, DayNightCycle.TimeOfDay time)
    {
        bool isNight = time == DayNightCycle.TimeOfDay.Night || time == DayNightCycle.TimeOfDay.Midnight;
        string suffix = isNight ? "_night" : "_day";
        PlayMusic(zoneID + suffix);
    }

    // ─── VOLUMEN GLOBAL ───────────────────────────────────────────────────
    public void SetMasterVolume(float v) { masterVolume = Mathf.Clamp01(v); }
    public void SetMusicVolume(float v)  { musicVolume  = Mathf.Clamp01(v); }
    public void SetSFXVolume(float v)    { sfxVolume    = Mathf.Clamp01(v); }

    // ─── SFX PREDEFINIDOS (atajos rápidos) ────────────────────────────────
    public void SFX_Capture()    => PlaySFX("sfx_capture");
    public void SFX_CookDone()   => PlaySFX("sfx_cook_done");
    public void SFX_OrderDone()  => PlaySFX("sfx_order_done");
    public void SFX_LevelUp()    => PlaySFX("sfx_levelup");
    public void SFX_Achievement()=> PlaySFX("sfx_achievement");
    public void SFX_ButtonClick()=> PlaySFX("sfx_click");
    public void SFX_Upgrade()    => PlaySFX("sfx_upgrade");
    public void SFX_Error()      => PlaySFX("sfx_error");
    public void SFX_Shiny()      => PlaySFX("sfx_shiny");
}
