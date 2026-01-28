using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vs.Progression;

namespace Vs.UI
{
    /// <summary>
    /// 레벨업 선택지 버튼 개별 컴포넌트.
    /// </summary>
    public class LevelUpChoiceButton : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _button;

        private LevelUpChoice _currentChoice;

        /// <summary>
        /// 버튼 클릭 시 발생하는 이벤트.
        /// </summary>
        public event Action<LevelUpChoice> OnClicked;

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(HandleButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleButtonClicked);
            }
        }

        /// <summary>
        /// 선택지 데이터를 설정하고 UI 업데이트.
        /// </summary>
        /// <param name="choice">표시할 선택지 데이터</param>
        public void SetChoice(LevelUpChoice choice)
        {
            _currentChoice = choice;

            if (choice == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_iconImage != null)
            {
                _iconImage.sprite = choice.Icon;
                _iconImage.enabled = choice.Icon != null;
            }

            if (_nameText != null)
            {
                _nameText.text = choice.DisplayName;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = choice.Description;
            }

            if (_levelText != null)
            {
                _levelText.text = choice.IsNewItem ? "NEW" : $"Lv.{choice.NewLevel}";
            }
        }

        /// <summary>
        /// 버튼 상호작용 가능 여부 설정.
        /// </summary>
        /// <param name="interactable">상호작용 가능 여부</param>
        public void SetInteractable(bool interactable)
        {
            if (_button != null)
            {
                _button.interactable = interactable;
            }
        }

        private void HandleButtonClicked()
        {
            if (_currentChoice != null)
            {
                OnClicked?.Invoke(_currentChoice);
            }
        }
    }
}
