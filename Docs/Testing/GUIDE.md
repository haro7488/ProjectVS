# ProjectVS 테스트 작성 가이드

## 시작하기

### 요구 사항

- Unity 2022.3.x
- Unity Test Framework 1.1.33+ (이미 설치됨)
- NUnit 기본 지식

### 테스트 실행 방법

**Unity 에디터에서:**
```
Window > General > Test Runner
- EditMode 탭: 로직 테스트
- PlayMode 탭: 통합 테스트
```

**Unity MCP 경유:**
```
mcp__UnityMCP__run_tests(mode="EditMode")
mcp__UnityMCP__get_test_job(job_id, wait_timeout=60)
```

**커맨드라인:**
```bash
unity -runTests -testPlatform EditMode -testResults results.xml
```

---

## Edit Mode 테스트 작성

### 기본 구조

```csharp
using NUnit.Framework;
using UnityEngine;
using Vs.Tests.Utilities;

namespace Vs.Progression.Tests
{
    [TestFixture]
    public class ExperienceManagerTests : TestBase
    {
        private ExperienceManager _manager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _manager = CreateComponent<ExperienceManager>();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void AddExperience_IncreasesCurrentExp()
        {
            // Arrange
            int expToAdd = 10;

            // Act
            _manager.AddExperience(expToAdd);

            // Assert
            Assert.AreEqual(expToAdd, _manager.CurrentExp);
        }

        [Test]
        public void AddExperience_TriggersLevelUp_WhenExpReachesThreshold()
        {
            // Arrange
            bool levelUpTriggered = false;
            _manager.OnLevelUp += (level) => levelUpTriggered = true;

            // Act
            _manager.AddExperience(100); // 레벨업 임계값 초과

            // Assert
            Assert.IsTrue(levelUpTriggered);
            Assert.Greater(_manager.CurrentLevel, 1);
        }
    }
}
```

### 명명 규칙

```
[테스트 대상 메서드]_[예상 결과]_[조건]

예:
- AddExperience_IncreasesCurrentExp
- AddExperience_TriggersLevelUp_WhenExpReachesThreshold
- GenerateChoices_ReturnsThreeChoices_WhenCalled
- CanEvolveWeapon_ReturnsTrue_WhenConditionsMet
```

### 테스트 카테고리

```csharp
[Test]
[Category("Progression")]
public void TestMethod() { }

[Test]
[Category("Combat")]
[Category("Weapons")]
public void TestWeaponMethod() { }
```

---

## Play Mode 테스트 작성

### 기본 구조

```csharp
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vs.Tests.Utilities;

namespace Vs.Integration.Tests
{
    [TestFixture]
    public class GameManagerIntegrationTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // 테스트 씬 로드
            yield return TestSceneLoader.LoadTestScene();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return TestSceneLoader.UnloadTestScene();
        }

        [UnityTest]
        public IEnumerator StartGame_ChangesStateToPlaying()
        {
            // Arrange
            var gameManager = Object.FindObjectOfType<GameManager>();

            // Act
            gameManager.StartGame();
            yield return null; // 한 프레임 대기

            // Assert
            Assert.AreEqual(GameState.Playing, gameManager.State);
            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator PauseGame_SetsTimeScaleToZero()
        {
            // Arrange
            var gameManager = Object.FindObjectOfType<GameManager>();
            gameManager.StartGame();
            yield return null;

            // Act
            gameManager.PauseGame();
            yield return null;

            // Assert
            Assert.AreEqual(GameState.Paused, gameManager.State);
            Assert.AreEqual(0f, Time.timeScale);
        }
    }
}
```

### 비동기 대기 패턴

```csharp
// 특정 조건까지 대기
[UnityTest]
public IEnumerator WaitForCondition_Example()
{
    yield return new WaitUntil(() => someCondition);
    // 또는
    yield return new WaitForSeconds(1f);
    // 또는
    yield return new WaitForEndOfFrame();
}

// 타임아웃 포함 대기
[UnityTest]
[Timeout(5000)] // 5초 타임아웃
public IEnumerator WithTimeout_Example()
{
    yield return new WaitUntil(() => someCondition);
}
```

---

## 공통 유틸리티 사용

### TestBase 클래스

```csharp
public class TestBase
{
    protected List<GameObject> _createdObjects = new();

    [SetUp]
    public virtual void SetUp()
    {
        _createdObjects.Clear();
    }

    [TearDown]
    public virtual void TearDown()
    {
        foreach (var obj in _createdObjects)
        {
            Object.DestroyImmediate(obj);
        }
        _createdObjects.Clear();
    }

    protected T CreateComponent<T>() where T : Component
    {
        var go = new GameObject(typeof(T).Name);
        _createdObjects.Add(go);
        return go.AddComponent<T>();
    }
}
```

