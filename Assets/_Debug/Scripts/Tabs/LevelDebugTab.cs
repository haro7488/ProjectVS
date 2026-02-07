using UnityEngine;
using Vs.Progression;

namespace Vs.Debug
{
    /// <summary>
    /// 레벨/경험치 디버그 탭.
    /// 경험치 추가, 레벨업 트리거, 레벨 설정 등 레벨 관련 디버그 기능 제공.
    /// </summary>
    public class LevelDebugTab : DebugTabBase
    {
        [Header("Level Settings")]
        [SerializeField] private int _maxLevel = 50;

        private ExperienceManager _experienceManager;

        public override void RefreshContent()
        {
            ClearContent();

            _experienceManager = ExperienceManager.HasInstance ? ExperienceManager.Instance : null;

            if (_experienceManager == null)
            {
                UnityEngine.Debug.LogWarning("[LevelDebugTab] ExperienceManager not found");
                CreateButton("ExperienceManager not found", null);
                return;
            }

            // 현재 상태 표시
            string statusLabel = $"Level: {_experienceManager.CurrentLevel} | EXP: {_experienceManager.CurrentExp}/{_experienceManager.ExpToNextLevel} ({_experienceManager.ExpProgress * 100f:F1}%)";
            CreateButton(statusLabel, RefreshContent);

            // 구분선
            CreateButton("--- EXP Controls ---", null);

            // 경험치 추가 버튼
            CreateButton("+10 EXP", () => AddExperience(10));
            CreateButton("+100 EXP", () => AddExperience(100));
            CreateButton("+1000 EXP", () => AddExperience(1000));

            // 레벨업 트리거
            CreateButton("Trigger Level Up", OnTriggerLevelUp);

            // 구분선
            CreateButton("--- Level Set ---", null);

            // 레벨 설정 버튼들
            CreateButton("Set Level 1", () => SetLevel(1));
            CreateButton("Set Level 10", () => SetLevel(10));
            CreateButton("Set Level 25", () => SetLevel(25));
            CreateButton("Set Level 50", () => SetLevel(50));
        }

        private void AddExperience(int amount)
        {
            if (_experienceManager == null)
            {
                UnityEngine.Debug.LogWarning("[LevelDebugTab] ExperienceManager not found");
                return;
            }

            _experienceManager.AddExperience(amount);
            UnityEngine.Debug.Log($"[LevelDebugTab] Added {amount} EXP");
            RefreshContent();
        }

        private void OnTriggerLevelUp()
        {
            if (_experienceManager == null)
            {
                UnityEngine.Debug.LogWarning("[LevelDebugTab] ExperienceManager not found");
                return;
            }

            int needed = _experienceManager.ExpToNextLevel - _experienceManager.CurrentExp;
            _experienceManager.AddExperience(needed + 1);
            UnityEngine.Debug.Log($"[LevelDebugTab] Triggered Level Up (added {needed + 1} EXP)");
            RefreshContent();
        }

        private void SetLevel(int targetLevel)
        {
            if (_experienceManager == null)
            {
                UnityEngine.Debug.LogWarning("[LevelDebugTab] ExperienceManager not found");
                return;
            }

            targetLevel = Mathf.Clamp(targetLevel, 1, _maxLevel);

            // Reset 후 목표 레벨까지 경험치 추가
            _experienceManager.Reset();

            for (int i = 1; i < targetLevel; i++)
            {
                int expRequired = _experienceManager.GetExpRequiredForLevel(i);
                _experienceManager.AddExperience(expRequired);
            }

            UnityEngine.Debug.Log($"[LevelDebugTab] Set Level to {targetLevel}");
            RefreshContent();
        }
    }
}
