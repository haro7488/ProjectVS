using System.Collections.Generic;
using UnityEngine;
using Vs.Core;
using Vs.Data;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 번개 무기 - 랜덤 적에게 즉시 데미지를 줍니다.
    /// 연쇄 타격을 지원합니다.
    /// </summary>
    public class LightningWeapon : WeaponBase
    {
        [Header("Lightning Settings")]
        [SerializeField] private float _strikeRange = 15f;
        [SerializeField] private float _chainRange = 5f;
        [SerializeField] private int _baseChainCount = 0;
        [SerializeField] private GameObject _lightningEffectPrefab;
        [SerializeField] private float _effectDuration = 0.3f;

        private readonly List<Transform> _potentialTargets = new List<Transform>();
        private readonly HashSet<int> _struckEnemies = new HashSet<int>();

        #region Public Methods

        public override void Initialize(Transform owner, WeaponData data)
        {
            base.Initialize(owner, data);

            // WeaponData의 프리팹이 있으면 이펙트로 사용
            if (_lightningEffectPrefab == null && data.ProjectilePrefab != null)
            {
                _lightningEffectPrefab = data.ProjectilePrefab;
            }
        }

        #endregion

        #region Protected Methods

        protected override void Fire()
        {
            _struckEnemies.Clear();

            // projectileCount만큼 초기 타격
            for (int i = 0; i < _projectileCount; i++)
            {
                Transform target = FindRandomTarget();
                if (target == null) break;

                StrikeLightning(_owner.position, target);
            }
        }

        #endregion

        #region Private Methods

        private Transform FindRandomTarget()
        {
            _potentialTargets.Clear();

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.TagEnemy);

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                // 이미 타격한 적 제외
                int instanceId = enemy.GetInstanceID();
                if (_struckEnemies.Contains(instanceId)) continue;

                // 범위 체크
                float distance = Vector3.Distance(_owner.position, enemy.transform.position);
                if (distance > _strikeRange) continue;

                // 죽은 적 제외
                if (enemy.TryGetComponent<IDamageable>(out var damageable) && damageable.IsDead)
                {
                    continue;
                }

                _potentialTargets.Add(enemy.transform);
            }

            if (_potentialTargets.Count == 0) return null;

            // 랜덤 선택
            int randomIndex = Random.Range(0, _potentialTargets.Count);
            return _potentialTargets[randomIndex];
        }

        private Transform FindChainTarget(Vector3 fromPosition)
        {
            _potentialTargets.Clear();

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.TagEnemy);

            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                // 이미 타격한 적 제외
                int instanceId = enemy.GetInstanceID();
                if (_struckEnemies.Contains(instanceId)) continue;

                // 연쇄 범위 체크
                float distance = Vector3.Distance(fromPosition, enemy.transform.position);
                if (distance > _chainRange) continue;

                // 죽은 적 제외
                if (enemy.TryGetComponent<IDamageable>(out var damageable) && damageable.IsDead)
                {
                    continue;
                }

                _potentialTargets.Add(enemy.transform);
            }

            if (_potentialTargets.Count == 0) return null;

            // 가장 가까운 적 선택
            Transform nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var target in _potentialTargets)
            {
                float dist = Vector3.Distance(fromPosition, target.position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearest = target;
                }
            }

            return nearest;
        }

        private void StrikeLightning(Vector3 fromPosition, Transform target)
        {
            if (target == null) return;

            int instanceId = target.gameObject.GetInstanceID();
            _struckEnemies.Add(instanceId);

            Vector3 targetPos = target.position;

            // 데미지 적용
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                Vector3 direction = (targetPos - fromPosition).normalized;

                var damageInfo = new DamageInfo(
                    amount: _damage,
                    type: DamageType.Electric,
                    position: targetPos,
                    direction: direction,
                    source: gameObject
                );

                damageable.TakeDamage(damageInfo);
            }

            // 이펙트 생성
            SpawnLightningEffect(fromPosition, targetPos);

            // 연쇄 처리 (area를 연쇄 횟수로 사용)
            int chainCount = _baseChainCount + Mathf.FloorToInt(_area);
            if (chainCount > 0)
            {
                ProcessChainLightning(targetPos, chainCount);
            }
        }

        private void ProcessChainLightning(Vector3 fromPosition, int remainingChains)
        {
            if (remainingChains <= 0) return;

            Transform chainTarget = FindChainTarget(fromPosition);
            if (chainTarget == null) return;

            int instanceId = chainTarget.gameObject.GetInstanceID();
            _struckEnemies.Add(instanceId);

            Vector3 targetPos = chainTarget.position;

            // 연쇄 데미지 (원래 데미지의 80%)
            float chainDamage = _damage * 0.8f;

            if (chainTarget.TryGetComponent<IDamageable>(out var damageable))
            {
                Vector3 direction = (targetPos - fromPosition).normalized;

                var damageInfo = new DamageInfo(
                    amount: chainDamage,
                    type: DamageType.Electric,
                    position: targetPos,
                    direction: direction,
                    source: gameObject
                );

                damageable.TakeDamage(damageInfo);
            }

            // 연쇄 이펙트
            SpawnLightningEffect(fromPosition, targetPos);

            // 추가 연쇄
            ProcessChainLightning(targetPos, remainingChains - 1);
        }

        private void SpawnLightningEffect(Vector3 fromPos, Vector3 toPos)
        {
            if (_lightningEffectPrefab == null) return;

            // 중간 지점에 이펙트 생성
            Vector3 midPoint = (fromPos + toPos) / 2f;
            midPoint.y = Mathf.Max(fromPos.y, toPos.y) + 1f;

            var effectObj = PoolManager.Instance.Spawn(
                _lightningEffectPrefab,
                midPoint,
                Quaternion.identity
            );

            // LineRenderer가 있으면 설정
            if (effectObj.TryGetComponent<LineRenderer>(out var lineRenderer))
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, fromPos + Vector3.up * 0.5f);
                lineRenderer.SetPosition(1, toPos + Vector3.up * 0.5f);
            }

            // 일정 시간 후 반환
            PoolManager.Instance.Despawn(effectObj, _effectDuration);
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_owner == null && Application.isPlaying) return;

            Transform reference = _owner != null ? _owner : transform;

            // 공격 범위
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(reference.position, _strikeRange);

            // 연쇄 범위
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.2f);
            Gizmos.DrawWireSphere(reference.position, _chainRange);
        }
#endif
    }
}
