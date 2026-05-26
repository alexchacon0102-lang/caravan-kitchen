using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Caravan Kitchen — UIRecipeBook.cs
/// Recetario visual interactivo.
/// Muestra recetas desbloqueadas con: nombre, ingredientes, calidad estimada,
/// precio base y estado (descubierta / oculta).
/// Permite filtrar por categoría de receta.
/// </summary>
public class UIRecipeBook : MonoBehaviour
{
    // ─── REFERENCIAS UI ────────────────────────────────────────────────────
    [Header("Panel principal")]
    public GameObject recipeBookPanel;
    public Transform  recipeListParent;
    public GameObject recipeCardPrefab;

    [Header("Panel de detalle")]
    public GameObject detailPanel;
    public Image      detailDishImage;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailCategory;
    public TextMeshProUGUI detailIngredients;
    public TextMeshProUGUI detailBasePrice;
    public TextMeshProUGUI detailQuality;
    public TextMeshProUGUI detailDescription;

    [Header("Filtros")]
    public Button btnAll;
    public Button btnSoups;
    public Button btnGrilled;
    public Button btnSweets;
    public Button btnTeas;
    public Button btnLegendary;

    [Header("Contador")]
    public TextMeshProUGUI recipeCountText;

    // ─── ESTADO ──────────────────────────────────────────────────────────
    private RecipeDatabase.Recipe selectedRecipe;
    private string currentFilter = "all";
    private List<GameObject> cardInstances = new List<GameObject>();

    // ─── INICIALIZACIÓN ───────────────────────────────────────────────────
    void Start()
    {
        btnAll?.onClick.AddListener(() => SetFilter("all"));
        btnSoups?.onClick.AddListener(() => SetFilter("soup"));
        btnGrilled?.onClick.AddListener(() => SetFilter("grilled"));
        btnSweets?.onClick.AddListener(() => SetFilter("sweet"));
        btnTeas?.onClick.AddListener(() => SetFilter("tea"));
        btnLegendary?.onClick.AddListener(() => SetFilter("legendary"));

        recipeBookPanel?.SetActive(false);
        detailPanel?.SetActive(false);
    }

    // ─── ABRIR / CERRAR ───────────────────────────────────────────────────
    public void OpenRecipeBook()
    {
        recipeBookPanel?.SetActive(true);
        SetFilter("all");
    }

    public void CloseRecipeBook()
    {
        recipeBookPanel?.SetActive(false);
        detailPanel?.SetActive(false);
    }

    // ─── FILTRO ──────────────────────────────────────────────────────────
    private void SetFilter(string filter)
    {
        currentFilter = filter;
        RefreshList();
    }

    // ─── REFRESCAR LISTA ──────────────────────────────────────────────────
    private void RefreshList()
    {
        foreach (var c in cardInstances) Destroy(c);
        cardInstances.Clear();

        var db = FindObjectOfType<RecipeDatabase>();
        if (db == null) return;

        int unlocked = 0;
        int total    = db.recipes.Count;

        foreach (var recipe in db.recipes)
        {
            if (currentFilter != "all" && !recipe.category.ToLower().Contains(currentFilter)) continue;

            var card = Instantiate(recipeCardPrefab, recipeListParent);
            cardInstances.Add(card);

            var nameText = card.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            var iconImg  = card.transform.Find("Icon")?.GetComponent<Image>();
            var lockIcon = card.transform.Find("LockIcon");
            var btn      = card.GetComponent<Button>();

            if (nameText) nameText.text  = recipe.isUnlocked ? recipe.recipeName : "??????????";
            if (lockIcon) lockIcon.gameObject.SetActive(!recipe.isUnlocked);

            if (recipe.isUnlocked)
            {
                unlocked++;
                var r = recipe;
                btn?.onClick.AddListener(() => ShowDetail(r));
            }
        }

        if (recipeCountText)
            recipeCountText.text = $"📖 {unlocked} / {total} recetas";
    }

    // ─── DETALLE DE RECETA ───────────────────────────────────────────────
    private void ShowDetail(RecipeDatabase.Recipe recipe)
    {
        selectedRecipe = recipe;
        detailPanel?.SetActive(true);

        detailName.text     = recipe.recipeName;
        detailCategory.text = recipe.category;
        detailBasePrice.text = $"🪙 Precio base: {recipe.basePrice}";

        string ingList = "";
        foreach (var ing in recipe.requiredIngredients)
            ingList += $"\u2022 {ing}\n";
        detailIngredients.text = ingList;

        string qualInfo = "Calidad estimada: ";
        for (int i = 1; i <= 5; i++)
        {
            var ql = DishQualitySystem.Instance?.GetQualityData(i);
            qualInfo += ql != null ? $"{ql.emoji} " : "";
        }
        detailQuality.text = qualInfo;

        if (detailDescription) detailDescription.text = recipe.description;
    }
}
