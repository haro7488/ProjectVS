using System.Collections.Generic;
using UnityEngine;
using Vs.Data;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 근접 무기 - 전방 범위를 휘둘러 적에게 데미지를 줍니다.
    /// 야구 방망이 등에 사용됩니다.
    /// </summary>
    public class MeleeWeapon : WeaponBase
    {
        [Header("Melee Settings")]
        [SerializeField] private float _swingAngle = 90f;
        [SerializeField] private float _baseRange = 2f;
        [SerializeField] private LayerMask _enemyLayer;

        private readonly Collider[] _hitBuffer = new Collider[32];
        private readonly HashSet<int> _hitEnemies = new HashSet<int>();

        #region Public Methods

        public override void Initialize(Transform owner, WeaponData data)
        {
            base.Initialize(owner, data);

            // Enemy 레이어 자동 설정
            if (_enemyLayer == 0)
            {
                _enemyLayer = LayerMask.GetMask(Constants.LayerEnemy);
            }
        }

        #endregion

        #region Protected Methods

        protected override void Fire()
        {
            PerformSwing();
        }

        #endregion

        #region Private Methods

        private void PerformSwing()
        {
            _hitEnemies.Clear();

            Vector3 ownerPos = _owner.position;
            Vector3 forward = _owner.forward;
            float range = _baseRange * _area;
            float halfAngle = _swingAngle / 2f;

            // 범위 내 모든 적 검색
            int hitCount = Physics.OverlapSphereNonAlloc(
                ownerPos,
                range,
                _hitBuffer,
                _enemyLayer
            );

            for (int i = 0; i < hitCount; i++)
            {
                var collider = _hitBuffer[i];
                if (collider == null) continue;

                // 이미 타격한 적은 제외
                int instanceId = collider.gameObject.GetInstanceID();
                if (_hitEnemies.Contains(instanceId)) continue;

                // 전방 각도 체크
                Vector3 directionToEnemy = (collider.transform.position - ownerPos).normalized;
                directionToEnemy.y = 0f;

                float angle = Vector3.Angle(forward, directionToEnemy);
                if (angle > halfAngle) continue;

                // 데미지 적용
                if (collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable.IsDead) continue;

                    var damageInfo = new DamageInfo(
                        amount: _damage,
                        type: DamageType.Physical,
                        position: collider.transform.position,
                        direction: directionToEnemy,
                        source: gameObject,
                        knockback: 2f
                    );

                    damageable.TakeDamage(damageInfo);
                    _hitEnemies.Add(instanceId);
                }
            }

            // 스윙 이펙트 (선택적)
            // TODO: 파티클 또는 트레일 이펙트 추가
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_owner == null && Application.isPlaying) return;

            Transform reference = _owner != null ? _owner : transform;
            Vector3 pos = reference.position;
            Vector3 forward = reference.forward;
            float range = _baseRange * (_area > 0 ? _area : 1f);

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);

            // 범위 원
            Gizmos.DrawWireSphere(pos, range);

            // 전방 각도 표시
            float halfAngle = _swingAngle / 2f;
            Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * forward;
            Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * forward;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + leftDir * range);
            Gizmos.DrawLine(pos, pos + rightDir * range);
        }
#endif
    }
}