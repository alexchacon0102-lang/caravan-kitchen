// ============================================================
// CARAVAN KITCHEN — CookingStation.cs
// Script #9 de 14
// PROGRESO: Cooking/CookingStation.cs ✅ completado
// SIGUIENTE: Cooking/RecipeDatabase.cs
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaravanKitchen.Core;

namespace CaravanKitchen.Cooking
{
    public enum StationType { Caldero, Parrilla, Secador, MesaDulce, Molino }

    /// <summary>
    /// Estación de cocina de la caravana.
    /// Verifica ingredientes, ejecuta recetas y produce platillos.
    /// </summary>
    public class CookingStation : MonoBehaviour
    {
        // ── Config ─────────────────────────────────────────────
        [Header("Estación")]
        public StationType stationType = StationType.Caldero;
        public string      stationId   = "caldero_1";

        [Header("Velocidad")]
        [SerializeField] private float baseCookTime = 5f;  // segundos

        [Header("UI Feedback")]
        [SerializeField] private GameObject cookingProgressBar;
        [SerializeField] private ParticleSystem cookingParticles;

        // ── Estado ─────────────────────────────────────────────
        private bool _isCooking = false;
        public bool IsCooking   => _isCooking;

        // ── Eventos ────────────────────────────────────────────
        public static event System.Action<RecipeData, string> OnDishCooked;
        public static event System.Action<float>              OnCookProgress;

        // ── Cocinar una receta ────────────────────────────────────
        public bool TryCook(RecipeData recipe)
        {
            if (_isCooking)
            {
                Debug.LogWarning("[CookingStation] Ya está cocinando.");
                return false;
            }

            if (recipe.requiredStation != stationType)
            {
                Debug.LogWarning($"[CookingStation] Receta requiere {recipe.requiredStation}, esta es {stationType}");
                return false;
            }

            if (!HasIngredients(recipe))
            {
                Debug.LogWarning("[CookingStation] Faltan ingredientes.");
                return false;
            }

            ConsumeIngredients(recipe);
            StartCoroutine(CookRoutine(recipe));
            return true;
        }

        // ── Rutina de cocción ───────────────────────────────────────
        private IEnumerator CookRoutine(RecipeData recipe)
        {
            _isCooking = true;
            if (cookingProgressBar != null) cookingProgressBar.SetActive(true);
            if (cookingParticles   != null) cookingParticles.Play();

            // Velocidad modificada por upgrade del caldero
            int upgradeLevel = GameData.Instance?.GetUpgradeLevel("velocidad") ?? 1;
            float cookTime   = baseCookTime * recipe.cookTimeMultiplier / upgradeLevel;

            float elapsed = 0f;
            while (elapsed < cookTime)
            {
                elapsed += Time.deltaTime;
                OnCookProgress?.Invoke(elapsed / cookTime);
                yield return null;
            }

            // Cocción completa
            GameData.Instance?.AddItem(recipe.resultItemId, 1);
            GameData.Instance?.fameCookery  /* += */ ;
            // Sumar fama
            if (GameData.Instance != null)
                GameData.Instance.fameCookery += recipe.fameReward;

            GameData.Instance.totalDishesCooked++;
            OnDishCooked?.Invoke(recipe, stationId);

            if (cookingProgressBar != null) cookingProgressBar.SetActive(false);
            if (cookingParticles   != null) cookingParticles.Stop();
            _isCooking = false;

            Debug.Log($"[CookingStation] ✅ Platillo listo: {recipe.resultItemName}");
        }

        // ── Verificar y consumir ingredientes ────────────────────────
        private bool HasIngredients(RecipeData recipe)
        {
            if (GameData.Instance == null) return false;
            foreach (var ing in recipe.ingredients)
            {
                if (!GameData.Instance.HasItem(ing.itemId, ing.amount))
                    return false;
            }
            return true;
        }

        private void ConsumeIngredients(RecipeData recipe)
        {
            if (GameData.Instance == null) return;
            foreach (var ing in recipe.ingredients)
                GameData.Instance.RemoveItem(ing.itemId, ing.amount);
        }
    }
}
