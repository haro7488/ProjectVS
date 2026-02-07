using UnityEngine;
using Vs.Core;
using Vs.Enemy;
using Vs.Combat;

namespace Vs.Debug
{
    /// <summary>
    /// 치트 디버그 탭.
    /// 시간 배속, 무적 모드, 게임 종료 트리거 등 치트 기능 제공.
    /// </summary>
    public class CheatDebugTab : DebugTabBase
    {
        private DebugController _debugController;
        private GameManager _gameManager;

        public override void RefreshContent()
        {
            ClearContent();

            _debugController = DebugController.HasInstance ? DebugController.Instance : null;
            _gameManager = GameManager.HasInstance ? GameManager.Instance : null;

            // 현재 상태 표시
            string godModeStatus = _debugController != null ? (_debugController.GodMode ? "ON" : "OFF") : "N/A";
            string timeStatus = $"{Time.timeScale}x";
            CreateButton($"God Mode: {godModeStatus} | Time: {timeStatus}", RefreshContent);

            // 구분선
            CreateButton("--- Time Scale ---", null);

            // 시간 배속 버튼
            CreateButton("0.5x Speed", () => SetTimeScale(0.5f));
            CreateButton("1x Speed (Normal)", () => SetTimeScale(1f));
            CreateButton("2x Speed", () => SetTimeScale(2f));
            CreateButton("4x Speed", () => SetTimeScale(4f));

            // 구분선
            CreateButton("--- Cheats ---", null);

            // 무적 모드 토글
            string godModeLabel = _debugController != null && _debugController.GodMode
                ? "Disable God Mode"
                : "Enable God Mode";
            CreateButton(godModeLabel, OnToggleGodMode);

            // 구분선
            CreateButton("--- Game End ---", null);

            // 게임 종료 트리거
            CreateButton("Trigger Victory", OnTriggerVictory);
            CreateButton("Trigger Defeat", OnTriggerDefeat);

            // 구분선
            CreateButton("--- Combat ---", null);

            // 모든 적 처치
            CreateButton("Kill All Enemies", OnKillAllEnemies);
        }

        private void SetTimeScale(float scale)
        {
            if (_debugController != null)
            {
                _debugController.SetTimeScale(scale);
            }
            else
            {
                Time.timeScale = scale;
                UnityEngine.Debug.Log($"[CheatDebugTab] Time Scale: {scale}x");
            }

            RefreshContent();
        }

        private void OnToggleGodMode()
        {
            if (_debugController == null)
            {
                UnityEngine.Debug.LogWarning("[CheatDebugTab] DebugController not found");
                return;
            }

            _debugController.ToggleGodMode();
            RefreshContent();
        }

        private void OnTriggerVictory()
        {
            if (_gameManager == null)
            {
                UnityEngine.Debug.LogWarning("[CheatDebugTab] GameManager not found");
                return;
            }

            _gameManager.EndGame(true);
            UnityEngine.Debug.Log("[CheatDebugTab] Victory Triggered");
        }

        private void OnTriggerDefeat()
        {
            if (_gameManager == null)
            {
                UnityEngine.Debug.LogWarning("[CheatDebugTab] GameManager not found");
                return;
            }

            _gameManager.EndGame(false);
            UnityEngine.Debug.Log("[CheatDebugTab] Defeat Triggered");
        }

        private void OnKillAllEnemies()
        {
            var enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

            if (enemies.Length == 0)
            {
                UnityEngine.Debug.Log("[CheatDebugTab] No enemies found");
                return;
            }

            int killCount = 0;
            foreach (var enemy in enemies)
            {
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.TakeDamage(new DamageInfo(99999f));
                    killCount++;
                }
            }

            UnityEngine.Debug.Log($"[CheatDebugTab] Killed {killCount} enemies");
        }
    }
}
