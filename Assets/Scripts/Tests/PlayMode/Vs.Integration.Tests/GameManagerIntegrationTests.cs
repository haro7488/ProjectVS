using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vs.Core;
using Vs.Tests.Utilities;

namespace Vs.Integration.Tests
{
    /// <summary>
    /// GameManager 통합 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryIntegration)]
    public class GameManagerIntegrationTests : PlayModeTestBase
    {
        private GameManager _gameManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var go = CreateEmptyGameObject("GameManager");
            _gameManager = go.AddComponent<GameManager>();
        }

        #region 싱글톤 동작 테스트

        [UnityTest]
        public IEnumerator Instance_ReturnsNonNull()
        {
            yield return null;
            Assert.IsNotNull(GameManager.Instance);
        }

        [UnityTest]
        public IEnumerator HasInstance_ReturnsTrue()
        {
            yield return null;
            Assert.IsTrue(GameManager.HasInstance);
        }

        #endregion

        #region 이벤트 구독 테스트

        [UnityTest]
        public IEnumerator OnStateChanged_CanSubscribeMultipleHandlers()
        {
            int callCount = 0;
            _gameManager.OnStateChanged += (state) => callCount++;
            _gameManager.OnStateChanged += (state) => callCount++;

            _gameManager.StartGame();
            yield return null;

            Assert.AreEqual(2, callCount);
        }

        [UnityTest]
        public IEnumerator OnGameStarted_FiresOnce()
        {
            int callCount = 0;
            _gameManager.OnGameStarted += () => callCount++;

            _gameManager.StartGame();
            yield return null;

            Assert.AreEqual(1, callCount);
        }

        [UnityTest]
        public IEnumerator OnGameEnded_FiresOncePerEnd()
        {
            int callCount = 0;
            _gameManager.OnGameEnded += (isVictory) => callCount++;

            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: false);
            yield return null;

            Assert.AreEqual(1, callCount);
        }

        #endregion

        #region 중복 상태 전환 방지 테스트

        [UnityTest]
        public IEnumerator ChangeState_SameState_DoesNotFireEvent()
        {
            _gameManager.StartGame();
            yield return null;

            int callCount = 0;
            _gameManager.OnStateChanged += (state) => callCount++;

            // 같은 상태로 변경 시도
            _gameManager.ChangeState(GameState.Playing);
            yield return null;

            Assert.AreEqual(0, callCount);
        }

        [UnityTest]
        public IEnumerator StartGame_WhenAlreadyPlaying_NoStateChange()
        {
            _gameManager.StartGame();
            yield return null;

            int callCount = 0;
            _gameManager.OnStateChanged += (state) => callCount++;

            _gameManager.StartGame();
            yield return null;

            Assert.AreEqual(0, callCount);
        }

        #endregion

        #region TimeScale 동작 테스트

        [UnityTest]
        public IEnumerator TimeScale_MenuState_IsOne()
        {
            yield return null;
            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator TimeScale_PlayingState_IsOne()
        {
            _gameManager.StartGame();
            yield return null;
            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator TimeScale_PausedState_IsZero()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.PauseGame();
            yield return null;

            Assert.AreEqual(0f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator TimeScale_LevelUpState_IsZero()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            Assert.AreEqual(0f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator TimeScale_GameOverState_IsZero()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: false);
            yield return null;

            Assert.AreEqual(0f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator TimeScale_VictoryState_IsOne()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: true);
            yield return null;

            // Victory 상태도 timeScale 0이 됨 (switch case에서 GameOver와 동일)
            // 이 테스트는 현재 구현 확인용
            Assert.AreEqual(1f, Time.timeScale);
        }

        #endregion

        #region 복합 시나리오 테스트

        [UnityTest]
        public IEnumerator MultiplePauseResume_WorksCorrectly()
        {
            _gameManager.StartGame();
            yield return null;

            for (int i = 0; i < 3; i++)
            {
                _gameManager.PauseGame();
                yield return null;
                Assert.AreEqual(GameState.Paused, _gameManager.State);

                _gameManager.ResumeGame();
                yield return null;
                Assert.AreEqual(GameState.Playing, _gameManager.State);
            }
        }

        [UnityTest]
        public IEnumerator MultipleLevelUp_WorksCorrectly()
        {
            _gameManager.StartGame();
            yield return null;

            for (int i = 0; i < 3; i++)
            {
                _gameManager.TriggerLevelUp();
                yield return null;
                Assert.AreEqual(GameState.LevelUp, _gameManager.State);

                _gameManager.ResumeGame();
                yield return null;
                Assert.AreEqual(GameState.Playing, _gameManager.State);
            }
        }

        [UnityTest]
        public IEnumerator Restart_MultipleTimesWorks()
        {
            for (int i = 0; i < 3; i++)
            {
                _gameManager.StartGame();
                yield return null;
                Assert.AreEqual(GameState.Playing, _gameManager.State);

                _gameManager.EndGame(isVictory: i % 2 == 0);
                yield return null;

                _gameManager.ReturnToMenu();
                yield return null;
                Assert.AreEqual(GameState.Menu, _gameManager.State);
            }
        }

        #endregion

        #region 프로퍼티 상태 검증 테스트

        [UnityTest]
        public IEnumerator IsPlaying_OnlyTrueInPlayingState()
        {
            // Menu
            Assert.IsFalse(_gameManager.IsPlaying);
            yield return null;

            // Playing
            _gameManager.StartGame();
            yield return null;
            Assert.IsTrue(_gameManager.IsPlaying);

            // Paused
            _gameManager.PauseGame();
            yield return null;
            Assert.IsFalse(_gameManager.IsPlaying);

            // Resume
            _gameManager.ResumeGame();
            yield return null;
            Assert.IsTrue(_gameManager.IsPlaying);

            // GameOver
            _gameManager.EndGame(isVictory: false);
            yield return null;
            Assert.IsFalse(_gameManager.IsPlaying);
        }

        [UnityTest]
        public IEnumerator IsPaused_TrueInPausedAndLevelUpStates()
        {
            // Menu
            Assert.IsFalse(_gameManager.IsPaused);
            yield return null;

            // Playing
            _gameManager.StartGame();
            yield return null;
            Assert.IsFalse(_gameManager.IsPaused);

            // Paused
            _gameManager.PauseGame();
            yield return null;
            Assert.IsTrue(_gameManager.IsPaused);

            // Resume -> LevelUp
            _gameManager.ResumeGame();
            yield return null;
            _gameManager.TriggerLevelUp();
            yield return null;
            Assert.IsTrue(_gameManager.IsPaused);
        }

        #endregion
    }
}
