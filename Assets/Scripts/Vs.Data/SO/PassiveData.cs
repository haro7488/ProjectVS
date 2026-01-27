using UnityEngine;

namespace Vs.Data
{
    public enum StatType
    {
        MaxHealth,
        HealthRegen,
        MoveSpeed,
        Damage,
        AttackSpeed,
        ProjectileCount,
        Area,
        Duration,
        CritChance,
        CritDamage,
        ExpGain,
        GoldGain,
        PickupRadius,
        Armor
    }

    [CreateAssetMenu(fileName = "Passive_", menuName = "VS/Passive Data")]
    public class PassiveData : ScriptableObject
    {
        [Header("기본 정보")]
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;

        [Header("효과")]
        public StatType PrimaryStat;
        public float BaseValue;
        public float ValuePerLevel;
        public bool IsPercentage;

        [Header("추가 효과")]
        public StatType SecondaryStat;
        public float SecondaryBaseValue;
        public float SecondaryValuePerLevel;
        public bool SecondaryIsPercentage;

        public float GetValue(int level)
        {
            return BaseValue + (ValuePerLevel * (level - 1));
        }

        public float GetSecondaryValue(int level)
        {
            if (SecondaryStat == PrimaryStat) return 0f;
            return SecondaryBaseValue + (SecondaryValuePerLevel * (level - 1));
        }

        public string GetFormattedValue(int level)
        {
            float value = GetValue(level);
            return IsPercentage ? $"+{value:F0}%" : $"+{value:F0}";
        }
    }
}
