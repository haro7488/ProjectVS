using System;
using UnityEngine;
using Vs.Data;

namespace Vs.Player
{
    /// <summary>
    /// 스탯 타입 열거형.
    /// </summary>
    public enum StatType
    {
        MaxHealth,
        MoveSpeed,
        Damage,
        Area,
        Duration,
        ProjectileSpeed,
        Armor,
        CooldownReduction,
        Luck,
        ExperienceGain,
        PickupRange
    }

    /// <summary>
    /// 플레이어 스탯 관리.
    /// 기본 스탯 + 보너스 스탯 시스템.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private PlayerController _playerController;

        [SerializeField] private PlayerHealth _playerHealth;

        // 기본 스탯 (CharacterData에서 로드)
        private float _baseMaxHealth;
        private float _baseMoveSpeed;
        private float _baseArmor;

        // 보너스 스탯 (레벨업/패시브에서 추가, 100% = 1.0)
        private float _bonusMaxHealth;
        private float _bonusMoveSpeed;
        private float _bonusDamage;
        private float _bonusArea;
        private float _bonusDuration;
        private float _bonusProjectileSpeed;
        private float _bonusCooldownReduction;
        private float _bonusLuck;
        private float _bonusExperienceGain;
        private float _bonusPickupRange;
        private float _bonusArmor;

        #region Properties - Final Stats

        public float MaxHealth => _baseMaxHealth * (1f + _bonusMaxHealth);
        public float MoveSpeed => _baseMoveSpeed * (1f + _bonusMoveSpeed);
        public float Armor => _baseArmor + _bonusArmor;

        // 승수 (무기 등에서 사용)
        public float DamageMultiplier => 1f + _bonusDamage;
        public float AreaMultiplier => 1f + _bonusArea;
        public float DurationMultiplier => 1f + _bonusDuration;
        public float ProjectileSpeedMultiplier => 1f + _bonusProjectileSpeed;
        public float CooldownMultiplier => 1f - Mathf.Clamp01(_bonusCooldownReduction);
        public float LuckMultiplier => 1f + _bonusLuck;
        public float ExperienceMultiplier => 1f + _bonusExperienceGain;
        public float PickupRangeMultiplier => 1f + _bonusPickupRange;

        #endregion

        /// <summary>
        /// 스탯 변경 시 발생 (변경된 스탯 타입).
        /// </summary>
        public event Action<StatType> OnStatChanged;

        /// <summary>
        /// 전체 스탯 리셋 시 발생.
        /// </summary>
        public event Action OnStatsReset;

        private void Awake()
        {
            // 참조 자동 획득
            if (_playerController == null)
                _playerController = GetComponent<PlayerController>();
            if (_playerHealth == null)
                _playerHealth = GetComponent<PlayerHealth>();
        }

        /// <summary>
        /// CharacterData로 스탯 초기화.
        /// </summary>
        public void Initialize(CharacterData data)
        {
            if (data == null)
            {
                Debug.LogError("[PlayerStats] CharacterData is null!");
                return;
            }

            // 기본 스탯 설정
            _baseMaxHealth = data.BaseMaxHealth;
            _baseMoveSpeed = data.BaseMoveSpeed;
            _baseArmor = data.BaseArmor;

            // 캐릭터 고유 보너스 적용 (% → 소수점)
            _bonusMaxHealth = data.BonusMaxHealth / 100f;
            _bonusMoveSpeed = data.BonusMoveSpeed / 100f;
            _bonusDamage = data.BonusDamage / 100f;
            _bonusArea = data.BonusArea / 100f;
            _bonusDuration = data.BonusDuration / 100f;
            _bonusProjectileSpeed = data.BonusProjectileSpeed / 100f;

            // 기타 보너스 초기화
            _bonusCooldownReduction = 0f;
            _bonusLuck = 0f;
            _bonusExperienceGain = 0f;
            _bonusPickupRange = 0f;
            _bonusArmor = 0f;

            // 연결된 컴포넌트에 스탯 적용
            ApplyStatsToComponents();
        }

