using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vs.Utility;

namespace Vs.TestRunner
{
    /// <summary>
    /// 인게임 테스트 러너.
    /// 런타임에서 테스트 시나리오를 실행하고 결과를 수집합니다.
    /// </summary>
    public class TestRunner : Singleton<TestRunner>
    {
        [Header("Settings")]
        [SerializeField] private bool _autoRunOnStart;
        [SerializeField] private bool _runInBackground = true;
        [SerializeField] private float _delayBetweenScenarios = 1f;

        [Header("Scenarios")]
        [SerializeField] private List<TestScenario> _scenarios = new();

        [Header("Debug")]
        [SerializeField] private bool _debugMode = true;

        private Queue<TestScenario> _scenarioQueue = new();
        private TestScenario _currentScenario;
        private bool _isRunning;
        private List<ScenarioResult> _allResults = new();

        #region Properties

        public bool IsRunning => _isRunning;
        public TestScenario CurrentScenario => _currentScenario;
        public IReadOnlyList<ScenarioResult> Results => _allResults;
        public IReadOnlyList<TestScenario> Scenarios => _scenarios;

        #endregion

        #region Events

        public event Action OnTestRunStarted;
        public event Action<List<ScenarioResult>> OnTestRunCompleted;
        public event Action<TestScenario> OnScenarioStarted;
        public event Action<ScenarioResult> OnScenarioCompleted;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // 자동으로 시나리오 수집
            if (_scenarios.Count == 0)
            {
                CollectScenarios();
            }

            if (_autoRunOnStart)
            {
                RunAllScenarios();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 씬의 모든 TestScenario 컴포넌트 수집
        /// </summary>
        public void CollectScenarios()
        {
            _scenarios.Clear();
            _scenarios.AddRange(FindObjectsByType<TestScenario>(FindObjectsSortMode.None));

            if (_debugMode)
            {
                Debug.Log($"[TestRunner] Collected {_scenarios.Count} scenarios");
            }
        }

        /// <summary>
        /// 시나리오 추가
        /// </summary>
        public void AddScenario(TestScenario scenario)
        {
            if (scenario != null && !_scenarios.Contains(scenario))
            {
                _scenarios.Add(scenario);
            }
        }

        /// <summary>
        /// 시나리오 제거
        /// </summary>
        public void RemoveScenario(TestScenario scenario)
        {
            _scenarios.Remove(scenario);
        }

        /// <summary>
        /// 모든 시나리오 실행
        /// </summary>
        public void RunAllScenarios()
        {
            if (_isRunning)
            {
                Debug.LogWarning("[TestRunner] Tests are already running");
                return;
            }

            if (_scenarios.Count == 0)
            {
                Debug.LogWarning("[TestRunner] No scenarios to run");
                return;
            }

            StartCoroutine(RunAllScenariosCoroutine());
        }

        /// <summary>
        /// 특정 시나리오만 실행
        /// </summary>
        public void RunScenario(TestScenario scenario)
        {
            if (_isRunning)
            {
                Debug.LogWarning("[TestRunner] Tests are already running");
                return;
            }

            if (scenario == null)
            {
                Debug.LogWarning("[TestRunner] Scenario is null");
                return;
            }

            StartCoroutine(RunSingleScenarioCoroutine(scenario));
        }

        /// <summary>
        /// 이름으로 시나리오 실행
        /// </summary>
        public void RunScenarioByName(string scenarioName)
        {
            var scenario = _scenarios.Find(s => s.ScenarioName == scenarioName);
            if (scenario != null)
            {
                RunScenario(scenario);
            }
            else
            {
                Debug.LogWarning($"[TestRunner] Scenario not found: {scenarioName}");
            }
        }

        /// <summary>
        /// 테스트 중지
        /// </summary>
        public void StopAll()
        {
            if (!_isRunning) return;

            StopAllCoroutines();

            if (_currentScenario != null)
            {
                _currentScenario.Stop();
            }

            _isRunning = false;
            _scenarioQueue.Clear();

            Debug.Log("[TestRunner] Tests stopped");
        }

        /// <summary>
        /// 결과 초기화
        /// </summary>
        public void ClearResults()
        {
            _allResults.Clear();
        }

        /// <summary>
        /// 결과 요약 로그 출력
        /// </summary>
        public void LogSummary()
        {
            if (_allResults.Count == 0)
            {
                Debug.Log("[TestRunner] No results to display");
                return;
            }

            int totalPassed = 0;
            int totalFailed = 0;
            int totalSkipped = 0;

            foreach (var result in _allResults)
            {
                totalPassed += result.PassedCount;
                totalFailed += result.FailedCount;
                totalSkipped += result.SkippedCount;
            }

            Debug.Log("=== Test Run Summary ===");
            Debug.Log($"Scenarios: {_allResults.Count}");
            Debug.Log($"Total Tests: {totalPassed + totalFailed + totalSkipped}");
            Debug.Log($"  Passed: {totalPassed}");
            Debug.Log($"  Failed: {totalFailed}");
            Debug.Log($"  Skipped: {totalSkipped}");
            Debug.Log("========================");
        }

        #endregion

        #region Private Methods

        private IEnumerator RunAllScenariosCoroutine()
        {
            _isRunning = true;
            _allResults.Clear();

            OnTestRunStarted?.Invoke();

            Debug.Log($"[TestRunner] Starting test run with {_scenarios.Count} scenarios");

            foreach (var scenario in _scenarios)
            {
                _scenarioQueue.Enqueue(scenario);
            }

            while (_scenarioQueue.Count > 0)
            {
                _currentScenario = _scenarioQueue.Dequeue();

                // 시나리오 이벤트 구독
                _currentScenario.OnScenarioCompleted += HandleScenarioCompleted;

                OnScenarioStarted?.Invoke(_currentScenario);
                _currentScenario.Run();

                // 시나리오 완료 대기
                yield return new WaitUntil(() => !_currentScenario.IsRunning);

                // 이벤트 구독 해제
                _currentScenario.OnScenarioCompleted -= HandleScenarioCompleted;

                // 시나리오 간 대기
                if (_scenarioQueue.Count > 0 && _delayBetweenScenarios > 0)
                {
                    yield return new WaitForSeconds(_delayBetweenScenarios);
                }
            }

            _isRunning = false;
            _currentScenario = null;

            LogSummary();
            OnTestRunCompleted?.Invoke(_allResults);
        }

        private IEnumerator RunSingleScenarioCoroutine(TestScenario scenario)
        {
            _isRunning = true;
            _currentScenario = scenario;

            OnTestRunStarted?.Invoke();

            // 시나리오 이벤트 구독
            scenario.OnScenarioCompleted += HandleScenarioCompleted;

            OnScenarioStarted?.Invoke(scenario);
            scenario.Run();

            // 시나리오 완료 대기
            yield return new WaitUntil(() => !scenario.IsRunning);

            // 이벤트 구독 해제
            scenario.OnScenarioCompleted -= HandleScenarioCompleted;

            _isRunning = false;
            _currentScenario = null;

            LogSummary();
            OnTestRunCompleted?.Invoke(_allResults);
        }

        private void HandleScenarioCompleted(TestScenario scenario, ScenarioResult result)
        {
            _allResults.Add(result);
            OnScenarioCompleted?.Invoke(result);
        }

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Run All Tests")]
        private void DebugRunAllTests()
        {
            RunAllScenarios();
        }

        [ContextMenu("Stop Tests")]
        private void DebugStopTests()
        {
            StopAll();
        }

        [ContextMenu("Log Summary")]
        private void DebugLogSummary()
        {
            LogSummary();
        }
#endif
    }
}
