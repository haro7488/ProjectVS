using UnityEngine;
using Vs.Data;

namespace Vs.Tests.Utilities
{
    /// <summary>
    /// 테스트용 Mock 객체 생성 팩토리
    /// </summary>
    public static class MockFactory
    {
        #region WeaponData

        /// <summary>
        /// 기본 WeaponData 생성
        /// </summary>
        public static WeaponData CreateWeaponData(
            string id = "test_weapon",
            string displayName = "테스트 무기",
            WeaponType type = WeaponType.Projectile,
            float baseDamage = 10f,
            float baseInterval = 1f,
            int baseProjectileCount = 1,
            float baseArea = 1f,
            float baseDuration = 0f,
            float baseSpeed = 15f)
        {
            var data = ScriptableObject.CreateInstance<WeaponData>();

            data.Id = id;
            data.DisplayName = displayName;
            data.Type = type;
            data.BaseDamage = baseDamage;
            data.BaseInterval = baseInterval;
            data.BaseProjectileCount = baseProjectileCount;
            data.BaseArea = baseArea;
            data.BaseDuration = baseDuration;
            data.BaseSpeed = baseSpeed;

            return data;
        }

        /// <summary>
        /// 진화 가능한 WeaponData 생성
        /// </summary>
        public static WeaponData CreateEvolvableWeaponData(
            string id,
            WeaponData evolvesTo,
            PassiveData evolutionRequirement)
        {
            var data = CreateWeaponData(id: id);
            data.EvolvesTo = evolvesTo;
            data.EvolutionRequirement = evolutionRequirement;
            return data;
        }

        #endregion

        #region EnemyData

        /// <summary>
        /// 기본 EnemyData 생성
        /// </summary>
        public static EnemyData CreateEnemyData(
            string id = "test_enemy",
            string displayName = "테스트 적",
            float maxHealth = 100f,
            float moveSpeed = 2f,
            float contactDamage = 10f,
            float attackCooldown = 1f,
            int expValue = 1)
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();

            data.Id = id;
            data.DisplayName = displayName;
            data.MaxHealth = maxHealth;
            data.MoveSpeed = moveSpeed;
            data.ContactDamage = contactDamage;
            data.AttackCooldown = attackCooldown;
            data.ExpValue = expValue;

            return data;
        }

        #endregion

        #region PassiveData

        /// <summary>
        /// 기본 PassiveData 생성
        /// </summary>
        public static PassiveData CreatePassiveData(
            string id = "test_passive",
            string displayName = "테스트 패시브",
            StatType primaryStat = StatType.MaxHealth,
            float baseValue = 10f,
            float valuePerLevel = 10f,
            bool isPercentage = true)
        {
            var data = ScriptableObject.CreateInstance<PassiveData>();

            data.Id = id;
            data.DisplayName = displayName;
            data.PrimaryStat = primaryStat;
            data.BaseValue = baseValue;
            data.ValuePerLevel = valuePerLevel;
            data.IsPercentage = isPercentage;

            return data;
        }

        #endregion

        #region CharacterData

        /// <summary>
        /// 기본 CharacterData 생성
        /// </summary>
        public static CharacterData CreateCharacterData(
            string id = "test_character",
            string displayName = "테스트 캐릭터",
            float baseMaxHealth = 100f,
            float baseMoveSpeed = 5f,
            float baseArmor = 0f)
        {
            var data = ScriptableObject.CreateInstance<CharacterData>();

            data.Id = id;
            data.DisplayName = displayName;
            data.BaseMaxHealth = baseMaxHealth;
            data.BaseMoveSpeed = baseMoveSpeed;
            data.BaseArmor = baseArmor;

            return data;
        }

        #endregion

        #region StageData

        /// <summary>
        /// 기본 StageData 생성
        /// </summary>
        public static StageData CreateStageData(
            string id = "test_stage",
            string displayName = "테스트 스테이지",
            float duration = 300f,
            float enemyHealthGrowth = 0.05f,
            float enemyDamageGrowth = 0.03f)
        {
            var data = ScriptableObject.CreateInstance<StageData>();

            data.Id = id;
            data.DisplayName = displayName;
            data.Duration = duration;
            data.EnemyHealthGrowth = enemyHealthGrowth;
            data.EnemyDamageGrowth = enemyDamageGrowth;

            return data;
        }

        #endregion
    }
}
