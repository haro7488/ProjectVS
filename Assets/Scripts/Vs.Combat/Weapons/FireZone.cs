using System.Collections.Generic;
using UnityEngine;
using Vs.Core;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 화염 지대 - 범위 내 적에게 지속 데미지를 줍니다.
    /// 화염병에서 생성됩니다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FireZone : MonoBehaviour, IPoolable
    {
        [Header("Settings")]
        [SerializeField] private float _tickInterval = 0.5f;
        [SerializeField] private LayerMask _enemyLayer;

        private float _damage;
        private float _duration;
        private float _radius;
        private float _spawnTime;
        private float _lastTickTime;
        private GameObject _prefabSource;

        private readonly Collider[] _hitBuffer = new Collider[32];
        private readonly HashSet<int> _damagedThisTick = new HashSet<int>();

        #region Unity Lifecycle

        private void Awake()
        {
            if (_enemyLayer == 0)
            {
                _enemyLayer = LayerMask.GetMask(Constants.LayerEnemy);
            }
        }

        private void Update()
        {
            // 지속 시간 체크
            if (Time.time - _spawnTime >= _duration)
            {
                ReturnToPool();
                return;
            }

            // Tick 간격 체크
            if (Time.time - _lastTickTime >= _tickInterval)
            {
                ApplyTickDamage();
                _lastTickTime = Time.time;
            }
        }

        #endregion

        #region IPoolable Implementation

        public void OnSpawn()
        {
            _spawnTime = Time.time;
            _lastTickTime = Time.time;
        }

        public void OnDespawn()
        {
            _damage = 0f;
            _duration = 0f;
            _radius = 0f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 화염 지대를 초기화합니다.
        /// </summary>
        public void Initialize(float damage, float duration, float radius)
        {
            _damage = damage;
            _duration = duration;
            _radius = radius;

            // 스케일 조정
            transform.localScale = Vector3.one * radius * 2f;
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

        private void ApplyTickDamage()
        {
            _damagedThisTick.Clear();

            Vector3 pos = transform.position;

            int hitCount = Physics.OverlapSphereNonAlloc(
                pos,
                _radius,
                _hitBuffer,
                _enemyLayer
            );

            for (int i = 0; i < hitCount; i++)
            {
                var collider = _hitBuffer[i];
                if (collider == null) continue;

                int instanceId = collider.gameObject.GetInstanceID();
                if (_damagedThisTick.Contains(instanceId)) continue;

                if (collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable.IsDead) continue;

                    var damageInfo = new DamageInfo(
                        amount: _damage,
                        type: DamageType.Fire,
                        position: collider.transform.position,
                        direction: Vector3.up,
                        source: gameObject
                    );

                    damageable.TakeDamage(damageInfo);
                    _damagedThisTick.Add(instanceId);
                }
            }
        }

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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.3f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, _radius > 0 ? _radius : 1f);
        }
#endif
    }
}