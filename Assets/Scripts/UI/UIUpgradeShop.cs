using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIUpgradeShop — Tienda visual de mejoras de caravana
/// Muestra todas las mejoras disponibles, sus costos, nivel actual
/// y efecto, con validación de recursos antes de comprar.
/// Fase 4 — Caravan Kitchen
/// </summary>
public class UIUpgradeShop : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────
    [Header("Panel")]
    public GameObject panelRoot;
    public Transform  upgradeGrid;
    public GameObject upgradeCardPrefab;
    public Text       txtCoinsAvailable;
    public Text       txtFameAvailable;

    [Header("Filtros de categoría")]
    public Button btnCatAll;
    public Button btnCatCocina;
    public Button btnCatAlmacen;
    public Button btnCatExploracion;
    public Button btnCatClientela;
    public Button btnCatDecoracion;

    [Header("Confirmación")]
    public GameObject confirmPanel;
    public Text       confirmText;
    public Button     confirmYes;
    public Button     confirmNo;

    [Header("Feedback")]
    public Text feedbackText;
    public float feedbackDuration = 2f;

    [Header("Referencias")]
    public CaravanUpgradeManager upgradeManager;
    public CurrencyManager       currencyManager;

    // ─── Estado ───────────────────────────────────────────
    private string _currentCategory = "All";
    private CaravanUpgradeManager.UpgradeData _pendingUpgrade;
    private Coroutine _feedbackCo;

    // ─── Unity ────────────────────────────────────────────
    void Start()
    {
        if (panelRoot    != null) panelRoot.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);
        if (feedbackText != null) feedbackText.gameObject.SetActive(false);

        SetupFilterButtons();
        if (confirmYes != null) confirmYes.onClick.AddListener(ConfirmPurchase);
        if (confirmNo  != null) confirmNo.onClick.AddListener(()  => confirmPanel?.SetActive(false));

        if (upgradeManager != null)
            upgradeManager.OnUpgradePurchased += _ => RefreshIfVisible();
    }

    // ─── Abrir / Cerrar ────────────────────────────────
    public void OpenShop()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
        _currentCategory = "All";
        RefreshShop();
    }

    public void CloseShop()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    void RefreshIfVisible()
    {
        if (panelRoot != null && panelRoot.activeSelf) RefreshShop();
    }

    // ─── Render ────────────────────────────────────────────
    public void RefreshShop()
    {
        if (upgradeManager == null || currencyManager == null) return;

        // Monedas disponibles
        if (txtCoinsAvailable != null) txtCoinsAvailable.text = $"🪙 {currencyManager.GetCoins()}";
        if (txtFameAvailable  != null) txtFameAvailable.text  = $"⭐ {currencyManager.GetFame()}";

        // Limpiar grid
        foreach (Transform child in upgradeGrid)
            Destroy(child.gameObject);

        var upgrades = upgradeManager.GetAllUpgrades();
        foreach (var upg in upgrades)
        {
            if (_currentCategory != "All" && upg.category != _currentCategory) continue;
            if (upgradeCardPrefab == null) break;

            var card = Instantiate(upgradeCardPrefab, upgradeGrid);
            PopulateCard(card, upg);
        }
    }

    void PopulateCard(GameObject card, CaravanUpgradeManager.UpgradeData upg)
    {
        bool maxLevel  = upg.currentLevel >= upg.maxLevel;
        bool canAfford = currencyManager.GetCoins() >= upg.costCoins &&
                         currencyManager.GetFame()  >= upg.costFame;

        // Nombre
        var nameT = card.transform.Find("NameText")?.GetComponent<Text>();
        if (nameT != null) nameT.text = upg.upgradeName;

        // Descripción
        var descT = card.transform.Find("DescText")?.GetComponent<Text>();
        if (descT != null) descT.text = upg.description;

        // Nivel
        var levelT = card.transform.Find("LevelText")?.GetComponent<Text>();
        if (levelT != null)
            levelT.text = maxLevel ? "MAX" : $"Nv {upg.currentLevel}/{upg.maxLevel}";

        // Costo
        var costT = card.transform.Find("CostText")?.GetComponent<Text>();
        if (costT != null)
            costT.text = maxLevel ? "---"
                : $"🪙 {upg.costCoins}" +
                  (upg.costFame > 0 ? $"  ⭐ {upg.costFame}" : "");

        // Color de costo
        if (costT != null)
            costT.color = canAfford ? Color.black : new Color(0.8f, 0.1f, 0.1f);

        // Efecto
        var effectT = card.transform.Find("EffectText")?.GetComponent<Text>();
        if (effectT != null)
            effectT.text = $"↑ {upg.effectDescription}";

        // Barra de nivel
        var fill = card.transform.Find("LevelBar/Fill")?.GetComponent<Image>();
        if (fill != null)
            fill.fillAmount = upg.maxLevel > 0 ? (float)upg.currentLevel / upg.maxLevel : 1f;

        // Botón comprar
        var btn = card.transform.Find("BuyButton")?.GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = !maxLevel && canAfford;
            var upgCopy = upg;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => RequestPurchase(upgCopy));

            var btnText = btn.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = maxLevel ? "MAX" : canAfford ? "Comprar" : "Sin fondos";
        }

        // Fondo
        var bg = card.GetComponent<Image>();
        if (bg != null)
            bg.color = maxLevel ? new Color(0.88f,1f,0.88f) :
                       canAfford ? Color.white :
                       new Color(1f, 0.94f, 0.94f);
    }

    // ─── Compra ──────────────────────────────────────────
    void RequestPurchase(CaravanUpgradeManager.UpgradeData upg)
    {
        _pendingUpgrade = upg;
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
            if (confirmText != null)
                confirmText.text = $"¿Comprar {upg.upgradeName} Nv{upg.currentLevel+1}?\n"
                    + $"Costo: 🪙{upg.costCoins}" + (upg.costFame>0 ? $"  ⭐{upg.costFame}":"")
                    + $"\n{upg.effectDescription}";
        }
        else ConfirmPurchase();
    }

    void ConfirmPurchase()
    {
        if (_pendingUpgrade == null) return;
        if (confirmPanel != null) confirmPanel.SetActive(false);

        bool success = upgradeManager.PurchaseUpgrade(_pendingUpgrade.upgradeId);
        ShowFeedback(success
            ? $"✅ {_pendingUpgrade.upgradeName} mejorada a Nv{_pendingUpgrade.currentLevel}!"
            : "❌ No se pudo completar la mejora.");
        _pendingUpgrade = null;
    }

    // ─── Feedback ────────────────────────────────────────
    void ShowFeedback(string msg)
    {
        if (feedbackText == null) return;
        if (_feedbackCo != null) StopCoroutine(_feedbackCo);
        _feedbackCo = StartCoroutine(FeedbackRoutine(msg));
    }

    System.Collections.IEnumerator FeedbackRoutine(string msg)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = msg;
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.gameObject.SetActive(false);
    }

    // ─── Filtros ─────────────────────────────────────────
    void SetupFilterButtons()
    {
        void Add(Button btn, string cat) {
            if (btn != null) btn.onClick.AddListener(() => { _currentCategory = cat; RefreshShop(); }); }

        Add(btnCatAll,        "All");
        Add(btnCatCocina,     "Cocina");
        Add(btnCatAlmacen,    "Almacenamiento");
        Add(btnCatExploracion,"Exploración");
        Add(btnCatClientela,  "Clientela");
        Add(btnCatDecoracion, "Decoración");
    }
}
