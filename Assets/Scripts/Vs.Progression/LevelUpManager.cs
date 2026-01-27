using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vs.Core;
using Vs.Data;
using Vs.Utility;

namespace Vs.Progression
{
    /// <summary>
    /// 레벨업 선택지 생성 및 적용 관리.
    /// </summary>
    public class LevelUpManager : Singleton<LevelUpManager>
    {
        [Header("데이터")] [SerializeField] private WeaponData[] _allWeapons;
        [SerializeField] private PassiveData[] _allPassives;

        [Header("Debug")] [SerializeField] private bool _debugMode;

        // 현재 보유 목록
        private readonly List<WeaponData> _ownedWeapons = new();
        private readonly Dictionary<string, int> _weaponLevels = new();
        private readonly List<PassiveData> _ownedPassives = new();
        private readonly Dictionary<string, int> _passiveLevels = new();

        // 이벤트
        public event Action<LevelUpChoice[]> OnChoicesGenerated;
        public event Action<LevelUpChoice> OnChoiceApplied;
        public event Action<WeaponData, int> OnWeaponAdded;
        public event Action<WeaponData, int> OnWeaponUpgraded;
        public event Action<PassiveData, int> OnPassiveAdded;
        public event Action<PassiveData, int> OnPassiveUpgraded;

        // 프로퍼티
        public IReadOnlyList<WeaponData> OwnedWeapons => _ownedWeapons;
        public IReadOnlyList<PassiveData> OwnedPassives => _ownedPassives;
        public int WeaponCount => _ownedWeapons.Count;
        public int PassiveCount => _ownedPassives.Count;

