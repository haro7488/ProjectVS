using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vs.Progression;

namespace Vs.UI
{
    /// <summary>
    /// 경험치 바 UI.
    /// ExperienceManager의 이벤트를 구독하여 경험치와 레벨 표시를 업데이트.
    /// </summary>
    public class ExperienceBar : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("표시 형식")]
        [SerializeField] private string _levelFormat = "Lv. {0}";

        private void Start()
        {
            SubscribeEvents();
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (ExperienceManager.HasInstance)
            {
                ExperienceManager.Instance.OnExpGained += HandleExpGained;
                ExperienceManager.Instance.OnLevelUp += HandleLevelUp;
            }
        }

        private void UnsubscribeEvents()
        {
            if (ExperienceManager.HasInstance)
            {
                ExperienceManager.Instance.OnExpGained -= HandleExpGained;
                ExperienceManager.Instance.OnLevelUp -= HandleLevelUp;
            }
        }

        private void HandleExpGained(int amount)
        {
            UpdateDisplay();
        }

        private void HandleLevelUp(int newLevel)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (!ExperienceManager.HasInstance) return;

            var manager = ExperienceManager.Instance;

            // Fill amount 업데이트 (ExpProgress는 0~1)
            if (_fillImage != null)
            {
                _fillImage.fillAmount = manager.ExpProgress;
            }

            // 레벨 텍스트 업데이트
            if (_levelText != null)
            {
                _levelText.text = string.Format(_levelFormat, manager.CurrentLevel);
            }
        }

        /// <summary>
        /// 디스플레이 강제 갱신.
        /// </summary>
        public void Refresh()
        {
            UpdateDisplay();
        }
    }
}
