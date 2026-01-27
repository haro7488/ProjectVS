using UnityEngine;
using Vs.Data;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 모든 무기의 기본 추상 클래스.
    /// 자동 발사, 레벨업, 밸런스 데이터 로딩을 처리합니다.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData _data;

        protected int _level = 1;
        protected float _lastFireTime;
        protected Transform _owner;

        // 런타임 스탯 (JSON 밸런스 적용)
        protected float _damage;
        protected float _interval;
        protected int _projectileCount;
        protected float _area;
        protected float _duration;
        protected float _speed;

        #region Properties

        public WeaponData Data => _data;
        public int Level => _level;
        public bool IsMaxLevel => _level >= Constants.MaxWeaponLevel;
        public bool CanEvolve => _data != null && _data.CanEvolve && IsMaxLevel;

        protected float Damage => _damage;
        protected float Interval => _interval;
        protected int ProjectileCount => _projectileCount;
        protected float Area => _area;
        protected float Duration => _duration;
        protected float Speed => _speed;

        #endregion

        #region Public Methods

        /// <summary>
        /// 무기를 초기화합니다.
        /// </summary>
        public virtual void Initialize(Transform owner, WeaponData data)
        {
            _owner = owner;
            _data = data;
            _level = 1;
            _lastFireTime = -999f; // 즉시 발사 가능하도록

            LoadBalanceData();
        }

        /// <summary>
        /// 무기 레벨을 1 증가시킵니다.
        /// </summary>
        public virtual void LevelUp()
        {
            if (IsMaxLevel)
            {
                Debug.LogWarning($"[WeaponBase] {_data.Id} is already at max level");
                return;
            }

            _level++;
            LoadBalanceData();

            Debug.Log($"[WeaponBase] {_data.Id} leveled up to {_level}");
        }

        #endregion

        #region Protected Methods

        protected virtual void Update()
        {
            if (_data == null || _owner == null) return;

            float timeSinceLastFire = Time.time - _lastFireTime;

            if (timeSinceLastFire >= _interval)
            {
                Fire();
                _lastFireTime = Time.time;
            }
        }

        /// <summary>
        /// 무기를 발사합니다. 각 무기 타입에서 구현해야 합니다.
        /// </summary>
        protected abstract void Fire();

        /// <summary>
        /// JSON 밸런스 데이터를 로드하여 스탯을 설정합니다.
        /// 밸런스 데이터가 없으면 SO의 기본값을 사용합니다.
        /// </summary>
        protected virtual void LoadBalanceData()
        {
            if (_data == null) return;

            var balanceData = BalanceLoader.GetWeaponLevel(_data.Id, _level);

            if (balanceData != null)
            {
                _damage = balanceData.damage;
                _interval = balanceData.interval;
                _projectileCount = balanceData.projectiles;
                _area = balanceData.area;
                _duration = balanceData.duration;
                _speed = balanceData.speed;
            }
            else
            {
                // 밸런스 데이터가 없으면 SO 기본값 사용
                _damage = _data.BaseDamage;
                _interval = _data.BaseInterval;
                _projectileCount = _data.BaseProjectileCount;
                _area = _data.BaseArea;
                _duration = _data.BaseDuration;
                _speed = _data.BaseSpeed;
            }
        }

        #endregion
    }
}