        private void OnEnable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnGameStarted += HandleGameStarted;
            }
        }

        private void OnDisable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnGameStarted -= HandleGameStarted;
            }
        }

        private void HandleGameStarted()
        {
            Reset();
        }

        /// <summary>
        /// 레벨업 선택지 생성.
        /// </summary>
        public LevelUpChoice[] GenerateChoices(int count = Constants.LevelUpChoices)
        {
            var choices = new List<LevelUpChoice>();
            var candidates = new List<LevelUpChoice>();

            // 새 무기 후보
            if (CanAddWeapon())
            {
                foreach (var weapon in _allWeapons)
                {
                    if (weapon == null || weapon.IsEvolved) continue;
                    if (_ownedWeapons.Contains(weapon)) continue;

                    candidates.Add(CreateWeaponChoice(weapon, true));
                }
            }

            // 무기 강화 후보
            foreach (var weapon in _ownedWeapons)
            {
                if (GetWeaponLevel(weapon) >= Constants.MaxWeaponLevel) continue;

                candidates.Add(CreateWeaponChoice(weapon, false));
            }

            // 새 패시브 후보
            if (CanAddPassive())
            {
                foreach (var passive in _allPassives)
                {
                    if (passive == null) continue;
                    if (_ownedPassives.Contains(passive)) continue;

                    candidates.Add(CreatePassiveChoice(passive, true));
                }
            }

            // 패시브 강화 후보
            foreach (var passive in _ownedPassives)
            {
                if (GetPassiveLevel(passive) >= Constants.MaxPassiveLevel) continue;

                candidates.Add(CreatePassiveChoice(passive, false));
            }

            // 랜덤하게 선택
            Shuffle(candidates);
            for (int i = 0; i < count && i < candidates.Count; i++)
            {
                choices.Add(candidates[i]);
            }

            var result = choices.ToArray();

            if (_debugMode)
            {
                Debug.Log($"[LevelUpManager] Generated {result.Length} choices:");
                foreach (var choice in result)
                {
                    Debug.Log($"  - {choice.Type}: {choice.DisplayName} (Lv.{choice.NewLevel})");
                }
            }

            OnChoicesGenerated?.Invoke(result);
            return result;
        }

        /// <summary>
        /// 선택지 적용.
        /// </summary>
        public void ApplyChoice(LevelUpChoice choice)
        {
            if (choice == null)
            {
                Debug.LogWarning("[LevelUpManager] Cannot apply null choice");
                return;
            }

            switch (choice.Type)
            {
                case LevelUpChoice.ChoiceType.NewWeapon:
                    AddWeapon(choice.Data as WeaponData);
                    break;

                case LevelUpChoice.ChoiceType.WeaponUpgrade:
                    UpgradeWeapon(choice.Data as WeaponData);
                    break;

                case LevelUpChoice.ChoiceType.NewPassive:
                    AddPassive(choice.Data as PassiveData);
                    break;

                case LevelUpChoice.ChoiceType.PassiveUpgrade:
                    UpgradePassive(choice.Data as PassiveData);
                    break;
            }

            if (_debugMode)
            {
                Debug.Log($"[LevelUpManager] Applied choice: {choice.Type} - {choice.DisplayName}");
            }

            OnChoiceApplied?.Invoke(choice);

            // 게임 재개
            if (GameManager.HasInstance && GameManager.Instance.State == GameState.LevelUp)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        private LevelUpChoice CreateWeaponChoice(WeaponData weapon, bool isNew)
        {
            int currentLevel = isNew ? 0 : GetWeaponLevel(weapon);
            int newLevel = currentLevel + 1;
            var type = isNew ? LevelUpChoice.ChoiceType.NewWeapon : LevelUpChoice.ChoiceType.WeaponUpgrade;

            string description = isNew
                ? weapon.Description
                : $"Lv.{currentLevel} -> Lv.{newLevel}";

            return new LevelUpChoice(
                type,
                weapon,
                newLevel,
                weapon.DisplayName,
                description,
                weapon.Icon
            );
        }

        private LevelUpChoice CreatePassiveChoice(PassiveData passive, bool isNew)
        {
            int currentLevel = isNew ? 0 : GetPassiveLevel(passive);
            int newLevel = currentLevel + 1;
            var type = isNew ? LevelUpChoice.ChoiceType.NewPassive : LevelUpChoice.ChoiceType.PassiveUpgrade;

            string description = isNew
                ? $"{passive.Description}\n{passive.GetFormattedValue(1)} {passive.PrimaryStat}"
                : $"Lv.{currentLevel} -> Lv.{newLevel}\n{passive.GetFormattedValue(newLevel)} {passive.PrimaryStat}";

            return new LevelUpChoice(
                type,
                passive,
                newLevel,
                passive.DisplayName,
                description,
                passive.Icon
            );
        }

        private void AddWeapon(WeaponData weapon)
        {
            if (weapon == null || !CanAddWeapon()) return;
            if (_ownedWeapons.Contains(weapon)) return;

            _ownedWeapons.Add(weapon);
            _weaponLevels[weapon.Id] = 1;

            OnWeaponAdded?.Invoke(weapon, 1);
        }

        private void UpgradeWeapon(WeaponData weapon)
        {
            if (weapon == null || !_ownedWeapons.Contains(weapon)) return;

            int currentLevel = GetWeaponLevel(weapon);
            if (currentLevel >= Constants.MaxWeaponLevel) return;

            int newLevel = currentLevel + 1;
            _weaponLevels[weapon.Id] = newLevel;

            OnWeaponUpgraded?.Invoke(weapon, newLevel);
        }

        private void AddPassive(PassiveData passive)
        {
            if (passive == null || !CanAddPassive()) return;
            if (_ownedPassives.Contains(passive)) return;

            _ownedPassives.Add(passive);
            _passiveLevels[passive.Id] = 1;

            OnPassiveAdded?.Invoke(passive, 1);
        }

        private void UpgradePassive(PassiveData passive)
        {
            if (passive == null || !_ownedPassives.Contains(passive)) return;

            int currentLevel = GetPassiveLevel(passive);
            if (currentLevel >= Constants.MaxPassiveLevel) return;

            int newLevel = currentLevel + 1;
            _passiveLevels[passive.Id] = newLevel;

            OnPassiveUpgraded?.Invoke(passive, newLevel);
        }

        public int GetWeaponLevel(WeaponData weapon)
        {
            if (weapon == null) return 0;
            return _weaponLevels.TryGetValue(weapon.Id, out int level) ? level : 0;
        }

        public int GetPassiveLevel(PassiveData passive)
        {
            if (passive == null) return 0;
            return _passiveLevels.TryGetValue(passive.Id, out int level) ? level : 0;
        }

        public bool HasWeapon(WeaponData weapon)
        {
            return weapon != null && _ownedWeapons.Contains(weapon);
        }

        public bool HasPassive(PassiveData passive)
        {
            return passive != null && _ownedPassives.Contains(passive);
        }

        private bool CanAddWeapon()
        {
            return _ownedWeapons.Count < Constants.MaxWeaponSlots;
        }

        private bool CanAddPassive()
        {
            return _ownedPassives.Count < Constants.MaxPassiveSlots;
        }

        /// <summary>
        /// 모든 보유 아이템 초기화.
        /// </summary>
        public void Reset()
        {
            _ownedWeapons.Clear();
            _weaponLevels.Clear();
            _ownedPassives.Clear();
            _passiveLevels.Clear();

            if (_debugMode)
            {
                Debug.Log("[LevelUpManager] Reset");
            }
        }

        /// <summary>
        /// 시작 무기 설정.
        /// </summary>
        public void SetStartingWeapon(WeaponData weapon)
        {
            if (weapon == null) return;

            Reset();
            AddWeapon(weapon);
        }

        private static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Debug Generate Choices")]
        private void DebugGenerateChoices()
        {
            GenerateChoices();
        }
#endif
    }
}