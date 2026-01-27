using UnityEngine;
using Vs.Data;

namespace Vs.Enemy
{
    /// <summary>
    /// 적 사망 시 발생하는 이벤트 데이터.
    /// </summary>
    public readonly struct EnemyDeathEvent
    {
        public Vector2 Position { get; }
        public int ExpValue { get; }
        public int GoldValue { get; }
        public bool DropGold { get; }

        public EnemyDeathEvent(Vector2 position, EnemyData data)
        {
            Position = position;
            ExpValue = data.ExpValue;
            GoldValue = data.GoldValue;
            DropGold = Random.value <= data.GoldDropChance;
        }

        public EnemyDeathEvent(Vector2 position, int expValue, int goldValue, bool dropGold)
        {
            Position = position;
            ExpValue = expValue;
            GoldValue = goldValue;
            DropGold = dropGold;
        }
    }
}