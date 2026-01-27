using UnityEngine;
using Vs.Core;

namespace Vs.Progression
{
    /// <summary>
    /// 경험치 젬 픽업.
    /// 적 처치 시 스폰되며, 플레이어 픽업 범위 진입 시 자동 수집.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ExpPickup : MonoBehaviour, IPoolable
    {
        [Header("경험치")] [SerializeField] private int _expValue = 1;

        [Header("이동")] [SerializeField] private float _magnetSpeed = 10f;
        [SerializeField] private float _collectDistance = 0.5f;
        [SerializeField] private float _acceleration = 5f;

        [Header("비주얼")] [SerializeField] private SpriteRenderer _spriteRenderer;

        private Transform _target;
        private bool _isCollecting;
        private float _currentSpeed;

        public int ExpValue => _expValue;

        private void Update()
        {
            if (!_isCollecting || _target == null) return;

            // 가속
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _magnetSpeed, _acceleration * Time.deltaTime);

            // 플레이어에게 이동
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * _currentSpeed * Time.deltaTime;

            // 수집 거리 체크
            float distance = Vector3.Distance(transform.position, _target.position);
            if (distance <= _collectDistance)
            {
                OnCollected();
            }
        }

        /// <summary>
        /// 경험치 젬 초기화.
        /// </summary>
        public void Initialize(int expValue, Transform target)
        {
            _expValue = expValue;
            _target = target;
        }

        /// <summary>
        /// 타겟만 설정 (경험치 값은 기본값 사용).
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// 수집 시작 (마그넷 범위 진입 시 호출).
        /// </summary>
        public void StartCollecting()
        {
            if (_isCollecting) return;

            _isCollecting = true;
            _currentSpeed = 0f;
        }

        private void OnCollected()
        {
            // 경험치 추가
            if (ExperienceManager.HasInstance)
            {
                ExperienceManager.Instance.AddExperience(_expValue);
            }

            // 풀로 반환
            if (PoolManager.HasInstance)
            {
                PoolManager.Instance.Despawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #region IPoolable

        public void OnSpawn()
        {
            _isCollecting = false;
            _currentSpeed = 0f;
        }

        public void OnDespawn()
        {
            _isCollecting = false;
            _target = null;
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _collectDistance);
        }
#endif
    }
}