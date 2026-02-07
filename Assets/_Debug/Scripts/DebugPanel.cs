using UnityEngine;
using UnityEngine.UI;
using Vs.UI;

namespace Vs.Debug
{
    /// <summary>
    /// 디버그 패널 UI.
    /// 탭 기반 인터페이스로 각 기능 그룹 제공.
    /// UIPanel을 상속하여 기존 UI 시스템과 통합.
    /// </summary>
    public class DebugPanel : UIPanel
    {
        [Header("Tabs")]
        [SerializeField] private DebugTabBase[] _tabs;
        [SerializeField] private Button[] _tabButtons;

        [Header("Visual")]
        [SerializeField] private Color _activeTabColor = new Color(0.3f, 0.6f, 1f, 1f);
        [SerializeField] private Color _inactiveTabColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private int _activeTabIndex = -1;

        protected override void Awake()
        {
            base.Awake();
            InitializeTabButtons();
        }

        private void Start()
        {
            // 컨트롤러에 패널 등록
            if (DebugController.HasInstance)
            {
                DebugController.Instance.SetPanel(this);
            }

            // 초기 상태: 숨김
            Hide();
        }

        private void InitializeTabButtons()
        {
            if (_tabButtons == null || _tabs == null) return;

            for (int i = 0; i < _tabButtons.Length && i < _tabs.Length; i++)
            {
                int index = i; // 클로저를 위한 로컬 복사
                if (_tabButtons[i] != null)
                {
                    _tabButtons[i].onClick.AddListener(() => SwitchTab(index));
                }
            }
        }

        protected override void OnShow()
        {
            base.OnShow();

            // 첫 번째 탭 활성화
            if (_activeTabIndex < 0 && _tabs != null && _tabs.Length > 0)
            {
                SwitchTab(0);
            }
            else if (_activeTabIndex >= 0)
            {
                // 현재 탭 새로고침
                RefreshCurrentTab();
            }
        }

        /// <summary>
        /// 탭 전환.
        /// </summary>
        public void SwitchTab(int index)
        {
            if (_tabs == null || index < 0 || index >= _tabs.Length) return;

            // 이전 탭 비활성화
            if (_activeTabIndex >= 0 && _activeTabIndex < _tabs.Length)
            {
                _tabs[_activeTabIndex]?.OnTabDeactivated();
                UpdateTabButtonVisual(_activeTabIndex, false);
            }

            // 새 탭 활성화
            _activeTabIndex = index;
            _tabs[_activeTabIndex]?.OnTabActivated();
            UpdateTabButtonVisual(_activeTabIndex, true);
        }

        private void UpdateTabButtonVisual(int index, bool isActive)
        {
            if (_tabButtons == null || index < 0 || index >= _tabButtons.Length) return;

            var button = _tabButtons[index];
            if (button == null) return;

            var colors = button.colors;
            colors.normalColor = isActive ? _activeTabColor : _inactiveTabColor;
            button.colors = colors;

            // 이미지 색상도 변경
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = isActive ? _activeTabColor : _inactiveTabColor;
            }
        }

        /// <summary>
        /// 현재 활성 탭 새로고침.
        /// </summary>
        public void RefreshCurrentTab()
        {
            if (_activeTabIndex >= 0 && _activeTabIndex < _tabs.Length)
            {
                _tabs[_activeTabIndex]?.RefreshContent();
            }
        }

        /// <summary>
        /// 탭 이름으로 전환.
        /// </summary>
        public void SwitchTab(string tabName)
        {
            if (_tabs == null) return;

            for (int i = 0; i < _tabs.Length; i++)
            {
                if (_tabs[i] != null && _tabs[i].TabName == tabName)
                {
                    SwitchTab(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 현재 활성 탭 인덱스.
        /// </summary>
        public int ActiveTabIndex => _activeTabIndex;

        /// <summary>
        /// 탭 개수.
        /// </summary>
        public int TabCount => _tabs?.Length ?? 0;
    }
}
