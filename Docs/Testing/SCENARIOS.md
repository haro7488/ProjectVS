# ProjectVS 테스트 시나리오 명세

## 개요

이 문서는 ProjectVS의 모든 테스트 시나리오를 정의합니다.
총 21개 시나리오가 5개 카테고리로 분류됩니다.

---

## 1. 기본 기능 (4개)

### T1.1: 이동

| 항목 | 내용 |
|------|------|
| **ID** | T1.1 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ⚠️ 제한적 |
| **테스트 대상** | `PlayerController.Move()` |

**수행 절차**:
1. 게임 시작
2. WASD 입력으로 8방향 이동
3. 이동 속도 및 방향 확인

**예상 결과**:
- 플레이어가 입력 방향으로 이동
- 이동 속도 = CharacterData.BaseMoveSpeed * (1 + bonus)
- 카메라가 플레이어 추적

**검증 코드**:
```csharp
[UnityTest]
public IEnumerator Movement_PlayerMovesInInputDirection()
{
    var player = FindPlayer();
    var startPos = player.transform.position;

    player.GetComponent<PlayerController>().SetMoveInput(Vector3.forward);
    yield return new WaitForSeconds(0.5f);

    Assert.Greater(player.transform.position.z, startPos.z);
}
```

---

### T1.2: 자동 공격

| 항목 | 내용 |
|------|------|
| **ID** | T1.2 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `ProjectileWeapon.Fire()` |

**수행 절차**:
1. 기본 무기(Pistol) 장착 확인
2. 적 스폰 대기
3. 자동 발사 확인

**예상 결과**:
- 무기 interval에 따라 자동 발사
- 가장 가까운 적 방향으로 투사체 발사
- 콘솔 로그 출력

**MCP 검증**:
```
manage_editor(action="play")
→ 5초 대기
read_console(filter_text="Fire")
→ 발사 로그 확인
```

---

### T1.3: 피격/무적

| 항목 | 내용 |
|------|------|
| **ID** | T1.3 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `PlayerHealth.TakeDamage()` |

**수행 절차**:
1. 플레이어 초기 체력 확인 (100)
2. 피격 발생
3. 체력 감소 확인
4. 무적 시간(0.5초) 동안 추가 피격 불가 확인

**예상 결과**:
- 체력 = MaxHealth - damage
- IsInvincible = true (0.5초)
- 무적 중 추가 피격 무시

**검증 코드**:
```csharp
[Test]
public void TakeDamage_ReducesHealth()
{
    var health = CreateComponent<PlayerHealth>();
    health.Initialize(100, 100);

    health.TakeDamage(new DamageInfo(10));

    Assert.AreEqual(90, health.CurrentHealth);
}

[Test]
public void TakeDamage_ActivatesInvincibility()
{
    var health = CreateComponent<PlayerHealth>();
    health.Initialize(100, 100);

    health.TakeDamage(new DamageInfo(10));

    Assert.IsTrue(health.IsInvincible);
}
```

---

### T1.4: 적 처치

| 항목 | 내용 |
|------|------|
| **ID** | T1.4 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `EnemyBase.Die()`, `ExpPickupSpawner` |

**수행 절차**:
1. 적 생성
2. 적 체력을 0으로 설정
3. 사망 처리 확인
4. 경험치 젬 스폰 확인

**예상 결과**:
- OnEnemyDied 이벤트 발생
- ExpGem 프리팹 스폰
- 적 오브젝트 풀에 반환

---

## 2. 무기별 테스트 (6개)

### T2.1: Pistol (투사체)

| 항목 | 내용 |
|------|------|
| **ID** | T2.1 |
| **테스트 타입** | EditMode + PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `ProjectileWeapon`, `Projectile` |

**테스트 포인트**:
- 가장 가까운 적 타겟팅
- 다중 투사체 발사 (spreadAngle)
- 투사체 속도 및 대미지

**검증 코드**:
```csharp
[Test]
public void FindNearestEnemy_ReturnsClosestEnemy()
{
    var weapon = CreateComponent<ProjectileWeapon>();
    var enemy1 = CreateEnemy(new Vector3(5, 0, 0));
    var enemy2 = CreateEnemy(new Vector3(2, 0, 0));

    var nearest = weapon.FindNearestEnemy();

    Assert.AreEqual(enemy2, nearest);
}
```

