// ============================================================
// CARAVAN KITCHEN — CaptureToolController.cs
// Fase 3 — Script 2 de 8
// PROGRESO: Player/CaptureToolController.cs ✅
// SIGUIENTE: World/CaravanUpgradeManager.cs
// ============================================================
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using CaravanKitchen.Creatures;

namespace CaravanKitchen.Player
{
    public enum ToolType { Net, BaitTrap, Bell, SteamPotion, ThermalGlove }

    /// <summary>
    /// Gestiona la herramienta activa del jugador para capturar criaturas.
    /// El jugador puede cambiar de herramienta según el tipo de criatura.
    /// Cada herramienta tiene cooldown y animación propia.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class CaptureToolController : MonoBehaviour
    {
        // ── Config de herramientas ─────────────────────────────────
        [System.Serializable]
        public class ToolData
        {
            public ToolType   type;
            public string     displayName;
            public float      range          = 2f;
            public float      cooldown       = 1.5f;
            public CaptureMethod captureMethod;
            public GameObject throwPrefab;     // proyectil opcional
            public Sprite     icon;
            [TextArea] public string tooltip;
        }

        [Header("Herramientas disponibles")]
        [SerializeField] private ToolData[] tools;

        [Header("Herramienta inicial")]
        [SerializeField] private int startToolIndex = 0;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer toolSprite;
        [SerializeField] private ParticleSystem useEffect;
        [SerializeField] private Transform      throwOrigin;

        // ── Estado ─────────────────────────────────────────────
        private int      _currentIndex = 0;
        private bool     _onCooldown   = false;
        private float    _cooldownTimer = 0f;
        private ToolData _activeTool;

        // ── Eventos ────────────────────────────────────────────
        public static event System.Action<ToolData, float> OnToolUsed;    // herramienta, cooldown%
        public static event System.Action<ToolData>        OnToolChanged;

        // ── Init ────────────────────────────────────────────────
        private void Start()
        {
            if (tools == null || tools.Length == 0) return;
            _currentIndex = Mathf.Clamp(startToolIndex, 0, tools.Length - 1);
            EquipTool(_currentIndex);
        }

        private void Update()
        {
            if (_onCooldown)
            {
                _cooldownTimer -= Time.deltaTime;
                OnToolUsed?.Invoke(_activeTool, 1f - (_cooldownTimer / _activeTool.cooldown));
                if (_cooldownTimer <= 0) _onCooldown = false;
            }
        }

        // ── Input System callbacks ────────────────────────────────
        public void OnUseTool(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) TryUseTool();
        }

        public void OnNextTool(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) CycleTool(1);
        }

        public void OnPrevTool(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) CycleTool(-1);
        }

        // ── Usar herramienta ───────────────────────────────────────
        private void TryUseTool()
        {
            if (_onCooldown || _activeTool == null) return;

            // Buscar criatura en rango
            Collider2D hit = Physics2D.OverlapCircle(
                transform.position, _activeTool.range,
                LayerMask.GetMask("Creature"));

            if (hit != null)
            {
                var creature = hit.GetComponent<CreatureBase>();
                if (creature != null)
                {
                    bool captured = creature.TryCapture(_activeTool.captureMethod);
                    if (captured && Core.GameData.Instance != null)
                        Core.GameData.Instance.totalCreaturesCaught++;
                }
            }
            else
            {
                Debug.Log($"[Tool] {_activeTool.displayName} usado (sin criatura en rango)");
            }

            // Efecto visual
            if (useEffect != null) useEffect.Play();

            // Lanzar proyectil si tiene
            if (_activeTool.throwPrefab != null && throwOrigin != null)
                Instantiate(_activeTool.throwPrefab, throwOrigin.position,
                            Quaternion.identity);

            // Cooldown
            _onCooldown    = true;
            _cooldownTimer = _activeTool.cooldown;
            OnToolUsed?.Invoke(_activeTool, 0f);
        }

        // ── Cambiar herramienta ──────────────────────────────────────
        private void CycleTool(int dir)
        {
            if (tools == null || tools.Length == 0) return;
            _currentIndex = (_currentIndex + dir + tools.Length) % tools.Length;
            EquipTool(_currentIndex);
        }

        private void EquipTool(int index)
        {
            _activeTool = tools[index];
            _onCooldown = false;
            if (toolSprite != null && _activeTool.icon != null)
                toolSprite.sprite = _activeTool.icon;
            OnToolChanged?.Invoke(_activeTool);
            Debug.Log($"[Tool] Equipado: {_activeTool.displayName}");
        }

        // ── API ──────────────────────────────────────────────────
        public ToolData ActiveTool     => _activeTool;
        public bool     IsOnCooldown   => _onCooldown;
        public float    CooldownPct    => _onCooldown ? _cooldownTimer / _activeTool.cooldown : 0f;

        // ── Gizmos ───────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            if (_activeTool == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _activeTool.range);
        }
    }
}
