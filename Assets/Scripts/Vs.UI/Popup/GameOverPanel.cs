using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vs.Core;
using Vs.Progression;

namespace Vs.UI
{
    /// <summary>
    /// 게임오버 화면 UI 패널.
    /// 생존 시간, 도달 레벨 표시 및 재시작/메뉴 버튼 제공.
    /// </summary>
    public class GameOverPanel : UIPanel
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _survivalTimeText;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Buttons")]
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _menuButton;

        [Header("Settings")]
        [SerializeField] private string _titleMessage = "GAME OVER";
        [SerializeField] private string _survivalTimeFormat = "Survival Time: {0}";
        [SerializeField] private string _levelFormat = "Level: {0}";

        protected override void Awake()
        {
            base.Awake();

            _retryButton?.onClick.AddListener(OnRetryClicked);
            _menuButton?.onClick.AddListener(OnMenuClicked);
        }

        private void OnDestroy()
        {
            _retryButton?.onClick.RemoveListener(OnRetryClicked);
            _menuButton?.onClick.RemoveListener(OnMenuClicked);
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

            // 생존 시간 표시
            if (_survivalTimeText != null)
            {
                string timeFormatted = TimeManager.HasInstance
                    ? TimeManager.Instance.ElapsedTimeFormatted
                    : "00:00";
                _survivalTimeText.text = string.Format(_survivalTimeFormat, timeFormatted);
            }

            // 도달 레벨 표시
            if (_levelText != null)
            {
                int level = ExperienceManager.HasInstance
                    ? ExperienceManager.Instance.CurrentLevel
                    : 1;
                _levelText.text = string.Format(_levelFormat, level);
            }
        }

        private void OnRetryClicked()
        {
            Hide();

            if (GameManager.HasInstance)
            {
                GameManager.Instance.StartGame();
            }
        }

        private void OnMenuClicked()
        {
            Hide();

            if (GameManager.HasInstance)
            {
                GameManager.Instance.ReturnToMenu();
            }
        }
    }
}
