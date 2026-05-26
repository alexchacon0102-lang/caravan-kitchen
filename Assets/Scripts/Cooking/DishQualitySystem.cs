using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Caravan Kitchen — DishQualitySystem.cs
/// Sistema de calidad de platillos.
/// La calidad base depende de la rareza de ingredientes.
/// Se puede MEJORAR el platillo agregando un ingrediente extra ("Mejora de Sabor")
/// para subir la calidad 1 nivel y aumentar su precio de venta.
/// Niveles: 1=Básico | 2=Bueno | 3=Excelente | 4=Maestro | 5=Legendario
/// </summary>
public class DishQualitySystem : MonoBehaviour
{
    public static DishQualitySystem Instance { get; private set; }

    // ─── CONFIGURACIÓN DE CALIDAD ───────────────────────────────────────────
    [System.Serializable]
    public class QualityLevel
    {
        public int level;          // 1–5
        public string displayName;
        public string emoji;
        public Color color;
        public float priceMultiplier;
        public int xpReward;
    }

    public QualityLevel[] qualityLevels = new QualityLevel[]
    {
        new QualityLevel { level=1, displayName="Básico",    emoji="⚪",  color=new Color(0.7f,0.7f,0.7f), priceMultiplier=1.0f, xpReward=15  },
        new QualityLevel { level=2, displayName="Bueno",     emoji="🟢",  color=new Color(0.4f,0.9f,0.4f), priceMultiplier=1.3f, xpReward=20  },
        new QualityLevel { level=3, displayName="Excelente", emoji="🔵",  color=new Color(0.3f,0.6f,1.0f), priceMultiplier=1.7f, xpReward=35  },
        new QualityLevel { level=4, displayName="Maestro",   emoji="🟣",  color=new Color(0.7f,0.3f,1.0f), priceMultiplier=2.2f, xpReward=60  },
        new QualityLevel { level=5, displayName="Legendario",emoji="🟡",  color=new Color(1.0f,0.85f,0.1f),priceMultiplier=3.5f, xpReward=100 },
    };

    // ─── INGREDIENTES DE MEJORA ─────────────────────────────────────────────
    // Estos ingredientes pueden agregarse después de cocinar para subir 1 nivel de calidad
    public static readonly HashSet<string> EnhancerIngredients = new HashSet<string>
    {
        "sal_de_nube",      // +1 calidad cualquier sopa/caldo
        "miel_legendaria",  // +1 calidad cualquier postre/dulce
        "esencia_tibia",    // +1 calidad cualquier té/esencia
        "cristal_de_caldo", // +1 calidad cualquier guiso
        "polvo_luminoso",   // +1 calidad cualquier platillo nocturno
        "vapor_aromatico",  // +1 calidad cualquier asado/brocheta
        "vainilla_negra",   // +1 calidad cualquier postre (nivel maestro req.)
        "polen_luminoso",   // +1 calidad cualquier platillo de zona 2+
    };

    void Awake() => Instance = this;

    // ─── CALCULAR CALIDAD BASE ─────────────────────────────────────────────
    /// <summary>
    /// Calcula la calidad base de un platillo según las rarezas de ingredientes usados.
    /// También aplica el bonus de la mejora kitchen_quality de CaravanUpgradeManager.
    /// </summary>
    public int CalculateBaseQuality(List<string> ingredientIDs, List<int> ingredientRarities)
    {
        if (ingredientIDs == null || ingredientIDs.Count == 0) return 1;

        float avgRarity = 0f;
        foreach (int r in ingredientRarities) avgRarity += r;
        avgRarity /= ingredientRarities.Count;

        // upgradeBonus viene de kitchen_quality (1.0–1.5x)
        float upgradeBonus = CaravanUpgradeManager.Instance != null
            ? CaravanUpgradeManager.Instance.GetUpgradeValue("kitchen_quality")
            : 1f;

        // Escala: raridad 0–4 → calidad 1–5
        int quality = Mathf.Clamp(Mathf.RoundToInt((avgRarity + 1f) * upgradeBonus), 1, 5);
        return quality;
    }

    // ─── MEJORAR PLATILLO ─────────────────────────────────────────────────
    /// <summary>
    /// Intenta mejorar un platillo ya cocinado usando un ingrediente extra.
    /// Si el ingrediente es un enhancer válido para la receta, sube 1 nivel de calidad.
    /// El costo del ingrediente se consume del inventario.
    /// </summary>
    public bool TryEnhanceDish(ref int currentQuality, string enhancerID, PlayerInventory inventory)
    {
        if (currentQuality >= 5)
        {
            FindObjectOfType<HUDController>()?.ShowFloatingText("¡Calidad máxima!", Color.yellow);
            return false;
        }

        if (!EnhancerIngredients.Contains(enhancerID))
        {
            FindObjectOfType<HUDController>()?.ShowFloatingText("Ingrediente no válido para mejora", Color.red);
            return false;
        }

        if (!inventory.ConsumeIngredient(enhancerID, 1))
        {
            FindObjectOfType<HUDController>()?.ShowFloatingText("No tienes ese ingrediente", Color.red);
            return false;
        }

        currentQuality = Mathf.Min(currentQuality + 1, 5);
        QualityLevel ql = GetQualityData(currentQuality);
        FindObjectOfType<HUDController>()?.ShowFloatingText($"⬆️ Calidad: {ql.emoji} {ql.displayName}", ql.color);

        AchievementManager.Instance?.IncrementProgress("cook_upgrade");
        return true;
    }

    // ─── CALCULAR PRECIO FINAL ─────────────────────────────────────────────
    public int CalculateFinalPrice(int basePrice, int qualityLevel)
    {
        QualityLevel ql = GetQualityData(qualityLevel);
        return Mathf.RoundToInt(basePrice * ql.priceMultiplier);
    }

    // ─── OBTENER DATOS DE CALIDAD ──────────────────────────────────────────
    public QualityLevel GetQualityData(int level)
    {
        level = Mathf.Clamp(level, 1, 5);
        return qualityLevels[level - 1];
    }
}
