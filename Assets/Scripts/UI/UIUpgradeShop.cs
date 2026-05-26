using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UIUpgradeShop — Tienda visual de mejoras de caravana.
/// Muestra las 10 mejoras con costo, estado, efecto y botón de compra.
/// Conectada a CaravanUpgradeManager y CurrencyManager.
/// </summary>
public class UIUpgradeShop : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelRoot;
    public Button closeButton;
    public TMP_Text goldText;
    public TMP_Text fameText;

    [Header("Categorías")]
    public Button[] categoryButtons;
    public Color activeTabColor  = new Color(1f, 0.8f, 0.3f);
    public Color inactiveTabColor = new Color(0.35f, 0.35f, 0.35f);

    [Header("Lista")]
    public Transform contentParent;
    public GameObject upgradeCardPrefab;

    [Header("Detalle seleccionado")]
    public GameObject detailPanel;
    public TMP_Text detailName;
    public TMP_Text detailDescription;
    public TMP_Text detailEffect;
    public TMP_Text detailCost;
    public Button confirmBuyButton;
    public TMP_Text confirmBuyText;

    [Header("Feedback")]
    public TMP_Text feedbackText;
    public float feedbackDuration = 2f;

    private string _activeCategory = "all";
    private List<GameObject> _cards = new List<GameObject>();
    private UpgradeData _selectedUpgrade;

    private static readonly string[] Categories = { "all", "kitchen", "storage", "exploration", "clientele", "decoration" };
    private static readonly string[] CategoryLabels = { "Todas", "Cocina", "Almacén", "Exploración", "Clientela", "Decor" };

    // ─── UNITY ───────────────────────────────────────────────────────────────
    void Start()
    {
        closeButton?.onClick.AddListener(Close);
        SetupCategoryButtons();
        panelRoot.SetActive(false);
        detailPanel?.SetActive(false);
    }

    void SetupCategoryButtons()
    {
        for (int i = 0; i < categoryButtons.Length && i < Categories.Length; i++)
        {
            int idx = i;
            var label = categoryButtons[i].GetComponentInChildren<TMP_Text>();
            if (label && idx < CategoryLabels.Length) label.text = CategoryLabels[idx];
            categoryButtons[i].onClick.AddListener(() => SetCategory(Categories[idx]));
        }
    }

    // ─── ABRIR / CERRAR ─────────────────────────────────────────────────────
    public void Open()
    {
        panelRoot.SetActive(true);
        SetCategory("all");
        RefreshCurrency();
        AudioManager.Instance?.PlaySFX("ui_shop_open");
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        _selectedUpgrade = null;
        AudioManager.Instance?.PlaySFX("ui_panel_close");
    }

    void RefreshCurrency()
    {
        if (CurrencyManager.Instance == null) return;
        if (goldText) goldText.text = $"🪙 {CurrencyManager.Instance.Coins}";
        if (fameText)  fameText.text  = $"⭐ {CurrencyManager.Instance.Fame}";
    }

    // ─── CATEGORÍAS ──────────────────────────────────────────────────────────
    void SetCategory(string cat)
    {
        _activeCategory = cat;
        for (int i = 0; i < categoryButtons.Length && i < Categories.Length; i++)
        {
            var img = categoryButtons[i].GetComponent<Image>();
            if (img) img.color = Categories[i] == cat ? activeTabColor : inactiveTabColor;
        }
        RebuildCards();
    }

    // ─── CARDS ───────────────────────────────────────────────────────────────
    void RebuildCards()
    {
        foreach (var c in _cards) Destroy(c);
        _cards.Clear();
        if (CaravanUpgradeManager.Instance == null) return;

        foreach (var upg in CaravanUpgradeManager.Instance.upgrades)
        {
            if (_activeCategory != "all" && upg.category != _activeCategory) continue;
            var card = Instantiate(upgradeCardPrefab, contentParent);
            SetupCard(card, upg);
            _cards.Add(card);
        }
    }

    void SetupCard(GameObject card, UpgradeData upg)
    {
        card.transform.Find("Name")?.GetComponent<TMP_Text>()?.SetText(upg.displayName);
        card.transform.Find("Level")?.GetComponent<TMP_Text>()?.SetText($"Nv {upg.currentLevel}/{upg.maxLevel}");
        card.transform.Find("Effect")?.GetComponent<TMP_Text>()?.SetText(upg.GetCurrentEffectDescription());

        // Barra de progreso de nivel
        var bar = card.transform.Find("LevelBar")?.GetComponent<Slider>();
        if (bar) bar.value = upg.maxLevel > 0 ? (float)upg.currentLevel / upg.maxLevel : 0;

        // Estado
        bool maxed    = upg.currentLevel >= upg.maxLevel;
        bool canAfford = CurrencyManager.Instance != null &&
                         CurrencyManager.Instance.Coins >= upg.GetNextCost();

        var costText = card.transform.Find("Cost")?.GetComponent<TMP_Text>();
        if (costText) costText.text = maxed ? "✅ Máximo" : $"🪙 {upg.GetNextCost()}";

        var bg = card.GetComponent<Image>();
        if (bg) bg.color = maxed
            ? new Color(0.3f, 0.7f, 0.3f, 0.4f)
            : canAfford
                ? new Color(0.9f, 0.8f, 0.4f, 0.3f)
                : new Color(0.5f, 0.5f, 0.5f, 0.3f);

        var btn = card.GetComponentInChildren<Button>();
        if (btn)
        {
            btn.interactable = !maxed && canAfford;
            btn.onClick.AddListener(() => SelectUpgrade(upg));
        }
    }

    // ─── SELECCIÓN Y COMPRA ─────────────────────────────────────────────────
    void SelectUpgrade(UpgradeData upg)
    {
        _selectedUpgrade = upg;
        if (detailPanel) detailPanel.SetActive(true);
        if (detailName) detailName.text = upg.displayName;
        if (detailDescription) detailDescription.text = upg.description;
        if (detailEffect) detailEffect.text = $"Efecto siguiente: {upg.GetNextEffectDescription()}";
        if (detailCost) detailCost.text = $"Costo: {upg.GetNextCost()} monedas";

        bool maxed = upg.currentLevel >= upg.maxLevel;
        if (confirmBuyButton)
        {
            confirmBuyButton.gameObject.SetActive(!maxed);
            confirmBuyButton.onClick.RemoveAllListeners();
            confirmBuyButton.onClick.AddListener(ConfirmPurchase);
        }
        if (confirmBuyText) confirmBuyText.text = maxed ? "Máximo" : $"Mejorar (🪙 {upg.GetNextCost()})";
        AudioManager.Instance?.PlaySFX("ui_select");
    }

    void ConfirmPurchase()
    {
        if (_selectedUpgrade == null) return;
        bool success = CaravanUpgradeManager.Instance?.TryUpgrade(_selectedUpgrade.upgradeId) ?? false;
        if (success)
        {
            ShowFeedback($"⬆️ {_selectedUpgrade.displayName} mejorada!", Color.green);
            RebuildCards();
            RefreshCurrency();
            SelectUpgrade(_selectedUpgrade); // refrescar detalle
        }
        else ShowFeedback("Monedas insuficientes", Color.red);
    }

    // ─── FEEDBACK ────────────────────────────────────────────────────────────
    void ShowFeedback(string msg, Color color)
    {
        if (!feedbackText) return;
        feedbackText.text = msg;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), feedbackDuration);
    }

    void HideFeedback() { if (feedbackText) feedbackText.gameObject.SetActive(false); }
}
