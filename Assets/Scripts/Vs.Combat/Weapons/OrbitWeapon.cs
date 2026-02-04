using System.Collections.Generic;
using UnityEngine;
using Vs.Core;
using Vs.Data;

namespace Vs.Combat
{
    /// <summary>
    /// 궤도 무기 - 플레이어 주변을 회전하는 오브젝트로 적에게 데미지를 줍니다.
    /// 드론 등에 사용됩니다.
    /// </summary>
    public class OrbitWeapon : WeaponBase
    {
        [Header("Orbit Settings")]
        [SerializeField] private GameObject _orbiterPrefab;
        [SerializeField] private float _baseOrbitRadius = 2f;
        [SerializeField] private float _baseOrbitSpeed = 180f; // degrees per second

        private readonly List<Orbiter> _activeOrbiters = new List<Orbiter>();
        private bool _isInitialized;

        #region Properties

        public int OrbiterCount => _activeOrbiters.Count;

        #endregion

        #region Unity Lifecycle

        protected override void Update()
        {
            // 자동 발사 대신 Orbiter 수 관리
            if (_data == null || _owner == null) return;

            ManageOrbiters();
        }

        private void OnDisable()
        {
            DespawnAllOrbiters();
        }

        private void OnDestroy()
        {
            DespawnAllOrbiters();
        }

        #endregion

        #region Public Methods

        public override void Initialize(Transform owner, WeaponData data)
        {
            base.Initialize(owner, data);

            // WeaponData의 프리팹이 있으면 사용
            if (_orbiterPrefab == null && data.ProjectilePrefab != null)
            {
                _orbiterPrefab = data.ProjectilePrefab;
            }

            _isInitialized = true;

            // 초기 Orbiter 생성
            SpawnOrbiters();
        }

        public override void LevelUp()
        {
            base.LevelUp();

            // 레벨업 시 Orbiter 재구성
            UpdateOrbiters();
        }

        #endregion

        #region Protected Methods

        protected override void Fire()
        {
            // OrbitWeapon은 Fire를 사용하지 않음
            // 대신 Orbiter들이 지속적으로 데미지를 줌
        }

        #endregion

        #region Private Methods

        private void ManageOrbiters()
        {
            if (!_isInitialized) return;

            // 죽은 Orbiter 제거
            _activeOrbiters.RemoveAll(o => o == null);

            // Orbiter 수가 projectileCount와 다르면 재구성
            if (_activeOrbiters.Count != _projectileCount)
            {
                SpawnOrbiters();
            }
        }

        private void SpawnOrbiters()
        {
            if (_orbiterPrefab == null)
            {
                Debug.LogWarning($"[OrbitWeapon] {_data.Id} has no orbiter prefab");
                return;
            }

            // 기존 Orbiter 정리
            DespawnAllOrbiters();

            // 새 Orbiter 생성
            float radius = _baseOrbitRadius * _area;
            float speed = _baseOrbitSpeed * _speed;

            for (int i = 0; i < _projectileCount; i++)
            {
                var orbiterObj = PoolManager.Instance.Spawn(
                    _orbiterPrefab,
                    _owner.position,
                    Quaternion.identity
                );

                if (orbiterObj.TryGetComponent<Orbiter>(out var orbiter))
                {
                    orbiter.Initialize(_owner, _damage, radius, speed, i, _projectileCount);
                    orbiter.SetPrefabSource(_orbiterPrefab);
                    _activeOrbiters.Add(orbiter);
                }
            }
        }

        private void UpdateOrbiters()
        {
            float radius = _baseOrbitRadius * _area;
            float speed = _baseOrbitSpeed * _speed;

            // 추가 Orbiter가 필요한 경우
            while (_activeOrbiters.Count < _projectileCount)
            {
                if (_orbiterPrefab == null) break;

                var orbiterObj = PoolManager.Instance.Spawn(
                    _orbiterPrefab,
                    _owner.position,
                    Quaternion.identity
                );

                if (orbiterObj.TryGetComponent<Orbiter>(out var orbiter))
                {
                    int index = _activeOrbiters.Count;
                    orbiter.Initialize(_owner, _damage, radius, speed, index, _projectileCount);
                    orbiter.SetPrefabSource(_orbiterPrefab);
                    _activeOrbiters.Add(orbiter);
                }
            }

            // 기존 Orbiter 스탯 업데이트
            for (int i = 0; i < _activeOrbiters.Count; i++)
            {
                var orbiter = _activeOrbiters[i];
                if (orbiter == null) continue;

                orbiter.UpdateDamage(_damage);
                orbiter.UpdateRadius(radius);
                orbiter.UpdateSpeed(speed);
            }

            // 각도 재분배
            RedistributeOrbiters();
        }

        private void RedistributeOrbiters()
        {
            int total = _activeOrbiters.Count;
            float radius = _baseOrbitRadius * _area;
            float speed = _baseOrbitSpeed * _speed;

            for (int i = 0; i < total; i++)
            {
                var orbiter = _activeOrbiters[i];
                if (orbiter == null) continue;

                // 새로운 인덱스와 총 개수로 재초기화
                orbiter.Initialize(_owner, _damage, radius, speed, i, total);
            }
        }

        private void DespawnAllOrbiters()
        {
            foreach (var orbiter in _activeOrbiters)
            {
                if (orbiter != null)
                {
                    orbiter.ReturnToPool();
                }
            }

            _activeOrbiters.Clear();
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_owner == null && Application.isPlaying) return;

            Transform reference = _owner != null ? _owner : transform;
            float radius = _baseOrbitRadius * (_area > 0 ? _area : 1f);

            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawWireSphere(reference.position, radius);
        }
#endif
    }
}
