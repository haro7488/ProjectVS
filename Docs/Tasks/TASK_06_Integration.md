# Task 06: 통합 및 테스트

## 메타 정보
- **blockedBy**: Task 01~05 완료

## 목표
모든 시스템 연결 + 플레이 테스트

---

## 통합 작업

### 1. 무기 → 플레이어 연결

**파일**: `PlayerController.cs` 또는 별도 초기화 스크립트

```csharp
// Player 초기화 시
private void Start()
{
    var weaponController = GetComponent<WeaponController>();
    var stats = GetComponent<PlayerStats>();
    var health = GetComponent<PlayerHealth>();

    // CharacterData에서 초기화
    if (_characterData != null)
    {
        stats.Initialize(_characterData);
        health.Initialize(_characterData.GetMaxHealth());
        weaponController.Initialize(transform);
        weaponController.AddWeapon(_characterData.StartingWeapon);
    }
}
```

### 2. 투사체 → 적 대미지 연결

**파일**: `Projectile.cs`

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.TryGetComponent<IDamageable>(out var damageable))
    {
        var damageInfo = new DamageInfo(
            _damage,
            DamageType.Physical,
            transform.position,
            _direction
        );
        damageable.TakeDamage(damageInfo);

        if (!_isPiercing || --_pierceCount <= 0)
        {
            PoolManager.Instance.Despawn(gameObject);
        }
    }
}
```

### 3. 적 사망 → 경험치 드롭 연결

**파일**: `EnemyBase.cs`

```csharp
protected virtual void Die()
{
    // 경험치 젬 스폰
    var expGem = PoolManager.Instance.Spawn(
        _expGemPrefab,
        transform.position,
        Quaternion.identity
    );

    if (expGem.TryGetComponent<ExpPickup>(out var pickup))
    {
        pickup.Initialize(_data.ExpValue, _target);
    }

    OnDeath?.Invoke();
    PoolManager.Instance.Despawn(gameObject);
}
```

### 4. 경험치 → 레벨업 UI 연결

**파일**: `ExperienceManager.cs`

```csharp
private void CheckLevelUp()
{
    while (_currentExp >= _expToNextLevel)
    {
        _currentExp -= _expToNextLevel;
        _currentLevel++;
        _expToNextLevel = CalculateExpRequired(_currentLevel);

        OnLevelUp?.Invoke(_currentLevel);

        // 게임 일시정지 + 레벨업 상태
        GameManager.Instance.TriggerLevelUp();
    }
}
```

### 5. 레벨업 선택 → 게임 재개

**파일**: 임시 키보드 입력 (UI 전까지)

```csharp
// 임시: 숫자 키로 선택
private void Update()
{
    if (GameManager.Instance.State != GameState.LevelUp) return;

    if (Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(0);
    if (Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(1);
    if (Input.GetKeyDown(KeyCode.Alpha3)) SelectChoice(2);
}

private void SelectChoice(int index)
{
    LevelUpManager.Instance.ApplyChoice(_currentChoices[index]);
    GameManager.Instance.ResumeGame();
}
```

---

## 테스트 시나리오

### 기본 플레이 테스트
1. [ ] Play 버튼 클릭
2. [ ] WASD로 이동
3. [ ] 적 스폰 확인
4. [ ] 자동 발사 확인
5. [ ] 투사체가 적에게 대미지
6. [ ] 적 사망 시 경험치 젬 드롭
7. [ ] 젬 자동 수집
8. [ ] 레벨업 시 게임 일시정지
9. [ ] 1/2/3 키로 선택
10. [ ] 게임 재개

### 엣지 케이스 테스트
- [ ] 플레이어 사망 → GameOver 상태
- [ ] 적 최대 수 도달 → 스폰 중지
- [ ] 무기 슬롯 가득 참 → 강화만 선택지
- [ ] 모든 무기 만렙 → 패시브만 선택지

---

## 수정 필요한 파일

| 파일 | 수정 내용 |
|------|-----------|
| `PlayerController.cs` | 초기화 로직 추가 |
| `Projectile.cs` | 충돌 시 대미지 적용 |
| `EnemyBase.cs` | 사망 시 경험치 드롭 |
| `EnemySpawner.cs` | 플레이어 참조, 적 프리팹 풀 등록 |
| `ExperienceManager.cs` | GameManager 연동 |

---

## 디버그 도구

### 인게임 디버그 UI (선택)
```
[F1] 레벨업 강제 트리거
[F2] 무적 모드 토글
[F3] 적 스폰 중지/재개
[F4] 시간 스케일 조절 (0.5x, 1x, 2x)
```

---

## 완료 기준 (M1 완료)

- [ ] 전체 플레이 루프 작동
- [ ] 이동 → 공격 → 적 처치 → 경험치 → 레벨업
- [ ] 게임 오버 조건 작동
- [ ] 주요 버그 없음
- [ ] 프레임 드랍 없음 (60fps 유지)
