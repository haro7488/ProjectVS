using TMPro;
using UnityEngine;
using Vs.Progression;

namespace Vs.UI
{
    /// <summary>
    /// 레벨업 시 3개 선택지를 표시하는 팝업 패널.
    /// </summary>
    public class LevelUpPanel : UIPanel
    {
        [SerializeField] private LevelUpChoiceButton[] _choiceButtons;
        [SerializeField] private TextMeshProUGUI _titleText;

        private LevelUpChoice[] _currentChoices;

        protected override void Awake()
        {
            base.Awake();

            // 선택지 버튼 이벤트 연결
            if (_choiceButtons != null)
            {
                for (int i = 0; i < _choiceButtons.Length; i++)
                {
                    if (_choiceButtons[i] != null)
                    {
                        _choiceButtons[i].OnClicked += HandleChoiceClicked;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            // 선택지 버튼 이벤트 해제
            if (_choiceButtons != null)
            {
                for (int i = 0; i < _choiceButtons.Length; i++)
                {
                    if (_choiceButtons[i] != null)
                    {
                        _choiceButtons[i].OnClicked -= HandleChoiceClicked;
                    }
                }
            }
        }

        protected override void OnShow()
        {
            var levelUpManager = LevelUpManager.Instance;
            if (levelUpManager != null)
            {
                levelUpManager.OnChoicesGenerated += HandleChoicesGenerated;
                levelUpManager.GenerateChoices();
            }

            if (_titleText != null)
            {
                _titleText.text = "LEVEL UP!";
            }
        }

        protected override void OnHide()
        {
            if (LevelUpManager.HasInstance)
            {
                LevelUpManager.Instance.OnChoicesGenerated -= HandleChoicesGenerated;
            }

            _currentChoices = null;
        }

        private void HandleChoicesGenerated(LevelUpChoice[] choices)
        {
            _currentChoices = choices;
            UpdateChoiceButtons();
        }

        private void UpdateChoiceButtons()
        {
            if (_choiceButtons == null) return;

            for (int i = 0; i < _choiceButtons.Length; i++)
            {
                if (_choiceButtons[i] == null) continue;

                if (_currentChoices != null && i < _currentChoices.Length)
                {
                    _choiceButtons[i].SetChoice(_currentChoices[i]);
                    _choiceButtons[i].SetInteractable(true);
                }
                else
                {
                    _choiceButtons[i].SetChoice(null);
                    _choiceButtons[i].SetInteractable(false);
                }
            }
        }

        private void HandleChoiceClicked(LevelUpChoice choice)
        {
            if (choice == null) return;

            // 모든 버튼 비활성화 (중복 클릭 방지)
            SetAllButtonsInteractable(false);

            // 선택 적용 (LevelUpManager에서 자동으로 ResumeGame 호출)
            if (LevelUpManager.HasInstance)
            {
                LevelUpManager.Instance.ApplyChoice(choice);
            }

            // 패널 숨기기
            Hide();
        }

        private void SetAllButtonsInteractable(bool interactable)
        {
            if (_choiceButtons == null) return;

            foreach (var button in _choiceButtons)
            {
                if (button != null)
                {
                    button.SetInteractable(interactable);
                }
            }
        }
    }
}
