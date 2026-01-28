using UnityEngine;

namespace Vs.UI
{
    /// <summary>
    /// HUD 패널.
    /// 게임 중 표시되는 모든 HUD 요소들의 컨테이너.
    /// UIPanel을 상속하여 Show/Hide 기능 제공.
    /// </summary>
    public class HUDPanel : UIPanel
    {
        [Header("HUD 컴포넌트")]
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private ExperienceBar _experienceBar;
        [SerializeField] private TimeDisplay _timeDisplay;
        [SerializeField] private ItemSlotsPanel _itemSlotsPanel;

        /// <summary>
        /// 체력 바 컴포넌트.
        /// </summary>
        public HealthBar HealthBar => _healthBar;

        /// <summary>
        /// 경험치 바 컴포넌트.
        /// </summary>
        public ExperienceBar ExperienceBar => _experienceBar;

        /// <summary>
        /// 시간 표시 컴포넌트.
        /// </summary>
        public TimeDisplay TimeDisplay => _timeDisplay;

        /// <summary>
        /// 아이템 슬롯 패널.
        /// </summary>
        public ItemSlotsPanel ItemSlotsPanel => _itemSlotsPanel;

        protected override void OnShow()
        {
            base.OnShow();
            RefreshAll();
        }

        /// <summary>
        /// 모든 HUD 컴포넌트 갱신.
        /// </summary>
        public void RefreshAll()
        {
            if (_healthBar != null)
            {
                _healthBar.RefreshPlayerReference();
            }

            if (_experienceBar != null)
            {
                _experienceBar.Refresh();
            }

            if (_timeDisplay != null)
            {
                _timeDisplay.Refresh();
            }

            if (_itemSlotsPanel != null)
            {
                _itemSlotsPanel.SyncWithManager();
            }
        }
    }
}
