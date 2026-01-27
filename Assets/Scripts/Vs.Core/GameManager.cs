using System;
using UnityEngine;
using Vs.Utility;

namespace Vs.Core
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        LevelUp,
        GameOver,
        Victory
    }

    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("Debug")]
        [SerializeField] private bool _debugMode;

        public GameState State { get; private set; } = GameState.Menu;
        public bool IsPlaying => State == GameState.Playing;
        public bool IsPaused => State == GameState.Paused || State == GameState.LevelUp;

        public event Action<GameState> OnStateChanged;
        public event Action OnGameStarted;
        public event Action<bool> OnGameEnded; // true = victory

        public void ChangeState(GameState newState)
        {
            if (State == newState) return;

            var previousState = State;
            State = newState;

            // 시간 스케일 관리
            Time.timeScale = newState switch
            {
                GameState.Paused or GameState.LevelUp or GameState.GameOver => 0f,
                _ => 1f
            };

            if (_debugMode)
            {
                Debug.Log($"[GameManager] State: {previousState} → {newState}");
            }

            OnStateChanged?.Invoke(newState);
        }

        public void StartGame()
        {
            if (State != GameState.Menu && State != GameState.GameOver && State != GameState.Victory)
            {
                Debug.LogWarning("[GameManager] Cannot start game from current state");
                return;
            }

            ChangeState(GameState.Playing);
            OnGameStarted?.Invoke();
        }

        public void PauseGame()
        {
            if (State != GameState.Playing) return;
            ChangeState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (State != GameState.Paused && State != GameState.LevelUp) return;
            ChangeState(GameState.Playing);
        }

        public void TriggerLevelUp()
        {
            if (State != GameState.Playing) return;
            ChangeState(GameState.LevelUp);
        }

        public void EndGame(bool isVictory)
        {
            if (State != GameState.Playing) return;

            ChangeState(isVictory ? GameState.Victory : GameState.GameOver);
            OnGameEnded?.Invoke(isVictory);
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Menu);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!_debugMode) return;

            GUI.Label(new Rect(10, 10, 200, 20), $"State: {State}");
            GUI.Label(new Rect(10, 30, 200, 20), $"TimeScale: {Time.timeScale}");
        }
#endif
    }
}
