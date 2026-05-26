// ============================================================
// CARAVAN KITCHEN — CreatureAI.cs
// Script #8 de 14
// PROGRESO: Creatures/CreatureAI.cs ✅ completado
// SIGUIENTE: Cooking/CookingStation.cs
// ============================================================
using System.Collections;
using UnityEngine;

namespace CaravanKitchen.Creatures
{
    /// <summary>
    /// IA de la criatura: deambula, detecta al jugador y huye.
    /// Se combina con CreatureBase (requiere ese componente).
    /// </summary>
    [RequireComponent(typeof(CreatureBase))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CreatureAI : MonoBehaviour
    {
        public enum AIState { Idle, Roaming, Fleeing, Tired, Captured }

        // ── Estado ─────────────────────────────────────────────
        public AIState CurrentState { get; private set; } = AIState.Idle;

        // ── Refs ────────────────────────────────────────────────
        private CreatureBase _data;
        private Rigidbody2D  _rb;
        private Transform    _player;

        // ── Navegación ──────────────────────────────────────────
        private Vector2 _roamTarget;
        private Vector3 _startPos;
        private float   _idleTimer;
        private float   _idleDuration;

        private void Awake()
        {
            _data   = GetComponent<CreatureBase>();
            _rb     = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _startPos = transform.position;
        }

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
            StartCoroutine(StateMachine());
        }

        // ── Máquina de estados ─────────────────────────────────────
        private IEnumerator StateMachine()
        {
            while (true)
            {
                if (_data.IsCaptured)
                {
                    SetState(AIState.Captured);
                    yield break;
                }

                if (_data.IsTired)
                {
                    SetState(AIState.Tired);
                    _rb.linearVelocity = Vector2.zero;
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                float distToPlayer = _player != null
                    ? Vector2.Distance(transform.position, _player.position)
                    : float.MaxValue;

                if (_data.fleesOnSight && distToPlayer < _data.detectionRange)
                {
                    SetState(AIState.Fleeing);
                    Flee();
                }
                else if (CurrentState != AIState.Roaming)
                {
                    SetState(AIState.Idle);
                    yield return new WaitForSeconds(Random.Range(1f, 3f));
                    SetRoamTarget();
                    SetState(AIState.Roaming);
                }
                else
                {
                    MoveTowardsRoamTarget();
                    if (Vector2.Distance(transform.position, _roamTarget) < 0.3f)
                        SetState(AIState.Idle);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        // ── Movimiento ───────────────────────────────────────────
        private void SetRoamTarget()
        {
            float rx = _startPos.x + Random.Range(-_data.roamRadius, _data.roamRadius);
            float ry = _startPos.y + Random.Range(-_data.roamRadius, _data.roamRadius);
            _roamTarget = new Vector2(rx, ry);
        }

        private void MoveTowardsRoamTarget()
        {
            Vector2 dir = (_roamTarget - (Vector2)transform.position).normalized;
            _rb.linearVelocity = dir * _data.moveSpeed;
        }

        private void Flee()
        {
            if (_player == null) return;
            Vector2 dir = ((Vector2)transform.position - (Vector2)_player.position).normalized;
            _rb.linearVelocity = dir * (_data.moveSpeed * 2f);
        }

        private void SetState(AIState state)
        {
            if (CurrentState == state) return;
            CurrentState = state;
        }
    }
}
