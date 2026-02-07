using UnityEngine;
using UnityEngine.UI;
using Vs.Enemy;

namespace Vs.Debug
{
    /// <summary>
    /// 스폰 디버그 탭.
    /// 적 스폰 제어 및 모니터링 기능 제공.
    /// </summary>
    public class SpawnDebugTab : DebugTabBase
    {
        [Header("Spawn Settings")]
        [SerializeField] private float _minSpawnRate = 0.5f;
        [SerializeField] private float _maxSpawnRate = 3f;
        [SerializeField] private float _defaultSpawnRate = 1f;

        [Header("UI References")]
        [SerializeField] private GameObject _sliderPrefab;
        [SerializeField] private GameObject _labelPrefab;

        private EnemySpawner _enemySpawner;
        private float _currentSpawnRate = 1f;
        private bool _isSpawningEnabled = true;
        private TMPro.TMP_Text _statusLabel;
        private Slider _rateSlider;
        private TMPro.TMP_Text _rateLabel;

        public override void OnTabActivated()
        {
            base.OnTabActivated();
            // 주기적 업데이트 시작
            InvokeRepeating(nameof(UpdateStatusDisplay), 0f, 0.5f);
        }

        public override void OnTabDeactivated()
        {
            CancelInvoke(nameof(UpdateStatusDisplay));
            base.OnTabDeactivated();
        }

        public override void RefreshContent()
        {
            ClearContent();

            _enemySpawner = FindFirstObjectByType<EnemySpawner>();

            if (_enemySpawner == null)
            {
                UnityEngine.Debug.LogWarning("[SpawnDebugTab] EnemySpawner not found in scene");
                CreateButton("EnemySpawner not found", null);
                return;
            }

            // 상태 표시 라벨 생성
            CreateStatusLabel();

            // 스폰 토글 버튼
            string toggleLabel = _isSpawningEnabled ? "Stop Spawning" : "Start Spawning";
            CreateButton(toggleLabel, OnToggleSpawning);

            // 스폰 속도 조절 슬라이더
            CreateSpawnRateSlider();

            // 속도 프리셋 버튼
            CreateButton("Rate: 0.5x (Slow)", () => SetSpawnRate(0.5f));
            CreateButton("Rate: 1.0x (Normal)", () => SetSpawnRate(1.0f));
            CreateButton("Rate: 2.0x (Fast)", () => SetSpawnRate(2.0f));
            CreateButton("Rate: 3.0x (Very Fast)", () => SetSpawnRate(3.0f));
        }

        private void CreateStatusLabel()
        {
            if (_labelPrefab != null && _contentRoot != null)
            {
                var labelObj = Instantiate(_labelPrefab, _contentRoot);
                _statusLabel = labelObj.GetComponent<TMPro.TMP_Text>();
                if (_statusLabel == null)
                {
                    _statusLabel = labelObj.GetComponentInChildren<TMPro.TMP_Text>();
                }

                UpdateStatusDisplay();
            }
            else
            {
                // labelPrefab이 없으면 버튼을 라벨 대용으로 사용
                var labelButton = CreateButton("Status: Loading...", null);
                if (labelButton != null)
                {
                    _statusLabel = labelButton.GetComponentInChildren<TMPro.TMP_Text>();
                }
            }
        }

        private void CreateSpawnRateSlider()
        {
            if (_sliderPrefab == null || _contentRoot == null)
            {
                // 슬라이더 프리팹이 없으면 스킵
                UnityEngine.Debug.Log("[SpawnDebugTab] Slider prefab not set, using preset buttons only");
                return;
            }

            var sliderObj = Instantiate(_sliderPrefab, _contentRoot);
            _rateSlider = sliderObj.GetComponent<Slider>();

            if (_rateSlider != null)
            {
                _rateSlider.minValue = _minSpawnRate;
                _rateSlider.maxValue = _maxSpawnRate;
                _rateSlider.value = _currentSpawnRate;
                _rateSlider.onValueChanged.AddListener(OnSliderValueChanged);
            }

            // 슬라이더 라벨
            _rateLabel = sliderObj.GetComponentInChildren<TMPro.TMP_Text>();
            if (_rateLabel != null)
            {
                _rateLabel.text = $"Spawn Rate: {_currentSpawnRate:F1}x";
            }
        }

        private void OnToggleSpawning()
        {
            if (_enemySpawner == null) return;

            _isSpawningEnabled = !_isSpawningEnabled;

            if (_isSpawningEnabled)
            {
                _enemySpawner.StartSpawning();
                UnityEngine.Debug.Log("[SpawnDebugTab] Spawning started");
            }
            else
            {
                _enemySpawner.StopSpawning();
                UnityEngine.Debug.Log("[SpawnDebugTab] Spawning stopped");
            }

            RefreshContent();
        }

        private void OnSliderValueChanged(float value)
        {
            SetSpawnRate(value);
        }

        private void SetSpawnRate(float rate)
        {
            _currentSpawnRate = Mathf.Clamp(rate, _minSpawnRate, _maxSpawnRate);

            // 참고: EnemySpawner에 스폰 간격 조절 API가 없으므로
            // 실제 적용은 EnemySpawner 확장 후 가능
            // 현재는 UI만 업데이트
            UnityEngine.Debug.Log($"[SpawnDebugTab] Spawn rate set to: {_currentSpawnRate:F1}x (requires EnemySpawner extension)");

            if (_rateSlider != null)
            {
                _rateSlider.value = _currentSpawnRate;
            }

            if (_rateLabel != null)
            {
                _rateLabel.text = $"Spawn Rate: {_currentSpawnRate:F1}x";
            }
        }

        private void UpdateStatusDisplay()
        {
            if (_statusLabel == null || _enemySpawner == null) return;

            float elapsed = _enemySpawner.ElapsedTime;
            int activeCount = _enemySpawner.ActiveEnemyCount;

            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);

            _statusLabel.text = $"Time: {minutes:00}:{seconds:00} | Enemies: {activeCount} | Rate: {_currentSpawnRate:F1}x";
        }
    }
}
