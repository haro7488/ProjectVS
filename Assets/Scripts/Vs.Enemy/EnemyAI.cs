using UnityEngine;

namespace Vs.Enemy
{
    /// <summary>
    /// 적 AI 행동 컴포넌트. 플레이어 추적 로직 담당.
    /// </summary>
    [RequireComponent(typeof(EnemyBase))]
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyAI : MonoBehaviour
    {
        private EnemyBase _enemy;
        private Transform _target;
        private Rigidbody _rb;

        private float _moveSpeed = 2f;

        [SerializeField] private float _stoppingDistance = 0.1f;

        private bool _isInitialized;

        private void Awake()
        {
            _enemy = GetComponent<EnemyBase>();
            _rb = GetComponent<Rigidbody>();
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
            // XZ 평면에서 방향 계산 (Y축 무시)
            Vector3 targetPos = new Vector3(_target.position.x, transform.position.y, _target.position.z);
            Vector3 direction = (targetPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPos);

            // 정지 거리 내에 있으면 이동 중지
            if (distance <= _stoppingDistance)
            {
                _rb.velocity = Vector3.zero;
                return;
            }

            // 플레이어 방향으로 이동
            _rb.velocity = direction * _moveSpeed;

            // 이동 방향으로 회전 (Y축)
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }

        /// <summary>
        /// AI 비활성화 (사망 등)
        /// </summary>
        public void Disable()
        {
            _isInitialized = false;
            _rb.velocity = Vector3.zero;
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