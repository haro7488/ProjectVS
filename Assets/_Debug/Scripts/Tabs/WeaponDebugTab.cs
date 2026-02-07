using System.Collections.Generic;
using UnityEngine;
using Vs.Combat;
using Vs.Data;

namespace Vs.Debug
{
    /// <summary>
    /// 무기 디버그 탭.
    /// 무기 추가, 레벨업 등 무기 관련 디버그 기능 제공.
    /// </summary>
    public class WeaponDebugTab : DebugTabBase
    {
        [Header("Weapon Data")]
        [SerializeField] private List<WeaponData> _weaponDataList = new();

        private WeaponController _weaponController;

        public override void RefreshContent()
        {
            ClearContent();

            _weaponController = FindFirstObjectByType<WeaponController>();

            if (_weaponController == null)
            {
                UnityEngine.Debug.LogWarning("[WeaponDebugTab] WeaponController not found in scene");
                CreateButton("WeaponController not found", null);
                return;
            }

            // 전체 무기 추가 버튼
            CreateButton("Add All Weapons", OnAddAllWeapons);

            // 전체 무기 레벨업 버튼
            CreateButton("Level Up All Weapons", OnLevelUpAllWeapons);

            // 구분선 역할의 빈 버튼 (비활성)
            CreateButton("--- Individual Weapons ---", null);

            // 개별 무기 버튼
            foreach (var weaponData in _weaponDataList)
            {
                if (weaponData == null) continue;

                string label = GetWeaponButtonLabel(weaponData);
                CreateButton(label, () => OnWeaponButtonClicked(weaponData));
            }
        }

        private string GetWeaponButtonLabel(WeaponData data)
        {
            if (_weaponController == null) return data.DisplayName;

            var weapon = _weaponController.GetWeapon(data.Id);
            if (weapon != null)
            {
                return $"{data.DisplayName} (Lv.{weapon.Level}) - Level Up";
            }

            return $"{data.DisplayName} - Add";
        }

        private void OnWeaponButtonClicked(WeaponData data)
        {
            if (_weaponController == null)
            {
                UnityEngine.Debug.LogWarning("[WeaponDebugTab] WeaponController not found");
                return;
            }

            if (_weaponController.HasWeapon(data.Id))
            {
                bool success = _weaponController.LevelUpWeapon(data);
                UnityEngine.Debug.Log($"[WeaponDebugTab] LevelUp {data.DisplayName}: {(success ? "Success" : "Failed")}");
            }
            else
            {
                bool success = _weaponController.AddWeapon(data);
                UnityEngine.Debug.Log($"[WeaponDebugTab] Add {data.DisplayName}: {(success ? "Success" : "Failed")}");
            }

            RefreshContent();
        }

        private void OnAddAllWeapons()
        {
            if (_weaponController == null)
            {
                UnityEngine.Debug.LogWarning("[WeaponDebugTab] WeaponController not found");
                return;
            }

            int addedCount = 0;
            foreach (var weaponData in _weaponDataList)
            {
                if (weaponData == null) continue;

                if (!_weaponController.HasWeapon(weaponData.Id) && _weaponController.HasEmptySlot)
                {
                    if (_weaponController.AddWeapon(weaponData))
                    {
                        addedCount++;
                    }
                }
            }

            UnityEngine.Debug.Log($"[WeaponDebugTab] Added {addedCount} weapons");
            RefreshContent();
        }

        private void OnLevelUpAllWeapons()
        {
            if (_weaponController == null)
            {
                UnityEngine.Debug.LogWarning("[WeaponDebugTab] WeaponController not found");
                return;
            }

            int leveledCount = 0;
            foreach (var weapon in _weaponController.Weapons)
            {
                if (weapon != null && !weapon.IsMaxLevel)
                {
                    if (_weaponController.LevelUpWeapon(weapon.Data))
                    {
                        leveledCount++;
                    }
                }
            }

            UnityEngine.Debug.Log($"[WeaponDebugTab] Leveled up {leveledCount} weapons");
            RefreshContent();
        }
    }
}
