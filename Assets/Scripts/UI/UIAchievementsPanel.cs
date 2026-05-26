using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIAchievementsPanel — Panel visual de logros
/// Muestra todos los logros, su progreso, estado y recompensas.
/// Soporta filtros por categoría y logros secretos con spoiler.
/// Fase 4 — Caravan Kitchen
/// </summary>
public class UIAchievementsPanel : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────
    [Header("Contenedor")]
    public GameObject panelRoot;
    public Transform  achievementsGrid;     // ScrollView Content
    public GameObject achievementCardPrefab;

    [Header("Filtros")]
    public Button btnFilterAll;
    public Button btnFilterCapture;
    public Button btnFilterCooking;
    public Button btnFilterExplore;
    public Button btnFilterRank;
    public Button btnFilterSecret;

    [Header("Estadísticas")]
    public Text txtTotalUnlocked;
    public Text txtTotalAchievements;
    public Text txtTotalXP;
    public Image progressBar;

    [Header("Detalle")]
    public GameObject detailPanel;
    public Image      detailIcon;
    public Text       detailName;
    public Text       detailDescription;
    public Text       detailReward;
    public Text       detailStatus;
    public Button     btnCloseDetail;

    [Header("Referencia")]
    public AchievementManager achievementManager;

    // ─── Estado ───────────────────────────────────────────
    private string _currentFilter = "All";
    private List<AchievementManager.Achievement> _cachedAchievements = new();

    // ─── Unity ────────────────────────────────────────────
    void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (detailPanel != null) detailPanel.SetActive(false);
        SetupFilterButtons();
        if (btnCloseDetail != null) btnCloseDetail.onClick.AddListener(CloseDetail);

        if (achievementManager != null)
            achievementManager.OnAchievementUnlocked += _ => RefreshIfVisible();
    }

    // ─── Abrir / Cerrar ─────────────────────────────────
    public void OpenPanel()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
        _currentFilter = "All";
        RefreshPanel();
    }

    public void ClosePanel()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    void RefreshIfVisible()
    {
        if (panelRoot != null && panelRoot.activeSelf)
            RefreshPanel();
    }

    // ─── Render ────────────────────────────────────────────
    public void RefreshPanel()
    {
        if (achievementManager == null) return;
        _cachedAchievements = achievementManager.GetAllAchievements();

        // Limpiar grid
        foreach (Transform child in achievementsGrid)
            Destroy(child.gameObject);

        // Filtrar
        var filtered = FilterAchievements(_cachedAchievements);

        // Ordenar: desbloqueados primero, luego por categoría
        filtered.Sort((a, b) =>
        {
            if (a.isUnlocked != b.isUnlocked) return a.isUnlocked ? -1 : 1;
            return string.Compare(a.category, b.category, StringComparison.OrdinalIgnoreCase);
        });

        // Instanciar tarjetas
        foreach (var ach in filtered)
        {
            if (achievementCardPrefab == null) break;
            var card = Instantiate(achievementCardPrefab, achievementsGrid);
            PopulateCard(card, ach);
        }

        UpdateStats();
    }

    void PopulateCard(GameObject card, AchievementManager.Achievement ach)
    {
        bool isSecret = ach.isSecret && !ach.isUnlocked;

        // Fondo: color según estado
        var bg = card.GetComponent<Image>();
        if (bg != null)
            bg.color = ach.isUnlocked ? new Color(0.85f,1f,0.85f)
                     : isSecret        ? new Color(0.25f,0.25f,0.35f)
                     :                   new Color(0.95f,0.95f,0.95f);

        // Nombre
        var nameText = card.transform.Find("NameText")?.GetComponent<Text>();
        if (nameText != null)
            nameText.text = isSecret ? "??? (Secreto)" : ach.name;

        // Descripción
        var descText = card.transform.Find("DescText")?.GetComponent<Text>();
        if (descText != null)
            descText.text = isSecret ? "Completa condiciones ocultas para revelar este logro."
                                     : ach.description;

        // Ícono de categoría
        var catIcon = card.transform.Find("CategoryIcon")?.GetComponent<Image>();
        if (catIcon != null) catIcon.color = CategoryColor(ach.category);

        // Barra de progreso (si aplica)
        var progressFill = card.transform.Find("ProgressBar/Fill")?.GetComponent<Image>();
        if (progressFill != null && ach.targetCount > 1)
        {
            float pct = ach.targetCount > 0 ? (float)ach.currentCount / ach.targetCount : 0f;
            progressFill.fillAmount = Mathf.Clamp01(pct);
        }

        // Texto de progreso
        var progText = card.transform.Find("ProgressText")?.GetComponent<Text>();
        if (progText != null)
            progText.text = isSecret ? "" :
                            ach.targetCount > 1 ? $"{ach.currentCount}/{ach.targetCount}" :
                            ach.isUnlocked ? "✓" : "";

        // Botón de detalle
        var btn = card.GetComponent<Button>();
        if (btn != null)
        {
            var achCopy = ach;
            btn.onClick.AddListener(() => ShowDetail(achCopy));
        }
    }

    void UpdateStats()
    {
        if (achievementManager == null) return;
        int total    = _cachedAchievements.Count;
        int unlocked = _cachedAchievements.FindAll(a => a.isUnlocked).Count;
        int xpTotal  = 0;
        _cachedAchievements.ForEach(a => { if (a.isUnlocked) xpTotal += a.rewardXP; });

        if (txtTotalUnlocked    != null) txtTotalUnlocked.text     = unlocked.ToString();
        if (txtTotalAchievements!= null) txtTotalAchievements.text = total.ToString();
        if (txtTotalXP          != null) txtTotalXP.text           = $"+{xpTotal} XP";
        if (progressBar         != null) progressBar.fillAmount    = total > 0 ? (float)unlocked / total : 0f;
    }

    // ─── Detalle ──────────────────────────────────────────
    void ShowDetail(AchievementManager.Achievement ach)
    {
        if (detailPanel == null) return;
        detailPanel.SetActive(true);

        bool isSecret = ach.isSecret && !ach.isUnlocked;
        if (detailName        != null) detailName.text        = isSecret ? "Logro Secreto" : ach.name;
        if (detailDescription != null) detailDescription.text = isSecret
            ? "Descubre las condiciones ocultas para revelar este logro."
            : ach.description;
        if (detailStatus      != null) detailStatus.text      = ach.isUnlocked ? "✅ Desbloqueado" : "🔒 Bloqueado";
        if (detailReward      != null)
            detailReward.text = isSecret ? "???"
                : $"XP: +{ach.rewardXP}  |  Monedas: +{ach.rewardCoins}  |  Fama: +{ach.rewardFame}\n"
                + (string.IsNullOrEmpty(ach.rewardCosmetic) ? "" : $"Cosmético: {ach.rewardCosmetic}");
    }

    void CloseDetail()
    {
        if (detailPanel != null) detailPanel.SetActive(false);
    }

    // ─── Filtros ──────────────────────────────────────────
    void SetupFilterButtons()
    {
        void AddFilter(Button btn, string cat) {
            if (btn != null) btn.onClick.AddListener(() => { _currentFilter = cat; RefreshPanel(); }); }

        AddFilter(btnFilterAll,     "All");
        AddFilter(btnFilterCapture, "Captura");
        AddFilter(btnFilterCooking, "Cocina");
        AddFilter(btnFilterExplore, "Exploración");
        AddFilter(btnFilterRank,    "Rango");
        AddFilter(btnFilterSecret,  "Secret");
    }

    List<AchievementManager.Achievement> FilterAchievements(
        List<AchievementManager.Achievement> list)
    {
        if (_currentFilter == "All")    return new List<AchievementManager.Achievement>(list);
        if (_currentFilter == "Secret") return list.FindAll(a => a.isSecret);
        return list.FindAll(a => a.category == _currentFilter);
    }

    Color CategoryColor(string cat) => cat switch
    {
        "Captura"      => new Color(0.20f, 0.65f, 1.00f),
        "Cocina"       => new Color(1.00f, 0.65f, 0.10f),
        "Exploración" => new Color(0.30f, 0.85f, 0.45f),
        "Rango"        => new Color(0.85f, 0.70f, 0.10f),
        "Secreto"      => new Color(0.60f, 0.30f, 0.90f),
        _              => Color.white
    };
}
