# ProjectVS 테스트 아키텍처

## 개요

ProjectVS의 테스트 시스템은 3계층 구조로 설계되어 있습니다.

```
┌─────────────────────────────────────────────────────────┐
│                   Test Infrastructure                    │
├─────────────────────────────────────────────────────────┤
│  Layer 3: In-Game Test Runner (밸런스/QA)               │
│    - 무기 조합 시뮬레이션                                │
│    - 난이도 밸런스 검증                                  │
│    - 성능 프로파일링                                     │
├─────────────────────────────────────────────────────────┤
│  Layer 2: Play Mode Tests (통합 검증)                   │
│    - GameManager 상태 전환                               │
│    - WeaponBase 발사 시뮬레이션                          │
│    - UI 연동 검증                                        │
├─────────────────────────────────────────────────────────┤
│  Layer 1: Edit Mode Tests (로직 검증)                   │
│    - LevelUpManager 선택지 생성                          │
│    - ExperienceManager 경험치 계산                       │
│    - BalanceLoader JSON 파싱                             │
├─────────────────────────────────────────────────────────┤
│  Foundation: Test Utilities (공통 기반)                  │
│    - TestBase, MockFactory, AssertExtensions             │
└─────────────────────────────────────────────────────────┘
```

---

## 폴더 구조

```
Assets/Scripts/Tests/
├── TestUtilities/                    # 공통 유틸리티
│   ├── Vs.TestUtilities.asmdef
│   ├── TestBase.cs                   # 테스트 기반 클래스
│   ├── MockFactory.cs                # Mock 객체 팩토리
│   ├── AssertExtensions.cs           # 커스텀 Assert
│   └── TestConstants.cs              # 테스트용 상수
│
├── EditMode/                         # Edit Mode 테스트
│   ├── Vs.Progression.Tests/
│   │   ├── Vs.Progression.Tests.asmdef
│   │   ├── LevelUpManagerTests.cs
│   │   ├── ExperienceManagerTests.cs
│   │   └── LevelUpChoiceTests.cs
│   ├── Vs.Combat.Tests/
│   │   ├── Vs.Combat.Tests.asmdef
│   │   ├── WeaponBaseTests.cs
│   │   ├── DamageCalculationTests.cs
│   │   └── WeaponDataTests.cs
│   ├── Vs.Core.Tests/
│   │   ├── Vs.Core.Tests.asmdef
│   │   ├── PoolManagerTests.cs
│   │   └── SingletonTests.cs
│   └── Vs.Data.Tests/
│       ├── Vs.Data.Tests.asmdef
│       └── BalanceLoaderTests.cs
│
├── PlayMode/                         # Play Mode 테스트
│   ├── Vs.Integration.Tests/
│   │   ├── Vs.Integration.Tests.asmdef
│   │   ├── GameManagerIntegrationTests.cs
│   │   ├── WeaponFiringTests.cs
│   │   ├── LevelUpFlowTests.cs
│   │   └── GameStateTransitionTests.cs
│   └── Scenes/
│       └── TestScene.unity
│
└── InGame/                           # In-Game 테스트 러너
    ├── Vs.TestRunner.asmdef
    ├── TestRunner.cs                 # 테스트 관리
    ├── TestScenario.cs               # 시나리오 기반 클래스
    ├── TestReporter.cs               # 결과 리포트
    └── Scenarios/
        ├── BasicGameplayScenario.cs
        ├── WeaponTestScenario.cs
        ├── LevelUpScenario.cs
        └── DifficultyScalingScenario.cs
```

---

## 계층별 설명

### Layer 1: Edit Mode Tests

**목적**: 순수 로직 검증 (MonoBehaviour 없이)

**특징**:
- 빠른 실행 속도
- 격리된 단위 테스트
- CI/CD 파이프라인 통합 용이

**테스트 대상**:
| 모듈 | 클래스 | 테스트 내용 |
|------|--------|-------------|
| Vs.Progression | LevelUpManager | 선택지 생성 로직 |
| Vs.Progression | ExperienceManager | 경험치 계산 |
| Vs.Combat | WeaponBase | 레벨업, 스탯 계산 |
| Vs.Data | BalanceLoader | JSON 파싱 |
| Vs.Core | Singleton | 인스턴스 관리 |

### Layer 2: Play Mode Tests

**목적**: GameObject/MonoBehaviour 기반 통합 검증

**특징**:
- 실제 Unity 런타임 환경
- 씬 로드 및 컴포넌트 상호작용
- 코루틴/비동기 테스트 지원

**테스트 대상**:
| 시나리오 | 테스트 내용 |
|----------|-------------|
| 게임 상태 전환 | Menu → Playing → Paused → GameOver |
| 무기 발사 | 투사체 생성, 타겟팅, 대미지 |
| 레벨업 플로우 | 경험치 → 레벨업 → 선택지 → 적용 |
| UI 연동 | 체력바, 경험치바, 아이템 슬롯 |

### Layer 3: In-Game Test Runner

**목적**: 복잡한 게임플레이 시나리오 및 밸런스 검증

**특징**:
- 게임 내에서 실행
- 시간 기반 테스트 지원
- 통계 및 리포트 생성

**테스트 대상**:
| 시나리오 | 테스트 내용 |
|----------|-------------|
| 기본 게임플레이 | 5분 생존 시뮬레이션 |
| 무기 조합 | 모든 무기 조합 효과 검증 |
| 난이도 스케일링 | 웨이브별 적 강도 확인 |
| 밸런스 검증 | DPS, 생존율 통계 |

---

## Assembly 의존성

```
Vs.TestUtilities (독립)
    ↑
├── Vs.Progression.Tests
├── Vs.Combat.Tests
├── Vs.Core.Tests
└── Vs.Data.Tests
    ↑
Vs.Integration.Tests (Play Mode)
    ↑
Vs.TestRunner (In-Game)
```

---

## Unity MCP 연동

### MCP로 가능한 작업

| 기능 | MCP 도구 |
|------|----------|
| 테스트 실행 | `run_tests` |
| 결과 확인 | `get_test_job` |
| 게임 플레이/정지 | `manage_editor(action="play/stop")` |
| 콘솔 확인 | `read_console` |
| 상태 검증 | `find_gameobjects`, 컴포넌트 리소스 |

### MCP 테스트 워크플로우

```
1. run_tests(mode="EditMode") → job_id
2. get_test_job(job_id, wait_timeout=60) → 결과
3. 실패 시 read_console로 상세 로그 확인
```

---

## 참고 문서

- [테스트 작성 가이드](./GUIDE.md)
- [테스트 시나리오 명세](./SCENARIOS.md)