---

### T2.2: Knife (이동 방향)

| 항목 | 내용 |
|------|------|
| **ID** | T2.2 |
| **테스트 타입** | EditMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `ProjectileWeapon (useMoveDirection=true)` |

**테스트 포인트**:
- 이동 방향으로 발사
- 정지 시 마지막 이동 방향 유지

---

### T2.3: Bat (근접)

| 항목 | 내용 |
|------|------|
| **ID** | T2.3 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `MeleeWeapon.PerformSwing()` |

**테스트 포인트**:
- 전방 90도 범위 공격
- OverlapSphere 히트 검출
- 중복 타격 방지 (_hitEnemies)

---

### T2.4: Molotov (범위)

| 항목 | 내용 |
|------|------|
| **ID** | T2.4 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `AreaWeapon`, `FireZone` |

**테스트 포인트**:
- 투척 목표 지점 계산
- FireZone 생성 및 지속 시간
- 범위 내 틱 데미지

---

### T2.5: Drone (궤도)

| 항목 | 내용 |
|------|------|
| **ID** | T2.5 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `OrbitWeapon`, `Orbiter` |

**테스트 포인트**:
- 플레이어 주변 회전
- Orbiter 개수 = projectileCount
- 레벨업 시 Orbiter 재구성

---

### T2.6: Lightning (연쇄)

| 항목 | 내용 |
|------|------|
| **ID** | T2.6 |
| **테스트 타입** | EditMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `LightningWeapon.StrikeLightning()` |

**테스트 포인트**:
- 랜덤 타겟 선택
- 연쇄 피해 (80% 감소)
- 중복 타격 방지

**검증 코드**:
```csharp
[Test]
public void ChainDamage_Reduces80Percent()
{
    float baseDamage = 100f;
    float chainDamage = baseDamage * 0.8f;

    Assert.AreEqual(80f, chainDamage);
}
```

---

## 3. 레벨업 시스템 (4개)

### T3.1: 레벨업 트리거

| 항목 | 내용 |
|------|------|
| **ID** | T3.1 |
| **테스트 타입** | EditMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `ExperienceManager.CheckLevelUp()` |

**수행 절차**:
1. 경험치 = 0, 레벨 = 1
2. 경험치 추가 (레벨업 임계값 이상)
3. OnLevelUp 이벤트 발생 확인

**검증 코드**:
```csharp
[Test]
public void AddExperience_TriggersLevelUp_AtThreshold()
{
    var manager = CreateComponent<ExperienceManager>();
    bool leveledUp = false;
    manager.OnLevelUp += (level) => leveledUp = true;

    manager.AddExperience(10); // baseExp = 10

    Assert.IsTrue(leveledUp);
    Assert.AreEqual(2, manager.CurrentLevel);
}
```

---

### T3.2: 선택지 생성

| 항목 | 내용 |
|------|------|
| **ID** | T3.2 |
| **테스트 타입** | EditMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `LevelUpManager.GenerateChoices()` |

**테스트 포인트**:
- 항상 3개 선택지 반환
- 진화 선택지 우선 배치
- 최대 레벨 무기는 강화에서 제외

**검증 코드**:
```csharp
[Test]
public void GenerateChoices_ReturnsThreeChoices()
{
    var manager = CreateComponent<LevelUpManager>();

    var choices = manager.GenerateChoices(3);

    Assert.AreEqual(3, choices.Length);
}

[Test]
public void GenerateChoices_EvolutionFirst_WhenAvailable()
{
    var manager = SetupManagerWithEvolutionReady();

    var choices = manager.GenerateChoices(3);

    Assert.AreEqual(ChoiceType.WeaponEvolution, choices[0].Type);
}
```

---

### T3.3: 선택지 적용

| 항목 | 내용 |
|------|------|
| **ID** | T3.3 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ⚠️ 제한적 |
| **테스트 대상** | `LevelUpManager.ApplyChoice()` |

**수행 절차**:
1. 레벨업 트리거
2. 선택지 생성
3. 선택지 적용
4. 효과 반영 확인

---

### T3.4: 무기 진화

| 항목 | 내용 |
|------|------|
| **ID** | T3.4 |
| **테스트 타입** | EditMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `LevelUpManager.EvolveWeapon()` |

