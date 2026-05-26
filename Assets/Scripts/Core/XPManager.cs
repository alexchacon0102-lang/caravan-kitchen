using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// XPManager — gestiona experiencia del jugador y eventos de subida de nivel.
/// Usar: XPManager.Instance.AddXP(cantidad, fuente)
/// </summary>
public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    [Header("XP Actual")]
    public int currentXP = 0;
    public int totalXPEarned = 0;

    [Header("Fuentes de XP")]
    public int xpPerCapture = 10;
    public int xpPerCook = 15;
    public int xpPerOrder = 25;
    public int xpPerRareCapture = 40;
    public int xpPerLegendaryDish = 100;
    public int xpPerNewRecipe = 50;
    public int xpPerNewZone = 200;

    [Header("Eventos")]
    public UnityEvent<int, string> onXPGained;   // (cantidad, fuente)
    public UnityEvent<int> onXPThreshold;        // (xp total)

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddXP(int amount, string source = "general")
    {
        if (amount <= 0) return;
        currentXP += amount;
        totalXPEarned += amount;
        Debug.Log($"[XP] +{amount} XP de: {source} | Total: {totalXPEarned}");
        onXPGained?.Invoke(amount, source);
        onXPThreshold?.Invoke(totalXPEarned);

        RankManager.Instance?.CheckRankUp(totalXPEarned);
        AchievementManager.Instance?.CheckXPAchievements(totalXPEarned);
    }

    public void LoadFromSave(SaveData data)
    {
        totalXPEarned = data.totalXP;
        currentXP = data.totalXP;
    }

    public void SaveToData(SaveData data)
    {
        data.totalXP = totalXPEarned;
    }
}
