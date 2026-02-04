using UnityEngine;
using Vs.Core;
using Vs.Data;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 범위 무기 - 투척 후 지면에 지속 피해 영역을 생성합니다.
    /// 화염병 등에 사용됩니다.
    /// </summary>
    public class AreaWeapon : WeaponBase
    {
        [Header("Area Settings")]
        [SerializeField] private GameObject _zonePrefab;
        [SerializeField] private float _throwRange = 8f;
        [SerializeField] private float _throwArcHeight = 3f;

        #region Public Methods

        public override void Initialize(Transform owner, WeaponData data)
        {
            base.Initialize(owner, data);

            // WeaponData의 프리팹이 있으면 사용
            if (_zonePrefab == null && data.ProjectilePrefab != null)
            {
                _zonePrefab = data.ProjectilePrefab;
            }
        }

        #endregion

        #region Protected Methods

        protected override void Fire()
        {
            if (_zonePrefab == null)
            {
                Debug.LogWarning($"[AreaWeapon] {_data.Id} has no zone prefab");
                return;
            }

            Vector3 targetPos = GetTargetPosition();
            SpawnFireZone(targetPos);
        }

        #endregion

        #region Private Methods

        private Vector3 GetTargetPosition()
        {
            // 가장 가까운 적 위치 탐색
            Transform nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                Vector3 dirToEnemy = nearestEnemy.position - _owner.position;
                float distance = dirToEnemy.magnitude;

                // 범위 내면 적 위치, 아니면 최대 범위
                if (distance <= _throwRange)
                {
                    return new Vector3(nearestEnemy.position.x, 0f, nearestEnemy.position.z);
                }
                else
                {
                    Vector3 dir = dirToEnemy.normalized;
                    return _owner.position + dir * _throwRange;
                }
            }

            // 적이 없으면 플레이어 전방
            return _owner.position + _owner.forward * (_throwRange * 0.5f);
        }

        private Transform FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.TagEnemy);

            if (enemies.Length == 0) return null;

            Transform nearest = null;
            float nearestDistance = float.MaxValue;
            Vector3 ownerPos = _owner.position;

            foreach (var enemy in enemies)
            {
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

        private void SpawnFireZone(Vector3 position)
        {
            // Y 좌표는 지면 높이로 고정
            position.y = 0.1f;

            var zoneObj = PoolManager.Instance.Spawn(_zonePrefab, position, Quaternion.identity);

            if (zoneObj.TryGetComponent<FireZone>(out var fireZone))
            {
                // area는 반지름, duration은 지속 시간
                fireZone.Initialize(_damage, _duration, _area);
                fireZone.SetPrefabSource(_zonePrefab);
            }
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_owner == null && Application.isPlaying) return;

            Transform reference = _owner != null ? _owner : transform;

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(reference.position, _throwRange);
        }
#endif
    }
}
