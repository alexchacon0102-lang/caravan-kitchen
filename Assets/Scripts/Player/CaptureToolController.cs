using UnityEngine;
using System.Collections;

/// <summary>
/// Caravan Kitchen — CaptureToolController.cs
/// Gestiona las herramientas de captura del jugador:
/// Red, Cebo, Trampa de Vapor, Campana, Guante Térmico, Jaula de Viento.
/// Cada herramienta tiene cooldown, durabilidad y nivel de mejora.
/// </summary>
public class CaptureToolController : MonoBehaviour
{
    // ─── ENUMS ────────────────────────────────────────────────────────────
    public enum ToolType { Net, Bait, SteamTrap, Bell, ThermalGlove, WindCage }

    [System.Serializable]
    public class CaptureTool
    {
        public ToolType type;
        public string displayName;
        public int level;           // 1–3
        public float cooldown;      // segundos entre usos
        public int maxDurability;
        public int currentDurability;
        public float captureBonus;  // % bonus de captura
        public bool isUnlocked;

        public bool IsReady => currentDurability > 0;
        public float EffectiveBonus => captureBonus * (1f + (level - 1) * 0.25f);
    }

    // ─── INSPECTOR ────────────────────────────────────────────────────────
    [Header("Herramientas")]
    public CaptureTool[] tools = new CaptureTool[]
    {
        new CaptureTool { type=ToolType.Net,          displayName="Red de Bruma",     level=1, cooldown=2f,  maxDurability=20, currentDurability=20, captureBonus=0.10f, isUnlocked=true  },
        new CaptureTool { type=ToolType.Bait,         displayName="Frasco de Cebo",   level=1, cooldown=5f,  maxDurability=10, currentDurability=10, captureBonus=0.25f, isUnlocked=true  },
        new CaptureTool { type=ToolType.SteamTrap,    displayName="Trampa de Vapor",  level=1, cooldown=8f,  maxDurability=8,  currentDurability=8,  captureBonus=0.30f, isUnlocked=false },
        new CaptureTool { type=ToolType.Bell,         displayName="Campana Sonora",   level=1, cooldown=6f,  maxDurability=15, currentDurability=15, captureBonus=0.20f, isUnlocked=false },
        new CaptureTool { type=ToolType.ThermalGlove, displayName="Guante Térmico",   level=1, cooldown=3f,  maxDurability=12, currentDurability=12, captureBonus=0.35f, isUnlocked=false },
        new CaptureTool { type=ToolType.WindCage,     displayName="Jaula de Viento",  level=1, cooldown=10f, maxDurability=6,  currentDurability=6,  captureBonus=0.40f, isUnlocked=false }
    };

    [Header("Estado")]
    public ToolType activeTool = ToolType.Net;
    private float[] cooldownTimers;

    // ─── REFERENCIAS ──────────────────────────────────────────────────────
    private PlayerInventory inventory;
    private HUDController hud;

    // ─── INICIALIZACIÓN ────────────────────────────────────────────────────
    void Awake()
    {
        cooldownTimers = new float[tools.Length];
        inventory = GetComponent<PlayerInventory>();
        hud = FindObjectOfType<HUDController>();
    }

    void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
            if (cooldownTimers[i] > 0f) cooldownTimers[i] -= Time.deltaTime;
    }

    // ─── SELECCIONAR HERRAMIENTA ───────────────────────────────────────────
    public void SelectTool(ToolType type)
    {
        activeTool = type;
        hud?.UpdateToolDisplay(type.ToString());
    }

    // ─── USAR HERRAMIENTA ──────────────────────────────────────────────────
    public bool UseTool(CreatureBase target)
    {
        int idx = (int)activeTool;
        CaptureTool tool = tools[idx];

        if (!tool.isUnlocked)    { Debug.Log($"[Capture] {tool.displayName} no desbloqueada."); return false; }
        if (!tool.IsReady)       { Debug.Log($"[Capture] {tool.displayName} sin durabilidad."); return false; }
        if (cooldownTimers[idx] > 0f) { Debug.Log($"[Capture] Cooldown activo: {cooldownTimers[idx]:0.0}s"); return false; }

        cooldownTimers[idx] = tool.cooldown;
        tool.currentDurability--;

        float successChance = CalculateCaptureChance(tool, target);
        bool captured = Random.value <= successChance;

        if (captured)
        {
            StartCoroutine(CaptureSequence(target));
        }
        else
        {
            target.OnCaptureFailed();
            hud?.ShowFloatingText("¡Escapó!", Color.red);
        }

        return captured;
    }

    // ─── CÁLCULO DE PROBABILIDAD ───────────────────────────────────────────
    private float CalculateCaptureChance(CaptureTool tool, CreatureBase creature)
    {
        float base_ = 0.5f;
        float toolBonus = tool.EffectiveBonus;
        float rarityPenalty = creature.rarityPenalty;  // definido en CreatureBase
        float nimbusBonus = 0f;  // se activa si Nimbus detectó la criatura

        float total = Mathf.Clamp01(base_ + toolBonus - rarityPenalty + nimbusBonus);
        return total;
    }

    // ─── SECUENCIA DE CAPTURA ──────────────────────────────────────────────
    private IEnumerator CaptureSequence(CreatureBase creature)
    {
        creature.OnCaptured();
        hud?.ShowFloatingText($"¡{creature.creatureName} capturado!", Color.green);
        yield return new WaitForSeconds(0.5f);
        inventory.AddCreature(creature.creatureID, creature.rarity);
        GameManager.Instance.AddXP(creature.xpReward);
        AchievementManager.Instance?.CheckCaptureAchievements(creature.creatureID);
    }

    // ─── REPARAR HERRAMIENTA ───────────────────────────────────────────────
    public void RepairTool(ToolType type)
    {
        CaptureTool tool = tools[(int)type];
        tool.currentDurability = tool.maxDurability;
        Debug.Log($"[Capture] {tool.displayName} reparada.");
    }

    // ─── MEJORAR HERRAMIENTA ───────────────────────────────────────────────
    public bool UpgradeTool(ToolType type, CurrencyManager currency)
    {
        CaptureTool tool = tools[(int)type];
        if (tool.level >= 3) { Debug.Log("Nivel máximo."); return false; }

        int cost = tool.level == 1 ? 300 : 800;
        if (!currency.SpendCoins(cost)) return false;

        tool.level++;
        tool.maxDurability += 5;
        tool.currentDurability = tool.maxDurability;
        Debug.Log($"[Capture] {tool.displayName} mejorada a nivel {tool.level}.");
        return true;
    }
}
