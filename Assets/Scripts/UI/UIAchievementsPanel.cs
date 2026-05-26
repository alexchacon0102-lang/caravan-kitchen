using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UIAchievementsPanel — Panel visual completo de logros.
/// Muestra logros desbloqueados, bloqueados, secretos y progreso.
/// Filtra por categoría y anima la entrada de logros nuevos.
/// </summary>
public class UIAchievementsPanel : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelRoot;
    public Button closeButton;
    public TMP_Text titleText;
    public TMP_Text progressText;         // "14 / 22 logros"

    [Header("Filtros")]
    public Button[] categoryButtons;      // Todos, Captura, Cocina, Exploración, Economía, Nimbus, Secretos
    public Color activeTabColor = new Color(1f, 0.8f, 0.3f);
    public Color inactiveTabColor = new Color(0.4f, 0.4f, 0.4f);

    [Header("Lista")]
    public Transform contentParent;       // ScrollView Content
    public GameObject achievementCardPrefab;
    public GameObject emptyLabel;

    [Header("Detalle")]
    public GameObject detailPanel;
    public Image detailIcon;
    public TMP_Text detailTitle;
    public TMP_Text detailDescription;
    public TMP_Text detailReward;
    public TMP_Text detailStatus;

    private string _activeCategory = "all";
    private List<GameObject> _cards = new List<GameObject>();

    private static readonly string[] Categories = { "all", "capture", "cooking", "exploration", "economy", "companion", "secret" };

    // ─── INICIO ──────────────────────────────────────────────────────────────
    void Start()
    {
        closeButton?.onClick.AddListener(Close);
        for (int i = 0; i < categoryButtons.Length && i < Categories.Length; i++)
        {
            int idx = i;
            categoryButtons[i].onClick.AddListener(() => SetCategory(Categories[idx]));
        }
        panelRoot.SetActive(false);
        if (AchievementManager.Instance)
            AchievementManager.Instance.OnAchievementUnlocked += OnNewAchievement;
    }

    void OnDestroy()
    {
        if (AchievementManager.Instance)
            AchievementManager.Instance.OnAchievementUnlocked -= OnNewAchievement;
    }

    // ─── ABRIR / CERRAR ──────────────────────────────────────────────────────
    public void Open()
    {
        panelRoot.SetActive(true);
        SetCategory("all");
        RefreshProgress();
        AudioManager.Instance?.PlaySFX("ui_panel_open");
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        detailPanel?.SetActive(false);
        AudioManager.Instance?.PlaySFX("ui_panel_close");
    }

    // ─── FILTROS ─────────────────────────────────────────────────────────────
    void SetCategory(string cat)
    {
        _activeCategory = cat;
        for (int i = 0; i < categoryButtons.Length && i < Categories.Length; i++)
        {
            var img = categoryButtons[i].GetComponent<Image>();
            if (img) img.color = Categories[i] == cat ? activeTabColor : inactiveTabColor;
        }
        RebuildList();
    }

    // ─── CONSTRUIR LISTA ──────────────────────────────────────────────────────
    void RebuildList()
    {
        foreach (var c in _cards) Destroy(c);
        _cards.Clear();

        if (AchievementManager.Instance == null) return;
        var all = AchievementManager.Instance.GetAllAchievements();
        int shown = 0;

        foreach (var ach in all)
        {
            if (_activeCategory != "all" && ach.category != _activeCategory) continue;
            // Secretos bloqueados: mostrar como "???"
            bool hidden = ach.isSecret && !ach.isUnlocked;
            var card = Instantiate(achievementCardPrefab, contentParent);
            SetupCard(card, ach, hidden);
            _cards.Add(card);
            shown++;
        }

        emptyLabel?.SetActive(shown == 0);
    }

    void SetupCard(GameObject card, AchievementData ach, bool hidden)
    {
        var title = card.transform.Find("Title")?.GetComponent<TMP_Text>();
        var desc  = card.transform.Find("Description")?.GetComponent<TMP_Text>();
        var icon  = card.transform.Find("Icon")?.GetComponent<Image>();
        var badge = card.transform.Find("UnlockedBadge");
        var lockIcon = card.transform.Find("LockIcon");

        if (title) title.text = hidden ? "???" : ach.displayName;
        if (desc)  desc.text  = hidden ? "Logro secreto — sigue explorando" : ach.description;
        if (badge) badge.gameObject.SetActive(ach.isUnlocked);
        if (lockIcon) lockIcon.gameObject.SetActive(!ach.isUnlocked);

        // Color de fondo según rareza
        var bg = card.GetComponent<Image>();
        if (bg) bg.color = ach.isUnlocked
            ? new Color(0.2f, 0.8f, 0.4f, 0.3f)
            : new Color(0.3f, 0.3f, 0.3f, 0.3f);

        // Click para ver detalle
        var btn = card.GetComponent<Button>();
        if (btn && !hidden) btn.onClick.AddListener(() => ShowDetail(ach));
    }

    // ─── DETALLE ─────────────────────────────────────────────────────────────
    void ShowDetail(AchievementData ach)
    {
        if (detailPanel == null) return;
        detailPanel.SetActive(true);
        if (detailTitle) detailTitle.text = ach.displayName;
        if (detailDescription) detailDescription.text = ach.description;
        if (detailReward) detailReward.text = $"Recompensa: {ach.rewardDescription}";
        if (detailStatus) detailStatus.text = ach.isUnlocked ? "✅ Desbloqueado" : "🔒 Bloqueado";
    }

    // ─── PROGRESO GLOBAL ─────────────────────────────────────────────────────
    void RefreshProgress()
    {
        if (AchievementManager.Instance == null) return;
        int total = AchievementManager.Instance.TotalAchievements;
        int done  = AchievementManager.Instance.UnlockedCount;
        if (progressText) progressText.text = $"{done} / {total} logros";
        if (titleText) titleText.text = "Logros";
    }

    // ─── NUEVO LOGRO (callback) ──────────────────────────────────────────────
    void OnNewAchievement(AchievementData ach)
    {
        if (panelRoot.activeSelf) { RebuildList(); RefreshProgress(); }
    }
}
