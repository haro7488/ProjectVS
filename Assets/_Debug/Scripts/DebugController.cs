using UnityEngine;
using Vs.Utility;
using Vs.Core;
using Vs.Progression;
using Vs.Player;

namespace Vs.Debug
{
    /// <summary>
    /// 디버그 시스템 핵심 컨트롤러.
    /// 키보드 단축키 처리 및 디버그 패널 관리.
    /// </summary>
    public class DebugController : Singleton<DebugController>
    {
        [Header("References")]
        [SerializeField] private DebugPanel _panel;

        [Header("Settings")]
        [SerializeField] private KeyCode _toggleKey = KeyCode.F1;
        [SerializeField] private KeyCode _godModeKey = KeyCode.F2;
        [SerializeField] private KeyCode _timeScaleKey = KeyCode.F3;
        [SerializeField] private KeyCode _levelUpKey = KeyCode.F4;

        [Header("State")]
        [SerializeField] private bool _godMode;
        [SerializeField] private int _timeScaleIndex;

        private readonly float[] _timeScales = { 1f, 2f, 4f, 0.5f };

        public bool GodMode => _godMode;

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                TogglePanel();
            }

            if (Input.GetKeyDown(_godModeKey))
            {
                ToggleGodMode();
            }

            if (Input.GetKeyDown(_timeScaleKey))
            {
                CycleTimeScale();
            }

            if (Input.GetKeyDown(_levelUpKey))
            {
                TriggerLevelUp();
            }
        }

        public void TogglePanel()
        {
            if (_panel != null)
            {
                _panel.Toggle();
            }
        }

        public void ToggleGodMode()
        {
            _godMode = !_godMode;

            var playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetInvincible(_godMode);
            }

            UnityEngine.Debug.Log($"[Debug] God Mode: {(_godMode ? "ON" : "OFF")}");
        }

        public void CycleTimeScale()
        {
            _timeScaleIndex = (_timeScaleIndex + 1) % _timeScales.Length;
            float newScale = _timeScales[_timeScaleIndex];
            Time.timeScale = newScale;
            UnityEngine.Debug.Log($"[Debug] Time Scale: {newScale}x");
        }

        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
            UnityEngine.Debug.Log($"[Debug] Time Scale: {scale}x");
        }

        public void TriggerLevelUp()
        {
            if (GameManager.HasInstance && GameManager.Instance.State == GameState.Playing)
            {
                GameManager.Instance.TriggerLevelUp();
                UnityEngine.Debug.Log("[Debug] Level Up Triggered");
            }
        }

        public void TriggerVictory()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.EndGame(true);
                UnityEngine.Debug.Log("[Debug] Victory Triggered");
            }
        }

        public void TriggerGameOver()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.EndGame(false);
                UnityEngine.Debug.Log("[Debug] Game Over Triggered");
            }
        }

        public void SetPanel(DebugPanel panel)
        {
            _panel = panel;
        }
    }
}