**조건**:
- 무기 레벨 8
- 대응 패시브 보유

**검증 코드**:
```csharp
[Test]
public void CanEvolveWeapon_ReturnsTrue_WhenConditionsMet()
{
    var manager = CreateManager();
    var weapon = CreateWeapon(level: 8);
    var passive = weapon.EvolutionRequirement;
    manager.AddPassive(passive);

    bool canEvolve = manager.CanEvolveWeapon(weapon);

    Assert.IsTrue(canEvolve);
}
```

---

## 4. 난이도 스케일링 (4개)

### T4.1: 웨이브 스폰

| 항목 | 내용 |
|------|------|
| **ID** | T4.1 |
| **테스트 타입** | InGame |
| **MCP 지원** | ⚠️ 제한적 |
| **테스트 대상** | `EnemySpawner.WaveRoutine()` |

**테스트 포인트**:
- 시간별 웨이브 활성화
- 적 종류 변화

---

### T4.2: 스탯 스케일링

| 항목 | 내용 |
|------|------|
| **ID** | T4.2 |
| **테스트 타입** | EditMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `StageData.GetEnemyHealthMultiplier()` |

**검증 코드**:
```csharp
[Test]
public void GetHealthMultiplier_IncreasesOverTime()
{
    var stageData = CreateStageData(healthGrowth: 0.05f);

    float mult0 = stageData.GetEnemyHealthMultiplier(0);
    float mult5 = stageData.GetEnemyHealthMultiplier(5);

    Assert.AreEqual(1f, mult0);
    Assert.Greater(mult5, mult0);
}
```

---

### T4.3: 스폰 수량 증가

| 항목 | 내용 |
|------|------|
| **ID** | T4.3 |
| **테스트 타입** | InGame |
| **MCP 지원** | ⚠️ 제한적 |
| **테스트 대상** | `EnemySpawner.SpawnWave()` |

---

### T4.4: 보스 스폰

| 항목 | 내용 |
|------|------|
| **ID** | T4.4 |
| **테스트 타입** | InGame |
| **MCP 지원** | ⚠️ 제한적 |
| **테스트 대상** | `EnemySpawner.BossSpawnRoutine()` |

---

## 5. 엣지 케이스 (3개)

### T5.1: 슬롯 풀

| 항목 | 내용 |
|------|------|
| **ID** | T5.1 |
| **테스트 타입** | EditMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `LevelUpManager.GenerateChoices()` |

**조건**:
- 무기 슬롯 6개 모두 사용
- 패시브 슬롯 6개 모두 사용

**검증 코드**:
```csharp
[Test]
public void GenerateChoices_ExcludesNewWeapon_WhenSlotsFull()
{
    var manager = CreateManagerWithFullWeaponSlots();

    var choices = manager.GenerateChoices(3);

    Assert.IsFalse(choices.Any(c => c.Type == ChoiceType.NewWeapon));
}
```

---

### T5.2: 게임 오버

| 항목 | 내용 |
|------|------|
| **ID** | T5.2 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `GameManager.EndGame(false)` |

**수행 절차**:
1. 플레이어 체력을 0으로 설정
2. HandleDeath() 호출
3. GameState.GameOver 전환 확인

---

### T5.3: 승리

| 항목 | 내용 |
|------|------|
| **ID** | T5.3 |
| **테스트 타입** | PlayMode |
| **MCP 지원** | ✅ 가능 |
| **테스트 대상** | `TimeManager.OnTimeUp`, `GameManager` |

**수행 절차**:
1. 제한 시간까지 생존
2. OnTimeUp 이벤트 발생
3. GameState.Victory 전환 확인

---

## 테스트 커버리지 요약

| 카테고리 | 시나리오 수 | EditMode | PlayMode | InGame |
|----------|-------------|----------|----------|--------|
| 기본 기능 | 4 | 1 | 3 | 0 |
| 무기별 | 6 | 3 | 3 | 0 |
| 레벨업 | 4 | 3 | 1 | 0 |
| 난이도 | 4 | 1 | 0 | 3 |
| 엣지 케이스 | 3 | 1 | 2 | 0 |
| **합계** | **21** | **9** | **9** | **3** |

---

## 참고 문서

- [테스트 아키텍처](./ARCHITECTURE.md)
- [테스트 작성 가이드](./GUIDE.md)
