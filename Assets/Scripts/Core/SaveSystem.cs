using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// SaveSystem — guarda y carga el estado completo del juego en JSON local.
/// Usar: SaveSystem.Save(data) / SaveSystem.Load()
/// </summary>
public static class SaveSystem
{
    private static readonly string SavePath = Application.persistentDataPath + "/caravan_save.json";

    public static void Save(SaveData data)
    {
        data.lastSaved = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveSystem] Guardado en: {SavePath}");
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveSystem] No existe guardado. Creando nuevo.");
            return new SaveData();
        }
        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"[SaveSystem] Cargado. Último guardado: {data.lastSaved}");
        return data;
    }

    public static bool HasSave() => File.Exists(SavePath);

    public static void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        Debug.Log("[SaveSystem] Guardado eliminado.");
    }
}

[System.Serializable]
public class SaveData
{
    public string lastSaved = "";

    // Progreso del jugador
    public int totalXP = 0;
    public int currentRankIndex = 0;
    public int coins = 0;
    public int fame = 0;
    public int crystals = 0;

    // Caravana
    public int caravanLevel = 1;
    public List<string> unlockedUpgrades = new List<string>();

    // Zonas
    public List<string> unlockedZones = new List<string>() { "pradera_de_bruma" };
    public string currentZone = "pradera_de_bruma";

    // Recetario
    public List<string> discoveredRecipes = new List<string>();

    // Bestiario
    public List<string> capturedCreatures = new List<string>();

    // Logros
    public List<string> unlockedAchievements = new List<string>();

    // Herramientas
    public int netLevel = 1;
    public int baitLevel = 1;
    public int trapLevel = 1;
    public int gloveLevel = 1;

    // Estadísticas generales
    public int totalDishesCooked = 0;
    public int totalCreaturesCaptured = 0;
    public int totalOrdersCompleted = 0;
    public float totalPlayTimeSeconds = 0f;
}
