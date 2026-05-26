using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Caravan Kitchen — SaveSystem.cs
/// Guarda y carga toda la partida en JSON local.
/// Incluye: inventario, monedas, fama, XP, rango, logros, upgrades, recetario.
/// </summary>
public static class SaveSystem
{
    private static readonly string SavePath = Application.persistentDataPath + "/save.json";

    // ─── GUARDAR ───────────────────────────────────────────────────────────
    public static void Save(GameData data)
    {
        try
        {
            data.lastSaved = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] Partida guardada en: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Error al guardar: {e.Message}");
        }
    }

    // ─── CARGAR ───────────────────────────────────────────────────────────
    public static GameData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveSystem] No existe partida. Creando nueva.");
            return new GameData();
        }
        try
        {
            string json = File.ReadAllText(SavePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"[SaveSystem] Partida cargada. Guardada el: {data.lastSaved}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Error al cargar: {e.Message}").
            return new GameData();
        }
    }

    // ─── EXISTE PARTIDA ────────────────────────────────────────────────────
    public static bool SaveExists() => File.Exists(SavePath);

    // ─── BORRAR PARTIDA ────────────────────────────────────────────────────
    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveSystem] Partida eliminada.");
        }
    }
}
