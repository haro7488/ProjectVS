# ProjectVS - Architecture

## 기술 스택

| 항목 | 선택 | 용도 |
|------|------|------|
| 엔진 | Unity 2022.x | URP |
| 언어 | C# | .NET Standard 2.1 |
| 비동기 | UniTask | async/await |
| 풀링 | Unity ObjectPool | 오브젝트 재사용 |
| 이벤트 | C# event/Action | 컴포넌트 통신 |
| 데이터 | ScriptableObject + JSON | 정적 + 밸런스 |

---

## 폴더 구조

```
Assets/
├── _Project/                    # 프로젝트 에셋
│   ├── Art/
│   │   ├── Sprites/
│   │   ├── Animations/
│   │   └── UI/
│   ├── Audio/
│   │   ├── BGM/
│   │   └── SFX/
│   ├── Data/                    # ScriptableObject 에셋
│   │   ├── Weapons/
│   │   ├── Passives/
│   │   ├── Characters/
│   │   ├── Enemies/
│   │   └── Stages/
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Weapons/
│   │   ├── Enemies/
│   │   ├── Pickups/
│   │   └── UI/
│   ├── Scenes/
│   └── Settings/
│
├── Scripts/                     # 소스 코드
│   ├── Vs.Core/                 # 코어 시스템
│   ├── Vs.Player/               # 플레이어
│   ├── Vs.Combat/               # 전투 (무기, 대미지)
│   ├── Vs.Enemy/                # 적
│   ├── Vs.Progression/          # 성장 (경험치, 레벨업)
│   ├── Vs.Meta/                 # 메타 (저장, 영구 성장)
│   ├── Vs.Data/                 # 데이터 정의
│   ├── Vs.UI/                   # UI
│   └── Vs.Utility/              # 유틸리티
│
├── StreamingAssets/             # 런타임 로드 데이터
│   └── Balance/                 # JSON 밸런스
│
└── Plugins/
    └── UniTask/
```

---

## Assembly Definition

```
Vs.Utility (독립)
     ↑
Vs.Core ← UniTask
     ↑
Vs.Data
     ↑
┌────┼────┬────────┐
│    │    │        │
Vs.Combat Vs.Progression Vs.Meta
     ↑         ↑
┌────┴────┐    │
│         │    │
Vs.Player Vs.Enemy
               ↑
             Vs.UI
```

| Assembly | 참조 |
|----------|------|
| `Vs.Utility` | - |
| `Vs.Core` | Vs.Utility, UniTask |
| `Vs.Data` | Vs.Utility |
| `Vs.Combat` | Vs.Core, Vs.Data |
| `Vs.Player` | Vs.Core, Vs.Data, Vs.Combat |
| `Vs.Enemy` | Vs.Core, Vs.Data, Vs.Combat |
| `Vs.Progression` | Vs.Core, Vs.Data |
| `Vs.Meta` | Vs.Core, Vs.Data |
| `Vs.UI` | Vs.Core, Vs.Data, Vs.Progression |

---

## 핵심 시스템

### GameManager

게임 상태를 관리하는 싱글톤.

```
Menu → Playing ↔ Paused
         ↓
      LevelUp
         ↓
  GameOver / Victory
```

### 데이터 구조

```
[ScriptableObject]              [JSON]
  - WeaponData                    - weapons.json
  - PassiveData                   - passives.json
  - CharacterData                 - balance.json
  - EnemyData
  - StageData
         ↓                            ↓
         └──────── BalanceLoader ─────┘
                        ↓
                 Runtime Instance
```

- **ScriptableObject**: 에디터에서 설정하는 정적 데이터 (이름, 아이콘, 프리팹)
- **JSON**: 빌드 후에도 수정 가능한 밸런스 데이터 (수치, 레벨별 스케일링)

---

## 주요 인터페이스

### IDamageable

```csharp
public interface IDamageable
{
    void TakeDamage(DamageInfo damage);
    event Action<DamageInfo> OnDamaged;
    event Action OnDeath;
}
```

### IPoolable

```csharp
public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}
```

---

## 이벤트 흐름

### 전투

```
WeaponBase.Fire()
    ↓
Projectile → Enemy.TakeDamage()
                 ↓
            OnDamaged 이벤트
                 ↓
            사망 시 OnDeath
                 ↓
         ExperienceManager.AddExp()
```

### 레벨업

```
ExperienceManager.OnLevelUp
         ↓
   GameManager.ChangeState(LevelUp)
         ↓
   LevelUpUI 표시
         ↓
   선택 완료 → GameManager.ChangeState(Playing)
```

---

## 네이밍 규칙

| 대상 | 규칙 | 예시 |
|------|------|------|
| 네임스페이스 | `Vs.{도메인}` | `Vs.Core`, `Vs.Combat` |
| 클래스 | PascalCase | `GameManager`, `WeaponData` |
| 인터페이스 | `I` + PascalCase | `IDamageable` |
| private 필드 | `_` + camelCase | `_health`, `_damage` |
| 상수 | PascalCase | `MaxWeapons`, `DefaultSpeed` |
| SO 에셋 | `{Type}_{Name}` | `Weapon_Pistol`, `Enemy_Zombie` |

---

## 참조

- `Docs/Design/GDD.md` - 게임 디자인
- `Docs/PROGRESS.md` - 진행 상황
