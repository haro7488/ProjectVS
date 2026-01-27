using System;
using UnityEngine;
using Vs.Utility;

namespace Vs.Core
{
    public class TimeManager : Singleton<TimeManager>
    {
        [Header("Settings")] [SerializeField] private float _stageDuration = Constants.DefaultStageDuration;

        private float _elapsedTime;
        private bool _isRunning;

        public float ElapsedTime => _elapsedTime;
        public float RemainingTime => Mathf.Max(0f, _stageDuration - _elapsedTime);
        public float Progress => Mathf.Clamp01(_elapsedTime / _stageDuration);
        public bool IsTimeUp => _elapsedTime >= _stageDuration;

        public string ElapsedTimeFormatted => FormatTime(_elapsedTime);
        public string RemainingTimeFormatted => FormatTime(RemainingTime);

        public event Action<float> OnTimeChanged;
        public event Action OnTimeUp;

        private void Start()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged += HandleGameStateChanged;
                GameManager.Instance.OnGameStarted += ResetTimer;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
                GameManager.Instance.OnGameStarted -= ResetTimer;
            }
        }

        private void Update()
        {
            if (!_isRunning) return;

            _elapsedTime += Time.deltaTime;
            OnTimeChanged?.Invoke(_elapsedTime);

            if (IsTimeUp)
            {
                _isRunning = false;
                OnTimeUp?.Invoke();
            }
        }

        public void ResetTimer()
        {
            _elapsedTime = 0f;
            _isRunning = true;
        }

        public void SetStageDuration(float duration)
        {
            _stageDuration = duration;
        }

        private void HandleGameStateChanged(GameState state)
        {
            _isRunning = state == GameState.Playing;
        }

        private static string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }
    }
}