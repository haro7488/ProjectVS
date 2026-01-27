# Task 03: 적 시스템

## 메타 정보
- **Assembly**: `Vs.Enemy`
- **폴더**: `Assets/Scripts/Vs.Enemy/`
- **의존**: `Vs.Core`, `Vs.Data`, `Vs.Combat`
- **blockedBy**: 없음

## 목표
웨이브 기반 스폰 + 플레이어 추적 AI 구현

---

## 생성 파일

### 1. EnemyBase.cs
```csharp
namespace Vs.Enemy
{
    public class EnemyBase : MonoBehaviour, IDamageable, IPoolable
    {
        // 필드
        [SerializeField] protected EnemyData _data;
        protected float _currentHealth;
        protected Transform _target; // 플레이어
        protected float _lastAttackTime;

        // IDamageable 구현
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _data.MaxHealth;
        public bool IsDead => _currentHealth <= 0;

        public event Action<DamageInfo> OnDamaged;
        public event Action OnDeath;

        public void TakeDamage(DamageInfo damage) { }
        public void Heal(float amount) { }

        // IPoolable 구현
        public void OnSpawn() { }
        public void OnDespawn() { }

        // 메서드
        public virtual void Initialize(EnemyData data, Transform target) { }
        protected virtual void Update() { /* AI 호출 */ }
        protected virtual void Die() { /* 경험치/골드 드롭, Despawn */ }
        protected virtual void OnCollisionStay2D(Collision2D other) { /* 접촉 대미지 */ }
    }
}
```

**요구사항**:
- `IDamageable`, `IPoolable` 구현
- 사망 시 경험치 값 이벤트 발생
- 접촉 대미지 (쿨다운 적용)
- 풀링 지원

### 2. EnemyAI.cs
```csharp
namespace Vs.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        // 필드
        private EnemyBase _enemy;
        private Transform _target;
        private Rigidbody2D _rb;

        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _stoppingDistance = 0.5f;

        // 메서드
        public void Initialize(Transform target, float moveSpeed) { }
        private void FixedUpdate() { /* 플레이어 방향으로 이동 */ }
        private void ChaseTarget() { }
    }
}
```

**요구사항**:
- 단순 추적 AI (플레이어 방향으로 이동)
- Rigidbody2D 사용
- 이동 속도 `EnemyData`에서 로드

### 3. EnemySpawner.cs
```csharp
namespace Vs.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        // 필드
        [SerializeField] private StageData _stageData;
        [SerializeField] private Transform _player;
        [SerializeField] private float _spawnRadius = 15f;
        [SerializeField] private float _minSpawnDistance = 10f;

        private bool _isSpawning;
        private int _activeEnemyCount;

        [Header("성능")]
        [SerializeField] private int _maxActiveEnemies = 100;

        // 메서드
        public void StartSpawning() { }
        public void StopSpawning() { }
        private IEnumerator SpawnRoutine() { }
        private void SpawnWave(EnemyWave wave) { }
        private Vector2 GetSpawnPosition() { }
        private void OnEnemyDeath() { _activeEnemyCount--; }
    }
}
```

**요구사항**:
- `StageData`의 웨이브 설정 기반 스폰
- 화면 밖 랜덤 위치에 스폰
- 최대 적 수 제한
- `GameManager` 상태 연동 (Playing일 때만)

### 4. EnemyDeathEvent.cs (이벤트 데이터)
```csharp
namespace Vs.Enemy
{
    public readonly struct EnemyDeathEvent
    {
        public Vector2 Position { get; }
        public int ExpValue { get; }
        public int GoldValue { get; }
        public bool DropGold { get; }

        public EnemyDeathEvent(Vector2 position, EnemyData data)
        {
            Position = position;
            ExpValue = data.ExpValue;
            GoldValue = data.GoldValue;
            DropGold = Random.value <= data.GoldDropChance;
        }
    }
}
```

---

## 참조 파일

| 파일 | 용도 |
|------|------|
| `Vs.Combat/Damage/IDamageable.cs` | 인터페이스 구현 |
| `Vs.Core/PoolManager.cs` | 적 풀링 |
| `Vs.Core/GameManager.cs` | 게임 상태 확인 |
| `Vs.Data/SO/EnemyData.cs` | 적 데이터 |
| `Vs.Data/SO/StageData.cs` | 웨이브 설정 |

---

## 테스트 방법

1. 씬에 Player, EnemySpawner 배치
2. `Stage_City` SO 할당 (간단한 웨이브 1개)
3. Play 모드에서 적 스폰 확인
4. 적이 플레이어 추적하는지 확인
5. 적에게 대미지 주어 사망 확인

---

## 완료 기준

- [ ] 웨이브 기반 스폰
- [ ] 화면 밖 랜덤 위치 스폰
- [ ] 플레이어 추적 AI
- [ ] 접촉 대미지
- [ ] `IDamageable` 구현
- [ ] 사망 시 이벤트 발생
- [ ] 풀링 적용
- [ ] 최대 적 수 제한
