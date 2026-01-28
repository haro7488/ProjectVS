using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vs.Player;

namespace Vs.UI
{
    /// <summary>
    /// 플레이어 체력 바 UI.
    /// PlayerHealth.OnHealthChanged 이벤트를 구독하여 체력 표시를 업데이트.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _healthText;

        [Header("설정")]
        [SerializeField] private bool _showText = true;

        private PlayerHealth _playerHealth;

        private void Start()
        {
            FindPlayerHealth();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void FindPlayerHealth()
        {
            _playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged += HandleHealthChanged;

                // 초기값 설정
                UpdateDisplay(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            }
            else
            {
                Debug.LogWarning("[HealthBar] PlayerHealth not found");
            }
        }

        private void UnsubscribeEvents()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= HandleHealthChanged;
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            UpdateDisplay(current, max);
        }

        private void UpdateDisplay(float current, float max)
        {
            // Fill amount 업데이트
            if (_fillImage != null)
            {
                _fillImage.fillAmount = max > 0f ? current / max : 0f;
            }

            // 텍스트 업데이트
            if (_healthText != null && _showText)
            {
                _healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }
        }

        /// <summary>
        /// PlayerHealth 참조 재설정 (씬 전환 등에서 사용).
        /// </summary>
        public void RefreshPlayerReference()
        {
            UnsubscribeEvents();
            FindPlayerHealth();
        }
    }
}
