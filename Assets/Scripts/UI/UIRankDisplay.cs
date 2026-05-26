// ============================================================
// CARAVAN KITCHEN — UIRankDisplay.cs
// Script #32 — Fase 4
// Pantalla de progreso de rango del chef.
// Muestra rango actual, siguiente, XP necesaria y logros
// de rango desbloqueados.
// Compatible con Unity 6.3 LTS
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRankDisplay : MonoBehaviour
{
    public static UIRankDisplay Instance { get; private set; }

    // ─── INSPECTOR ───────────────────────────────────────────────────────
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Rango actual")]
    [SerializeField] private TextMeshProUGUI currentRankEmoji;
    [SerializeField] private TextMeshProUGUI currentRankName;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private Image           currentRankBadge;

    [Header("Progreso de XP")]
    [SerializeField] private Slider          xpProgressBar;
    [SerializeField] private TextMeshProUGUI xpText;         // "1250 / 2000 XP"
    [SerializeField] private TextMeshProUGUI nextRankLabel;  // "Siguiente: Chef ⭐⭐"

    [Header("Desbloqueo del rango")]
    [SerializeField] private TextMeshProUGUI unlockDescText; // Qué desbloquea el siguiente rango
    [SerializeField] private TextMeshProUGUI rankProgressText; // "Rango 3 / 8"

    [Header("Todos los rangos (visual)")]
    [SerializeField] private Transform rankListParent;
    [SerializeField] private GameObject rankRowPrefab;

    // ─── UNITY ───────────────────────────────────────────────────────────
    private void Awake()
    {
        Instance = this;
        panel?.SetActive(false);
    }

    // ─── ABRIR / CERRAR ───────────────────────────────────────────────────
    public void Open()
    {
        panel?.SetActive(true);
        Refresh();
    }

    public void Close()   => panel?.SetActive(false);
    public void Toggle()  { if (panel == null) return; if (panel.activeSelf) Close(); else Open(); }

    // ─── REFRESH ──────────────────────────────────────────────────────────
    public void Refresh()
    {
        var xp = XPManager.Instance;
        if (xp == null) return;

        var current = xp.CurrentRankData;
        var next    = xp.NextRankData;

        // Rango actual
        if (currentRankEmoji) currentRankEmoji.text = current?.emoji ?? "🎓";
        if (currentRankName)  currentRankName.text  = current?.displayName ?? "Aprendiz";
        if (currentLevelText) currentLevelText.text = $"Nivel {xp.CurrentLevel}";
        if (currentRankBadge && current != null) currentRankBadge.color = current.rankColor;

        // Barra de XP
        float progress = xp.GetXPProgressToNextLevel();
        if (xpProgressBar) xpProgressBar.value = progress;
        if (xpText) xpText.text = $"{xp.CurrentXP} / {xp.XPForNextLevel} XP";

        // Siguiente rango
        if (nextRankLabel)
            nextRankLabel.text = next != null
                ? $"Siguiente rango: {next.emoji} {next.displayName}"
                : "✅ Rango máximo alcanzado";

        if (unlockDescText)
            unlockDescText.text = next != null
                ? $"Al subir: {next.unlockDescription}"
                : "Has desbloqueado todo el contenido de rango.";

        // Posición en la cadena de rangos
        if (rankProgressText)
            rankProgressText.text = $"Rango {xp.CurrentRankIndex + 1} / {xp.TotalRanks}";

        // Tabla de rangos
        BuildRankList();
    }

    // ─── TABLA DE RANGOS ──────────────────────────────────────────────────
    private void BuildRankList()
    {
        if (rankListParent == null || rankRowPrefab == null) return;
        foreach (Transform child in rankListParent) Destroy(child.gameObject);

        var xp    = XPManager.Instance;
        if (xp == null) return;

        int currentIdx = xp.CurrentRankIndex;
        var allRanks   = xp.GetAllRanks();

        for (int i = 0; i < allRanks.Count; i++)
        {
            var rank = allRanks[i];
            var row  = Instantiate(rankRowPrefab, rankListParent);

            var emojiT = row.transform.Find("Emoji")?.GetComponent<TextMeshProUGUI>();
            var nameT  = row.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            var lockT  = row.transform.Find("Lock")?.GetComponent<TextMeshProUGUI>();

            if (emojiT) emojiT.text = rank.emoji;
            if (nameT)  nameT.text  = rank.displayName;
            if (lockT)  lockT.text  = i <= currentIdx ? "✅" : "🔒";

            var bg = row.GetComponent<Image>();
            if (bg)
            {
                if (i == currentIdx)
                    bg.color = new Color(rank.rankColor.r, rank.rankColor.g, rank.rankColor.b, 0.35f);
                else if (i < currentIdx)
                    bg.color = new Color(0.3f, 0.75f, 0.4f, 0.15f);
                else
                    bg.color = new Color(0.3f, 0.3f, 0.3f, 0.15f);
            }
        }
    }
}
