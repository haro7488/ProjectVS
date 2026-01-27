using System.Collections.Generic;
using UnityEngine;
using Vs.Data;
using Vs.Utility;

namespace Vs.Combat
{
    /// <summary>
    /// 플레이어의 무기 슬롯을 관리합니다.
    /// 최대 6개의 무기를 장착할 수 있으며, 동적으로 추가/레벨업이 가능합니다.
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private Transform _weaponContainer;

        private readonly List<WeaponBase> _weapons = new();
        private Transform _owner;

        #region Properties

        public int WeaponCount => _weapons.Count;
        public bool HasEmptySlot => _weapons.Count < Constants.MaxWeaponSlots;
        public IReadOnlyList<WeaponBase> Weapons => _weapons;

        #endregion

        #region Public Methods

        /// <summary>
        /// WeaponController를 초기화합니다.
        /// </summary>
        public void Initialize(Transform owner)
        {
            _owner = owner;

            if (_weaponContainer == null)
            {
                _weaponContainer = transform;
            }
        }

        /// <summary>
        /// 새로운 무기를 추가합니다.
        /// </summary>
        /// <returns>추가 성공 여부</returns>
        public bool AddWeapon(WeaponData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[WeaponController] Cannot add null weapon data");
                return false;
            }

            // 이미 보유한 무기인지 확인
            if (HasWeapon(data.Id))
            {
                Debug.Log($"[WeaponController] Already has weapon: {data.Id}, leveling up instead");
                return LevelUpWeapon(data);
            }

            // 슬롯 확인
            if (!HasEmptySlot)
            {
                Debug.LogWarning($"[WeaponController] No empty slots for weapon: {data.Id}");
                return false;
            }

            // 무기 프리팹 확인
            if (data.WeaponPrefab == null)
            {
                Debug.LogWarning($"[WeaponController] Weapon {data.Id} has no prefab");
                return false;
            }

            // 무기 인스턴스화
            var weaponObj = Instantiate(data.WeaponPrefab, _weaponContainer);
            weaponObj.name = $"Weapon_{data.Id}";

            var weapon = weaponObj.GetComponent<WeaponBase>();

            if (weapon == null)
            {
                Debug.LogError($"[WeaponController] Weapon prefab {data.Id} has no WeaponBase component");
                Destroy(weaponObj);
                return false;
            }

            weapon.Initialize(_owner, data);
            _weapons.Add(weapon);

            Debug.Log($"[WeaponController] Added weapon: {data.Id}");
            return true;
        }

        /// <summary>
        /// 무기를 레벨업합니다.
        /// </summary>
        /// <returns>레벨업 성공 여부</returns>
        public bool LevelUpWeapon(WeaponData data)
        {
            var weapon = GetWeapon(data.Id);

            if (weapon == null)
            {
                Debug.LogWarning($"[WeaponController] Cannot level up, weapon not found: {data.Id}");
                return false;
            }

            if (weapon.IsMaxLevel)
            {
                Debug.LogWarning($"[WeaponController] Weapon {data.Id} is already at max level");
                return false;
            }

            weapon.LevelUp();
            return true;
        }

        /// <summary>
        /// ID로 무기를 찾습니다.
        /// </summary>
        public WeaponBase GetWeapon(string weaponId)
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.Data != null && weapon.Data.Id == weaponId)
                {
                    return weapon;
                }
            }

            return null;
        }

        /// <summary>
        /// 해당 무기를 보유하고 있는지 확인합니다.
        /// </summary>
        public bool HasWeapon(string weaponId)
        {
            return GetWeapon(weaponId) != null;
        }

        /// <summary>
        /// 무기를 제거합니다.
        /// </summary>
        public bool RemoveWeapon(string weaponId)
        {
            var weapon = GetWeapon(weaponId);

            if (weapon == null)
            {
                return false;
            }

            _weapons.Remove(weapon);
            Destroy(weapon.gameObject);

            Debug.Log($"[WeaponController] Removed weapon: {weaponId}");
            return true;
        }

        /// <summary>
        /// 모든 무기를 제거합니다.
        /// </summary>
        public void ClearAllWeapons()
        {
            foreach (var weapon in _weapons)
            {
                if (weapon != null)
                {
                    Destroy(weapon.gameObject);
                }
            }

            _weapons.Clear();
            Debug.Log("[WeaponController] Cleared all weapons");
        }

        /// <summary>
        /// 시작 무기를 설정합니다 (기존 무기 초기화 후 추가).
        /// </summary>
        public void SetStartingWeapon(WeaponData data)
        {
            ClearAllWeapons();

            if (data != null)
            {
                // owner가 설정되지 않았으면 부모를 owner로 사용
                if (_owner == null)
                {
                    _owner = transform.parent != null ? transform.parent : transform;
                }

                AddWeapon(data);
            }
        }

        #endregion
    }
}