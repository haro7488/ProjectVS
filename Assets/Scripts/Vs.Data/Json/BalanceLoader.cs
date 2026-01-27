using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Vs.Utility;

namespace Vs.Data
{
    [Serializable]
    public class WeaponLevelData
    {
        public float damage;
        public float interval;
        public int projectiles;
        public float area;
        public float duration;
        public float speed;
    }

    [Serializable]
    public class WeaponBalanceData
    {
        public WeaponLevelData[] levels;
    }

    [Serializable]
    public class WeaponBalanceRoot
    {
        public Dictionary<string, WeaponBalanceData> weapons;
    }

    public static class BalanceLoader
    {
        private static Dictionary<string, WeaponBalanceData> _weaponBalance;
        private static bool _isLoaded;

        public static void Load()
        {
            if (_isLoaded) return;

            LoadWeaponBalance();
            _isLoaded = true;
        }

        public static void Reload()
        {
            _isLoaded = false;
            _weaponBalance = null;
            Load();
        }

        private static void LoadWeaponBalance()
        {
            _weaponBalance = new Dictionary<string, WeaponBalanceData>();

            string path = Path.Combine(Application.streamingAssetsPath, Constants.BalancePath, Constants.WeaponsBalanceFile);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[BalanceLoader] Weapon balance file not found: {path}");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                var root = JsonUtility.FromJson<WeaponBalanceWrapper>(json);

                if (root?.weapons != null)
                {
                    foreach (var entry in root.weapons)
                    {
                        _weaponBalance[entry.id] = entry.data;
                    }
                }

                Debug.Log($"[BalanceLoader] Loaded {_weaponBalance.Count} weapon balance entries");
            }
            catch (Exception e)
            {
                Debug.LogError($"[BalanceLoader] Failed to load weapon balance: {e.Message}");
            }
        }

        public static WeaponLevelData GetWeaponLevel(string weaponId, int level)
        {
            if (!_isLoaded) Load();

            if (_weaponBalance.TryGetValue(weaponId, out var balance))
            {
                int index = Mathf.Clamp(level - 1, 0, balance.levels.Length - 1);
                return balance.levels[index];
            }

            Debug.LogWarning($"[BalanceLoader] Weapon balance not found: {weaponId}");
            return null;
        }

        public static bool HasWeaponBalance(string weaponId)
        {
            if (!_isLoaded) Load();
            return _weaponBalance.ContainsKey(weaponId);
        }
    }

    // JsonUtility용 Wrapper (Dictionary 직접 지원 안됨)
    [Serializable]
    public class WeaponBalanceWrapper
    {
        public WeaponBalanceEntry[] weapons;
    }

    [Serializable]
    public class WeaponBalanceEntry
    {
        public string id;
        public WeaponBalanceData data;
    }
}
