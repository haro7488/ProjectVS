using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vs.Core;
using Vs.Data;

namespace Vs.Enemy
{
    /// <summary>
    /// 적 스폰 관리자. 웨이브 기반으로 적을 스폰.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("참조")] [SerializeField] private StageData _stageData;
        [SerializeField] private Transform _player;

        [Header("스폰 설정")] [SerializeField] private float _spawnRadius = 15f;
        [SerializeField] private float _minSpawnDistance = 10f;

        [Header("성능")] [SerializeField] private int _maxActiveEnemies = 100;

        private bool _isSpawning;
        private int _activeEnemyCount;
        private float _elapsedTime;
        private readonly List<Coroutine> _waveCoroutines = new();

        public int ActiveEnemyCount => _activeEnemyCount;
        public float ElapsedTime => _elapsedTime;

        private void OnEnable()
        {
            EnemyBase.OnEnemyDied += OnEnemyDeath;

            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }

        private void OnDisable()
        {
            EnemyBase.OnEnemyDied -= OnEnemyDeath;

            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    if (!_isSpawning)
                    {
                        StartSpawning();
                    }

                    break;
                case GameState.GameOver:
                case GameState.Victory:
                    StopSpawning();
                    break;
            }
        }

        /// <summary>
        /// 스폰 시작.
        /// </summary>
        public void StartSpawning()
        {
            if (_stageData == null)
            {
                Debug.LogError("[EnemySpawner] StageData is not assigned!");
                return;
            }

            if (_player == null)
            {
                Debug.LogError("[EnemySpawner] Player transform is not assigned!");
                return;
            }

            _isSpawning = true;
            _elapsedTime = 0f;

            // 각 웨이브에 대한 코루틴 시작
            foreach (var wave in _stageData.Waves)
            {
                var coroutine = StartCoroutine(WaveRoutine(wave));
                _waveCoroutines.Add(coroutine);
            }

            // 보스 스폰 코루틴
            if (_stageData.BossSpawns != null && _stageData.BossSpawns.Length > 0)
            {
                foreach (var bossSpawn in _stageData.BossSpawns)
                {
                    var coroutine = StartCoroutine(BossSpawnRoutine(bossSpawn));
                    _waveCoroutines.Add(coroutine);
                }
            }

            // 시간 추적 코루틴
            StartCoroutine(TimeTrackingRoutine());
        }

        /// <summary>
        /// 스폰 중지.
        /// </summary>
        public void StopSpawning()
        {
            _isSpawning = false;

            foreach (var coroutine in _waveCoroutines)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }

            _waveCoroutines.Clear();
        }

        private IEnumerator TimeTrackingRoutine()
        {
            while (_isSpawning && _elapsedTime < _stageData.Duration)
            {
                yield return null;
                _elapsedTime += Time.deltaTime;
            }
        }

        private IEnumerator WaveRoutine(EnemyWave wave)
        {
            // 시작 시간까지 대기
            while (_elapsedTime < wave.StartTime)
            {
                yield return null;
                if (!_isSpawning) yield break;
            }

            float endTime = wave.EndTime < 0 ? _stageData.Duration : wave.EndTime;

            while (_isSpawning && _elapsedTime < endTime)
            {
                // 게임이 일시정지 상태면 대기
                if (!GameManager.Instance.IsPlaying)
                {
                    yield return null;
                    continue;
                }

                // 최대 적 수 체크
                if (_activeEnemyCount < _maxActiveEnemies)
                {
                    SpawnWave(wave);
                }

                yield return new WaitForSeconds(wave.SpawnInterval);
            }
        }

        private IEnumerator BossSpawnRoutine(BossSpawn bossSpawn)
        {
            // 보스 스폰 시간까지 대기
            while (_elapsedTime < bossSpawn.SpawnTime)
            {
                yield return null;
                if (!_isSpawning) yield break;
            }

            // 보스 스폰
            SpawnEnemy(bossSpawn.Boss, isBoss: true);
        }

        private void SpawnWave(EnemyWave wave)
        {
            // 시간에 따른 스폰 수 증가
            float elapsedMinutes = _elapsedTime / 60f;
            int spawnCount = Mathf.RoundToInt(wave.SpawnCount * (1f + wave.SpawnCountGrowth * elapsedMinutes));
            spawnCount = Mathf.Max(1, spawnCount);

            for (int i = 0; i < spawnCount; i++)
            {
                if (_activeEnemyCount >= _maxActiveEnemies) break;

                SpawnEnemy(wave.Enemy, isBoss: false);
            }
        }

        private void SpawnEnemy(EnemyData enemyData, bool isBoss)
        {
            if (enemyData == null || enemyData.Prefab == null)
            {
                Debug.LogWarning("[EnemySpawner] Invalid enemy data or missing prefab!");
                return;
            }

            Vector2 spawnPos = GetSpawnPosition();

            // 난이도 배율 계산
            float elapsedMinutes = _elapsedTime / 60f;
            float healthMultiplier = _stageData.GetEnemyHealthMultiplier(elapsedMinutes);
            float damageMultiplier = _stageData.GetEnemyDamageMultiplier(elapsedMinutes);

            if (isBoss && enemyData.BossHealthMultiplier > 0)
            {
                healthMultiplier *= enemyData.BossHealthMultiplier;
            }

            // 풀에서 스폰
            GameObject enemyObj;
            if (PoolManager.HasInstance)
            {
                enemyObj = PoolManager.Instance.Spawn(enemyData.Prefab, spawnPos, Quaternion.identity);
            }
            else
            {
                enemyObj = Instantiate(enemyData.Prefab, spawnPos, Quaternion.identity);
            }

            // 초기화
            var enemy = enemyObj.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.Initialize(enemyData, _player, healthMultiplier, damageMultiplier);
            }

            _activeEnemyCount++;
        }

        private Vector2 GetSpawnPosition()
        {
            // 화면 밖, 플레이어 주변에 스폰
            Vector2 playerPos = _player.position;

            // 랜덤 각도 선택
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            // 최소/최대 거리 사이에서 랜덤 거리 선택
            float distance = Random.Range(_minSpawnDistance, _spawnRadius);

            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector2 spawnPos = playerPos + offset;

            // 맵 범위 클램핑 (StageData에서 MapSize 사용)
            if (_stageData != null)
            {
                float halfWidth = _stageData.MapSize.x / 2f;
                float halfHeight = _stageData.MapSize.y / 2f;

                spawnPos.x = Mathf.Clamp(spawnPos.x, -halfWidth, halfWidth);
                spawnPos.y = Mathf.Clamp(spawnPos.y, -halfHeight, halfHeight);
            }

            return spawnPos;
        }

        private void OnEnemyDeath(EnemyDeathEvent deathEvent)
        {
            _activeEnemyCount--;
            _activeEnemyCount = Mathf.Max(0, _activeEnemyCount);
        }

        /// <summary>
        /// 스테이지 데이터 설정.
        /// </summary>
        public void SetStageData(StageData stageData)
        {
            _stageData = stageData;
        }

        /// <summary>
        /// 플레이어 Transform 설정.
        /// </summary>
        public void SetPlayer(Transform player)
        {
            _player = player;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_player == null) return;

            // 최소 스폰 거리
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_player.position, _minSpawnDistance);

            // 최대 스폰 거리
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_player.position, _spawnRadius);
        }
#endif
    }
}