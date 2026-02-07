using System.Collections.Generic;
using UnityEngine;
using Vs.Combat;
using Vs.Core;
using Vs.Data;
using Vs.Enemy;

namespace Vs.Debug
{
    /// <summary>
    /// 적 디버그 탭.
    /// 적 스폰, 제거 등 적 관련 디버그 기능 제공.
    /// </summary>
    public class EnemyDebugTab : DebugTabBase
    {
        [Header("Enemy Data")]
        [SerializeField] private List<EnemyData> _enemyDataList = new();

        [Header("Spawn Settings")]
        [SerializeField] private float _spawnDistance = 5f;

        private Transform _playerTransform;

        public override void RefreshContent()
        {
            ClearContent();

            _playerTransform = FindPlayerTransform();

            if (_playerTransform == null)
            {
                UnityEngine.Debug.LogWarning("[EnemyDebugTab] Player not found in scene");
                CreateButton("Player not found", null);
                return;
            }

            // 전체 적 제거 버튼
            CreateButton("Kill All Enemies", OnKillAllEnemies);

            // 전체 타입 스폰 버튼
            CreateButton("Spawn All Types (1 each)", OnSpawnAllTypes);

            // 보스 스폰 버튼
            CreateButton("Spawn Boss", OnSpawnBoss);

            // 구분선 역할
            CreateButton("--- Individual Enemies ---", null);

            // 개별 적 스폰 버튼
            foreach (var enemyData in _enemyDataList)
            {
                if (enemyData == null) continue;

                string label = $"Spawn {enemyData.DisplayName}";
                if (enemyData.IsBoss)
                {
                    label = $"[BOSS] {label}";
                }

                CreateButton(label, () => OnSpawnEnemy(enemyData));
            }
        }

        private Transform FindPlayerTransform()
        {
            var player = GameObject.FindWithTag(Vs.Utility.Constants.TagPlayer);
            return player != null ? player.transform : null;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            if (_playerTransform == null) return Vector3.zero;

            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _spawnDistance;
            return _playerTransform.position + offset;
        }

        private void OnSpawnEnemy(EnemyData data)
        {
            if (data == null || data.Prefab == null)
            {
                UnityEngine.Debug.LogWarning("[EnemyDebugTab] Invalid enemy data or prefab is null");
                return;
            }

            if (_playerTransform == null)
            {
                UnityEngine.Debug.LogWarning("[EnemyDebugTab] Player transform not found");
                return;
            }

            Vector3 spawnPos = GetRandomSpawnPosition();

            // 풀에서 스폰
            if (!PoolManager.HasInstance)
            {
                UnityEngine.Debug.LogWarning("[EnemyDebugTab] PoolManager not found");
                return;
            }

            var enemyObj = PoolManager.Instance.Spawn(data.Prefab, spawnPos, Quaternion.identity);
            var enemy = enemyObj.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                float healthMult = data.IsBoss ? data.BossHealthMultiplier : 1f;
                enemy.Initialize(data, _playerTransform, healthMult, 1f);
                UnityEngine.Debug.Log($"[EnemyDebugTab] Spawned {data.DisplayName} at {spawnPos}");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"[EnemyDebugTab] EnemyBase component not found on {data.Prefab.name}");
            }
        }

        private void OnSpawnAllTypes()
        {
            if (_playerTransform == null)
            {
                UnityEngine.Debug.LogWarning("[EnemyDebugTab] Player transform not found");
                return;
            }

            int spawnedCount = 0;
            foreach (var enemyData in _enemyDataList)
            {
                if (enemyData == null || enemyData.Prefab == null) continue;
                if (enemyData.IsBoss) continue; // 보스는 제외

                OnSpawnEnemy(enemyData);
                spawnedCount++;
            }

            UnityEngine.Debug.Log($"[EnemyDebugTab] Spawned {spawnedCount} enemy types");
        }

        private void OnSpawnBoss()
        {
            EnemyData bossData = null;

            // 보스 데이터 찾기
            foreach (var enemyData in _enemyDataList)
            {
                if (enemyData != null && enemyData.IsBoss)
                {
                    bossData = enemyData;
                    break;
                }
            }

            if (bossData != null)
            {
                OnSpawnEnemy(bossData);
            }
            else
            {
                UnityEngine.Debug.LogWarning("[EnemyDebugTab] No boss data found in enemy list");
            }
        }

        private void OnKillAllEnemies()
        {
            var enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

            if (enemies.Length == 0)
            {
                UnityEngine.Debug.Log("[EnemyDebugTab] No enemies to kill");
                return;
            }

            int killedCount = 0;
            foreach (var enemy in enemies)
            {
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.TakeDamage(new DamageInfo(99999f));
                    killedCount++;
                }
            }

            UnityEngine.Debug.Log($"[EnemyDebugTab] Killed {killedCount} enemies");
        }
    }
}