        /// <summary>
        /// 보너스 스탯 추가.
        /// </summary>
        /// <param name="stat">스탯 타입</param>
        /// <param name="value">추가할 값 (0.1 = 10%)</param>
        public void AddBonus(StatType stat, float value)
        {
            switch (stat)
            {
                case StatType.MaxHealth:
                    _bonusMaxHealth += value;
                    _playerHealth?.SetMaxHealth(MaxHealth);
                    break;
                case StatType.MoveSpeed:
                    _bonusMoveSpeed += value;
                    _playerController?.SetMoveSpeed(MoveSpeed);
                    break;
                case StatType.Damage:
                    _bonusDamage += value;
                    break;
                case StatType.Area:
                    _bonusArea += value;
                    break;
                case StatType.Duration:
                    _bonusDuration += value;
                    break;
                case StatType.ProjectileSpeed:
                    _bonusProjectileSpeed += value;
                    break;
                case StatType.Armor:
                    _bonusArmor += value;
                    break;
                case StatType.CooldownReduction:
                    _bonusCooldownReduction += value;
                    break;
                case StatType.Luck:
                    _bonusLuck += value;
                    break;
                case StatType.ExperienceGain:
                    _bonusExperienceGain += value;
                    break;
                case StatType.PickupRange:
                    _bonusPickupRange += value;
                    break;
            }

            OnStatChanged?.Invoke(stat);
        }

        /// <summary>
        /// 모든 보너스 스탯 초기화 (게임 재시작 시).
        /// </summary>
        public void ResetBonuses()
        {
            _bonusMaxHealth = 0f;
            _bonusMoveSpeed = 0f;
            _bonusDamage = 0f;
            _bonusArea = 0f;
            _bonusDuration = 0f;
            _bonusProjectileSpeed = 0f;
            _bonusCooldownReduction = 0f;
            _bonusLuck = 0f;
            _bonusExperienceGain = 0f;
            _bonusPickupRange = 0f;
            _bonusArmor = 0f;

            ApplyStatsToComponents();
            OnStatsReset?.Invoke();
        }

        /// <summary>
        /// 특정 스탯의 현재 보너스 값 조회.
        /// </summary>
        public float GetBonus(StatType stat)
        {
            return stat switch
            {
                StatType.MaxHealth => _bonusMaxHealth,
                StatType.MoveSpeed => _bonusMoveSpeed,
                StatType.Damage => _bonusDamage,
                StatType.Area => _bonusArea,
                StatType.Duration => _bonusDuration,
                StatType.ProjectileSpeed => _bonusProjectileSpeed,
                StatType.Armor => _bonusArmor,
                StatType.CooldownReduction => _bonusCooldownReduction,
                StatType.Luck => _bonusLuck,
                StatType.ExperienceGain => _bonusExperienceGain,
                StatType.PickupRange => _bonusPickupRange,
                _ => 0f
            };
        }

        private void ApplyStatsToComponents()
        {
            _playerController?.SetMoveSpeed(MoveSpeed);
            _playerHealth?.SetMaxHealth(MaxHealth, true);
        }

#if UNITY_EDITOR
        [ContextMenu("Debug: Add 10% Move Speed")]
        private void DebugAddMoveSpeed()
        {
            AddBonus(StatType.MoveSpeed, 0.1f);
            Debug.Log($"[PlayerStats] MoveSpeed: {MoveSpeed}");
        }

        [ContextMenu("Debug: Add 10% Damage")]
        private void DebugAddDamage()
        {
            AddBonus(StatType.Damage, 0.1f);
            Debug.Log($"[PlayerStats] DamageMultiplier: {DamageMultiplier}");
        }

        [ContextMenu("Debug: Log All Stats")]
        private void DebugLogStats()
        {
            Debug.Log($"[PlayerStats] MaxHealth: {MaxHealth}, MoveSpeed: {MoveSpeed}, " +
                      $"Damage: {DamageMultiplier}x, Area: {AreaMultiplier}x, " +
                      $"Duration: {DurationMultiplier}x, Cooldown: {CooldownMultiplier}x");
        }
#endif
    }
}