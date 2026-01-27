using System;
using UnityEngine;

namespace Vs.Data
{
    [Serializable]
    public class EnemyWave
    {
        public float StartTime;
        public float EndTime = -1f; // -1 = 스테이지 끝까지
        public EnemyData Enemy;
        public float SpawnInterval = 2f;
        public int SpawnCount = 1;
        public float SpawnCountGrowth = 0.1f; // 시간당 증가율
    }

    [Serializable]
    public class BossSpawn
    {
        public float SpawnTime;
        public EnemyData Boss;
    }

    [CreateAssetMenu(fileName = "Stage_", menuName = "VS/Stage Data")]
    public class StageData : ScriptableObject
    {
        [Header("기본 정보")]
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Thumbnail;

        [Header("맵")]
        public GameObject MapPrefab;
        public Vector2 MapSize = new(50f, 50f);
        public Color BackgroundColor = Color.gray;

        [Header("시간")]
        public float Duration = 1200f; // 20분

        [Header("적 웨이브")]
        public EnemyWave[] Waves;

        [Header("보스")]
        public BossSpawn[] BossSpawns;

        [Header("난이도")]
        public float DifficultyMultiplier = 1f;
        public float EnemyHealthGrowth = 0.05f; // 분당 증가율
        public float EnemyDamageGrowth = 0.03f;

        [Header("언락")]
        public bool IsUnlockedByDefault = true;
        public string UnlockConditionDescription;

        public float GetEnemyHealthMultiplier(float elapsedMinutes)
        {
            return 1f + (EnemyHealthGrowth * elapsedMinutes * DifficultyMultiplier);
        }

        public float GetEnemyDamageMultiplier(float elapsedMinutes)
        {
            return 1f + (EnemyDamageGrowth * elapsedMinutes * DifficultyMultiplier);
        }
    }
}
