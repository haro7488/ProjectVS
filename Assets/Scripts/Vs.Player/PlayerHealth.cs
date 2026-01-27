using System;
using UnityEngine;
using Vs.Combat;
using Vs.Core;

namespace Vs.Player
{
    /// <summary>
    /// 플레이어 체력 및 피격 처리.
    /// IDamageable 인터페이스 구현.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health")] [SerializeField] private float _maxHealth = 100f;

        [Header("Invincibility")] [SerializeField]
        private float _invincibilityDuration = 0.5f;

        private float _currentHealth;
        private float _invincibilityTimer;
        private bool _isInvincible;

        /// <summary>
        /// 현재 체력.
        /// </summary>
        public float CurrentHealth => _currentHealth;

        /// <summary>
        /// 최대 체력.
        /// </summary>
        public float MaxHealth => _maxHealth;

        /// <summary>
        /// 사망 여부.
        /// </summary>
        public bool IsDead => _currentHealth <= 0;

        /// <summary>
        /// 현재 무적 상태 여부.
        /// </summary>
        public bool IsInvincible => _isInvincible;

        /// <summary>
        /// 피격 시 발생.
        /// </summary>
        public event Action<DamageInfo> OnDamaged;

        /// <summary>
        /// 사망 시 발생.
        /// </summary>
        public event Action OnDeath;

        /// <summary>
        /// 회복 시 발생.
        /// </summary>
        public event Action<float> OnHealed;

        /// <summary>
        /// 체력 변경 시 발생 (현재 체력, 최대 체력).
        /// </summary>
        public event Action<float, float> OnHealthChanged;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void Update()
        {
            UpdateInvincibility();
        }

        private void UpdateInvincibility()
        {
            if (!_isInvincible) return;

            _invincibilityTimer -= Time.deltaTime;
            if (_invincibilityTimer <= 0f)
            {
                _isInvincible = false;
            }
        }

        /// <summary>
        /// 체력 시스템 초기화.
        /// </summary>
        public void Initialize(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
            _isInvincible = false;
            _invincibilityTimer = 0f;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// 대미지 적용.
        /// </summary>
        public void TakeDamage(DamageInfo damage)
        {
            if (IsDead) return;
            if (_isInvincible) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damage.Amount);
            OnDamaged?.Invoke(damage);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (IsDead)
            {
                HandleDeath();
            }
            else
            {
                StartInvincibility();
            }
        }

        /// <summary>
        /// 체력 회복.
        /// </summary>
        public void Heal(float amount)
        {
            if (IsDead) return;
            if (amount <= 0f) return;

            float previousHealth = _currentHealth;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);

            float actualHeal = _currentHealth - previousHealth;
            if (actualHeal > 0f)
            {
                OnHealed?.Invoke(actualHeal);
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
        }

        /// <summary>
        /// 체력 퍼센트 (0~1).
        /// </summary>
        public float GetHealthPercent()
        {
            return _maxHealth > 0f ? _currentHealth / _maxHealth : 0f;
        }

        /// <summary>
        /// 최대 체력 업데이트 (스탯 변경 시).
        /// </summary>
        public void SetMaxHealth(float newMaxHealth, bool healToFull = false)
        {
            float healthPercent = GetHealthPercent();
            _maxHealth = Mathf.Max(1f, newMaxHealth);

            if (healToFull)
            {
                _currentHealth = _maxHealth;
            }
            else
            {
                // 비율 유지
                _currentHealth = _maxHealth * healthPercent;
            }

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        private void StartInvincibility()
        {
            _isInvincible = true;
            _invincibilityTimer = _invincibilityDuration;
        }

        private void HandleDeath()
        {
            OnDeath?.Invoke();

            // GameManager에 게임 오버 알림
            if (GameManager.HasInstance)
            {
                GameManager.Instance.EndGame(false);
            }
        }

        /// <summary>
        /// 강제 무적 설정 (스킬 등에서 사용).
        /// </summary>
        public void SetInvincible(bool invincible, float duration = 0f)
        {
            _isInvincible = invincible;
            if (invincible && duration > 0f)
            {
                _invincibilityTimer = duration;
            }
            else if (!invincible)
            {
                _invincibilityTimer = 0f;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Debug: Take 10 Damage")]
        private void DebugTakeDamage()
        {
            TakeDamage(new DamageInfo(10f));
        }

        [ContextMenu("Debug: Heal 10")]
        private void DebugHeal()
        {
            Heal(10f);
        }
#endif
    }
}