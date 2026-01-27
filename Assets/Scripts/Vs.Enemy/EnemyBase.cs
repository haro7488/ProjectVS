using System;
using UnityEngine;
using Vs.Combat;
using Vs.Core;
using Vs.Data;

namespace Vs.Enemy
{
    /// <summary>
    /// 적 기본 클래스. IDamageable과 IPoolable 인터페이스 구현.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyBase : MonoBehaviour, IDamageable, IPoolable
    {
        [SerializeField] protected EnemyData _data;

        protected float _currentHealth;
        protected Transform _target;
        protected float _lastContactDamageTime;
        protected Rigidbody2D _rb;
        protected SpriteRenderer _spriteRenderer;

        private float _healthMultiplier = 1f;
        private float _damageMultiplier = 1f;

        #region IDamageable Implementation

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _data != null ? _data.MaxHealth * _healthMultiplier : 0f;
        public bool IsDead => _currentHealth <= 0;

        public event Action<DamageInfo> OnDamaged;
        public event Action OnDeath;

        #endregion

        /// <summary>
        /// 적 사망 시 발생하는 이벤트.
        /// </summary>
        public static event Action<EnemyDeathEvent> OnEnemyDied;

        public EnemyData Data => _data;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // Rigidbody2D 설정
            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        /// <summary>
        /// 적 초기화. 스폰 시 호출.
        /// </summary>
        public virtual void Initialize(EnemyData data, Transform target, float healthMultiplier = 1f,
            float damageMultiplier = 1f)
        {
            _data = data;
            _target = target;
            _healthMultiplier = healthMultiplier;
            _damageMultiplier = damageMultiplier;

            _currentHealth = MaxHealth;
            _lastContactDamageTime = -_data.AttackCooldown; // 즉시 접촉 대미지 가능

            // 비주얼 설정
            if (_spriteRenderer != null && _data.Sprite != null)
            {
                _spriteRenderer.sprite = _data.Sprite;
            }

            // 크기 설정
            transform.localScale = Vector3.one * _data.Scale;

            // AI 컴포넌트 초기화
            var ai = GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.Initialize(_target, _data.MoveSpeed);
            }
        }

        #region IDamageable Methods

        public void TakeDamage(DamageInfo damage)
        {
            if (IsDead) return;

            _currentHealth -= damage.Amount;
            _currentHealth = Mathf.Max(0f, _currentHealth);

            OnDamaged?.Invoke(damage);

            // 넉백 적용
            if (damage.Knockback > 0f && _rb != null)
            {
                _rb.AddForce(damage.Direction.normalized * damage.Knockback, ForceMode2D.Impulse);
            }

            if (IsDead)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, MaxHealth);
        }

        #endregion

        #region IPoolable Implementation

        public void OnSpawn()
        {
            gameObject.SetActive(true);
        }

        public void OnDespawn()
        {
            // 이벤트 정리
            OnDamaged = null;
            OnDeath = null;

            // 속도 초기화
            if (_rb != null)
            {
                _rb.velocity = Vector2.zero;
            }
        }

        #endregion

        protected virtual void Die()
        {
            OnDeath?.Invoke();

            // 사망 이벤트 발생 (경험치, 골드 드롭)
            var deathEvent = new EnemyDeathEvent(transform.position, _data);
            OnEnemyDied?.Invoke(deathEvent);

            // 풀에 반환
            if (PoolManager.HasInstance)
            {
                PoolManager.Instance.Despawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnCollisionStay2D(Collision2D other)
        {
            // 접촉 대미지 쿨다운 확인
            if (Time.time < _lastContactDamageTime + _data.AttackCooldown) return;

            // 플레이어에게만 접촉 대미지
            if (!other.gameObject.CompareTag(Vs.Utility.Constants.TagPlayer)) return;

            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable == null) return;

            float damage = _data.ContactDamage * _damageMultiplier;
            Vector2 direction = (other.transform.position - transform.position).normalized;

            var damageInfo = new DamageInfo(
                amount: damage,
                type: DamageType.Physical,
                position: other.GetContact(0).point,
                direction: direction,
                source: gameObject
            );

            damageable.TakeDamage(damageInfo);
            _lastContactDamageTime = Time.time;
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (_data == null) return;

            // 충돌 범위 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _data.ColliderRadius);
        }
#endif
    }
}