### MockFactory 사용

```csharp
// WeaponData Mock 생성
var weaponData = MockFactory.CreateWeaponData(
    id: "test_pistol",
    damage: 10,
    interval: 1f
);

// EnemyData Mock 생성
var enemyData = MockFactory.CreateEnemyData(
    id: "test_zombie",
    health: 100,
    speed: 2f
);
```

### AssertExtensions 사용

```csharp
// 근사값 비교
AssertExtensions.AreApproximatelyEqual(expected, actual, tolerance);

// 컬렉션 검증
AssertExtensions.ContainsType<WeaponUpgrade>(choices);

// 이벤트 발생 검증
AssertExtensions.EventWasFired(eventHandler);
```

---

## In-Game 테스트 작성

### TestScenario 상속

```csharp
public class BasicGameplayScenario : TestScenario
{
    public override string Name => "기본 게임플레이";
    public override float Duration => 300f; // 5분

    protected override void OnStart()
    {
        // 시나리오 시작 시 실행
        Log("기본 게임플레이 시나리오 시작");
    }

    protected override void OnUpdate()
    {
        // 매 프레임 실행
        CheckPlayerHealth();
        CheckEnemyCount();
    }

    protected override void OnComplete()
    {
        // 시나리오 종료 시 실행
        ReportResults();
    }

    private void CheckPlayerHealth()
    {
        var health = PlayerHealth.Instance;
        if (health.IsDead)
        {
            Fail("플레이어 사망");
        }
    }
}
```

### 테스트 실행

```csharp
// 코드에서 실행
TestRunner.Instance.RunScenario<BasicGameplayScenario>();

// MCP에서 트리거
mcp__UnityMCP__manage_components(
    action="set_property",
    target="TestRunner",
    component_type="TestRunner",
    property="CurrentScenario",
    value="BasicGameplayScenario"
)
```

---

## 베스트 프랙티스

### DO (권장)

```csharp
// 1. 하나의 테스트에 하나의 검증
[Test]
public void AddWeapon_IncreasesWeaponCount()
{
    controller.AddWeapon(weaponData);
    Assert.AreEqual(1, controller.WeaponCount);
}

// 2. Arrange-Act-Assert 패턴 사용
[Test]
public void ExampleTest()
{
    // Arrange - 준비
    var manager = CreateComponent<Manager>();

    // Act - 실행
    manager.DoSomething();

    // Assert - 검증
    Assert.IsTrue(manager.IsDone);
}

// 3. 의미 있는 테스트 이름
[Test]
public void LevelUp_IncreasesAllStats_WhenPassiveIsMaxHealth() { }
```

### DON'T (비권장)

```csharp
// 1. 여러 동작을 한 테스트에서 검증
[Test]
public void BadTest()
{
    // 여러 기능을 동시에 테스트하지 않기
    manager.AddWeapon(weapon1);
    manager.AddWeapon(weapon2);
    manager.LevelUpWeapon(weapon1);
    Assert.AreEqual(2, manager.WeaponCount); // 어떤 동작 때문에 실패?
}

// 2. 구현 세부사항에 의존
[Test]
public void BadTest()
{
    // private 필드에 직접 접근하지 않기
    var field = typeof(Manager).GetField("_privateField", ...);
}

// 3. 하드코딩된 대기 시간
[UnityTest]
public IEnumerator BadTest()
{
    yield return new WaitForSeconds(5f); // 조건 기반 대기 사용
}
```

---

## 디버깅 팁

### 테스트 실패 시

1. **로그 확인**: `read_console`으로 상세 오류 확인
2. **단계별 실행**: 복잡한 테스트는 단계별로 분리
3. **격리 테스트**: 실패하는 테스트만 단독 실행

### 플레이크(Flaky) 테스트 방지

```csharp
// 타이밍 의존성 제거
[UnityTest]
public IEnumerator AvoidTimingIssue()
{
    // Bad: 고정 시간 대기
    yield return new WaitForSeconds(1f);

    // Good: 조건 기반 대기 + 타임아웃
    float timeout = 2f;
    float elapsed = 0f;
    while (!condition && elapsed < timeout)
    {
        elapsed += Time.deltaTime;
        yield return null;
    }
    Assert.IsTrue(condition, "조건이 타임아웃 내에 충족되지 않음");
}
```

---

## 참고 문서

- [테스트 아키텍처](./ARCHITECTURE.md)
- [테스트 시나리오 명세](./SCENARIOS.md)
- [Unity Test Framework 공식 문서](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/index.html)
