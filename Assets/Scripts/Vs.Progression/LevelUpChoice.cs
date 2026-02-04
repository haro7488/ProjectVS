using UnityEngine;

namespace Vs.Progression
{
    /// <summary>
    /// 레벨업 선택지 데이터.
    /// </summary>
    public class LevelUpChoice
    {
        public enum ChoiceType
        {
            NewWeapon,
            WeaponUpgrade,
            NewPassive,
            PassiveUpgrade,
            WeaponEvolution
        }

        public ChoiceType Type { get; }
        public ScriptableObject Data { get; } // WeaponData 또는 PassiveData
        public ScriptableObject SourceData { get; } // 진화 시 원본 무기
        public int NewLevel { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public Sprite Icon { get; }

        public LevelUpChoice(ChoiceType type, ScriptableObject data, int newLevel, string displayName,
            string description, Sprite icon, ScriptableObject sourceData = null)
        {
            Type = type;
            Data = data;
            SourceData = sourceData;
            NewLevel = newLevel;
            DisplayName = displayName;
            Description = description;
            Icon = icon;
        }

        public bool IsNewItem => Type == ChoiceType.NewWeapon || Type == ChoiceType.NewPassive;
        public bool IsUpgrade => Type == ChoiceType.WeaponUpgrade || Type == ChoiceType.PassiveUpgrade;
        public bool IsWeapon => Type == ChoiceType.NewWeapon || Type == ChoiceType.WeaponUpgrade;
        public bool IsPassive => Type == ChoiceType.NewPassive || Type == ChoiceType.PassiveUpgrade;
        public bool IsEvolution => Type == ChoiceType.WeaponEvolution;
    }
}