using UnityEngine;
using Vs.Combat;
using Vs.Core;
using Vs.Data;
using Vs.Progression;

namespace Vs.Player
{
    /// <summary>
    /// 플레이어 초기화 담당.
    /// CharacterData를 기반으로 스탯, 체력, 시작 무기 설정.
    /// </summary>
    public class PlayerInitializer : MonoBehaviour
    {
        [Header("참조")] [SerializeField] private CharacterData _characterData;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private WeaponController _weaponController;
        [SerializeField] private PickupMagnet _pickupMagnet;

        [Header("Debug")] [SerializeField] private bool _debugMode;

        private bool _isInitialized;

        private void Awake()
        {
            // 자동으로 컴포넌트 참조 획득
            if (_playerController == null)
                _playerController = GetComponent<PlayerController>();
            if (_playerStats == null)
                _playerStats = GetComponent<PlayerStats>();
            if (_playerHealth == null)
                _playerHealth = GetComponent<PlayerHealth>();
            if (_weaponController == null)
                _weaponController = GetComponent<WeaponController>();
            if (_pickupMagnet == null)
                _pickupMagnet = GetComponent<PickupMagnet>();
        }

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

        private void Start()
        {
            // 게임이 이미 Playing 상태면 초기화
            if (GameManager.HasInstance && GameManager.Instance.IsPlaying)
            {
                Initialize();
            }
            // Menu 상태에서 시작하면 GameStarted 이벤트 대기
            else if (!_isInitialized && _characterData != null)
            {
                // 테스트용: 자동 시작
                if (_debugMode)
                {
                    Initialize();
                    GameManager.Instance?.StartGame();
                }
            }
        }

        private void HandleGameStarted()
        {
            Initialize();
        }

        /// <summary>
        /// CharacterData로 초기화.
        /// </summary>
        public void Initialize()
        {
            if (_characterData == null)
            {
                Debug.LogError("[PlayerInitializer] CharacterData is not assigned!");
                return;
            }

            if (_isInitialized)
            {
                if (_debugMode)
                {
                    Debug.Log("[PlayerInitializer] Already initialized, skipping...");
                }

                return;
            }

            InitializeStats();
            InitializeHealth();
            InitializeWeapon();
            RegisterWithManagers();

            _isInitialized = true;

            if (_debugMode)
            {
                Debug.Log($"[PlayerInitializer] Initialized with {_characterData.DisplayName}");
            }
        }

        /// <summary>
        /// CharacterData 설정 (캐릭터 선택 화면에서 호출).
        /// </summary>
        public void SetCharacterData(CharacterData data)
        {
            _characterData = data;
            _isInitialized = false;
        }

        private void InitializeStats()
        {
            if (_playerStats != null)
            {
                _playerStats.Initialize(_characterData);

                if (_debugMode)
                {
                    Debug.Log(
                        $"[PlayerInitializer] Stats initialized - HP: {_playerStats.MaxHealth}, Speed: {_playerStats.MoveSpeed}");
                }
            }
        }

        private void InitializeHealth()
        {
            if (_playerHealth != null)
            {
                _playerHealth.Initialize(_characterData.GetMaxHealth());

                if (_debugMode)
                {
                    Debug.Log(
                        $"[PlayerInitializer] Health initialized - {_playerHealth.CurrentHealth}/{_playerHealth.MaxHealth}");
                }
            }
        }

        private void InitializeWeapon()
        {
            if (_weaponController != null && _characterData.StartingWeapon != null)
            {
                _weaponController.SetStartingWeapon(_characterData.StartingWeapon);

                // LevelUpManager에도 시작 무기 등록
                if (LevelUpManager.HasInstance)
                {
                    LevelUpManager.Instance.SetStartingWeapon(_characterData.StartingWeapon);
                }

                if (_debugMode)
                {
                    Debug.Log($"[PlayerInitializer] Starting weapon: {_characterData.StartingWeapon.DisplayName}");
                }
            }
        }

        private void RegisterWithManagers()
        {
            Transform playerTransform = transform;

            // EnemySpawner에 플레이어 등록
            var enemySpawner = FindObjectOfType<Vs.Enemy.EnemySpawner>();
            if (enemySpawner != null)
            {
                enemySpawner.SetPlayer(playerTransform);
            }

            // ExpPickupSpawner에 플레이어 등록
            if (ExpPickupSpawner.HasInstance)
            {
                ExpPickupSpawner.Instance.SetPlayer(playerTransform);
            }
        }

        /// <summary>
        /// 플레이어 리셋 (게임 재시작 시).
        /// </summary>
        public void Reset()
        {
            _isInitialized = false;

            // 위치 초기화
            transform.position = Vector3.zero;

            // 무기 초기화
            _weaponController?.ClearAllWeapons();

            // 스탯 보너스 초기화
            _playerStats?.ResetBonuses();

            // 다시 초기화
            Initialize();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // 에디터에서 자동으로 컴포넌트 참조
            if (_playerController == null)
                _playerController = GetComponent<PlayerController>();
            if (_playerStats == null)
                _playerStats = GetComponent<PlayerStats>();
            if (_playerHealth == null)
                _playerHealth = GetComponent<PlayerHealth>();
            if (_weaponController == null)
                _weaponController = GetComponent<WeaponController>();
            if (_pickupMagnet == null)
                _pickupMagnet = GetComponent<PickupMagnet>();
        }

        [ContextMenu("Force Initialize")]
        private void DebugForceInitialize()
        {
            _isInitialized = false;
            Initialize();
        }
#endif
    }
}