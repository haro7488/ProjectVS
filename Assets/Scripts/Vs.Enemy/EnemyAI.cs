using UnityEngine;

namespace Vs.Enemy
{
    /// <summary>
    /// 적 AI 행동 컴포넌트. 플레이어 추적 로직 담당.
    /// </summary>
    [RequireComponent(typeof(EnemyBase))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAI : MonoBehaviour
    {
        private EnemyBase _enemy;
        private Transform _target;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;

        private float _moveSpeed = 2f;

        [SerializeField] private float _stoppingDistance = 0.1f;

        private bool _isInitialized;

        private void Awake()
        {
            _enemy = GetComponent<EnemyBase>();
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// AI 초기화. EnemyBase.Initialize에서 호출됨.
        /// </summary>
        public void Initialize(Transform target, float moveSpeed)
        {
            _target = target;
            _moveSpeed = moveSpeed;
            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized || _enemy.IsDead || _target == null) return;

            ChaseTarget();
        }

        private void ChaseTarget()
        {
            Vector2 direction = (_target.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, _target.position);

            // 정지 거리 내에 있으면 이동 중지
            if (distance <= _stoppingDistance)
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            // 플레이어 방향으로 이동
            _rb.velocity = direction * _moveSpeed;

            // 스프라이트 방향 설정 (좌우 반전)
            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = direction.x < 0;
            }
        }

        /// <summary>
        /// AI 비활성화 (사망 등)
        /// </summary>
        public void Disable()
        {
            _isInitialized = false;
            _rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// 타겟 변경
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_target == null) return;

            // 타겟까지의 선 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _target.position);

            // 정지 거리 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _stoppingDistance);
        }
#endif
    }
}