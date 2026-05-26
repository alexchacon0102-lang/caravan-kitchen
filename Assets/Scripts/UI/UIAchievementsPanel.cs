// ============================================================
// CARAVAN KITCHEN — UIAchievementsPanel.cs
// Script #30 — Fase 4
// Panel visual de todos los logros con progreso, categorías
// y animación de desbloqueo.
// Compatible con Unity 6.3 LTS
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAchievementsPanel : MonoBehaviour
{
    public static UIAchievementsPanel Instance { get; private set; }

    // ─── INSPECTOR ───────────────────────────────────────────────────────
    [Header("Panel raíz")]
    [SerializeField] private GameObject panel;

    [Header("Filtro por categoría")]
    [SerializeField] private Transform  categoryButtonParent;
    [SerializeField] private GameObject categoryButtonPrefab;

    [Header("Lista de logros")]
    [SerializeField] private Transform  achievementListParent;
    [SerializeField] private GameObject achievementCardPrefab;

    [Header("Resumen")]
    [SerializeField] private TextMeshProUGUI summaryText;   // "12 / 22 logros"
    [SerializeField] private Slider          progressBar;

    // ─── ESTADO ──────────────────────────────────────────────────────────
    private string _activeCategory = "Todos";
    private List<string> _categories = new List<string> { "Todos", "Cocina", "Captura", "Exploración", "Economía", "Colección", "Social" };

    // ─── UNITY ───────────────────────────────────────────────────────────
    private void Awake()
    {
        Instance = this;
        panel?.SetActive(false);
    }

    private void Start() => BuildCategoryButtons();

    // ─── ABRIR / CERRAR ───────────────────────────────────────────────────
    public void Open()
    {
        panel?.SetActive(true);
        Refresh();
    }

    public void Close() => panel?.SetActive(false);

    public void Toggle()
    {
        if (panel == null) return;
        if (panel.activeSelf) Close(); else Open();
    }

    // ─── BOTONES DE CATEGORÍA ─────────────────────────────────────────────
    private void BuildCategoryButtons()
    {
        if (categoryButtonParent == null || categoryButtonPrefab == null) return;
        foreach (var cat in _categories)
        {
            var btn = Instantiate(categoryButtonPrefab, categoryButtonParent);
            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = cat;
            string c = cat;
            btn.GetComponent<Button>()?.onClick.AddListener(() => SelectCategory(c));
        }
    }

    private void SelectCategory(string category)
    {
        _activeCategory = category;
        Refresh();
    }

    // ─── REFRESH ──────────────────────────────────────────────────────────
    public void Refresh()
    {
        if (AchievementManager.Instance == null) return;

        // Limpiar lista
        foreach (Transform child in achievementListParent)
            Destroy(child.gameObject);

        var all      = AchievementManager.Instance.GetAllAchievements();
        int unlocked = 0;

        foreach (var ach in all)
        {
            bool matchCat = _activeCategory == "Todos" || ach.category == _activeCategory;
            if (!matchCat) continue;

            var card = Instantiate(achievementCardPrefab, achievementListParent);

            // Emoji
            var emojiTxt = card.transform.Find("Emoji")?.GetComponent<TextMeshProUGUI>();
            if (emojiTxt) emojiTxt.text = ach.isUnlocked ? ach.emoji : "🔒";

            // Título
            var titleTxt = card.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            if (titleTxt)
            {
                titleTxt.text  = ach.isUnlocked ? ach.title : "???";
                titleTxt.color = ach.isUnlocked ? Color.white : Color.gray;
            }

            // Descripción
            var descTxt = card.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
            if (descTxt) descTxt.text = ach.isUnlocked ? ach.description : "Completa más misiones para descubrir este logro.";

            // Barra de progreso del logro
            var bar = card.transform.Find("ProgressBar")?.GetComponent<Slider>();
            if (bar)
            {
                bar.value = ach.maxProgress > 0 ? (float)ach.currentProgress / ach.maxProgress : (ach.isUnlocked ? 1f : 0f);
            }

            // Fondo de tarjeta (bloqueado vs desbloqueado)
            var bg = card.GetComponent<Image>();
            if (bg) bg.color = ach.isUnlocked
                ? new Color(0.85f, 0.75f, 0.30f, 0.25f)   // dorado suave
                : new Color(0.30f, 0.30f, 0.30f, 0.25f);  // gris oscuro

            if (ach.isUnlocked) unlocked++;
        }

        // Resumen
        if (summaryText) summaryText.text = $"{unlocked} / {all.Count} logros desbloqueados";
        if (progressBar) progressBar.value = all.Count > 0 ? (float)unlocked / all.Count : 0f;
    }
}
