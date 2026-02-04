using System.Collections;
using UnityEngine;
using Vs.Core;

namespace Vs.TestRunner.Scenarios
{
    /// <summary>
    /// 기본 게임플레이 시나리오 테스트.
    /// 게임 시작, 상태 전환, 기본 동작을 검증합니다.
    /// </summary>
    public class BasicGameplayScenario : TestScenario
    {
        private void Awake()
        {
            _scenarioName = "Basic Gameplay";
            _description = "Tests basic gameplay functionality: game states, time scale, etc.";
            _timeout = 30f;
        }

        protected override IEnumerator ExecuteTests()
        {
            // T1: GameManager 존재 확인
            BeginTest("GameManager Exists");
            if (AssertNotNull(GameManager.Instance, "GameManager"))
            {
                PassTest();
            }
            yield return null;

            // T2: 초기 상태 확인
            BeginTest("Initial State is Menu");
            if (Assert(GameManager.Instance.State == GameState.Menu,
                $"Expected Menu, got {GameManager.Instance.State}"))
            {
                PassTest();
            }
            yield return null;

            // T3: 게임 시작
            BeginTest("Game Start Changes State to Playing");
            GameManager.Instance.StartGame();
            yield return new WaitForSecondsRealtime(0.1f);

            if (Assert(GameManager.Instance.State == GameState.Playing,
                $"Expected Playing, got {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // T4: TimeScale 확인
            BeginTest("Playing State TimeScale is 1");
            if (Assert(Mathf.Approximately(Time.timeScale, 1f),
                $"Expected TimeScale 1, got {Time.timeScale}"))
            {
                PassTest();
            }
            yield return null;

            // T5: 일시정지
            BeginTest("Pause Changes State to Paused");
            GameManager.Instance.PauseGame();
            yield return new WaitForSecondsRealtime(0.1f);

            if (Assert(GameManager.Instance.State == GameState.Paused,
                $"Expected Paused, got {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // T6: 일시정지 TimeScale
            BeginTest("Paused State TimeScale is 0");
            if (Assert(Mathf.Approximately(Time.timeScale, 0f),
                $"Expected TimeScale 0, got {Time.timeScale}"))
            {
                PassTest();
            }
            yield return null;

            // T7: 재개
            BeginTest("Resume Changes State to Playing");
            GameManager.Instance.ResumeGame();
            yield return new WaitForSecondsRealtime(0.1f);

            if (Assert(GameManager.Instance.State == GameState.Playing,
                $"Expected Playing, got {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // T8: 게임 오버
            BeginTest("End Game (Defeat) Changes State to GameOver");
            GameManager.Instance.EndGame(isVictory: false);
            yield return new WaitForSecondsRealtime(0.1f);

            if (Assert(GameManager.Instance.State == GameState.GameOver,
                $"Expected GameOver, got {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // T9: 메뉴로 복귀
            BeginTest("Return to Menu Works");
            GameManager.Instance.ReturnToMenu();
            yield return new WaitForSecondsRealtime(0.1f);

            if (Assert(GameManager.Instance.State == GameState.Menu,
                $"Expected Menu, got {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // T10: 승리 테스트
            BeginTest("End Game (Victory) Changes State to Victory");
            GameManager.Instance.StartGame();
            yield return new WaitForSecondsRealtime(0.1f);

            GameManager.Instance.EndGame(isVictory: true);
            yield return new WaitForSecondsRealtime(0.1f);

            if (Assert(GameManager.Instance.State == GameState.Victory,
                $"Expected Victory, got {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // 정리
            GameManager.Instance.ReturnToMenu();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
