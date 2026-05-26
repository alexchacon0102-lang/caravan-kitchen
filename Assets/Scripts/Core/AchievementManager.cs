using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// AchievementManager — sistema de logros del juego.
/// Los logros se desbloquean automáticamente al cumplir condiciones.
/// </summary>
[System.Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public string icon;          // nombre del sprite
    public bool isUnlocked = false;
    public int xpReward = 0;
    public int coinsReward = 0;
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [Header("Logros")]
    public List<Achievement> achievements = new List<Achievement>()
    {
        // Cocina
        new Achievement { id="first_cook",         title="¡Primera Llama!",        description="Cocina tu primer platillo.",                    icon="icon_cook",     xpReward=20,  coinsReward=10 },
        new Achievement { id="cook_10",             title="Cocinero Constante",     description="Cocina 10 platillos.",                         icon="icon_cook",     xpReward=30,  coinsReward=25 },
        new Achievement { id="cook_50",             title="Fogón Incansable",       description="Cocina 50 platillos.",                         icon="icon_cook",     xpReward=75,  coinsReward=100 },
        new Achievement { id="cook_100",            title="Chef Profesional",       description="Cocina 100 platillos.",                        icon="icon_chef",     xpReward=150, coinsReward=250 },
        new Achievement { id="legendary_dish",      title="Sabor Legendario",       description="Cocina tu primer platillo legendario.",         icon="icon_star",     xpReward=200, coinsReward=500 },
        new Achievement { id="quality_upgrade",     title="Maestro del Detalle",    description="Mejora la calidad de un platillo por primera vez.", icon="icon_plus",  xpReward=50,  coinsReward=50 },

        // Captura
        new Achievement { id="first_capture",       title="¡Primera Captura!",      description="Captura tu primera criatura.",                  icon="icon_net",      xpReward=20,  coinsReward=10 },
        new Achievement { id="capture_10",          title="Cazador Novato",         description="Captura 10 criaturas.",                        icon="icon_net",      xpReward=40,  coinsReward=30 },
        new Achievement { id="capture_50",          title="Cazador Experto",        description="Captura 50 criaturas.",                        icon="icon_net",      xpReward=100, coinsReward=150 },
        new Achievement { id="first_rare",          title="¡Rareza!",               description="Captura una criatura rara.",                    icon="icon_rare",     xpReward=80,  coinsReward=100 },
        new Achievement { id="first_epic",          title="¡Épico!",                description="Captura una criatura épica.",                   icon="icon_epic",     xpReward=150, coinsReward=300 },
        new Achievement { id="first_shiny",         title="¡Brillante!",            description="Captura una criatura brillante.",               icon="icon_shiny",    xpReward=300, coinsReward=1000 },

        // Pedidos
        new Achievement { id="first_order",         title="Primer Servicio",        description="Completa tu primer pedido.",                    icon="icon_order",    xpReward=20,  coinsReward=10 },
        new Achievement { id="order_25",            title="Repartidor Confiable",   description="Completa 25 pedidos.",                         icon="icon_order",    xpReward=60,  coinsReward=75 },
        new Achievement { id="order_100",           title="Negocio Floreciente",    description="Completa 100 pedidos.",                        icon="icon_business", xpReward=200, coinsReward=500 },
        new Achievement { id="urgent_order",        title="¡Entrega Express!",      description="Completa un pedido urgente.",                   icon="icon_urgent",   xpReward=50,  coinsReward=75 },

        // Rangos
        new Achievement { id="rank_chef_viajero",   title="Chef Viajero",           description="Alcanza el rango Chef Viajero.",                icon="icon_rank",     xpReward=0,   coinsReward=100 },
        new Achievement { id="rank_explorador",     title="Explorador",             description="Alcanza el rango Explorador Culinario.",        icon="icon_rank",     xpReward=0,   coinsReward=200 },
        new Achievement { id="rank_legendario",     title="¡Leyenda!",              description="Alcanza el rango Chef Legendario.",             icon="icon_legend",   xpReward=0,   coinsReward=1000 },

        // Caravana
        new Achievement { id="caravan_lvl2",        title="Caravana Mejorada",      description="Lleva la caravana al nivel 2.",                 icon="icon_caravan",  xpReward=50,  coinsReward=50 },
        new Achievement { id="caravan_lvl5",        title="Caravana Legendaria",    description="Lleva la caravana al nivel máximo.",            icon="icon_caravan",  xpReward=300, coinsReward=500 },

        // Exploración
        new Achievement { id="first_zone",          title="Primer Horizonte",       description="Desbloquea tu primera región nueva.",           icon="icon_map",      xpReward=100, coinsReward=50 },
        new Achievement { id="all_zones",           title="El Gran Viajero",        description="Desbloquea todas las regiones.",                icon="icon_map",      xpReward=500, coinsReward=2000 },

        // Especiales
        new Achievement { id="play_7days",          title="7 Días de Bruma",        description="Juega 7 días consecutivos.",                    icon="icon_calendar", xpReward=200, coinsReward=300 },
        new Achievement { id="nimbus_max",          title="Mejor que Nunca",        description="Llega a afinidad máxima con Nimbus.",           icon="icon_nimbus",   xpReward=250, coinsReward=400 },
    };

    public UnityEvent<Achievement> onAchievementUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Unlock(string id)
    {
        Achievement a = achievements.Find(x => x.id == id);
        if (a == null || a.isUnlocked) return;
        a.isUnlocked = true;
        Debug.Log($"[Logro] Desbloqueado: {a.title}");
        XPManager.Instance?.AddXP(a.xpReward, "logro");
        CurrencyManager.Instance?.AddCoins(a.coinsReward);
        onAchievementUnlocked?.Invoke(a);
    }

    public bool IsUnlocked(string id)
    {
        Achievement a = achievements.Find(x => x.id == id);
        return a != null && a.isUnlocked;
    }

    // Chequeos automáticos por categoría
    public void CheckCookAchievements(int totalCooked)
    {
        if (totalCooked >= 1)   Unlock("first_cook");
        if (totalCooked >= 10)  Unlock("cook_10");
        if (totalCooked >= 50)  Unlock("cook_50");
        if (totalCooked >= 100) Unlock("cook_100");
    }

    public void CheckCaptureAchievements(int totalCaptured, string rarity = "common")
    {
        if (totalCaptured >= 1)  Unlock("first_capture");
        if (totalCaptured >= 10) Unlock("capture_10");
        if (totalCaptured >= 50) Unlock("capture_50");
        if (rarity == "rare")  Unlock("first_rare");
        if (rarity == "epic")  Unlock("first_epic");
        if (rarity == "shiny") Unlock("first_shiny");
    }

    public void CheckOrderAchievements(int totalOrders, bool isUrgent = false)
    {
        if (totalOrders >= 1)   Unlock("first_order");
        if (totalOrders >= 25)  Unlock("order_25");
        if (totalOrders >= 100) Unlock("order_100");
        if (isUrgent)           Unlock("urgent_order");
    }

    public void CheckRankAchievements(int rankIndex)
    {
        if (rankIndex >= 1) Unlock("rank_chef_viajero");
        if (rankIndex >= 2) Unlock("rank_explorador");
        if (rankIndex >= 6) Unlock("rank_legendario");
    }

    public void CheckXPAchievements(int totalXP) { /* extensible */ }

    public void LoadFromSave(SaveData data)
    {
        foreach (string id in data.unlockedAchievements)
        {
            Achievement a = achievements.Find(x => x.id == id);
            if (a != null) a.isUnlocked = true;
        }
    }

    public void SaveToData(SaveData data)
    {
        data.unlockedAchievements.Clear();
        foreach (Achievement a in achievements)
            if (a.isUnlocked) data.unlockedAchievements.Add(a.id);
    }
}
