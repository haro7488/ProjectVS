using UnityEngine;

namespace Vs.Data
{
    public enum WeaponType
    {
        Projectile,     // 투사체 (권총, 나이프)
        Melee,          // 근접 (방망이)
        Area,           // 범위 (화염병)
        Orbital,        // 궤도 (드론)
        Magic           // 마법 (번개)
    }

    [CreateAssetMenu(fileName = "Weapon_", menuName = "VS/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("기본 정보")]
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;

        [Header("프리팹")]
        public GameObject WeaponPrefab;
        public GameObject ProjectilePrefab;

        [Header("타입")]
        public WeaponType Type;

        [Header("기본 스탯 (Lv.1)")]
        public float BaseDamage = 10f;
        public float BaseInterval = 1f;
        public int BaseProjectileCount = 1;
        public float BaseArea = 1f;
        public float BaseDuration = 0f;
        public float BaseSpeed = 10f;

        [Header("진화")]
        public WeaponData EvolvesTo;
        public PassiveData EvolutionRequirement;
        public bool IsEvolved;

        [Header("비주얼")]
        public Color ProjectileColor = Color.white;

        public bool CanEvolve => EvolvesTo != null && EvolutionRequirement != null;
    }
}
