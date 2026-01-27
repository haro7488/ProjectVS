using UnityEngine;

namespace Vs.Data
{
    public enum EnemyBehavior
    {
        ChasePlayer,    // 플레이어 추적
        Patrol,         // 순찰
        Ranged,         // 원거리 공격
        Explode,        // 자폭
        Boss            // 보스
    }

    [CreateAssetMenu(fileName = "Enemy_", menuName = "VS/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("기본 정보")]
        public string Id;
        public string DisplayName;
        public Sprite Sprite;
        public RuntimeAnimatorController Animator;
        public GameObject Prefab;

        [Header("스탯")]
        public float MaxHealth = 10f;
        public float MoveSpeed = 2f;
        public float ContactDamage = 10f;
        public float AttackCooldown = 1f;

        [Header("행동")]
        public EnemyBehavior Behavior = EnemyBehavior.ChasePlayer;

        [Header("보상")]
        public int ExpValue = 1;
        public int GoldValue = 0;
        [Range(0f, 1f)] public float GoldDropChance = 0.1f;

        [Header("크기")]
        public float Scale = 1f;
        public float ColliderRadius = 0.5f;

        [Header("보스 전용")]
        public bool IsBoss = false;
        public float BossHealthMultiplier = 10f;
    }
}
