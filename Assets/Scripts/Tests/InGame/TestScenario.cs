using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.TestRunner
{
    /// <summary>
    /// 테스트 결과 상태
    /// </summary>
    public enum TestResult
    {
        NotRun,
        Running,
        Passed,
        Failed,
        Skipped,
        Timeout
    }

    /// <summary>
    /// 개별 테스트 케이스 결과
    /// </summary>
    [Serializable]
    public class TestCaseResult
    {
        public string Name;
        public TestResult Result;
        public string Message;
        public float Duration;
        public DateTime Timestamp;

        public TestCaseResult(string name)
        {
            Name = name;
            Result = TestResult.NotRun;
            Message = string.Empty;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// 시나리오 실행 결과
    /// </summary>
    [Serializable]
    public class ScenarioResult
    {
        public string ScenarioName;
        public TestResult OverallResult;
        public List<TestCaseResult> TestCases = new();
        public float TotalDuration;
        public DateTime StartTime;
        public DateTime EndTime;

        public int PassedCount => TestCases.FindAll(t => t.Result == TestResult.Passed).Count;
        public int FailedCount => TestCases.FindAll(t => t.Result == TestResult.Failed).Count;
        public int SkippedCount => TestCases.FindAll(t => t.Result == TestResult.Skipped).Count;
    }

    /// <summary>
    /// 테스트 시나리오 기반 클래스.
    /// 런타임 테스트 시나리오를 정의하는 추상 클래스입니다.
    /// </summary>
    public abstract class TestScenario : MonoBehaviour
    {
        [Header("Scenario Info")]
        [SerializeField] protected string _scenarioName;
        [SerializeField] protected string _description;
        [SerializeField] protected float _timeout = 60f;

        protected ScenarioResult _result;
        protected TestCaseResult _currentTest;
        protected bool _isRunning;
        protected float _startTime;

        #region Properties

        public string ScenarioName => _scenarioName;
        public string Description => _description;
        public float Timeout => _timeout;
        public bool IsRunning => _isRunning;
        public ScenarioResult Result => _result;

        #endregion

        #region Events

        public event Action<TestScenario> OnScenarioStarted;
        public event Action<TestScenario, ScenarioResult> OnScenarioCompleted;
        public event Action<TestCaseResult> OnTestCaseCompleted;

        #endregion

        #region Public Methods

        /// <summary>
        /// 시나리오 실행
        /// </summary>
        public void Run()
        {
            if (_isRunning)
            {
                Debug.LogWarning($"[TestScenario] {_scenarioName} is already running");
                return;
            }

            StartCoroutine(RunScenarioCoroutine());
        }

        /// <summary>
        /// 시나리오 중지
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;

            StopAllCoroutines();
            _isRunning = false;

            if (_result != null)
            {
                _result.OverallResult = TestResult.Skipped;
                _result.EndTime = DateTime.Now;
                OnScenarioCompleted?.Invoke(this, _result);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 시나리오 실행 코루틴
        /// </summary>
        private IEnumerator RunScenarioCoroutine()
        {
            _isRunning = true;
            _startTime = Time.realtimeSinceStartup;

            _result = new ScenarioResult
            {
                ScenarioName = _scenarioName,
                OverallResult = TestResult.Running,
                StartTime = DateTime.Now
            };

            OnScenarioStarted?.Invoke(this);

            Debug.Log($"[TestScenario] Starting: {_scenarioName}");

            // 시나리오별 테스트 실행
            yield return ExecuteTests();

            // 결과 집계
            _result.EndTime = DateTime.Now;
            _result.TotalDuration = Time.realtimeSinceStartup - _startTime;

            if (_result.FailedCount > 0)
            {
                _result.OverallResult = TestResult.Failed;
            }
            else if (_result.PassedCount == _result.TestCases.Count)
            {
                _result.OverallResult = TestResult.Passed;
            }
            else
            {
                _result.OverallResult = TestResult.Skipped;
            }

            _isRunning = false;

            Debug.Log($"[TestScenario] Completed: {_scenarioName} - {_result.OverallResult} " +
                      $"(Passed: {_result.PassedCount}, Failed: {_result.FailedCount}, Skipped: {_result.SkippedCount})");

            OnScenarioCompleted?.Invoke(this, _result);
        }

        /// <summary>
        /// 테스트 케이스들 실행 (서브클래스에서 구현)
        /// </summary>
        protected abstract IEnumerator ExecuteTests();

        /// <summary>
        /// 테스트 케이스 시작
        /// </summary>
        protected void BeginTest(string testName)
        {
            _currentTest = new TestCaseResult(testName)
            {
                Result = TestResult.Running
            };

            Debug.Log($"  [Test] Running: {testName}");
        }

        /// <summary>
        /// 테스트 케이스 성공
        /// </summary>
        protected void PassTest(string message = null)
        {
            if (_currentTest == null) return;

            _currentTest.Result = TestResult.Passed;
            _currentTest.Message = message ?? "Passed";
            _currentTest.Duration = Time.realtimeSinceStartup - _startTime;

            _result.TestCases.Add(_currentTest);
            OnTestCaseCompleted?.Invoke(_currentTest);

            Debug.Log($"  [Test] PASSED: {_currentTest.Name}");
            _currentTest = null;
        }

        /// <summary>
        /// 테스트 케이스 실패
        /// </summary>
        protected void FailTest(string message)
        {
            if (_currentTest == null) return;

            _currentTest.Result = TestResult.Failed;
            _currentTest.Message = message;
            _currentTest.Duration = Time.realtimeSinceStartup - _startTime;

            _result.TestCases.Add(_currentTest);
            OnTestCaseCompleted?.Invoke(_currentTest);

            Debug.LogError($"  [Test] FAILED: {_currentTest.Name} - {message}");
            _currentTest = null;
        }

        /// <summary>
        /// 테스트 케이스 스킵
        /// </summary>
        protected void SkipTest(string reason)
        {
            if (_currentTest == null) return;

            _currentTest.Result = TestResult.Skipped;
            _currentTest.Message = reason;
            _currentTest.Duration = 0;

            _result.TestCases.Add(_currentTest);
            OnTestCaseCompleted?.Invoke(_currentTest);

            Debug.LogWarning($"  [Test] SKIPPED: {_currentTest.Name} - {reason}");
            _currentTest = null;
        }

        /// <summary>
        /// 조건 검증
        /// </summary>
        protected bool Assert(bool condition, string failMessage)
        {
            if (!condition)
            {
                FailTest(failMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 값 동일 검증
        /// </summary>
        protected bool AssertEqual<T>(T expected, T actual, string testName) where T : IEquatable<T>
        {
            if (!expected.Equals(actual))
            {
                FailTest($"Expected {expected}, but got {actual}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// null이 아님 검증
        /// </summary>
        protected bool AssertNotNull(object obj, string objectName)
        {
            if (obj == null)
            {
                FailTest($"{objectName} is null");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 대기 (타임아웃 포함)
        /// </summary>
        protected IEnumerator WaitForCondition(Func<bool> condition, float timeout, string timeoutMessage)
        {
            float elapsed = 0f;
            while (!condition() && elapsed < timeout)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            if (!condition())
            {
                FailTest(timeoutMessage);
            }
        }

        #endregion
    }
}
