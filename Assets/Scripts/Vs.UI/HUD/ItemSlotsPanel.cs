using System.Collections.Generic;
using UnityEngine;
using Vs.Data;
using Vs.Progression;

namespace Vs.UI
{
    /// <summary>
    /// 아이템 슬롯 패널.
    /// 무기 슬롯 6개와 패시브 슬롯 6개를 관리.
    /// LevelUpManager의 이벤트를 구독하여 슬롯을 업데이트.
    /// </summary>
    public class ItemSlotsPanel : MonoBehaviour
    {
        [Header("무기 슬롯")]
        [SerializeField] private ItemSlotDisplay[] _weaponSlots = new ItemSlotDisplay[6];

        [Header("패시브 슬롯")]
        [SerializeField] private ItemSlotDisplay[] _passiveSlots = new ItemSlotDisplay[6];

        // 아이템 ID -> 슬롯 인덱스 매핑
        private readonly Dictionary<string, int> _weaponSlotMap = new();
        private readonly Dictionary<string, int> _passiveSlotMap = new();

        private void Start()
        {
            SubscribeEvents();
            ClearAllSlots();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (LevelUpManager.HasInstance)
            {
                var manager = LevelUpManager.Instance;
                manager.OnWeaponAdded += HandleWeaponAdded;
                manager.OnWeaponUpgraded += HandleWeaponUpgraded;
                manager.OnWeaponEvolved += HandleWeaponEvolved;
                manager.OnPassiveAdded += HandlePassiveAdded;
                manager.OnPassiveUpgraded += HandlePassiveUpgraded;
            }
        }

        private void UnsubscribeEvents()
        {
            if (LevelUpManager.HasInstance)
            {
                var manager = LevelUpManager.Instance;
                manager.OnWeaponAdded -= HandleWeaponAdded;
                manager.OnWeaponUpgraded -= HandleWeaponUpgraded;
                manager.OnWeaponEvolved -= HandleWeaponEvolved;
                manager.OnPassiveAdded -= HandlePassiveAdded;
                manager.OnPassiveUpgraded -= HandlePassiveUpgraded;
            }
        }

        private void HandleWeaponAdded(WeaponData weapon, int level)
        {
            if (weapon == null) return;

            int slotIndex = GetNextAvailableWeaponSlot();
            if (slotIndex < 0) return;

            _weaponSlotMap[weapon.Id] = slotIndex;
            _weaponSlots[slotIndex].SetItem(weapon.Icon, level);
        }

        private void HandleWeaponUpgraded(WeaponData weapon, int level)
        {
            if (weapon == null) return;

            if (_weaponSlotMap.TryGetValue(weapon.Id, out int slotIndex))
            {
                _weaponSlots[slotIndex].UpdateLevel(level);
            }
        }

        private void HandleWeaponEvolved(WeaponData oldWeapon, WeaponData newWeapon)
        {
            if (oldWeapon == null || newWeapon == null) return;

            // 기존 무기의 슬롯 찾기
            if (_weaponSlotMap.TryGetValue(oldWeapon.Id, out int slotIndex))
            {
                // 매핑 업데이트
                _weaponSlotMap.Remove(oldWeapon.Id);
                _weaponSlotMap[newWeapon.Id] = slotIndex;

                // 슬롯 UI 업데이트 (진화 무기는 Lv.1로 시작)
                _weaponSlots[slotIndex].SetItem(newWeapon.Icon, 1);
            }
        }

        private void HandlePassiveAdded(PassiveData passive, int level)
        {
            if (passive == null) return;

            int slotIndex = GetNextAvailablePassiveSlot();
            if (slotIndex < 0) return;

            _passiveSlotMap[passive.Id] = slotIndex;
            _passiveSlots[slotIndex].SetItem(passive.Icon, level);
        }

        private void HandlePassiveUpgraded(PassiveData passive, int level)
        {
            if (passive == null) return;

            if (_passiveSlotMap.TryGetValue(passive.Id, out int slotIndex))
            {
                _passiveSlots[slotIndex].UpdateLevel(level);
            }
        }

        private int GetNextAvailableWeaponSlot()
        {
            for (int i = 0; i < _weaponSlots.Length; i++)
            {
                if (_weaponSlots[i] != null && !_weaponSlots[i].HasItem)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetNextAvailablePassiveSlot()
        {
            for (int i = 0; i < _passiveSlots.Length; i++)
            {
                if (_passiveSlots[i] != null && !_passiveSlots[i].HasItem)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 모든 슬롯 비우기.
        /// </summary>
        public void ClearAllSlots()
        {
            _weaponSlotMap.Clear();
            _passiveSlotMap.Clear();

            foreach (var slot in _weaponSlots)
            {
                if (slot != null)
                {
                    slot.Clear();
                }
            }

            foreach (var slot in _passiveSlots)
            {
                if (slot != null)
                {
                    slot.Clear();
                }
            }
        }

        /// <summary>
        /// 현재 LevelUpManager 상태로 슬롯 동기화.
        /// </summary>
        public void SyncWithManager()
        {
            ClearAllSlots();

            if (!LevelUpManager.HasInstance) return;

            var manager = LevelUpManager.Instance;

            // 무기 동기화
            foreach (var weapon in manager.OwnedWeapons)
            {
                int level = manager.GetWeaponLevel(weapon);
                HandleWeaponAdded(weapon, level);
            }

            // 패시브 동기화
            foreach (var passive in manager.OwnedPassives)
            {
                int level = manager.GetPassiveLevel(passive);
                HandlePassiveAdded(passive, level);
            }
        }
    }
}
