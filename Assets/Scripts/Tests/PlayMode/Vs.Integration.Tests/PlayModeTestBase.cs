using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vs.Core;
using Vs.Tests.Utilities;

namespace Vs.Integration.Tests
{
    /// <summary>
    /// Play Mode 테스트의 기반 클래스.
    /// 비동기 유틸리티와 게임 상태 관리를 제공합니다.
    /// </summary>
    public abstract class PlayModeTestBase : TestBase
    {
        /// <summary>
        /// 지정된 프레임 수만큼 대기
        /// </summary>
        protected IEnumerator WaitForFrames(int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// 지정된 시간(초)만큼 대기
        /// </summary>
        protected IEnumerator WaitForSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// 지정된 시간(초)만큼 대기 (TimeScale 무시)
        /// </summary>
        protected IEnumerator WaitForSecondsRealtime(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
        }

        /// <summary>
        /// 조건이 true가 될 때까지 대기 (타임아웃 있음)
        /// </summary>
        protected IEnumerator WaitUntil(System.Func<bool> condition, float timeoutSeconds = 5f)
        {
            float elapsed = 0f;
            while (!condition() && elapsed < timeoutSeconds)
            {
                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }

            if (!condition())
            {
                Assert.Fail($"Condition not met within {timeoutSeconds} seconds");
            }
        }

        /// <summary>
        /// 조건이 false가 될 때까지 대기 (타임아웃 있음)
        /// </summary>
        protected IEnumerator WaitWhile(System.Func<bool> condition, float timeoutSeconds = 5f)
        {
            float elapsed = 0f;
            while (condition() && elapsed < timeoutSeconds)
            {
                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }

            if (condition())
            {
                Assert.Fail($"Condition still true after {timeoutSeconds} seconds");
            }
        }

        /// <summary>
        /// GameManager가 존재하면 상태를 Menu로 초기화
        /// </summary>
        protected void ResetGameState()
        {
            Time.timeScale = 1f;

            if (GameManager.HasInstance)
            {
                GameManager.Instance.ReturnToMenu();
            }
        }

        /// <summary>
        /// 게임을 시작하고 Playing 상태가 될 때까지 대기
        /// </summary>
        protected IEnumerator StartGameAndWait()
        {
            if (!GameManager.HasInstance)
            {
                yield break;
            }

            GameManager.Instance.StartGame();
            yield return WaitUntil(() => GameManager.Instance.IsPlaying, 2f);
        }

        /// <summary>
        /// 게임을 일시정지하고 상태 전환 대기
        /// </summary>
        protected IEnumerator PauseGameAndWait()
        {
            if (!GameManager.HasInstance)
            {
                yield break;
            }

            GameManager.Instance.PauseGame();
            yield return WaitUntil(() => GameManager.Instance.IsPaused, 2f);
        }

        /// <summary>
        /// 게임을 재개하고 상태 전환 대기
        /// </summary>
        protected IEnumerator ResumeGameAndWait()
        {
            if (!GameManager.HasInstance)
            {
                yield break;
            }

            GameManager.Instance.ResumeGame();
            yield return WaitUntil(() => GameManager.Instance.IsPlaying, 2f);
        }

        [TearDown]
        public override void TearDown()
        {
            ResetGameState();
            base.TearDown();
        }
    }
}
