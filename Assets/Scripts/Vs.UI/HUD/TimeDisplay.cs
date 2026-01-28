using TMPro;
using UnityEngine;
using Vs.Core;

namespace Vs.UI
{
    /// <summary>
    /// 시간 표시 UI.
    /// TimeManager.OnTimeChanged 이벤트를 구독하여 시간을 MM:SS 형식으로 표시.
    /// </summary>
    public class TimeDisplay : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] private TextMeshProUGUI _timeText;

        [Header("설정")]
        [SerializeField] private bool _showRemainingTime = true;

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
            if (TimeManager.HasInstance)
            {
                TimeManager.Instance.OnTimeChanged += HandleTimeChanged;
            }
        }

        private void UnsubscribeEvents()
        {
            if (TimeManager.HasInstance)
            {
                TimeManager.Instance.OnTimeChanged -= HandleTimeChanged;
            }
        }

        private void HandleTimeChanged(float elapsedTime)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_timeText == null) return;
            if (!TimeManager.HasInstance) return;

            var manager = TimeManager.Instance;

            if (_showRemainingTime)
            {
                _timeText.text = manager.RemainingTimeFormatted;
            }
            else
            {
                _timeText.text = manager.ElapsedTimeFormatted;
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
