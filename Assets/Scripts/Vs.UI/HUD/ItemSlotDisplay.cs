using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vs.UI
{
    /// <summary>
    /// 개별 아이템 슬롯 표시.
    /// 무기 또는 패시브 아이템의 아이콘과 레벨을 표시.
    /// </summary>
    public class ItemSlotDisplay : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private GameObject _emptyState;
        [SerializeField] private GameObject _filledState;

        [Header("설정")]
        [SerializeField] private string _levelFormat = "{0}";

        private bool _hasItem;

        /// <summary>
        /// 아이템이 설정되어 있는지 여부.
        /// </summary>
        public bool HasItem => _hasItem;

        private void Awake()
        {
            // 초기 상태: 비어있음
            UpdateVisualState();
        }

        /// <summary>
        /// 슬롯에 아이템 설정.
        /// </summary>
        /// <param name="icon">아이템 아이콘</param>
        /// <param name="level">아이템 레벨</param>
        public void SetItem(Sprite icon, int level)
        {
            _hasItem = true;

            if (_iconImage != null)
            {
                _iconImage.sprite = icon;
                _iconImage.enabled = icon != null;
            }

            if (_levelText != null)
            {
                _levelText.text = string.Format(_levelFormat, level);
            }

            UpdateVisualState();
        }

        /// <summary>
        /// 아이템 레벨만 업데이트.
        /// </summary>
        /// <param name="level">새로운 레벨</param>
        public void UpdateLevel(int level)
        {
            if (_levelText != null)
            {
                _levelText.text = string.Format(_levelFormat, level);
            }
        }

        /// <summary>
        /// 슬롯 비우기.
        /// </summary>
        public void Clear()
        {
            _hasItem = false;

            if (_iconImage != null)
            {
                _iconImage.sprite = null;
                _iconImage.enabled = false;
            }

            if (_levelText != null)
            {
                _levelText.text = string.Empty;
            }

            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (_emptyState != null)
            {
                _emptyState.SetActive(!_hasItem);
            }

            if (_filledState != null)
            {
                _filledState.SetActive(_hasItem);
            }
        }
    }
}
