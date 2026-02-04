using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vs.Core;
using Vs.Tests.Utilities;

namespace Vs.Integration.Tests
{
    /// <summary>
    /// GameManager 상태 전환 통합 테스트
    /// </summary>
    [TestFixture]
    [Category(TestConstants.CategoryIntegration)]
    public class GameStateTransitionTests : PlayModeTestBase
    {
        private GameManager _gameManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            // GameManager Singleton 생성
            var go = CreateEmptyGameObject("GameManager");
            _gameManager = go.AddComponent<GameManager>();
        }

        #region 초기 상태 테스트

        [UnityTest]
        public IEnumerator InitialState_IsMenu()
        {
            yield return null;
            Assert.AreEqual(GameState.Menu, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator InitialState_IsNotPlaying()
        {
            yield return null;
            Assert.IsFalse(_gameManager.IsPlaying);
        }

        [UnityTest]
        public IEnumerator InitialState_TimeScaleIsOne()
        {
            yield return null;
            Assert.AreEqual(1f, Time.timeScale);
        }

        #endregion

        #region StartGame 전환 테스트

        [UnityTest]
        public IEnumerator StartGame_ChangesStateToPlaying()
        {
            _gameManager.StartGame();
            yield return null;

            Assert.AreEqual(GameState.Playing, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator StartGame_IsPlayingReturnsTrue()
        {
            _gameManager.StartGame();
            yield return null;

            Assert.IsTrue(_gameManager.IsPlaying);
        }

        [UnityTest]
        public IEnumerator StartGame_TimeScaleRemainsOne()
        {
            _gameManager.StartGame();
            yield return null;

            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator StartGame_FiresOnGameStartedEvent()
        {
            bool eventFired = false;
            _gameManager.OnGameStarted += () => eventFired = true;

            _gameManager.StartGame();
            yield return null;

            Assert.IsTrue(eventFired);
        }

        [UnityTest]
        public IEnumerator StartGame_FiresOnStateChangedEvent()
        {
            GameState? receivedState = null;
            _gameManager.OnStateChanged += (state) => receivedState = state;

            _gameManager.StartGame();
            yield return null;

            Assert.AreEqual(GameState.Playing, receivedState);
        }

        #endregion

        #region PauseGame 전환 테스트

        [UnityTest]
        public IEnumerator PauseGame_ChangesStateToPaused()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.PauseGame();
            yield return null;

            Assert.AreEqual(GameState.Paused, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator PauseGame_IsPausedReturnsTrue()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.PauseGame();
            yield return null;

            Assert.IsTrue(_gameManager.IsPaused);
        }

        [UnityTest]
        public IEnumerator PauseGame_SetsTimeScaleToZero()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.PauseGame();
            yield return null;

            Assert.AreEqual(0f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator PauseGame_FromMenu_DoesNotChangesState()
        {
            _gameManager.PauseGame();
            yield return null;

            // Menu 상태에서는 Pause 불가
            Assert.AreEqual(GameState.Menu, _gameManager.State);
        }

        #endregion

        #region ResumeGame 전환 테스트

        [UnityTest]
        public IEnumerator ResumeGame_ChangesStateToPlaying()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.PauseGame();
            yield return null;

            _gameManager.ResumeGame();
            yield return null;

            Assert.AreEqual(GameState.Playing, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator ResumeGame_RestoresTimeScaleToOne()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.PauseGame();
            yield return null;

            _gameManager.ResumeGame();
            yield return null;

            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator ResumeGame_FromMenu_DoesNotChangesState()
        {
            _gameManager.ResumeGame();
            yield return null;

            // Menu 상태에서는 Resume 불가
            Assert.AreEqual(GameState.Menu, _gameManager.State);
        }

        #endregion

        #region LevelUp 전환 테스트

        [UnityTest]
        public IEnumerator TriggerLevelUp_ChangesStateToLevelUp()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            Assert.AreEqual(GameState.LevelUp, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator TriggerLevelUp_SetsTimeScaleToZero()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            Assert.AreEqual(0f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator TriggerLevelUp_IsPausedReturnsTrue()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            // LevelUp 상태도 IsPaused에 포함
            Assert.IsTrue(_gameManager.IsPaused);
        }

        [UnityTest]
        public IEnumerator ResumeGame_FromLevelUp_ChangesStateToPlaying()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.TriggerLevelUp();
            yield return null;

            _gameManager.ResumeGame();
            yield return null;

            Assert.AreEqual(GameState.Playing, _gameManager.State);
        }

        #endregion

        #region EndGame 전환 테스트

        [UnityTest]
        public IEnumerator EndGame_Victory_ChangesStateToVictory()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: true);
            yield return null;

            Assert.AreEqual(GameState.Victory, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator EndGame_Defeat_ChangesStateToGameOver()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: false);
            yield return null;

            Assert.AreEqual(GameState.GameOver, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator EndGame_FiresOnGameEndedEvent_WithCorrectValue()
        {
            bool? isVictoryResult = null;
            _gameManager.OnGameEnded += (isVictory) => isVictoryResult = isVictory;

            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: true);
            yield return null;

            Assert.IsTrue(isVictoryResult);
        }

        [UnityTest]
        public IEnumerator EndGame_FromMenu_DoesNotChangeState()
        {
            _gameManager.EndGame(isVictory: false);
            yield return null;

            // Menu 상태에서는 EndGame 불가
            Assert.AreEqual(GameState.Menu, _gameManager.State);
        }

        #endregion

        #region ReturnToMenu 전환 테스트

        [UnityTest]
        public IEnumerator ReturnToMenu_FromPlaying_ChangesStateToMenu()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.ReturnToMenu();
            yield return null;

            Assert.AreEqual(GameState.Menu, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator ReturnToMenu_FromGameOver_ChangesStateToMenu()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: false);
            yield return null;

            _gameManager.ReturnToMenu();
            yield return null;

            Assert.AreEqual(GameState.Menu, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator ReturnToMenu_RestoresTimeScaleToOne()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.PauseGame();
            yield return null;

            _gameManager.ReturnToMenu();
            yield return null;

            Assert.AreEqual(1f, Time.timeScale);
        }

        #endregion

        #region 연속 상태 전환 테스트

        [UnityTest]
        public IEnumerator FullGameCycle_Menu_Playing_Paused_Playing_Victory()
        {
            // Menu
            Assert.AreEqual(GameState.Menu, _gameManager.State);
            yield return null;

            // Menu -> Playing
            _gameManager.StartGame();
            yield return null;
            Assert.AreEqual(GameState.Playing, _gameManager.State);

            // Playing -> Paused
            _gameManager.PauseGame();
            yield return null;
            Assert.AreEqual(GameState.Paused, _gameManager.State);

            // Paused -> Playing
            _gameManager.ResumeGame();
            yield return null;
            Assert.AreEqual(GameState.Playing, _gameManager.State);

            // Playing -> Victory
            _gameManager.EndGame(isVictory: true);
            yield return null;
            Assert.AreEqual(GameState.Victory, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator FullGameCycle_WithLevelUp()
        {
            // Menu -> Playing
            _gameManager.StartGame();
            yield return null;
            Assert.AreEqual(GameState.Playing, _gameManager.State);

            // Playing -> LevelUp
            _gameManager.TriggerLevelUp();
            yield return null;
            Assert.AreEqual(GameState.LevelUp, _gameManager.State);

            // LevelUp -> Playing
            _gameManager.ResumeGame();
            yield return null;
            Assert.AreEqual(GameState.Playing, _gameManager.State);

            // Playing -> GameOver
            _gameManager.EndGame(isVictory: false);
            yield return null;
            Assert.AreEqual(GameState.GameOver, _gameManager.State);

            // GameOver -> Menu
            _gameManager.ReturnToMenu();
            yield return null;
            Assert.AreEqual(GameState.Menu, _gameManager.State);
        }

        [UnityTest]
        public IEnumerator Restart_AfterGameOver()
        {
            _gameManager.StartGame();
            yield return null;

            _gameManager.EndGame(isVictory: false);
            yield return null;

            _gameManager.ReturnToMenu();
            yield return null;

            // 재시작
            _gameManager.StartGame();
            yield return null;

            Assert.AreEqual(GameState.Playing, _gameManager.State);
            Assert.IsTrue(_gameManager.IsPlaying);
        }

        #endregion
    }
}
