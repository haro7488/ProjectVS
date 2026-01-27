using System;

namespace Vs.Combat
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }

        void TakeDamage(DamageInfo damage);
        void Heal(float amount);

        event Action<DamageInfo> OnDamaged;
        event Action OnDeath;
    }
}
