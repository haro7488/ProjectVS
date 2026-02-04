using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Vs.TestRunner
{
    /// <summary>
    /// 테스트 결과 리포터.
    /// 테스트 결과를 다양한 형식으로 출력합니다.
    /// </summary>
    public class TestReporter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _autoReport = true;
        [SerializeField] private bool _saveToFile;
        [SerializeField] private string _reportPath = "TestReports";

        [Header("Output Format")]
        [SerializeField] private ReportFormat _format = ReportFormat.Console;

        public enum ReportFormat
        {
            Console,
            Json,
            Markdown
        }

        private void OnEnable()
        {
            if (TestRunner.HasInstance)
            {
                TestRunner.Instance.OnTestRunCompleted += HandleTestRunCompleted;
            }
        }

        private void OnDisable()
        {
            if (TestRunner.HasInstance)
            {
                TestRunner.Instance.OnTestRunCompleted -= HandleTestRunCompleted;
            }
        }

        private void HandleTestRunCompleted(List<ScenarioResult> results)
        {
            if (!_autoReport) return;

            GenerateReport(results);
        }

        /// <summary>
        /// 리포트 생성
        /// </summary>
        public void GenerateReport(List<ScenarioResult> results)
        {
            string report = _format switch
            {
                ReportFormat.Console => GenerateConsoleReport(results),
                ReportFormat.Json => GenerateJsonReport(results),
                ReportFormat.Markdown => GenerateMarkdownReport(results),
                _ => GenerateConsoleReport(results)
            };

            if (_saveToFile)
            {
                SaveReportToFile(report);
            }

            Debug.Log(report);
        }

        #region Report Generators

        private string GenerateConsoleReport(List<ScenarioResult> results)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine("╔════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║               IN-GAME TEST REPORT                          ║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════╣");

            int totalPassed = 0, totalFailed = 0, totalSkipped = 0;
            float totalDuration = 0f;

            foreach (var scenario in results)
            {
                totalPassed += scenario.PassedCount;
                totalFailed += scenario.FailedCount;
                totalSkipped += scenario.SkippedCount;
                totalDuration += scenario.TotalDuration;

                string status = scenario.OverallResult switch
                {
                    TestResult.Passed => "[PASS]",
                    TestResult.Failed => "[FAIL]",
                    _ => "[SKIP]"
                };

                sb.AppendLine($"║ {status} {scenario.ScenarioName,-40} {scenario.TotalDuration:F2}s ║");

                foreach (var test in scenario.TestCases)
                {
                    string testStatus = test.Result switch
                    {
                        TestResult.Passed => "  ✓",
                        TestResult.Failed => "  ✗",
                        _ => "  ○"
                    };

                    sb.AppendLine($"║   {testStatus} {test.Name,-47} ║");

                    if (test.Result == TestResult.Failed)
                    {
                        sb.AppendLine($"║       {test.Message,-45} ║");
                    }
                }
            }

            sb.AppendLine("╠════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║ SUMMARY                                                    ║");
            sb.AppendLine($"║   Scenarios: {results.Count,-47} ║");
            sb.AppendLine($"║   Passed:    {totalPassed,-47} ║");
            sb.AppendLine($"║   Failed:    {totalFailed,-47} ║");
            sb.AppendLine($"║   Skipped:   {totalSkipped,-47} ║");
            sb.AppendLine($"║   Duration:  {totalDuration:F2}s{new string(' ', 44 - totalDuration.ToString("F2").Length)} ║");
            sb.AppendLine("╚════════════════════════════════════════════════════════════╝");

            return sb.ToString();
        }

        private string GenerateJsonReport(List<ScenarioResult> results)
        {
            var report = new TestRunReport
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Scenarios = results
            };

            return JsonUtility.ToJson(report, true);
        }

        private string GenerateMarkdownReport(List<ScenarioResult> results)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# In-Game Test Report");
            sb.AppendLine();
            sb.AppendLine($"**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            int totalPassed = 0, totalFailed = 0, totalSkipped = 0;

            foreach (var scenario in results)
            {
                totalPassed += scenario.PassedCount;
                totalFailed += scenario.FailedCount;
                totalSkipped += scenario.SkippedCount;
            }

            sb.AppendLine("## Summary");
            sb.AppendLine();
            sb.AppendLine($"| Metric | Value |");
            sb.AppendLine($"|--------|-------|");
            sb.AppendLine($"| Scenarios | {results.Count} |");
            sb.AppendLine($"| Passed | {totalPassed} |");
            sb.AppendLine($"| Failed | {totalFailed} |");
            sb.AppendLine($"| Skipped | {totalSkipped} |");
            sb.AppendLine();

            sb.AppendLine("## Scenarios");
            sb.AppendLine();

            foreach (var scenario in results)
            {
                string emoji = scenario.OverallResult switch
                {
                    TestResult.Passed => "✅",
                    TestResult.Failed => "❌",
                    _ => "⏭️"
                };

                sb.AppendLine($"### {emoji} {scenario.ScenarioName}");
                sb.AppendLine();
                sb.AppendLine($"- **Duration:** {scenario.TotalDuration:F2}s");
                sb.AppendLine($"- **Result:** {scenario.OverallResult}");
                sb.AppendLine();

                if (scenario.TestCases.Count > 0)
                {
                    sb.AppendLine("| Test | Result | Message |");
                    sb.AppendLine("|------|--------|---------|");

                    foreach (var test in scenario.TestCases)
                    {
                        string testEmoji = test.Result switch
                        {
                            TestResult.Passed => "✓",
                            TestResult.Failed => "✗",
                            _ => "○"
                        };

                        sb.AppendLine($"| {test.Name} | {testEmoji} {test.Result} | {test.Message} |");
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        #endregion

        #region File Output

        private void SaveReportToFile(string report)
        {
            try
            {
                string directory = Path.Combine(Application.persistentDataPath, _reportPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string extension = _format switch
                {
                    ReportFormat.Json => "json",
                    ReportFormat.Markdown => "md",
                    _ => "txt"
                };

                string filename = $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}";
                string path = Path.Combine(directory, filename);

                File.WriteAllText(path, report);

                Debug.Log($"[TestReporter] Report saved to: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[TestReporter] Failed to save report: {e.Message}");
            }
        }

        #endregion

        [Serializable]
        private class TestRunReport
        {
            public string Timestamp;
            public List<ScenarioResult> Scenarios;
        }
    }
}
