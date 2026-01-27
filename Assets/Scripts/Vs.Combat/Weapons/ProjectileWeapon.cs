using UnityEngine;
using Vs.Core;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 투사체를 발사하는 무기.
    /// 가장 가까운 적 방향으로 발사하며, 다중 투사체를 지원합니다.
    /// </summary>
    public class ProjectileWeapon : WeaponBase
    {
        [Header("Projectile Settings")] [SerializeField]
        private GameObject _projectilePrefab;

        [SerializeField] private float _spreadAngle = 15f;
        [SerializeField] private bool _isPiercing;
        [SerializeField] private int _pierceCount = 1;

        private Vector3 _lastMoveDirection = Vector3.forward;

        #region Public Methods

        public override void Initialize(Transform owner, Data.WeaponData data)
        {
            base.Initialize(owner, data);

            // WeaponData의 프리팹이 있으면 사용
            if (_projectilePrefab == null && data.ProjectilePrefab != null)
            {
                _projectilePrefab = data.ProjectilePrefab;
            }
        }

        /// <summary>
        /// 마지막 이동 방향을 업데이트합니다 (플레이어 이동 시 호출).
        /// </summary>
        public void UpdateMoveDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                _lastMoveDirection = new Vector3(direction.x, 0f, direction.z).normalized;
            }
        }

        #endregion

        #region Protected Methods

        protected override void Fire()
        {
            if (_projectilePrefab == null)
            {
                Debug.LogWarning($"[ProjectileWeapon] {_data.Id} has no projectile prefab");
                return;
            }

            Vector3 fireDirection = GetFireDirection();

            if (_projectileCount <= 1)
            {
                SpawnProjectile(fireDirection);
            }
            else
            {
                SpawnMultipleProjectiles(fireDirection);
            }
        }

        #endregion

        #region Private Methods

        private Vector3 GetFireDirection()
        {
            Transform nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                Vector3 dir = nearestEnemy.position - _owner.position;
                dir.y = 0f; // XZ 평면에서만
                return dir.normalized;
            }

            return _lastMoveDirection;
        }

        private Transform FindNearestEnemy()
        {
            // 모든 Enemy 태그 오브젝트 검색
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.TagEnemy);

            if (enemies.Length == 0) return null;

            Transform nearest = null;
            float nearestDistance = float.MaxValue;
            Vector3 ownerPos = _owner.position;

            foreach (var enemy in enemies)
            {
                // 죽은 적은 제외
                if (enemy.TryGetComponent<IDamageable>(out var damageable) && damageable.IsDead)
                {
                    continue;
                }

                float distance = Vector3.Distance(ownerPos, enemy.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy.transform;
                }
            }

            return nearest;
        }

        private void SpawnProjectile(Vector3 direction)
        {
            Vector3 spawnPos = _owner.position;
            Quaternion rotation = Quaternion.LookRotation(direction);

            var projectileObj = PoolManager.Instance.Spawn(_projectilePrefab, spawnPos, rotation);

            if (projectileObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(_damage, _speed, direction, _duration > 0 ? _duration : 5f);
                projectile.SetPiercing(_isPiercing, _pierceCount);
                projectile.SetPrefabSource(_projectilePrefab);
            }
        }

        private void SpawnMultipleProjectiles(Vector3 centerDirection)
        {
            float totalSpread = _spreadAngle * (_projectileCount - 1);
            float startAngle = -totalSpread / 2f;
            // XZ 평면에서 각도 계산
            float baseAngle = Mathf.Atan2(centerDirection.x, centerDirection.z) * Mathf.Rad2Deg;

            for (int i = 0; i < _projectileCount; i++)
            {
                float angle = baseAngle + startAngle + (_spreadAngle * i);
                float rad = angle * Mathf.Deg2Rad;
                // XZ 평면에서 방향 (Y축 회전)
                Vector3 direction = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));

                SpawnProjectile(direction);
            }
        }

        #endregion
    }
}