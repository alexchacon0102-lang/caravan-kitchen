// ============================================================
// CARAVAN KITCHEN — PlayerController.cs
// Script #3 de 14
// PROGRESO: Player/PlayerController.cs ✅ completado
// SIGUIENTE: Player/PlayerInventory.cs
// ============================================================
using UnityEngine;
using UnityEngine.InputSystem;   // Input System Package requerido

namespace CaravanKitchen.Player
{
    /// <summary>
    /// Controlador del jugador (chef explorador).
    /// Maneja movimiento 2D top-down, animaciones y
    /// detección de interacciones con el mundo.
    /// Requiere: Rigidbody2D, Animator, Collider2D en el GameObject.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        // ── Config ───────────────────────────────────────────────
        [Header("Movimiento")]
        [SerializeField] private float moveSpeed       = 4f;
        [SerializeField] private float sprintMultiplier = 1.5f;

        [Header("Interacción")]
        [SerializeField] private float interactRadius  = 1.2f;
        [SerializeField] private LayerMask interactLayer;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        // ── Componentes ──────────────────────────────────────────
        private Rigidbody2D   _rb;
        private Animator      _anim;

        // ── Estado de movimiento ─────────────────────────────────
        private Vector2 _moveInput;
        private bool    _isSprinting;
        private bool    _isMoving;
        private bool    _canMove = true;

        // ── Animator hashes (performance) ────────────────────────
        private static readonly int AnimSpeed     = Animator.StringToHash("Speed");
        private static readonly int AnimMoveX     = Animator.StringToHash("MoveX");
        private static readonly int AnimMoveY     = Animator.StringToHash("MoveY");
        private static readonly int AnimInteract  = Animator.StringToHash("Interact");

        // ── Eventos ──────────────────────────────────────────────
        public static event System.Action<GameObject> OnInteractableFound;
        public static event System.Action             OnInteractableLost;

        // ── Init ─────────────────────────────────────────────────
        private void Awake()
        {
            _rb   = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _rb.gravityScale  = 0f;
            _rb.freezeRotation = true;
        }

        // ── Input System callbacks ────────────────────────────────
        // (Conectar desde PlayerInput component en Inspector)
        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext ctx)
        {
            _isSprinting = ctx.ReadValueAsButton();
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) TryInteract();
        }

        // ── Update ───────────────────────────────────────────────
        private void Update()
        {
            UpdateAnimator();
            CheckInteractables();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        // ── Movimiento ───────────────────────────────────────────
        private void MovePlayer()
        {
            if (!_canMove)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            float speed = moveSpeed * (_isSprinting ? sprintMultiplier : 1f);
            _rb.linearVelocity = _moveInput.normalized * speed;
            _isMoving = _moveInput.sqrMagnitude > 0.01f;

            // Flip sprite según dirección horizontal
            if (_moveInput.x != 0 && spriteRenderer != null)
                spriteRenderer.flipX = _moveInput.x < 0;
        }

        // ── Animaciones ──────────────────────────────────────────
        private void UpdateAnimator()
        {
            if (_anim == null) return;
            _anim.SetFloat(AnimSpeed,  _isMoving ? _moveInput.magnitude : 0f);
            if (_isMoving)
            {
                _anim.SetFloat(AnimMoveX, _moveInput.x);
                _anim.SetFloat(AnimMoveY, _moveInput.y);
            }
        }

        // ── Interacción ──────────────────────────────────────────
        private void TryInteract()
        {
            Collider2D hit = Physics2D.OverlapCircle(
                transform.position, interactRadius, interactLayer);

            if (hit != null)
            {
                _anim.SetTrigger(AnimInteract);
                IInteractable interactable = hit.GetComponent<IInteractable>();
                interactable?.Interact(this);
            }
        }

        private void CheckInteractables()
        {
            Collider2D hit = Physics2D.OverlapCircle(
                transform.position, interactRadius, interactLayer);

            if (hit != null) OnInteractableFound?.Invoke(hit.gameObject);
            else             OnInteractableLost?.Invoke();
        }

        // ── API pública ──────────────────────────────────────────
        public void SetCanMove(bool value) => _canMove = value;
        public bool IsMoving               => _isMoving;
        public Vector2 FacingDirection     => _moveInput.normalized;

        // ── Gizmos debug ─────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
    }

    // ── Interfaz de interacción (usada por recursos, criaturas, etc.) ──
    public interface IInteractable
    {
        void Interact(PlayerController player);
    }
}
