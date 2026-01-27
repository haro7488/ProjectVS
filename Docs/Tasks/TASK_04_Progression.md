# Task 04: 경험치/레벨업 시스템

## 메타 정보
- **Assembly**: `Vs.Progression`
- **폴더**: `Assets/Scripts/Vs.Progression/`
- **의존**: `Vs.Core`, `Vs.Data`
- **blockedBy**: 없음

## 목표
경험치 수집 → 레벨업 → 선택지 제공 시스템 구현

---

## 생성 파일

### 1. ExperienceManager.cs
```csharp
namespace Vs.Progression
{
    public class ExperienceManager : Singleton<ExperienceManager>
    {
        // 필드
        private int _currentExp;
        private int _currentLevel = 1;
        private int _expToNextLevel;

        [Header("레벨업 공식")]
        [SerializeField] private int _baseExpRequired = 10;
        [SerializeField] private float _expGrowthRate = 1.2f;

        // 프로퍼티
        public int CurrentExp => _currentExp;
        public int CurrentLevel => _currentLevel;
        public int ExpToNextLevel => _expToNextLevel;
        public float ExpProgress => (float)_currentExp / _expToNextLevel;

        // 이벤트
        public event Action<int> OnExpGained;
        public event Action<int> OnLevelUp;

        // 메서드
        public void AddExperience(int amount) { }
        private void CheckLevelUp() { }
        private int CalculateExpRequired(int level) { }
        public void Reset() { }
    }
}
```

**요구사항**:
- 경험치 누적 관리
- 레벨업 시 `GameManager.Instance.TriggerLevelUp()` 호출
- 레벨업 필요 경험치 공식: `base * (growthRate ^ level)`

### 2. LevelUpManager.cs
```csharp
namespace Vs.Progression
{
    public class LevelUpManager : Singleton<LevelUpManager>
    {
        // 필드
        [SerializeField] private WeaponData[] _allWeapons;
        [SerializeField] private PassiveData[] _allPassives;

        // 현재 보유 목록
        private List<WeaponData> _ownedWeapons = new();
        private Dictionary<string, int> _weaponLevels = new();
        private List<PassiveData> _ownedPassives = new();
        private Dictionary<string, int> _passiveLevels = new();

        // 메서드
        public LevelUpChoice[] GenerateChoices(int count = 3) { }
        public void ApplyChoice(LevelUpChoice choice) { }
        private LevelUpChoice CreateWeaponChoice() { }
        private LevelUpChoice CreatePassiveChoice() { }
        private bool CanAddWeapon() { }
        private bool CanAddPassive() { }
        public void Reset() { }
    }

    public class LevelUpChoice
    {
        public enum ChoiceType { NewWeapon, WeaponUpgrade, NewPassive, PassiveUpgrade }

        public ChoiceType Type { get; }
        public ScriptableObject Data { get; } // WeaponData 또는 PassiveData
        public int NewLevel { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public Sprite Icon { get; }
    }
}
```

**요구사항**:
- 3개 랜덤 선택지 생성
- 선택지 타입: 새 무기, 무기 강화, 새 패시브, 패시브 강화
- 슬롯 가득 차면 강화만 제공
- 만렙 아이템은 제외

### 3. ExpPickup.cs
```csharp
namespace Vs.Progression
{
    public class ExpPickup : MonoBehaviour, IPoolable
    {
        // 필드
        [SerializeField] private int _expValue = 1;
        private Transform _target;
        private bool _isCollecting;

        [Header("이동")]
        [SerializeField] private float _magnetSpeed = 10f;
        [SerializeField] private float _collectDistance = 0.5f;

        // IPoolable 구현
        public void OnSpawn() { }
        public void OnDespawn() { }

        // 메서드
        public void Initialize(int expValue, Transform target) { }
        public void StartCollecting() { _isCollecting = true; }
        private void Update() { /* 플레이어에게 이동 */ }
        private void OnCollected() { }
    }
}
```

**요구사항**:
- 스폰 시 제자리 대기
- 플레이어 픽업 범위 진입 시 자동 수집
- 부드러운 이동 (Lerp)
- 수집 시 `ExperienceManager.AddExperience()` 호출

### 4. PickupMagnet.cs
```csharp
namespace Vs.Progression
{
    public class PickupMagnet : MonoBehaviour
    {
        // 플레이어에 부착
        [SerializeField] private float _magnetRadius = 3f;
        [SerializeField] private LayerMask _pickupLayer;

        private void FixedUpdate()
        {
            // 범위 내 ExpPickup 찾아서 StartCollecting() 호출
            var colliders = Physics2D.OverlapCircleAll(transform.position, _magnetRadius, _pickupLayer);
            foreach (var col in colliders)
            {
                if (col.TryGetComponent<ExpPickup>(out var pickup))
                {
                    pickup.StartCollecting();
                }
            }
        }
    }
}
```

---

## 참조 파일

| 파일 | 용도 |
|------|------|
| `Vs.Core/GameManager.cs` | 레벨업 상태 전환 |
| `Vs.Core/PoolManager.cs` | 경험치 젬 풀링 |
| `Vs.Data/SO/WeaponData.cs` | 무기 선택지 |
| `Vs.Data/SO/PassiveData.cs` | 패시브 선택지 |

---

## 테스트 방법

1. 씬에 Player + ExperienceManager + LevelUpManager 배치
2. ExpPickup 프리팹 수동 배치
3. 픽업 수집 시 경험치 증가 확인
4. 레벨업 시 GameState.LevelUp 전환 확인
5. 선택지 생성 확인 (Debug.Log)

---

## 완료 기준

- [ ] 경험치 수집 및 누적
- [ ] 레벨업 트리거
- [ ] 3개 선택지 생성
- [ ] 선택지 적용
- [ ] 경험치 젬 자동 수집 (마그넷)
- [ ] 풀링 적용
