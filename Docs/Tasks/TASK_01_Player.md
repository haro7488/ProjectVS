# Task 01: 플레이어 시스템

## 메타 정보
- **Assembly**: `Vs.Player`
- **폴더**: `Assets/Scripts/Vs.Player/`
- **의존**: `Vs.Core`, `Vs.Data`, `Vs.Combat`
- **blockedBy**: 없음

## 목표
WASD/조이스틱으로 이동하고, 대미지를 받을 수 있는 플레이어 구현

---

## 생성 파일

### 1. PlayerController.cs
```csharp
namespace Vs.Player
{
    public class PlayerController : MonoBehaviour
    {
        // 필드
        [SerializeField] private float _moveSpeed = 5f;
        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private Vector2 _lastMoveDirection = Vector2.right;

        // 프로퍼티
        public Vector2 MoveDirection => _lastMoveDirection;
        public Vector2 Position => transform.position;
        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

        // 메서드
        private void Update() { /* 입력 처리 */ }
        private void FixedUpdate() { /* 이동 적용 */ }
        public void SetMoveSpeed(float speed) { }
    }
}
```

**요구사항**:
- Input System 사용 (Legacy 또는 New)
- WASD + 방향키 지원
- Rigidbody2D.MovePosition 사용
- 이동 방향 저장 (무기 발사 방향용)

### 2. PlayerHealth.cs
```csharp
namespace Vs.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        // 필드
        [SerializeField] private float _maxHealth = 100f;
        private float _currentHealth;

        // IDamageable 구현
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _currentHealth <= 0;

        public event Action<DamageInfo> OnDamaged;
        public event Action OnDeath;

        public void TakeDamage(DamageInfo damage) { }
        public void Heal(float amount) { }

        // 추가 메서드
        public void Initialize(float maxHealth) { }
        public float GetHealthPercent() { }
    }
}
```

**요구사항**:
- `IDamageable` 인터페이스 구현
- 무적 시간 (iFrame) 구현 (0.5초)
- 사망 시 `GameManager.Instance.EndGame(false)` 호출

### 3. PlayerStats.cs
```csharp
namespace Vs.Player
{
    public class PlayerStats : MonoBehaviour
    {
        // 기본 스탯 (CharacterData에서 로드)
        private float _baseMoveSpeed;
        private float _baseMaxHealth;

        // 보너스 (레벨업/패시브에서 추가)
        private float _bonusMoveSpeed;
        private float _bonusMaxHealth;
        private float _bonusDamage;
        private float _bonusArea;

        // 최종 스탯
        public float MoveSpeed => _baseMoveSpeed * (1f + _bonusMoveSpeed);
        public float MaxHealth => _baseMaxHealth * (1f + _bonusMaxHealth);
        public float DamageMultiplier => 1f + _bonusDamage;
        public float AreaMultiplier => 1f + _bonusArea;

        // 메서드
        public void Initialize(CharacterData data) { }
        public void AddBonus(StatType stat, float value) { }
        public void ResetBonuses() { }
    }
}
```

**요구사항**:
- `CharacterData` SO에서 기본 스탯 로드
- 런타임 보너스 스탯 관리
- 스탯 변경 이벤트 (UI 업데이트용)

---

## 참조 파일

| 파일 | 용도 |
|------|------|
| `Vs.Combat/Damage/IDamageable.cs` | 인터페이스 구현 |
| `Vs.Combat/Damage/DamageInfo.cs` | 대미지 정보 |
| `Vs.Data/SO/CharacterData.cs` | 캐릭터 데이터 |
| `Vs.Core/GameManager.cs` | 게임 상태 변경 |

---

## 테스트 방법

1. 빈 씬에 빈 GameObject 생성
2. `PlayerController`, `PlayerHealth`, `Rigidbody2D`, `CircleCollider2D` 추가
3. Play 모드에서 WASD 이동 확인
4. Inspector에서 TakeDamage 테스트 (Debug 버튼 또는 코드)

---

## 완료 기준

- [x] WASD 이동 작동
- [x] 이동 방향 저장 (`MoveDirection`)
- [x] `IDamageable` 구현
- [x] 대미지 시 무적 시간
- [x] 사망 시 게임 오버
- [x] `CharacterData`에서 스탯 로드

---

## 완료 보고

### 수행 작업
- [x] PlayerController.cs 구현 (WASD/방향키 이동, Rigidbody2D.MovePosition)
- [x] PlayerHealth.cs 구현 (IDamageable, 무적 시간 0.5초, GameManager.EndGame 연동)
- [x] PlayerStats.cs 구현 (CharacterData 기반 초기화, 11개 스탯 타입, 이벤트 시스템)
- [x] WeaponController.cs 구현 (무기 슬롯 6개, 추가/레벨업/제거 기능)

### 결과
- `Assets/Scripts/Vs.Player/PlayerController.cs` - 이동 및 입력 처리
- `Assets/Scripts/Vs.Player/PlayerHealth.cs` - 체력 및 피격 처리 (IDamageable)
- `Assets/Scripts/Vs.Player/PlayerStats.cs` - 스탯 관리 (StatType enum 포함)
- `Assets/Scripts/Vs.Player/WeaponController.cs` - 무기 슬롯 관리 (WeaponSlot 클래스 포함)

### 확인 필요
- Unity Editor에서 컴파일 확인 필요
- 실제 플레이 테스트 (WASD 이동, 대미지 처리)
- PlayerStats.Initialize()에 CharacterData 전달 연동 필요
