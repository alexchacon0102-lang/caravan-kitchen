using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Caravan Kitchen — AchievementManager.cs
/// Sistema de logros con categorías, progreso y recompensas.
/// Logros dan XP, Monedas, Fama o cosméticos al desbloquearse.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    // ─── MODELO ───────────────────────────────────────────────────────────
    public enum AchievementCategory { Captura, Cocina, Pedidos, Exploracion, Caravana, Especial }

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public string emoji;
        public AchievementCategory category;
        public int targetValue;        // meta numérica (ej: capturar 10 criaturas)
        public int currentValue;       // progreso actual
        public bool isUnlocked;
        // Recompensas
        public int rewardXP;
        public int rewardCoins;
        public int rewardFama;
        public string rewardCosmetic;  // ID de cosmético o "" si no aplica

        public float Progress => targetValue > 0 ? (float)currentValue / targetValue : 0f;
        public bool IsComplete => currentValue >= targetValue;
    }

    // ─── CATÁLOGO DE LOGROS ──────────────────────────────────────────────
    public List<Achievement> achievements = new List<Achievement>
    {
        // ── CAPTURA ──
        new Achievement { id="cap_first",       title="Primera Captura",        emoji="🐾", description="Captura tu primera criatura.",               category=AchievementCategory.Captura,     targetValue=1,   rewardXP=75,  rewardCoins=50,  rewardFama=10 },
        new Achievement { id="cap_10",          title="Caza menor",             emoji="🦅", description="Captura 10 criaturas.",                      category=AchievementCategory.Captura,     targetValue=10,  rewardXP=100, rewardCoins=100, rewardFama=20 },
        new Achievement { id="cap_50",          title="Cazador de Bruma",       emoji="🌫️", description="Captura 50 criaturas.",                     category=AchievementCategory.Captura,     targetValue=50,  rewardXP=200, rewardCoins=300, rewardFama=50 },
        new Achievement { id="cap_shiny",       title="Criatura Brillante",     emoji="✨", description="Captura una criatura brillante (Shiny).",     category=AchievementCategory.Captura,     targetValue=1,   rewardXP=150, rewardCoins=200, rewardFama=100, rewardCosmetic="skin_nimbus_gold" },
        new Achievement { id="cap_all_zone1",   title="Bestiario Zona 1",       emoji="📖", description="Captura todas las criaturas de la Pradera.",  category=AchievementCategory.Captura,     targetValue=6,   rewardXP=300, rewardCoins=500, rewardFama=80 },
        new Achievement { id="cap_nocturna",    title="Cazador Nocturno",       emoji="🌙", description="Captura 5 criaturas nocturnas.",               category=AchievementCategory.Captura,     targetValue=5,   rewardXP=120, rewardCoins=150, rewardFama=30 },
        // ── COCINA ──
        new Achievement { id="cook_first",      title="Primera Receta",         emoji="🍲", description="Cocina tu primera receta.",                   category=AchievementCategory.Cocina,      targetValue=1,   rewardXP=50,  rewardCoins=30,  rewardFama=5 },
        new Achievement { id="cook_10",         title="Cocinero de Ruta",       emoji="🧑‍🍳", description="Cocina 10 recetas distintas.",               category=AchievementCategory.Cocina,      targetValue=10,  rewardXP=150, rewardCoins=200, rewardFama=40 },
        new Achievement { id="cook_legendary",  title="Receta Legendaria",      emoji="🏆", description="Cocina una receta legendaria.",                category=AchievementCategory.Cocina,      targetValue=1,   rewardXP=250, rewardCoins=500, rewardFama=150 },
        new Achievement { id="cook_quality",    title="Maestro de Calidad",     emoji="🌟", description="Cocina 5 platillos de calidad máxima.",      category=AchievementCategory.Cocina,      targetValue=5,   rewardXP=200, rewardCoins=400, rewardFama=100, rewardCosmetic="apron_gold" },
        new Achievement { id="cook_upgrade",    title="Platillo Mejorado",      emoji="⬆️", description="Mejora un platillo con ingrediente extra.",   category=AchievementCategory.Cocina,      targetValue=3,   rewardXP=120, rewardCoins=250, rewardFama=60 },
        // ── PEDIDOS ──
        new Achievement { id="order_first",     title="Primer Pedido",          emoji="📝", description="Entrega tu primer pedido.",                   category=AchievementCategory.Pedidos,     targetValue=1,   rewardXP=50,  rewardCoins=50,  rewardFama=10 },
        new Achievement { id="order_20",        title="Repartidor de Sabores",  emoji="📦", description="Entrega 20 pedidos.",                          category=AchievementCategory.Pedidos,     targetValue=20,  rewardXP=180, rewardCoins=300, rewardFama=60 },
        new Achievement { id="order_streak5",   title="Racha de Pedidos",       emoji="🔥", description="Entrega 5 pedidos seguidos sin fallar.",       category=AchievementCategory.Pedidos,     targetValue=5,   rewardXP=150, rewardCoins=200, rewardFama=50 },
        new Achievement { id="order_premium5",  title="Servicio Premium",       emoji="💎", description="Completa 5 pedidos premium.",                 category=AchievementCategory.Pedidos,     targetValue=5,   rewardXP=200, rewardCoins=400, rewardFama=100 },
        // ── EXPLORACIÓN ──
        new Achievement { id="exp_zone2",       title="Más allá de la Bruma",   emoji="🗺️", description="Desbloquea el Bosque de Vapor Dulce.",       category=AchievementCategory.Exploracion, targetValue=1,   rewardXP=200, rewardCoins=300, rewardFama=80 },
        new Achievement { id="exp_zone4",       title="Explorador Veterano",    emoji="🧳", description="Llega al Barranco del Caldero.",               category=AchievementCategory.Exploracion, targetValue=1,   rewardXP=300, rewardCoins=600, rewardFama=150 },
        new Achievement { id="exp_expeditions", title="Viajero Incansable",     emoji="👣", description="Realiza 30 expediciones.",                     category=AchievementCategory.Exploracion, targetValue=30,  rewardXP=250, rewardCoins=500, rewardFama=100 },
        // ── CARAVANA ──
        new Achievement { id="car_upgrade5",    title="Constructor de Sueños",  emoji="🔨", description="Compra 5 mejoras de caravana.",                category=AchievementCategory.Caravana,    targetValue=5,   rewardXP=200, rewardCoins=400, rewardFama=80, rewardCosmetic="banner_bronze" },
        new Achievement { id="car_maxlevel",    title="Caravana Maestra",       emoji="🌟", description="Lleva la caravana al nivel máximo.",           category=AchievementCategory.Caravana,    targetValue=1,   rewardXP=500, rewardCoins=1000,rewardFama=300, rewardCosmetic="banner_gold" },
        // ── ESPECIALES ──
        new Achievement { id="sp_nimbus_bond",  title="El Vínculo de Nimbus",   emoji="☁️", description="Alcanza afinidad máxima con Nimbus.",          category=AchievementCategory.Especial,    targetValue=100, rewardXP=400, rewardCoins=800, rewardFama=200, rewardCosmetic="nimbus_rainbow" },
        new Achievement { id="sp_first_day30",  title="Un Mes en la Bruma",     emoji="🌞", description="Juega 30 días en el mundo.",                   category=AchievementCategory.Especial,    targetValue=30,  rewardXP=300, rewardCoins=600, rewardFama=150 },
    };

    private HUDController hud;

    void Awake()
    {
        Instance = this;
        hud = FindObjectOfType<HUDController>();
    }

    // ─── INCREMENTAR PROGRESO ─────────────────────────────────────────────
    public void IncrementProgress(string id, int amount = 1)
    {
        Achievement ach = achievements.Find(a => a.id == id);
        if (ach == null || ach.isUnlocked) return;

        ach.currentValue += amount;
        if (ach.IsComplete) UnlockAchievement(ach);
    }

    // ─── DESBLOQUEAR LOGRO ──────────────────────────────────────────────
    private void UnlockAchievement(Achievement ach)
    {
        ach.isUnlocked = true;
        hud?.ShowAchievementPopup(ach.emoji, ach.title);

        if (ach.rewardXP    > 0) XPManager.Instance?.AddXP(ach.rewardXP);
        if (ach.rewardCoins > 0) FindObjectOfType<CurrencyManager>()?.AddCoins(ach.rewardCoins);
        if (ach.rewardFama  > 0) FindObjectOfType<CurrencyManager>()?.AddFama(ach.rewardFama);

        Debug.Log($"[Achievement] Desbloqueado: {ach.title} | XP:{ach.rewardXP} Coins:{ach.rewardCoins}");
        GameManager.Instance?.SaveGame();
    }

    // ─── CHECKS ESPECÍFICOS ───────────────────────────────────────────────
    public void CheckCaptureAchievements(string creatureID)
    {
        IncrementProgress("cap_first");
        IncrementProgress("cap_10");
        IncrementProgress("cap_50");
        if (creatureID.EndsWith("_shiny")) IncrementProgress("cap_shiny");
        if (DayNightCycle.Instance != null && DayNightCycle.Instance.IsNight) IncrementProgress("cap_nocturna");
    }

    public void CheckCookAchievements(string recipeID, int qualityLevel)
    {
        IncrementProgress("cook_first");
        IncrementProgress("cook_10");
        if (recipeID.StartsWith("legendary_")) IncrementProgress("cook_legendary");
        if (qualityLevel >= 3) IncrementProgress("cook_quality");
    }

    public void CheckOrderAchievements(string orderType)
    {
        IncrementProgress("order_first");
        IncrementProgress("order_20");
        if (orderType == "premium") IncrementProgress("order_premium5");
    }

    public void CheckExpeditionAchievements()
    {
        IncrementProgress("exp_expeditions");
    }

    public void CheckUpgradeAchievements()
    {
        IncrementProgress("car_upgrade5");
    }
}
