using System;
using UnityEngine;
using Vs.Core;
using Vs.Utility;

namespace Vs.Progression
{
    /// <summary>
    /// 경험치 및 레벨 관리.
    /// 경험치 획득 시 레벨업 체크 및 GameManager 상태 전환 트리거.
    /// </summary>
    public class ExperienceManager : Singleton<ExperienceManager>
    {
        [Header("현재 상태")] [SerializeField] private int _currentExp;
        [SerializeField] private int _currentLevel = 1;
        [SerializeField] private int _expToNextLevel;

        [Header("레벨업 공식")] [SerializeField] private int _baseExpRequired = 10;
        [SerializeField] private float _expGrowthRate = 1.2f;

        [Header("Debug")] [SerializeField] private bool _debugMode;

        // 프로퍼티
        public int CurrentExp => _currentExp;
        public int CurrentLevel => _currentLevel;
        public int ExpToNextLevel => _expToNextLevel;
        public float ExpProgress => _expToNextLevel > 0 ? (float)_currentExp / _expToNextLevel : 0f;
        public int TotalExpGained { get; private set; }

        // 이벤트
        public event Action<int> OnExpGained;
        public event Action<int> OnLevelUp;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void OnEnable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnGameStarted += HandleGameStarted;
            }
        }

        private void OnDisable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnGameStarted -= HandleGameStarted;
            }
        }

        private void Initialize()
        {
            _expToNextLevel = CalculateExpRequired(_currentLevel);
        }

        private void HandleGameStarted()
        {
            Reset();
        }

        /// <summary>
        /// 경험치 추가. 레벨업 조건 충족 시 자동 레벨업 처리.
        /// </summary>
        public void AddExperience(int amount)
        {
            if (amount <= 0) return;
            if (!GameManager.HasInstance || !GameManager.Instance.IsPlaying) return;

            _currentExp += amount;
            TotalExpGained += amount;

            if (_debugMode)
            {
                Debug.Log($"[ExperienceManager] +{amount} EXP (Total: {_currentExp}/{_expToNextLevel})");
            }

            OnExpGained?.Invoke(amount);
            CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            while (_currentExp >= _expToNextLevel)
            {
                _currentExp -= _expToNextLevel;
                _currentLevel++;
                _expToNextLevel = CalculateExpRequired(_currentLevel);

                if (_debugMode)
                {
                    Debug.Log($"[ExperienceManager] Level Up! Lv.{_currentLevel} (Next: {_expToNextLevel} EXP)");
                }

                OnLevelUp?.Invoke(_currentLevel);

                // 레벨업 상태로 전환
                if (GameManager.HasInstance)
                {
                    GameManager.Instance.TriggerLevelUp();
                }
            }
        }

        /// <summary>
        /// 레벨에 필요한 경험치 계산.
        /// 공식: baseExp * (growthRate ^ (level - 1))
        /// </summary>
        private int CalculateExpRequired(int level)
        {
            return Mathf.RoundToInt(_baseExpRequired * Mathf.Pow(_expGrowthRate, level - 1));
        }

        /// <summary>
        /// 경험치 및 레벨 초기화.
        /// </summary>
        public void Reset()
        {
            _currentExp = 0;
            _currentLevel = 1;
            TotalExpGained = 0;
            _expToNextLevel = CalculateExpRequired(_currentLevel);

            if (_debugMode)
            {
                Debug.Log("[ExperienceManager] Reset");
            }
        }

        /// <summary>
        /// 특정 레벨의 필요 경험치 조회 (UI용).
        /// </summary>
        public int GetExpRequiredForLevel(int level)
        {
            return CalculateExpRequired(level);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!_debugMode) return;

            GUI.Label(new Rect(10, 50, 300, 20), $"Level: {_currentLevel}");
            GUI.Label(new Rect(10, 70, 300, 20), $"EXP: {_currentExp}/{_expToNextLevel} ({ExpProgress * 100:F1}%)");
        }

        [ContextMenu("Add 10 EXP")]
        private void DebugAddExp()
        {
            AddExperience(10);
        }

        [ContextMenu("Force Level Up")]
        private void DebugLevelUp()
        {
            AddExperience(_expToNextLevel - _currentExp);
        }
#endif
    }
}