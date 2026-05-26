using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Caravan Kitchen — HUDController.cs v2.0
/// HUD completo con:
/// - Barra de XP + nivel + rango
/// - Monedas y Fama en tiempo real
/// - Herramienta activa
/// - Popups: LevelUp, RankUp, Achievement
/// - Floating text animado
/// - Panel de mejora de platillo (enhance)
/// - Barra de energia de expedicion
/// </summary>
public class HUDController : MonoBehaviour
{
    // ─── XP Y RANGO ──────────────────────────────────────────────────────
    [Header("XP y Rango")]
    public Slider           xpBar;
    public TextMeshProUGUI  levelText;
    public TextMeshProUGUI  rankNameText;
    public TextMeshProUGUI  rankEmojiText;
    public Image            rankBadgeImage;

    // ─── ECONOMÍA ────────────────────────────────────────────────────────
    [Header("Economia")]
    public TextMeshProUGUI  coinsText;
    public TextMeshProUGUI  famaText;
    public TextMeshProUGUI  energyText;

    // ─── HERRAMIENTA ACTIVA ───────────────────────────────────────────────
    [Header("Herramienta")]
    public TextMeshProUGUI  toolNameText;
    public Image            toolIconImage;

    // ─── POPUPS ──────────────────────────────────────────────────────────
    [Header("Popups")]
    public GameObject       levelUpPopup;
    public TextMeshProUGUI  levelUpText;
    public GameObject       rankUpPopup;
    public TextMeshProUGUI  rankUpTitleText;
    public TextMeshProUGUI  rankUpUnlockText;
    public Image            rankUpBadge;
    public GameObject       achievementPopup;
    public TextMeshProUGUI  achievementEmojiText;
    public TextMeshProUGUI  achievementTitleText;

    // ─── FLOATING TEXT ───────────────────────────────────────────────────
    [Header("Floating Text")]
    public GameObject floatingTextPrefab;
    public Transform  floatingTextCanvas;

    // ─── PANEL ENHANCE ───────────────────────────────────────────────────
    [Header("Panel Enhance (mejora platillo)")]
    public GameObject       enhancePanel;
    public TextMeshProUGUI  enhancePanelTitle;
    public Transform        enhanceIngredientListParent;
    public GameObject       enhanceIngredientButtonPrefab;

    // ─── HORA DEL DÍA ───────────────────────────────────────────────────
    [Header("Hora del dia")]
    public TextMeshProUGUI  timeText;
    public TextMeshProUGUI  dayText;

    // ────────────────────────────────────────────────────────────
    void Start()
    {
        levelUpPopup?.SetActive(false);
        rankUpPopup?.SetActive(false);
        achievementPopup?.SetActive(false);
        enhancePanel?.SetActive(false);
    }

    void Update()
    {
        if (DayNightCycle.Instance != null)
        {
            if (timeText) timeText.text = DayNightCycle.Instance.GetTimeString();
            if (dayText)  dayText.text  = $"Día {DayNightCycle.Instance.CurrentDay}";
        }
    }

    // ─── ACTUALIZAR XP BAR ───────────────────────────────────────────────
    public void UpdateXPBar(float progress, int level)
    {
        if (xpBar)    xpBar.value   = progress;
        if (levelText) levelText.text = $"Nv {level}";
    }

    // ─── ACTUALIZAR MONEDAS ───────────────────────────────────────────────
    public void UpdateCoins(int coins) { if (coinsText) coinsText.text = $"🪙 {coins}"; }
    public void UpdateFama(int fama)   { if (famaText)  famaText.text  = $"⭐ {fama}"; }
    public void UpdateEnergy(int cur, int max) { if (energyText) energyText.text = $"⚡ {cur}/{max}"; }

    // ─── HERRAMIENTA ───────────────────────────────────────────────────────
    public void UpdateToolDisplay(string toolName) { if (toolNameText) toolNameText.text = toolName; }

