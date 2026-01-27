# M1 프로토타입 - 작업 개요

## 목표
플레이 가능한 최소 프로토타입: 이동 + 공격 + 적 + 경험치

## 작업 구조

```
Round 1: 병렬 스크립트 구현
├── TASK_01_Player.md      (Vs.Player)
├── TASK_02_Weapon.md      (Vs.Combat/Weapons)
├── TASK_03_Enemy.md       (Vs.Enemy)
└── TASK_04_Progression.md (Vs.Progression)

Round 2: 에디터 설정
└── TASK_05_EditorSetup.md (프리팹/씬/SO)

Round 3: 통합
└── TASK_06_Integration.md (연결 + 테스트)
```

## 의존성 그래프

```
[Round 1 - 병렬]
┌─────────────────────────────────────┐
│  Task1    Task2    Task3    Task4   │
│  Player   Weapon   Enemy    Exp     │
│    ↓        ↓        ↓       ↓      │
└─────────────────────────────────────┘
                    ↓
[Round 2 - 순차]
┌─────────────────────────────────────┐
│           Task5: EditorSetup        │
│    (프리팹 + 씬 + SO 에셋 생성)      │
└─────────────────────────────────────┘
                    ↓
[Round 3 - 순차]
┌─────────────────────────────────────┐
│         Task6: Integration          │
│       (연결 + 플레이 테스트)          │
└─────────────────────────────────────┘
```

## 완료 기준

### M1 완료 = 다음 시나리오 플레이 가능

1. Play 버튼 클릭
2. WASD로 플레이어 이동
3. 무기가 자동으로 발사
4. 적이 스폰되어 플레이어 추적
5. 투사체가 적에게 대미지
6. 적 사망 시 경험치 젬 드롭
7. 젬 수집 → 레벨업 → 게임 일시정지

## 파일 소유권

| Task | 소유 폴더 | 충돌 위험 |
|------|-----------|-----------|
| Task 1 | `Scripts/Vs.Player/*` | 없음 |
| Task 2 | `Scripts/Vs.Combat/Weapons/*` | 없음 |
| Task 3 | `Scripts/Vs.Enemy/*` | 없음 |
| Task 4 | `Scripts/Vs.Progression/*` | 없음 |
| Task 5 | `Assets/_Project/*`, `Scripts/Editor/*` | 없음 |
| Task 6 | 기존 파일 수정 | **있음** (순차 실행) |

## 실행 명령

```
# Round 1: 4개 병렬 에이전트
단일 메시지에 Task tool 4회 호출 (model: opus)

# Round 2-3: 메인 직접 또는 순차 에이전트
```
