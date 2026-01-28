using UnityEngine;
using Vs.Core;
using Vs.Utility;

namespace Vs.UI
{
    /// <summary>
    /// UI 전역 관리자. GameState에 따라 적절한 UI 패널 표시/숨김.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [Header("Panels")]
        [SerializeField] private UIPanel _hudPanel;
        [SerializeField] private UIPanel _levelUpPanel;
        [SerializeField] private UIPanel _gameOverPanel;
        [SerializeField] private UIPanel _victoryPanel;

        [Header("Layers")]
        [SerializeField] private GameObject _dimBackground;

        private void Start()
        {
            // 초기 상태 설정
            HideAllPanels();

            // GameManager 이벤트 구독
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                // 현재 상태에 맞게 UI 설정
                HandleStateChanged(GameManager.Instance.State);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameState state)
        {
            // 모든 팝업 숨김
            HideAllPopups();

            // HUD는 Playing 또는 LevelUp 상태에서만 표시
            bool showHUD = state == GameState.Playing || state == GameState.LevelUp;
            SetPanelActive(_hudPanel, showHUD);

            // 상태별 팝업 표시
            switch (state)
            {
                case GameState.LevelUp:
                    ShowPanel(_levelUpPanel);
                    SetDimBackground(true);
                    break;

                case GameState.GameOver:
                    ShowPanel(_gameOverPanel);
                    SetDimBackground(true);
                    break;

                case GameState.Victory:
                    ShowPanel(_victoryPanel);
                    SetDimBackground(true);
                    break;

                default:
                    SetDimBackground(false);
                    break;
            }
        }

        private void HideAllPanels()
        {
            HidePanel(_hudPanel);
            HideAllPopups();
        }

        private void HideAllPopups()
        {
            HidePanel(_levelUpPanel);
            HidePanel(_gameOverPanel);
            HidePanel(_victoryPanel);
            SetDimBackground(false);
        }

        private void ShowPanel(UIPanel panel)
        {
            if (panel != null)
            {
                panel.Show();
            }
        }

        private void HidePanel(UIPanel panel)
        {
            if (panel != null)
            {
                panel.Hide();
            }
        }

        private void SetPanelActive(UIPanel panel, bool active)
        {
            if (panel == null) return;

            if (active)
                panel.Show();
            else
                panel.Hide();
        }

        private void SetDimBackground(bool active)
        {
            if (_dimBackground != null)
            {
                _dimBackground.SetActive(active);
            }
        }

        #region Public API

        /// <summary>
        /// HUD 패널 참조 (외부에서 필요 시 접근용)
        /// </summary>
        public UIPanel HUDPanel => _hudPanel;

        #endregion
    }
}
