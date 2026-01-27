# Task 02: 무기 시스템

## 메타 정보
- **Assembly**: `Vs.Combat`
- **폴더**: `Assets/Scripts/Vs.Combat/Weapons/`
- **의존**: `Vs.Core`, `Vs.Data`
- **blockedBy**: 없음

## 목표
자동으로 발사되는 무기와 투사체 시스템 구현

---

## 생성 파일

### 1. WeaponBase.cs
```csharp
namespace Vs.Combat
{
    public abstract class WeaponBase : MonoBehaviour
    {
        // 필드
        [SerializeField] protected WeaponData _data;
        protected int _level = 1;
        protected float _lastFireTime;
        protected Transform _owner;

        // 프로퍼티
        public WeaponData Data => _data;
        public int Level => _level;
        public bool IsMaxLevel => _level >= Constants.MaxWeaponLevel;
        public bool CanEvolve => _data.CanEvolve && IsMaxLevel;

        // 런타임 스탯 (JSON 밸런스 적용)
        protected float Damage { get; private set; }
        protected float Interval { get; private set; }
        protected int ProjectileCount { get; private set; }

        // 추상 메서드
        protected abstract void Fire();

        // 공통 메서드
        public virtual void Initialize(Transform owner, WeaponData data) { }
        public virtual void LevelUp() { }
        protected virtual void Update() { /* 쿨다운 체크 후 Fire */ }
        protected void LoadBalanceData() { /* BalanceLoader 사용 */ }
    }
}
```

**요구사항**:
- 자동 발사 (Update에서 쿨다운 체크)
- `WeaponData` SO + JSON 밸런스 통합
- 레벨업 시 스탯 재계산

### 2. ProjectileWeapon.cs
```csharp
namespace Vs.Combat
{
    public class ProjectileWeapon : WeaponBase
    {
        // 추가 필드
        [SerializeField] private GameObject _projectilePrefab;

        protected override void Fire()
        {
            // 1. 발사 방향 계산 (가장 가까운 적 또는 이동 방향)
            // 2. ProjectileCount만큼 투사체 생성 (PoolManager 사용)
            // 3. 각 투사체 초기화
        }

        private Vector2 GetFireDirection() { }
        private Transform FindNearestEnemy() { }
    }
}
```

**요구사항**:
- 가장 가까운 적 방향으로 발사 (없으면 이동 방향)
- 다중 투사체 지원 (부채꼴 또는 직선)
- `PoolManager.Instance.Spawn()` 사용

### 3. Projectile.cs
```csharp
namespace Vs.Combat
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        // 필드
        private float _damage;
        private float _speed;
        private float _duration;
        private Vector2 _direction;
        private float _spawnTime;
        private bool _isPiercing;
        private int _pierceCount;

        // IPoolable 구현
        public void OnSpawn() { }
        public void OnDespawn() { }

        // 메서드
        public void Initialize(float damage, float speed, Vector2 direction, float duration = 5f) { }
        private void Update() { /* 이동 + 수명 체크 */ }
        private void OnTriggerEnter2D(Collider2D other) { /* 적 충돌 처리 */ }
    }
}
```

**요구사항**:
- 직선 이동
- 적(`IDamageable`)과 충돌 시 대미지
- 수명 후 자동 Despawn
- 관통 옵션 (piercing)

### 4. WeaponController.cs
```csharp
namespace Vs.Combat
{
    public class WeaponController : MonoBehaviour
    {
        // 필드
        private List<WeaponBase> _weapons = new();
        private Transform _owner;

        // 프로퍼티
        public int WeaponCount => _weapons.Count;
        public bool HasEmptySlot => _weapons.Count < Constants.MaxWeaponSlots;
        public IReadOnlyList<WeaponBase> Weapons => _weapons;

        // 메서드
        public void Initialize(Transform owner) { }
        public bool AddWeapon(WeaponData data) { }
        public bool LevelUpWeapon(WeaponData data) { }
        public WeaponBase GetWeapon(string weaponId) { }
        public bool HasWeapon(string weaponId) { }
    }
}
```

**요구사항**:
- 최대 6개 무기 슬롯 관리
- 동적 무기 추가/레벨업
- 무기 프리팹 인스턴스화

---

## 참조 파일

| 파일 | 용도 |
|------|------|
| `Vs.Data/SO/WeaponData.cs` | 무기 정적 데이터 |
| `Vs.Data/Json/BalanceLoader.cs` | 레벨별 스탯 |
| `Vs.Core/PoolManager.cs` | 투사체 풀링 |
| `Vs.Combat/Damage/DamageInfo.cs` | 대미지 정보 |

---

## 테스트 방법

1. 빈 씬에 Player 오브젝트 (이동 가능)
2. `WeaponController` 추가
3. `Weapon_Pistol` SO 할당
4. Play 모드에서 자동 발사 확인
5. 적 오브젝트 배치 후 대미지 확인

---

## 완료 기준

- [ ] 자동 발사 (쿨다운 기반)
- [ ] 투사체 직선 이동
- [ ] 적에게 대미지 적용
- [ ] 풀링 적용 (Spawn/Despawn)
- [ ] 다중 투사체 지원
- [ ] 레벨업 시 스탯 증가
