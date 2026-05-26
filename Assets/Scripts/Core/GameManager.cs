// ============================================================
// CARAVAN KITCHEN — GameManager.cs
// Script #1 de 14
// PROGRESO: Core/GameManager.cs ✅ completado
// SIGUIENTE: Core/GameData.cs
// ============================================================
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CaravanKitchen.Core
{
    /// <summary>
    /// Singleton principal del juego. Controla estado global,
    /// persistencia de escena y orquesta todos los managers.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // ── Estado del juego ────────────────────────────────────
        public enum GameState { MainMenu, Exploring, InCaravan, Cooking, Paused }
        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        // ── Refs a otros managers (asignados en Inspector) ──────
        [Header("Managers")]
        public CurrencyManager currencyManager;
        public OrderManager    orderManager;
        public ZoneManager     zoneManager;

        // ── Eventos globales ────────────────────────────────────
        public static event System.Action<GameState> OnStateChanged;

        // ── Configuración general ───────────────────────────────
        [Header("Config")]
        [SerializeField] private string mainMenuScene  = "MainMenu";
        [SerializeField] private string gameScene      = "GameScene";

        // ── Singleton setup ─────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            SetState(GameState.MainMenu);
        }

        // ── Control de estado ────────────────────────────────────
        public void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
            Debug.Log($"[GameManager] Estado: {newState}");
        }

        // ── Navegación de escenas ────────────────────────────────
        public void StartNewGame()
        {
            GameData.Instance.ResetData();
            SceneManager.LoadScene(gameScene);
            SetState(GameState.InCaravan);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(mainMenuScene);
            SetState(GameState.MainMenu);
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Paused) return;
            Time.timeScale = 0f;
            SetState(GameState.Paused);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            SetState(GameState.Exploring);
        }

        // ── Salida a exploración ─────────────────────────────────
        public void GoExplore(string zoneId)
        {
            if (zoneManager != null)
                zoneManager.LoadZone(zoneId);
            SetState(GameState.Exploring);
        }

        public void ReturnToCaravan()
        {
            SetState(GameState.InCaravan);
        }
    }
}
