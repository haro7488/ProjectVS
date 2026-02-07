# Debug Panel System

Unity 에디터에서 직접 플레이하며 각 기능을 시각적으로 검증할 수 있는 디버그 패널 시스템.

## 사용법

### 키보드 단축키

| 키 | 기능 |
|----|------|
| F1 | 디버그 패널 토글 |
| F2 | 갓모드 (무적) 토글 |
| F3 | 시간 배속 순환 (1x→2x→4x→0.5x) |
| F4 | 레벨업 트리거 |

### 탭 구성

| 탭 | 기능 |
|----|------|
| Weapon | 무기 추가, 전체 추가, 레벨업 |
| Enemy | 적 스폰, 전체 스폰, 보스, 전체 제거 |
| Level | 경험치 추가, 레벨업 트리거, 레벨 설정 |
| Spawn | 스폰 토글, 스폰 속도 조절 |
| Stats | 스탯 조회/수정, 갓모드, 체력 회복 |
| Cheat | 시간 배속, 승리/패배 트리거 |

## 빌드 정책

- **에디터**: 항상 활성화
- **Development Build**: 활성화
- **Release Build**: 비활성화 (defineConstraints 설정)

## 제거 방법

프로젝트에서 디버그 시스템을 완전히 제거하려면:

1. GameScene에서 DebugController GameObject 삭제
2. Assets/_Debug 폴더 삭제

```bash
rm -rf Assets/_Debug/
rm -rf Assets/_Debug.meta
```

3. Unity 에디터에서 Refresh (Ctrl+R)

## 구조

```
Assets/_Debug/
├── Scripts/
│   ├── Vs.Debug.asmdef
│   ├── DebugController.cs      # 핵심 컨트롤러 (Singleton)
│   ├── DebugPanel.cs           # UIPanel 상속
│   ├── DebugTabBase.cs         # 탭 추상 클래스
│   └── Tabs/
│       ├── WeaponDebugTab.cs
│       ├── EnemyDebugTab.cs
│       ├── LevelDebugTab.cs
│       ├── SpawnDebugTab.cs
│       ├── StatsDebugTab.cs
│       └── CheatDebugTab.cs
├── Prefabs/
│   ├── DebugPanel.prefab
│   └── ...
└── README.md
```

## 의존성

- 단방향 의존성: Debug → Core/Combat/Enemy/Progression
- 다른 어셈블리에서 Vs.Debug를 참조하지 않음
- 폴더 삭제만으로 완전 분리 가능
