using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Caravan Kitchen — UIOrderCard.cs
/// Tarjeta visual de pedido en el HUD.
/// Muestra: icono de platillo, nombre, tiempo restante (barra), precio, tipo y botón de entrega.
/// La barra de tiempo cambia de color: verde > amarillo > rojo según urgencia.
/// </summary>
public class UIOrderCard : MonoBehaviour
{
    // ─── REFERENCIAS UI ─────────────────────────────────────────────────────
    [Header("Referencias UI")]
    public Image   dishIcon;
    public TextMeshProUGUI dishNameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI orderTypeText;
    public TextMeshProUGUI qualityBadgeText;
    public Slider  timerSlider;
    public Image   timerFill;
    public Button  deliverButton;
    public Button  upgradeButton;   // botón para mejorar calidad del platillo
    public Image   cardBackground;
    public GameObject completedOverlay;  // overlay cuando el pedido está listo

    [Header("Colores de timer")]
    public Color colorOk     = new Color(0.3f, 0.9f, 0.3f);
    public Color colorWarn   = new Color(1.0f, 0.8f, 0.1f);
    public Color colorUrgent = new Color(1.0f, 0.2f, 0.2f);

    [Header("Colores de tipo")]
    public Color colorNormal    = new Color(0.6f, 0.8f, 1.0f);
    public Color colorPremium   = new Color(1.0f, 0.7f, 0.1f);
    public Color colorRare      = new Color(0.8f, 0.3f, 1.0f);
    public Color colorEvent     = new Color(1.0f, 0.4f, 0.6f);
    public Color colorLegendary = new Color(1.0f, 0.9f, 0.1f);

    // ─── ESTADO INTERNO ────────────────────────────────────────────────────
    private OrderManager.Order currentOrder;
    private float maxTime;
    private float remainingTime;
    private bool isActive = false;

    // ─── INICIALIZAR TARJETA ──────────────────────────────────────────────
    public void SetOrder(OrderManager.Order order)
    {
        currentOrder  = order;
        maxTime       = order.timeLimit;
        remainingTime = order.timeLimit;
        isActive      = true;

        dishNameText.text  = order.dishName;
        priceText.text     = $"🪙 {order.basePrice}";
        orderTypeText.text = order.orderType.ToString();

        // Color del badge de tipo
        orderTypeText.color = order.orderType switch
        {
            OrderManager.OrderType.Premium   => colorPremium,
            OrderManager.OrderType.Rare      => colorRare,
            OrderManager.OrderType.Event     => colorEvent,
            OrderManager.OrderType.Legendary => colorLegendary,
            _                                => colorNormal,
        };

        completedOverlay?.SetActive(false);
        deliverButton.interactable  = false;
        upgradeButton.interactable  = true;

        deliverButton.onClick.RemoveAllListeners();
        deliverButton.onClick.AddListener(OnDeliverClicked);

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClicked);

        UpdateQualityBadge(order.qualityLevel);
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────
    void Update()
    {
        if (!isActive || currentOrder == null) return;

        remainingTime -= Time.deltaTime;
        remainingTime  = Mathf.Max(0f, remainingTime);

        float t = remainingTime / maxTime;
        timerSlider.value = t;

        // Colores progresivos del timer
        if      (t > 0.5f) timerFill.color = colorOk;
        else if (t > 0.25f) timerFill.color = colorWarn;
        else                timerFill.color = colorUrgent;

        // Parpadeo urgente
        if (t < 0.15f) timerFill.color = Time.time % 0.4f < 0.2f ? colorUrgent : Color.white;

        if (remainingTime <= 0f) OnOrderExpired();
    }

    // ─── PLATILLO LISTO ───────────────────────────────────────────────────
    public void MarkDishReady(int qualityLevel)
    {
        currentOrder.qualityLevel = qualityLevel;
        completedOverlay?.SetActive(true);
        deliverButton.interactable = true;
        UpdateQualityBadge(qualityLevel);

        // Precio actualizado con calidad
        int finalPrice = DishQualitySystem.Instance != null
            ? DishQualitySystem.Instance.CalculateFinalPrice(currentOrder.basePrice, qualityLevel)
            : currentOrder.basePrice;
        priceText.text = $"🪙 {finalPrice}";
    }

    private void UpdateQualityBadge(int level)
    {
        if (qualityBadgeText == null || DishQualitySystem.Instance == null) return;
        var ql = DishQualitySystem.Instance.GetQualityData(level);
        qualityBadgeText.text  = $"{ql.emoji} {ql.displayName}";
        qualityBadgeText.color = ql.color;
    }

    // ─── BOTONES ─────────────────────────────────────────────────────────
    private void OnDeliverClicked()
    {
        FindObjectOfType<OrderManager>()?.DeliverOrder(currentOrder.id);
        AchievementManager.Instance?.CheckOrderAchievements(currentOrder.orderType.ToString().ToLower());
        isActive = false;
        gameObject.SetActive(false);
    }

    private void OnUpgradeClicked()
    {
        // Abre panel de selección de ingrediente mejorador
        FindObjectOfType<HUDController>()?.OpenEnhancePanel(currentOrder, this);
    }

    private void OnOrderExpired()
    {
        isActive = false;
        StartCoroutine(ExpireAnimation());
    }

    private IEnumerator ExpireAnimation()
    {
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            cardBackground.color = Color.Lerp(Color.white, new Color(0.4f,0.4f,0.4f), t / 0.5f);
            yield return null;
        }
        FindObjectOfType<OrderManager>()?.ExpireOrder(currentOrder.id);
        gameObject.SetActive(false);
    }
}
