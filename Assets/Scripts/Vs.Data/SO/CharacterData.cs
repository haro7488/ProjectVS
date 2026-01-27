using UnityEngine;

namespace Vs.Data
{
    [CreateAssetMenu(fileName = "Character_", menuName = "VS/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("기본 정보")]
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Portrait;
        public RuntimeAnimatorController Animator;

        [Header("시작 무기")]
        public WeaponData StartingWeapon;

        [Header("기본 스탯")]
        public float BaseMaxHealth = 100f;
        public float BaseMoveSpeed = 5f;
        public float BaseArmor = 0f;

        [Header("보너스 스탯 (%)")]
        [Range(-50f, 100f)] public float BonusMaxHealth = 0f;
        [Range(-50f, 100f)] public float BonusMoveSpeed = 0f;
        [Range(-50f, 100f)] public float BonusDamage = 0f;
        [Range(-50f, 100f)] public float BonusArea = 0f;
        [Range(-50f, 100f)] public float BonusDuration = 0f;
        [Range(-50f, 100f)] public float BonusProjectileSpeed = 0f;

        [Header("언락")]
        public bool IsUnlockedByDefault = false;
        public string UnlockConditionDescription;

        public float GetMaxHealth() => BaseMaxHealth * (1f + BonusMaxHealth / 100f);
        public float GetMoveSpeed() => BaseMoveSpeed * (1f + BonusMoveSpeed / 100f);
    }
}
