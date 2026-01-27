using UnityEngine;
using Vs.Core;
using Vs.Enemy;
using Vs.Utility;

namespace Vs.Progression
{
    /// <summary>
    /// 적 사망 시 경험치 젬을 스폰하는 매니저.
    /// EnemyBase.OnEnemyDied 이벤트를 구독하여 처리.
    /// </summary>
    public class ExpPickupSpawner : Singleton<ExpPickupSpawner>
    {
        [Header("설정")] [SerializeField] private GameObject _expPickupPrefab;
        [SerializeField] private Transform _player;

        [Header("Debug")] [SerializeField] private bool _debugMode;

        private void OnEnable()
        {
            EnemyBase.OnEnemyDied += HandleEnemyDied;
        }

        private void OnDisable()
        {
            EnemyBase.OnEnemyDied -= HandleEnemyDied;
        }

        /// <summary>
        /// 플레이어 Transform 설정.
        /// </summary>
        public void SetPlayer(Transform player)
        {
            _player = player;
        }

        /// <summary>
        /// 경험치 젬 프리팹 설정.
        /// </summary>
        public void SetExpPickupPrefab(GameObject prefab)
        {
            _expPickupPrefab = prefab;
        }

        private void HandleEnemyDied(EnemyDeathEvent deathEvent)
        {
            if (_expPickupPrefab == null)
            {
                if (_debugMode)
                {
                    Debug.LogWarning("[ExpPickupSpawner] ExpPickup prefab not assigned!");
                }

                return;
            }

            if (deathEvent.ExpValue <= 0) return;

            SpawnExpPickup(deathEvent.Position, deathEvent.ExpValue);
        }

        private void SpawnExpPickup(Vector2 position, int expValue)
        {
            GameObject pickupObj;

            if (PoolManager.HasInstance)
            {
                pickupObj = PoolManager.Instance.Spawn(_expPickupPrefab, position, Quaternion.identity);
            }
            else
            {
                pickupObj = Instantiate(_expPickupPrefab, position, Quaternion.identity);
            }

            if (pickupObj.TryGetComponent<ExpPickup>(out var pickup))
            {
                pickup.Initialize(expValue, _player);
            }

            if (_debugMode)
            {
                Debug.Log($"[ExpPickupSpawner] Spawned ExpPickup at {position} with {expValue} EXP");
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Debug: Spawn Test ExpPickup")]
        private void DebugSpawnTestPickup()
        {
            if (_player != null)
            {
                SpawnExpPickup(_player.position + Vector3.right * 2f, 5);
            }
        }
#endif
    }
}