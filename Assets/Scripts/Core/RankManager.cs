using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// RankManager — controla el sistema de rangos del Chef Nómada.
/// Los rangos se desbloquean por XP total acumulado.
/// </summary>
[System.Serializable]
public class ChefRank
{
    public string rankName;
    public string title;
    public int xpRequired;
    public string description;
    public string rewardDescription;
    public Color rankColor = Color.white;
}

public class RankManager : MonoBehaviour
{
    public static RankManager Instance { get; private set; }

    [Header("Configuración de Rangos")]
    public List<ChefRank> ranks = new List<ChefRank>()
    {
        new ChefRank { rankName = "aprendiz",       title = "Aprendiz Brumoso",      xpRequired = 0,     description = "Apenas empiezas tu viaje.",                     rewardDescription = "Caldero básico desbloqueado" },
        new ChefRank { rankName = "chef_viajero",   title = "Chef Viajero",          xpRequired = 500,   description = "Has cocinado tus primeros platillos en la bruma.", rewardDescription = "+1 slot de pedido activo" },
        new ChefRank { rankName = "explorador",     title = "Explorador Culinario",  xpRequired = 1500,  description = "La bruma ya no te asusta.",                      rewardDescription = "Trampa de vapor desbloqueada" },
        new ChefRank { rankName = "maestro_cebos",  title = "Maestro de Cebos",      xpRequired = 3000,  description = "Las criaturas sienten tu presencia.",             rewardDescription = "Cebo aromático Nv2 desbloqueado" },
        new ChefRank { rankName = "chef_brumoso",   title = "Chef de la Bruma",      xpRequired = 6000,  description = "Tu caravana tiene fama en todas las regiones.",    rewardDescription = "+15% valor de platillos" },
        new ChefRank { rankName = "cazador_sabores",title = "Cazador de Sabores",     xpRequired = 10000, description = "Has capturado criaturas que nadie creía reales.",  rewardDescription = "Zona Barranco del Caldero desbloqueada" },
        new ChefRank { rankName = "chef_legendario",title = "Chef Legendario",        xpRequired = 18000, description = "Tu nombre es leyenda en Aetherea.",               rewardDescription = "+1 estación de cocina simultánea" },
        new ChefRank { rankName = "guardian_velo",  title = "Guardián del Gran Velo", xpRequired = 30000, description = "Solo tú puedes reconstruir lo que fue roto.",     rewardDescription = "Acceso a la Cumbre del Gran Velo" }
    };

    [Header("Estado")]
    public int currentRankIndex = 0;

    [Header("Eventos")]
    public UnityEvent<ChefRank> onRankUp;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ChefRank CurrentRank => ranks[currentRankIndex];

    public void CheckRankUp(int totalXP)
    {
        for (int i = ranks.Count - 1; i >= 0; i--)
        {
            if (totalXP >= ranks[i].xpRequired && i > currentRankIndex)
            {
                currentRankIndex = i;
                Debug.Log($"[Rango] ¡Subiste a: {ranks[i].title}!");
                onRankUp?.Invoke(ranks[i]);
                AchievementManager.Instance?.CheckRankAchievements(currentRankIndex);
                break;
            }
        }
    }

    public int XPToNextRank(int totalXP)
    {
        if (currentRankIndex >= ranks.Count - 1) return 0;
        return ranks[currentRankIndex + 1].xpRequired - totalXP;
    }

    public float RankProgress(int totalXP)
    {
        if (currentRankIndex >= ranks.Count - 1) return 1f;
        int current = ranks[currentRankIndex].xpRequired;
        int next = ranks[currentRankIndex + 1].xpRequired;
        return Mathf.Clamp01((float)(totalXP - current) / (next - current));
    }

    public void LoadFromSave(SaveData data) => currentRankIndex = data.currentRankIndex;
    public void SaveToData(SaveData data) => data.currentRankIndex = currentRankIndex;
}
