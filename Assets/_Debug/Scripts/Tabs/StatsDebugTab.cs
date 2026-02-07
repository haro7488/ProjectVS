using UnityEngine;
using Vs.Player;

namespace Vs.Debug
{
    /// <summary>
    /// 스탯 디버그 탭.
    /// 플레이어 스탯 조회, 갓모드, 체력 회복 등 디버그 기능 제공.
    /// </summary>
    public class StatsDebugTab : DebugTabBase
    {
        private PlayerHealth _playerHealth;
        private PlayerStats _playerStats;
        private bool _isGodMode;

        public override void RefreshContent()
        {
            ClearContent();

            _playerHealth = FindFirstObjectByType<PlayerHealth>();
            _playerStats = FindFirstObjectByType<PlayerStats>();

            // 현재 스탯 표시
            CreateStatsDisplay();

            // 구분선
            CreateButton("--- Actions ---", null);

            // 갓모드 토글 버튼
            UpdateGodModeState();
            string godModeLabel = _isGodMode ? "God Mode: ON (Click to OFF)" : "God Mode: OFF (Click to ON)";
            CreateButton(godModeLabel, OnToggleGodMode);

            // 체력 회복 버튼
            CreateButton("Full Heal", OnFullHeal);
            CreateButton("Heal 50", OnHeal50);

            // 구분선
            CreateButton("--- Stat Modifiers ---", null);

            // 스탯 추가 버튼
            CreateButton("Add +10% Move Speed", OnAddMoveSpeed);
            CreateButton("Add +10% Damage", OnAddDamage);
            CreateButton("Add +10% Max Health", OnAddMaxHealth);
            CreateButton("Add +10% Area", OnAddArea);

            // 리셋 버튼
            CreateButton("Reset All Bonuses", OnResetBonuses);
        }

        private void CreateStatsDisplay()
        {
            // 체력 정보
            if (_playerHealth != null)
            {
                float healthPercent = _playerHealth.GetHealthPercent() * 100f;
                CreateButton($"HP: {_playerHealth.CurrentHealth:F0} / {_playerHealth.MaxHealth:F0} ({healthPercent:F0}%)", null);
            }
            else
            {
                CreateButton("HP: PlayerHealth not found", null);
            }

            // 스탯 정보
            if (_playerStats != null)
            {
                CreateButton($"Move Speed: {_playerStats.MoveSpeed:F1}", null);
                CreateButton($"Damage Mult: {_playerStats.DamageMultiplier:F2}x", null);
                CreateButton($"Area Mult: {_playerStats.AreaMultiplier:F2}x", null);
                CreateButton($"Cooldown Mult: {_playerStats.CooldownMultiplier:F2}x", null);
            }
            else
            {
                CreateButton("Stats: PlayerStats not found", null);
            }
        }

        private void UpdateGodModeState()
        {
            // DebugController에서 갓모드 상태 가져오기
            if (DebugController.HasInstance)
            {
                _isGodMode = DebugController.Instance.GodMode;
            }
            else if (_playerHealth != null)
            {
                _isGodMode = _playerHealth.IsInvincible;
            }
        }

        private void OnToggleGodMode()
        {
            if (DebugController.HasInstance)
            {
                DebugController.Instance.ToggleGodMode();
                _isGodMode = DebugController.Instance.GodMode;
            }
            else if (_playerHealth != null)
            {
                _isGodMode = !_isGodMode;
                _playerHealth.SetInvincible(_isGodMode);
                UnityEngine.Debug.Log($"[StatsDebugTab] God Mode: {(_isGodMode ? "ON" : "OFF")}");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] Cannot toggle god mode - no controller or health found");
            }

            RefreshContent();
        }

        private void OnFullHeal()
        {
            if (_playerHealth != null)
            {
                float healAmount = _playerHealth.MaxHealth - _playerHealth.CurrentHealth;
                if (healAmount > 0f)
                {
                    _playerHealth.Heal(healAmount);
                    UnityEngine.Debug.Log($"[StatsDebugTab] Full heal: +{healAmount:F0} HP");
                }
                else
                {
                    UnityEngine.Debug.Log("[StatsDebugTab] Already at full health");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] PlayerHealth not found");
            }

            RefreshContent();
        }

        private void OnHeal50()
        {
            if (_playerHealth != null)
            {
                _playerHealth.Heal(50f);
                UnityEngine.Debug.Log("[StatsDebugTab] Healed 50 HP");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] PlayerHealth not found");
            }

            RefreshContent();
        }

        private void OnAddMoveSpeed()
        {
            if (_playerStats != null)
            {
                _playerStats.AddBonus(StatType.MoveSpeed, 0.1f);
                UnityEngine.Debug.Log($"[StatsDebugTab] Added +10% Move Speed. New: {_playerStats.MoveSpeed:F1}");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] PlayerStats not found");
            }

            RefreshContent();
        }

        private void OnAddDamage()
        {
            if (_playerStats != null)
            {
                _playerStats.AddBonus(StatType.Damage, 0.1f);
                UnityEngine.Debug.Log($"[StatsDebugTab] Added +10% Damage. New: {_playerStats.DamageMultiplier:F2}x");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] PlayerStats not found");
            }

            RefreshContent();
        }

        private void OnAddMaxHealth()
        {
            if (_playerStats != null)
            {
                _playerStats.AddBonus(StatType.MaxHealth, 0.1f);
                UnityEngine.Debug.Log($"[StatsDebugTab] Added +10% Max Health. New: {_playerStats.MaxHealth:F0}");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] PlayerStats not found");
            }

            RefreshContent();
        }

        private void OnAddArea()
        {
            if (_playerStats != null)
            {
                _playerStats.AddBonus(StatType.Area, 0.1f);
                UnityEngine.Debug.Log($"[StatsDebugTab] Added +10% Area. New: {_playerStats.AreaMultiplier:F2}x");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] PlayerStats not found");
            }

            RefreshContent();
        }

        private void OnResetBonuses()
        {
            if (_playerStats != null)
            {
                _playerStats.ResetBonuses();
                UnityEngine.Debug.Log("[StatsDebugTab] All bonuses reset");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[StatsDebugTab] PlayerStats not found");
            }

            RefreshContent();
        }
    }
}
