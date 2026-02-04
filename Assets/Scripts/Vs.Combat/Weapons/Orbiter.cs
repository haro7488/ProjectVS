using UnityEngine;
using Vs.Core;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 궤도 오브젝트 - 플레이어 주변을 회전하며 적과 접촉 시 데미지를 줍니다.
    /// 드론 무기에서 생성됩니다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Orbiter : MonoBehaviour, IPoolable
    {
        [Header("Settings")]
        [SerializeField] private float _hitCooldown = 0.5f;

        private float _damage;
        private Transform _center;
        private float _orbitRadius;
        private float _orbitSpeed;
        private float _currentAngle;
        private int _orbitIndex;
        private int _totalOrbiters;

        private float _lastHitTime;
        private GameObject _prefabSource;

        #region Properties

        public int OrbitIndex => _orbitIndex;

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            if (_center == null) return;

            UpdateOrbitPosition();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Constants.TagEnemy)) return;

            // 쿨다운 체크
            if (Time.time - _lastHitTime < _hitCooldown) return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.IsDead) return;

                Vector3 direction = (other.transform.position - transform.position).normalized;

                var damageInfo = new DamageInfo(
                    amount: _damage,
                    type: DamageType.Physical,
                    position: transform.position,
                    direction: direction,
                    source: gameObject,
                    knockback: 1f
                );

                damageable.TakeDamage(damageInfo);
                _lastHitTime = Time.time;
            }
        }

        #endregion

        #region IPoolable Implementation

        public void OnSpawn()
        {
            _lastHitTime = -999f;
            _currentAngle = 0f;
        }

        public void OnDespawn()
        {
            _center = null;
            _damage = 0f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 궤도 오브젝트를 초기화합니다.
        /// </summary>
        public void Initialize(Transform center, float damage, float radius, float speed, int index, int total)
        {
            _center = center;
            _damage = damage;
            _orbitRadius = radius;
            _orbitSpeed = speed;
            _orbitIndex = index;
            _totalOrbiters = total;

            // 초기 각도 계산 (균등 분배)
            _currentAngle = (360f / _totalOrbiters) * _orbitIndex;

            // 즉시 위치 업데이트
            UpdateOrbitPosition();
        }

        /// <summary>
        /// 궤도 반지름을 업데이트합니다.
        /// </summary>
        public void UpdateRadius(float radius)
        {
            _orbitRadius = radius;
        }

        /// <summary>
        /// 궤도 속도를 업데이트합니다.
        /// </summary>
        public void UpdateSpeed(float speed)
        {
            _orbitSpeed = speed;
        }

        /// <summary>
        /// 데미지를 업데이트합니다.
        /// </summary>
        public void UpdateDamage(float damage)
        {
            _damage = damage;
        }

        /// <summary>
        /// 풀 반환용 프리팹 소스를 설정합니다.
        /// </summary>
        public void SetPrefabSource(GameObject prefab)
        {
            _prefabSource = prefab;
        }

        /// <summary>
        /// 풀로 반환합니다.
        /// </summary>
        public void ReturnToPool()
        {
            if (PoolManager.HasInstance)
            {
                PoolManager.Instance.Despawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Private Methods

        private void UpdateOrbitPosition()
        {
            _currentAngle += _orbitSpeed * Time.deltaTime;

            if (_currentAngle >= 360f)
            {
                _currentAngle -= 360f;
            }

            float rad = _currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * _orbitRadius;
            float z = Mathf.Sin(rad) * _orbitRadius;

            Vector3 offset = new Vector3(x, 0f, z);
            transform.position = _center.position + offset;

            // 회전 방향을 바라보도록 설정
            Vector3 tangent = new Vector3(-Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            if (tangent.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(tangent);
            }
        }

        #endregion
    }
}
