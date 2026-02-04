using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vs.Core;
using Vs.Progression;
using Vs.Tests.Utilities;

namespace Vs.Integration.Tests
{
    /// <summary>
    /// 레벨업 플로우 통합 테스트.
    /// 경험치 획득 → 레벨업 → 선택지 표시 → 효과 적용 플로우 검증.
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryIntegration)]
    public class LevelUpFlowTests : PlayModeTestBase
    {
        private GameManager _gameManager;
        private ExperienceManager _expManager;
        private LevelUpManager _levelUpManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            // GameManager 생성
            var gmGo = CreateEmptyGameObject("GameManager");
            _gameManager = gmGo.AddComponent<GameManager>();

            // ExperienceManager 생성
            var expGo = CreateEmptyGameObject("ExperienceManager");
            _expManager = expGo.AddComponent<ExperienceManager>();

            // LevelUpManager 생성
            var lvlGo = CreateEmptyGameObject("LevelUpManager");
            _levelUpManager = lvlGo.AddComponent<LevelUpManager>();
        }

        #region 초기 상태 테스트

        [UnityTest]
        public IEnumerator InitialState_ExperienceManagerLevel1()
        {
            yield return null;
            Assert.AreEqual(1, _expManager.CurrentLevel);
        }

        [UnityTest]
        public IEnumerator InitialState_ExperienceIsZero()
        {
            yield return null;
            Assert.AreEqual(0, _expManager.CurrentExp);
        }

        [UnityTest]
        public IEnumerator InitialState_LevelUpManagerWeaponsEmpty()
        {
            yield return null;
            Assert.AreEqual(0, _levelUpManager.WeaponCount);
        }

        [UnityTest]
        public IEnumerator InitialState_LevelUpManagerPassivesEmpty()
        {
            yield return null;
            Assert.AreEqual(0, _levelUpManager.PassiveCount);
        }

        #endregion

        #region 경험치 획득 테스트

        [UnityTest]
        public IEnumerator AddExperience_IncreasesCurrentExp()
        {
            // 게임 시작해야 경험치 추가 가능
            _gameManager.StartGame();
            yield return null;

            int initialExp = _expManager.CurrentExp;
            _expManager.AddExperience(5);

            Assert.AreEqual(initialExp + 5, _expManager.CurrentExp);
        }

        [UnityTest]
        public IEnumerator AddExperience_FiresOnExpGained()
        {
            bool eventFired = false;
            _expManager.OnExpGained += (amount) => eventFired = true;

            _gameManager.StartGame();
            yield return null;

            _expManager.AddExperience(1);

            Assert.IsTrue(eventFired);
        }

        #endregion

        #region 레벨업 트리거 테스트

        [UnityTest]
        public IEnumerator LevelUp_IncreasesLevel()
        {
            _gameManager.StartGame();
            yield return null;

            int initialLevel = _expManager.CurrentLevel;

            // 레벨업에 필요한 경험치 획득
            int required = _expManager.ExpToNextLevel;
            _expManager.AddExperience(required);

            Assert.AreEqual(initialLevel + 1, _expManager.CurrentLevel);
        }

        [UnityTest]
        public IEnumerator LevelUp_FiresOnLevelUp()
        {
            bool eventFired = false;
            _expManager.OnLevelUp += (level) => eventFired = true;

            _gameManager.StartGame();
            yield return null;

            // 레벨업에 필요한 경험치 획득
            int required = _expManager.ExpToNextLevel;
            _expManager.AddExperience(required);

            Assert.IsTrue(eventFired);
        }

        [UnityTest]
        public IEnumerator LevelUp_EventContainsNewLevel()
        {
            int receivedLevel = 0;
            _expManager.OnLevelUp += (level) => receivedLevel = level;

            _gameManager.StartGame();
            yield return null;

            // 레벨업에 필요한 경험치 획득
            int required = _expManager.ExpToNextLevel;
            _expManager.AddExperience(required);

            Assert.AreEqual(2, receivedLevel);
        }

        #endregion

        #region GameManager 연동 테스트

        [UnityTest]
        public IEnumerator TriggerLevelUp_GameManagerStateChanges()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            Assert.AreEqual(GameState.LevelUp, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator TriggerLevelUp_GameIsPaused()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            Assert.IsTrue(_gameManager.IsPaused);
        }

        [UnityTest]
        public IEnumerator AfterLevelUpChoice_GameResumes()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            // 선택 완료 시뮬레이션
            _gameManager.ResumeGame();
            yield return null;

            Assert.IsTrue(_gameManager.IsPlaying);
        }

        #endregion

        #region 연속 레벨업 테스트

        [UnityTest]
        public IEnumerator MultipleLevelUps_WorkCorrectly()
        {
            _gameManager.StartGame();
            yield return null;

            int levelUpCount = 0;
            _expManager.OnLevelUp += (level) => levelUpCount++;

            // 3번 레벨업 (레벨업마다 GameManager가 LevelUp 상태로 전환되므로 Resume 필요)
            for (int i = 0; i < 3; i++)
            {
                int required = _expManager.ExpToNextLevel;
                _expManager.AddExperience(required);
                // LevelUp 상태에서 경험치 추가 안 되므로 Resume
                _gameManager.ResumeGame();
                yield return null;
            }

            Assert.AreEqual(3, levelUpCount);
            Assert.AreEqual(4, _expManager.CurrentLevel);
        }

        #endregion

        #region 경험치 오버플로우 테스트

        [UnityTest]
        public IEnumerator ExcessExperience_CarriesOver()
        {
            _gameManager.StartGame();
            yield return null;

            // 필요량보다 많은 경험치 추가
            int required = _expManager.ExpToNextLevel;
            int excess = 5;
            _expManager.AddExperience(required + excess);

            // 레벨업 후 초과 경험치가 보존되어야 함
            Assert.AreEqual(excess, _expManager.CurrentExp);
        }

        #endregion

        #region LevelUpManager 이벤트 테스트

        [UnityTest]
        public IEnumerator OnChoicesGenerated_EventExists()
        {
            bool canSubscribe = true;
            try
            {
                _levelUpManager.OnChoicesGenerated += (choices) => { };
            }
            catch
            {
                canSubscribe = false;
            }

            yield return null;

            Assert.IsTrue(canSubscribe);
        }

        [UnityTest]
        public IEnumerator OnChoiceApplied_EventExists()
        {
            bool canSubscribe = true;
            try
            {
                _levelUpManager.OnChoiceApplied += (choice) => { };
            }
            catch
            {
                canSubscribe = false;
            }

            yield return null;

            Assert.IsTrue(canSubscribe);
        }

        [UnityTest]
        public IEnumerator OnWeaponAdded_EventExists()
        {
            bool canSubscribe = true;
            try
            {
                _levelUpManager.OnWeaponAdded += (weapon, level) => { };
            }
            catch
            {
                canSubscribe = false;
            }

            yield return null;

            Assert.IsTrue(canSubscribe);
        }

        [UnityTest]
        public IEnumerator OnPassiveAdded_EventExists()
        {
            bool canSubscribe = true;
            try
            {
                _levelUpManager.OnPassiveAdded += (passive, level) => { };
            }
            catch
            {
                canSubscribe = false;
            }

            yield return null;

            Assert.IsTrue(canSubscribe);
        }

        #endregion
    }
}
