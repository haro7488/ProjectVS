using UnityEngine;
using Vs.Core;

namespace Vs.Progression
{
    /// <summary>
    /// 임시 레벨업 UI. 숫자 키로 선택지 선택.
    /// 실제 UI가 구현되면 교체될 예정.
    /// </summary>
    public class TempLevelUpUI : MonoBehaviour
    {
        private LevelUpChoice[] _currentChoices;
        private bool _isShowingChoices;

        private void OnEnable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void Update()
        {
            if (!_isShowingChoices) return;
            if (_currentChoices == null || _currentChoices.Length == 0) return;

            // 1, 2, 3 키로 선택
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                SelectChoice(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SelectChoice(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                SelectChoice(2);
            }
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.LevelUp)
            {
                ShowChoices();
            }
            else
            {
                _isShowingChoices = false;
            }
        }

        private void ShowChoices()
        {
            if (!LevelUpManager.HasInstance) return;

            _currentChoices = LevelUpManager.Instance.GenerateChoices();
            _isShowingChoices = true;

            Debug.Log("=== LEVEL UP! Press 1, 2, or 3 to choose ===");
            for (int i = 0; i < _currentChoices.Length; i++)
            {
                var choice = _currentChoices[i];
                Debug.Log($"[{i + 1}] {choice.DisplayName} (Lv.{choice.NewLevel}) - {choice.Description}");
            }
        }

        private void SelectChoice(int index)
        {
            if (index < 0 || index >= _currentChoices.Length) return;

            var choice = _currentChoices[index];
            Debug.Log($"Selected: {choice.DisplayName}");

            LevelUpManager.Instance.ApplyChoice(choice);
            _isShowingChoices = false;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!_isShowingChoices) return;
            if (_currentChoices == null || _currentChoices.Length == 0) return;

            // 간단한 GUI 표시
            float width = 400f;
            float height = 200f;
            float x = (Screen.width - width) / 2f;
            float y = (Screen.height - height) / 2f;

            GUI.Box(new Rect(x - 10, y - 10, width + 20, height + 20), "");

            GUI.Label(new Rect(x, y, width, 30), "<b>LEVEL UP! Choose an upgrade:</b>");
            y += 40;

            for (int i = 0; i < _currentChoices.Length; i++)
            {
                var choice = _currentChoices[i];
                string label = $"[{i + 1}] {choice.DisplayName} (Lv.{choice.NewLevel})";
                GUI.Label(new Rect(x, y, width, 25), label);
                y += 25;

                GUI.Label(new Rect(x + 20, y, width - 20, 25), choice.Description);
                y += 30;
            }
        }
#endif
    }
}