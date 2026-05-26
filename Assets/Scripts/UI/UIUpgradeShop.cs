// ============================================================
// CARAVAN KITCHEN — UIUpgradeShop.cs
// Script #31 — Fase 4
// Tienda visual de mejoras de la caravana.
// Muestra tarjetas con nombre, descripción, costo y estado.
// Compatible con Unity 6.3 LTS
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUpgradeShop : MonoBehaviour
{
    public static UIUpgradeShop Instance { get; private set; }

    // ─── INSPECTOR ───────────────────────────────────────────────────────
    [Header("Panel raíz")]
    [SerializeField] private GameObject panel;

    [Header("Lista")]
    [SerializeField] private Transform  upgradeListParent;
    [SerializeField] private GameObject upgradeCardPrefab;

    [Header("Info detalle")]
    [SerializeField] private TextMeshProUGUI detailTitle;
    [SerializeField] private TextMeshProUGUI detailDesc;
    [SerializeField] private TextMeshProUGUI detailCost;
    [SerializeField] private Button          buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonText;

    // ─── ESTADO ──────────────────────────────────────────────────────────
    private CaravanUpgradeManager.UpgradeData _selectedUpgrade;

    // ─── UNITY ───────────────────────────────────────────────────────────
    private void Awake()
    {
        Instance = this;
        panel?.SetActive(false);
    }

    // ─── ABRIR / CERRAR ───────────────────────────────────────────────────
    public void Open()
    {
        panel?.SetActive(true);
        BuildList();
    }

    public void Close()   => panel?.SetActive(false);
    public void Toggle()  { if (panel == null) return; if (panel.activeSelf) Close(); else Open(); }

    // ─── CONSTRUIR LISTA ──────────────────────────────────────────────────
    private void BuildList()
    {
        if (CaravanUpgradeManager.Instance == null) return;
        foreach (Transform child in upgradeListParent) Destroy(child.gameObject);

        foreach (var upg in CaravanUpgradeManager.Instance.GetAllUpgrades())
        {
            var card = Instantiate(upgradeCardPrefab, upgradeListParent);

            var nameT = card.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (nameT) nameT.text = upg.upgradeName;

            var levelT = card.transform.Find("Level")?.GetComponent<TextMeshProUGUI>();
            if (levelT) levelT.text = upg.isPurchased ? "✅ Comprada" : $"🪙 {upg.costCoins} | ⭐ {upg.costFama}";

            var icon = card.transform.Find("Icon")?.GetComponent<Image>();
            if (icon && upg.icon != null) icon.sprite = upg.icon;

            // Fondo por estado
            var bg = card.GetComponent<Image>();
            if (bg) bg.color = upg.isPurchased
                ? new Color(0.30f, 0.75f, 0.40f, 0.25f)   // verde
                : new Color(0.85f, 0.75f, 0.30f, 0.15f);  // amarillo

            var btn = card.GetComponent<Button>();
            var captured = upg;
            btn?.onClick.AddListener(() => SelectUpgrade(captured));
        }
    }

    // ─── SELECCIONAR MEJORA ───────────────────────────────────────────────
    private void SelectUpgrade(CaravanUpgradeManager.UpgradeData upg)
    {
        _selectedUpgrade = upg;
        if (detailTitle) detailTitle.text = upg.upgradeName;
        if (detailDesc)  detailDesc.text  = upg.description;
        if (detailCost)  detailCost.text  = upg.isPurchased
            ? "Ya comprada"
            : $"Costo: 🪙 {upg.costCoins}  ⭐ {upg.costFama}";

        if (buyButton)
        {
            buyButton.interactable = !upg.isPurchased;
            if (buyButtonText) buyButtonText.text = upg.isPurchased ? "Comprada" : "Comprar mejora";
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuySelected);
        }
    }

    // ─── COMPRAR ──────────────────────────────────────────────────────────
    private void BuySelected()
    {
        if (_selectedUpgrade == null) return;
        bool success = CaravanUpgradeManager.Instance.TryPurchaseUpgrade(_selectedUpgrade.upgradeID);
        if (success)
        {
            FindFirstObjectByType<HUDController>()
                ?.ShowFloatingText($"✅ {_selectedUpgrade.upgradeName}", Color.green);
            BuildList();
            SelectUpgrade(_selectedUpgrade); // refrescar detalle
        }
        else
        {
            FindFirstObjectByType<HUDController>()
                ?.ShowFloatingText("❌ Recursos insuficientes", Color.red);
        }
    }
}
