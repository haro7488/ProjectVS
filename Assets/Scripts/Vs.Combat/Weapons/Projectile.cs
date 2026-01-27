using UnityEngine;
using Vs.Core;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 직선으로 이동하는 투사체.
    /// 적과 충돌 시 대미지를 주고, 수명이 다하면 자동으로 반환됩니다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        [Header("Settings")] [SerializeField] private float _defaultDuration = 5f;

        private float _damage;
        private float _speed;
        private float _duration;
        private Vector3 _direction;
        private float _spawnTime;
        private bool _isPiercing;
        private int _pierceCount;
        private int _currentPierceCount;

        private Rigidbody _rb;
        private GameObject _prefabSource;
        private DamageType _damageType = DamageType.Physical;

        #region Properties

        public float Damage => _damage;
        public bool IsPiercing => _isPiercing;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }

        private void Update()
        {
            // 수명 체크
            if (Time.time - _spawnTime >= _duration)
            {
                ReturnToPool();
            }
        }

        private void FixedUpdate()
        {
            // 직선 이동
            _rb.velocity = _direction * _speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            // 적과 충돌 확인
            if (!other.CompareTag(Constants.TagEnemy)) return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                var damageInfo = new DamageInfo(
                    amount: _damage,
                    type: _damageType,
                    position: transform.position,
                    direction: _direction,
                    source: gameObject
                );

                damageable.TakeDamage(damageInfo);
            }

            // 관통 처리
            if (_isPiercing)
            {
                _currentPierceCount++;

                if (_currentPierceCount >= _pierceCount)
                {
                    ReturnToPool();
                }
            }
            else
            {
                ReturnToPool();
            }
        }

        #endregion

        #region IPoolable Implementation

        public void OnSpawn()
        {
            _spawnTime = Time.time;
            _currentPierceCount = 0;
        }

        public void OnDespawn()
        {
            _rb.velocity = Vector3.zero;
            _direction = Vector3.zero;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 투사체를 초기화합니다.
        /// </summary>
        public void Initialize(float damage, float speed, Vector3 direction, float duration = 0f)
        {
            _damage = damage;
            _speed = speed;
            _direction = new Vector3(direction.x, 0f, direction.z).normalized;
            _duration = duration > 0f ? duration : _defaultDuration;
            _isPiercing = false;
            _pierceCount = 0;
        }

        /// <summary>
        /// 관통 옵션을 설정합니다.
        /// </summary>
        public void SetPiercing(bool isPiercing, int pierceCount = 1)
        {
            _isPiercing = isPiercing;
            _pierceCount = pierceCount;
        }

        /// <summary>
        /// 대미지 타입을 설정합니다.
        /// </summary>
        public void SetDamageType(DamageType damageType)
        {
            _damageType = damageType;
        }

        /// <summary>
        /// 풀 반환용 프리팹 소스를 설정합니다.
        /// </summary>
        public void SetPrefabSource(GameObject prefab)
        {
            _prefabSource = prefab;
        }

        #endregion

        #region Private Methods

        private void ReturnToPool()
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
    }
}