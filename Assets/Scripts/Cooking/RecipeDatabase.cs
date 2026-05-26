// ============================================================
// CARAVAN KITCHEN — RecipeDatabase.cs
// Script #10 de 14
// PROGRESO: Cooking/RecipeDatabase.cs ✅ completado
// SIGUIENTE: Economy/OrderManager.cs
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CaravanKitchen.Cooking;

namespace CaravanKitchen.Cooking
{
    [System.Serializable]
    public class RecipeIngredient
    {
        public string itemId;
        public string itemName;
        public int    amount = 1;
    }

    [System.Serializable]
    public class RecipeData
    {
        public string       recipeId;
        public string       recipeName;
        [TextArea] public string description;
        public StationType  requiredStation;
        public RecipeIngredient[] ingredients;
        public string       resultItemId;
        public string       resultItemName;
        public int          sellPrice;
        public int          fameReward;
        [Range(0.5f, 5f)]
        public float        cookTimeMultiplier = 1f;  // 1 = normal, >1 = más lento
        public Sprite       dishSprite;
        public bool         isHidden           = false;  // Recetas ocultas que se descubren
    }

    /// <summary>
    /// Base de datos central de recetas.
    /// Singleton ScriptableObject-like usando MonoBehaviour.
    /// Las recetas se configuran en el Inspector.
    /// </summary>
    public class RecipeDatabase : MonoBehaviour
    {
        public static RecipeDatabase Instance { get; private set; }

        [Header("Recetas del juego")]
        [SerializeField] private List<RecipeData> allRecipes = new List<RecipeData>();

        private Dictionary<string, RecipeData> _recipeMap;

        // ── Singleton ──────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            BuildMap();
        }

        private void BuildMap()
        {
            _recipeMap = new Dictionary<string, RecipeData>();
            foreach (var r in allRecipes)
            {
                if (!_recipeMap.ContainsKey(r.recipeId))
                    _recipeMap[r.recipeId] = r;
                else
                    Debug.LogWarning($"[RecipeDB] ID duplicado: {r.recipeId}");
            }
            Debug.Log($"[RecipeDB] {_recipeMap.Count} recetas cargadas.");
        }

        // ── Consultas ───────────────────────────────────────────
        public RecipeData GetRecipe(string recipeId)
        {
            return _recipeMap.TryGetValue(recipeId, out var r) ? r : null;
        }

        public List<RecipeData> GetUnlockedRecipes()
        {
            var unlocked = Core.GameData.Instance?.unlockedRecipes;
            if (unlocked == null) return new List<RecipeData>();

            var result = new List<RecipeData>();
            foreach (var r in allRecipes)
            {
                if (!r.isHidden && unlocked.Contains(r.recipeId))
                    result.Add(r);
            }
            return result;
        }

        public List<RecipeData> GetRecipesByStation(StationType station)
        {
            var result = new List<RecipeData>();
            foreach (var r in allRecipes)
                if (r.requiredStation == station) result.Add(r);
            return result;
        }

        public bool IsUnlocked(string recipeId)
        {
            return Core.GameData.Instance?.unlockedRecipes.Contains(recipeId) ?? false;
        }

        public void UnlockRecipe(string recipeId)
        {
            if (Core.GameData.Instance != null)
            {
                Core.GameData.Instance.unlockedRecipes.Add(recipeId);
                Debug.Log($"[RecipeDB] Receta desbloqueada: {recipeId}");
            }
        }

        public int TotalRecipes => allRecipes.Count;
    }
}
