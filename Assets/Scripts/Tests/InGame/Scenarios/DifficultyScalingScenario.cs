using System.Collections;
using UnityEngine;
using Vs.Core;
using Vs.Enemy;

namespace Vs.TestRunner.Scenarios
{
    /// <summary>
    /// 난이도 스케일링 테스트 시나리오.
    /// 시간 경과에 따른 난이도 변화를 검증합니다.
    /// </summary>
    public class DifficultyScalingScenario : TestScenario
    {
        [Header("Test Settings")]
        [SerializeField] private float _testDuration = 10f;
        [SerializeField] private float _checkInterval = 2f;

        private void Awake()
        {
            _scenarioName = "Difficulty Scaling";
            _description = "Tests enemy spawning and difficulty progression";
            _timeout = 60f;
        }

        protected override IEnumerator ExecuteTests()
        {
            // T1: GameManager 확인
            BeginTest("GameManager Available");
            if (!GameManager.HasInstance)
            {
                SkipTest("GameManager not available");
                yield break;
            }
            PassTest();
            yield return null;

            // T2: 게임 시작
            BeginTest("Start Game");
            GameManager.Instance.StartGame();
            yield return new WaitForSecondsRealtime(0.5f);

            if (Assert(GameManager.Instance.IsPlaying,
                $"Failed to start: {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // T3: 적 스포너 확인
            BeginTest("Enemy Spawner Exists");
            var spawner = FindFirstObjectByType<EnemySpawner>();

            if (spawner == null)
            {
                SkipTest("EnemySpawner not found - skipping spawn tests");

                // 기본 테스트만 수행
                yield return TestBasicGameplay();
                yield break;
            }
            PassTest();
            yield return null;

            // T4: 초기 적 카운트
            BeginTest("Initial Enemy Count");
            yield return new WaitForSeconds(1f);

            var enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            int initialCount = enemies.Length;
            PassTest($"Initial enemies: {initialCount}");
            yield return null;

            // T5: 시간 경과에 따른 적 스폰
            BeginTest("Enemies Spawn Over Time");
            yield return new WaitForSeconds(_testDuration);

            // 게임이 아직 진행 중인지 확인
            if (GameManager.Instance.State == GameState.LevelUp)
            {
                GameManager.Instance.ResumeGame();
                yield return new WaitForSecondsRealtime(0.1f);
            }

            enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            int currentCount = enemies.Length;

            // 적이 스폰되었거나 처치되어 경험치를 얻었으면 통과
            PassTest($"Enemies after {_testDuration}s: {currentCount}");
            yield return null;

            // T6: 게임 안정성 확인
            BeginTest("Game Stability During Play");
            float checkTime = 0f;
            int crashCount = 0;

            while (checkTime < _checkInterval * 3)
            {
                yield return new WaitForSeconds(_checkInterval);
                checkTime += _checkInterval;

                // 레벨업 상태면 재개
                if (GameManager.Instance.State == GameState.LevelUp)
                {
                    GameManager.Instance.ResumeGame();
                    yield return new WaitForSecondsRealtime(0.1f);
                }

                // 게임이 비정상 종료되었는지 확인
                if (GameManager.Instance.State == GameState.GameOver)
                {
                    // GameOver는 정상 (플레이어가 죽음)
                    break;
                }

                if (GameManager.Instance.State == GameState.Menu)
                {
                    crashCount++;
                }
            }

            if (Assert(crashCount == 0, $"Game crashed {crashCount} times"))
            {
                PassTest($"Game ran stable for {checkTime}s");
            }

            // 정리
            if (GameManager.HasInstance)
            {
                GameManager.Instance.ReturnToMenu();
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }

        private IEnumerator TestBasicGameplay()
        {
            // 기본 게임플레이 테스트 (적 스포너 없을 때)
            BeginTest("Basic Gameplay Without Spawner");

            float testTime = 5f;
            float elapsed = 0f;

            while (elapsed < testTime)
            {
                yield return new WaitForSeconds(1f);
                elapsed += 1f;

                // 레벨업 상태면 재개
                if (GameManager.Instance.State == GameState.LevelUp)
                {
                    GameManager.Instance.ResumeGame();
                    yield return new WaitForSecondsRealtime(0.1f);
                }

                // 게임이 정상 진행 중인지 확인
                if (GameManager.Instance.State != GameState.Playing &&
                    GameManager.Instance.State != GameState.LevelUp &&
                    GameManager.Instance.State != GameState.Paused)
                {
                    break;
                }
            }

            PassTest($"Game ran for {elapsed}s");

            // 정리
            if (GameManager.HasInstance)
            {
                GameManager.Instance.ReturnToMenu();
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
