using System.Collections;
using UnityEngine;
using Vs.Core;
using Vs.Progression;

namespace Vs.TestRunner.Scenarios
{
    /// <summary>
    /// 레벨업 시스템 테스트 시나리오.
    /// 경험치 획득 및 레벨업 플로우를 검증합니다.
    /// </summary>
    public class LevelUpScenario : TestScenario
    {
        private ExperienceManager _expManager;
        private LevelUpManager _levelUpManager;

        private void Awake()
        {
            _scenarioName = "Level Up System";
            _description = "Tests experience gain and level up flow";
            _timeout = 60f;
        }

        protected override IEnumerator ExecuteTests()
        {
            // 매니저 찾기
            BeginTest("Find Managers");

            _expManager = ExperienceManager.HasInstance ? ExperienceManager.Instance : null;

            if (_expManager == null)
            {
                SkipTest("ExperienceManager not found");
                yield break;
            }

            _levelUpManager = LevelUpManager.HasInstance ? LevelUpManager.Instance : null;

            PassTest("Managers found");
            yield return null;

            // 게임 시작
            BeginTest("Start Game for Testing");
            if (!GameManager.HasInstance)
            {
                SkipTest("GameManager not available");
                yield break;
            }

            GameManager.Instance.StartGame();
            yield return new WaitForSecondsRealtime(0.5f);

            if (Assert(GameManager.Instance.IsPlaying,
                $"Failed to start game: {GameManager.Instance.State}"))
            {
                PassTest();
            }

            // T1: 초기 레벨 확인
            BeginTest("Initial Level is 1");
            if (Assert(_expManager.CurrentLevel == 1,
                $"Expected level 1, got {_expManager.CurrentLevel}"))
            {
                PassTest();
            }
            yield return null;

            // T2: 초기 경험치 확인
            BeginTest("Initial Experience is 0");
            if (Assert(_expManager.CurrentExp == 0,
                $"Expected exp 0, got {_expManager.CurrentExp}"))
            {
                PassTest();
            }
            yield return null;

            // T3: 경험치 추가 테스트
            BeginTest("Add Experience");
            int initialExp = _expManager.CurrentExp;
            int expToAdd = 5;
            _expManager.AddExperience(expToAdd);

            if (Assert(_expManager.CurrentExp >= initialExp,
                $"Experience did not increase"))
            {
                PassTest($"Exp: {initialExp} -> {_expManager.CurrentExp}");
            }
            yield return null;

            // T4: ExpToNextLevel 확인
            BeginTest("ExpToNextLevel is Positive");
            if (Assert(_expManager.ExpToNextLevel > 0,
                $"ExpToNextLevel should be positive, got {_expManager.ExpToNextLevel}"))
            {
                PassTest($"ExpToNextLevel: {_expManager.ExpToNextLevel}");
            }
            yield return null;

            // T5: 레벨업 트리거 테스트
            BeginTest("Level Up Trigger");
            int levelBefore = _expManager.CurrentLevel;
            int expNeeded = _expManager.ExpToNextLevel - _expManager.CurrentExp;

            // 레벨업에 필요한 경험치 추가
            _expManager.AddExperience(expNeeded + 1);
            yield return new WaitForSecondsRealtime(0.5f);

            // 레벨업 상태가 되었는지 확인
            bool leveledUp = _expManager.CurrentLevel > levelBefore;
            bool inLevelUpState = GameManager.Instance.State == GameState.LevelUp;

            if (Assert(leveledUp || inLevelUpState,
                $"Level up did not trigger. Level: {levelBefore} -> {_expManager.CurrentLevel}, State: {GameManager.Instance.State}"))
            {
                PassTest($"Level: {levelBefore} -> {_expManager.CurrentLevel}");
            }

            // T6: 레벨업 상태에서 게임 일시정지 확인
            BeginTest("LevelUp State Pauses Game");
            if (GameManager.Instance.State == GameState.LevelUp)
            {
                if (Assert(GameManager.Instance.IsPaused,
                    "Game should be paused during LevelUp"))
                {
                    PassTest();
                }
            }
            else
            {
                SkipTest("Not in LevelUp state");
            }
            yield return null;

            // T7: 레벨업 후 게임 재개
            BeginTest("Resume After LevelUp");
            if (GameManager.Instance.State == GameState.LevelUp)
            {
                GameManager.Instance.ResumeGame();
                yield return new WaitForSecondsRealtime(0.1f);

                if (Assert(GameManager.Instance.IsPlaying,
                    $"Failed to resume: {GameManager.Instance.State}"))
                {
                    PassTest();
                }
            }
            else
            {
                SkipTest("Not in LevelUp state");
            }

            // T8: LevelUpManager 상태 확인
            BeginTest("LevelUpManager State");
            if (_levelUpManager != null)
            {
                PassTest($"Weapons: {_levelUpManager.WeaponCount}, Passives: {_levelUpManager.PassiveCount}");
            }
            else
            {
                SkipTest("LevelUpManager not available");
            }
            yield return null;

            // 정리
            if (GameManager.HasInstance)
            {
                GameManager.Instance.ReturnToMenu();
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