    // ─── POPUP LEVEL UP ─────────────────────────────────────────────────
    public void ShowLevelUpPopup(int newLevel)
    {
        if (levelUpPopup == null) return;
        if (levelUpText) levelUpText.text = $"¡NIVEL {newLevel}!";
        levelUpPopup.SetActive(true);
        AudioManager.Instance?.SFX_LevelUp();
        StartCoroutine(HideAfter(levelUpPopup, 2.5f));
    }

    // ─── POPUP RANK UP ──────────────────────────────────────────────────
    public void ShowRankUpPopup(XPManager.RankData rankData)
    {
        if (rankUpPopup == null) return;
        if (rankUpTitleText)  rankUpTitleText.text  = $"{rankData.emoji} {rankData.displayName}";
        if (rankUpUnlockText) rankUpUnlockText.text  = rankData.unlockDescription;
        if (rankUpBadge)      rankUpBadge.color      = rankData.rankColor;
        rankUpPopup.SetActive(true);
        AudioManager.Instance?.SFX_LevelUp();
        StartCoroutine(HideAfter(rankUpPopup, 4f));
    }

    // ─── POPUP ACHIEVEMENT ──────────────────────────────────────────────
    public void ShowAchievementPopup(string emoji, string title)
    {
        if (achievementPopup == null) return;
        if (achievementEmojiText) achievementEmojiText.text = emoji;
        if (achievementTitleText) achievementTitleText.text = title;
        achievementPopup.SetActive(true);
        AudioManager.Instance?.SFX_Achievement();
        StartCoroutine(HideAfter(achievementPopup, 3f));
    }

    // ─── FLOATING TEXT ──────────────────────────────────────────────────
    public void ShowFloatingText(string message, Color color)
    {
        if (floatingTextPrefab == null || floatingTextCanvas == null) return;
        var go  = Instantiate(floatingTextPrefab, floatingTextCanvas);
        var tmp = go.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp) { tmp.text = message; tmp.color = color; }
        StartCoroutine(FloatAndFade(go));
    }

    private IEnumerator FloatAndFade(GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        var tmp  = go.GetComponentInChildren<TextMeshProUGUI>();
        float t = 0f;
        Vector3 startPos = rect.anchoredPosition3D;

        while (t < 1.2f)
        {
            t += Time.deltaTime;
            rect.anchoredPosition3D = startPos + Vector3.up * (t * 60f);
            if (tmp) tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f - t / 1.2f);
            yield return null;
        }
        Destroy(go);
    }

    // ─── PANEL ENHANCE ───────────────────────────────────────────────────
    public void OpenEnhancePanel(OrderManager.Order order, UIOrderCard card)
    {
        if (enhancePanel == null) return;
        enhancePanel.SetActive(true);
        if (enhancePanelTitle) enhancePanelTitle.text = $"Mejorar: {order.dishName}";

        // Limpiar botones anteriores
        foreach (Transform child in enhanceIngredientListParent) Destroy(child.gameObject);

        // Generar botones por cada enhancer disponible en inventario
        var inventory = FindObjectOfType<PlayerInventory>();
        foreach (var enhancerID in DishQualitySystem.EnhancerIngredients)
        {
            if (inventory == null || !inventory.HasIngredient(enhancerID)) continue;

            var btn = Instantiate(enhanceIngredientButtonPrefab, enhanceIngredientListParent);
            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = enhancerID.Replace("_", " ");

            string id = enhancerID;
            btn.GetComponent<Button>()?.onClick.AddListener(() =>
            {
                int q = order.qualityLevel;
                if (DishQualitySystem.Instance.TryEnhanceDish(ref q, id, inventory))
                {
                    order.qualityLevel = q;
                    card.MarkDishReady(q);
                }
                CloseEnhancePanel();
            });
        }
    }

    public void CloseEnhancePanel() => enhancePanel?.SetActive(false);

    // ─── UTILIDAD: OCULTAR POPUP TRAS TIEMPO ──────────────────────────────
    private IEnumerator HideAfter(GameObject go, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        go?.SetActive(false);
    }
}
