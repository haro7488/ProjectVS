using UnityEngine;

namespace Vs.Combat
{
    public enum DamageType
    {
        Physical,
        Fire,
        Electric,
        Poison
    }

    public readonly struct DamageInfo
    {
        public float Amount { get; }
        public DamageType Type { get; }
        public Vector2 Position { get; }
        public Vector2 Direction { get; }
        public GameObject Source { get; }
        public bool IsCritical { get; }
        public float Knockback { get; }

        public DamageInfo(
            float amount,
            DamageType type = DamageType.Physical,
            Vector2 position = default,
            Vector2 direction = default,
            GameObject source = null,
            bool isCritical = false,
            float knockback = 0f)
        {
            Amount = amount;
            Type = type;
            Position = position;
            Direction = direction;
            Source = source;
            IsCritical = isCritical;
            Knockback = knockback;
        }

        public DamageInfo WithAmount(float amount)
        {
            return new DamageInfo(amount, Type, Position, Direction, Source, IsCritical, Knockback);
        }

        public DamageInfo AsCritical(float multiplier = 2f)
        {
            return new DamageInfo(Amount * multiplier, Type, Position, Direction, Source, true, Knockback);
        }
    }
}
