using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vs.Core;
using Vs.Progression;

namespace Vs.UI
{
    /// <summary>
    /// 승리 화면 UI 패널.
    /// 클리어 시간, 최종 레벨 표시 및 메뉴 버튼 제공.
    /// </summary>
    public class VictoryPanel : UIPanel
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _survivalTimeText;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Buttons")]
        [SerializeField] private Button _continueButton;

        [Header("Settings")]
        [SerializeField] private string _titleMessage = "VICTORY!";
        [SerializeField] private string _survivalTimeFormat = "Clear Time: {0}";
        [SerializeField] private string _levelFormat = "Final Level: {0}";

        protected override void Awake()
        {
            base.Awake();

            _continueButton?.onClick.AddListener(OnContinueClicked);
        }

        private void OnDestroy()
        {
            _continueButton?.onClick.RemoveListener(OnContinueClicked);
        }

        protected override void OnShow()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // 타이틀 설정
            if (_titleText != null)
            {
                _titleText.text = _titleMessage;
            }

            // 클리어 시간 표시
            if (_survivalTimeText != null)
            {
                string timeFormatted = TimeManager.HasInstance
                    ? TimeManager.Instance.ElapsedTimeFormatted
                    : "00:00";
                _survivalTimeText.text = string.Format(_survivalTimeFormat, timeFormatted);
            }

            // 최종 레벨 표시
            if (_levelText != null)
            {
                int level = ExperienceManager.HasInstance
                    ? ExperienceManager.Instance.CurrentLevel
                    : 1;
                _levelText.text = string.Format(_levelFormat, level);
            }
        }

        private void OnContinueClicked()
        {
            Hide();

            if (GameManager.HasInstance)
            {
                GameManager.Instance.ReturnToMenu();
            }
        }
    }
}
