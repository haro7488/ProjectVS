# Task 05: 에디터 설정 (프리팹/씬/SO)

## 메타 정보
- **폴더**: `Assets/_Project/`, `Assets/Scripts/Editor/`
- **blockedBy**: Task 01~04 완료

## 목표
Unity 에디터에서 테스트 가능한 환경 구성

---

## 생성할 에셋

### 1. 프리팹 (`Assets/_Project/Prefabs/`)

#### Player.prefab
```
Player (GameObject)
├── Components:
│   ├── SpriteRenderer (임시 흰색 사각형)
│   ├── Rigidbody2D (Dynamic, Freeze Rotation Z)
│   ├── CircleCollider2D (Radius: 0.3)
│   ├── PlayerController
│   ├── PlayerHealth
│   ├── PlayerStats
│   ├── WeaponController
│   └── PickupMagnet
├── Layer: Player
└── Tag: Player
```

#### Projectile.prefab
```
Projectile (GameObject)
├── Components:
│   ├── SpriteRenderer (작은 원, 노란색)
│   ├── CircleCollider2D (Trigger, Radius: 0.1)
│   └── Projectile
└── Layer: Projectile
```

#### Enemy_Zombie.prefab
```
Enemy_Zombie (GameObject)
├── Components:
│   ├── SpriteRenderer (빨간 사각형)
│   ├── Rigidbody2D (Dynamic, Freeze Rotation Z)
│   ├── CircleCollider2D (Radius: 0.4)
│   ├── EnemyBase
│   └── EnemyAI
└── Layer: Enemy
```

#### ExpGem.prefab
```
ExpGem (GameObject)
├── Components:
│   ├── SpriteRenderer (작은 파란 다이아몬드)
│   ├── CircleCollider2D (Trigger, Radius: 0.2)
│   └── ExpPickup
└── Layer: Pickup
```

---

### 2. ScriptableObject 에셋 (`Assets/_Project/Data/`)

#### Character_Survivor.asset
```yaml
Id: "survivor"
DisplayName: "생존자"
BaseMaxHealth: 100
BaseMoveSpeed: 5
StartingWeapon: Weapon_Pistol
IsUnlockedByDefault: true
```

#### Weapon_Pistol.asset
```yaml
Id: "pistol"
DisplayName: "권총"
Type: Projectile
BaseDamage: 10
BaseInterval: 1.0
BaseProjectileCount: 1
BaseSpeed: 15
ProjectilePrefab: Projectile.prefab
```

#### Enemy_Zombie.asset
```yaml
Id: "zombie"
DisplayName: "좀비"
MaxHealth: 10
MoveSpeed: 2
ContactDamage: 10
ExpValue: 1
Prefab: Enemy_Zombie.prefab
```

#### Stage_City.asset
```yaml
Id: "city"
DisplayName: "도시 외곽"
Duration: 300  # 5분 (테스트용)
Waves:
  - StartTime: 0
    Enemy: Enemy_Zombie
    SpawnInterval: 2
    SpawnCount: 1
  - StartTime: 60
    SpawnInterval: 1.5
    SpawnCount: 2
  - StartTime: 120
    SpawnInterval: 1
    SpawnCount: 3
```

---

### 3. 씬 (`Assets/_Project/Scenes/GameScene.unity`)

#### 하이어라키 구조
```
GameScene
├── --- Managers ---
│   ├── GameManager
│   ├── TimeManager
│   ├── PoolManager
│   ├── ExperienceManager
│   └── LevelUpManager
│
├── --- Spawners ---
│   └── EnemySpawner
│       └── StageData: Stage_City
│       └── Player: (참조)
│
├── --- Gameplay ---
│   ├── Player (Prefab Instance)
│   │   └── CharacterData: Character_Survivor
│   └── Main Camera
│       └── (플레이어 추적 스크립트 또는 Cinemachine)
│
└── --- Environment ---
    └── Background
        └── SpriteRenderer (큰 회색 사각형)
```

#### 레이어 설정
```
Layer 6: Player
Layer 7: Enemy
Layer 8: Projectile
Layer 9: Pickup
```

#### 충돌 매트릭스
```
           Player  Enemy  Projectile  Pickup
Player       -       ✓        -         ✓
Enemy        ✓       -        ✓         -
Projectile   -       ✓        -         -
Pickup       ✓       -        -         -
```

---

### 4. 에디터 스크립트 (선택)

#### M1SetupEditor.cs
```csharp
// Assets/Scripts/Editor/M1SetupEditor.cs
// 메뉴: Tools > ProjectVS > Setup M1

[MenuItem("Tools/ProjectVS/Setup M1 Prototype")]
public static void SetupM1()
{
    // 1. 레이어 생성
    // 2. 프리팹 생성
    // 3. SO 에셋 생성
    // 4. 씬 생성 및 구성
}
```

---

## 실행 방법

### 수동 설정 (권장)
1. Unity 에디터에서 폴더 구조 확인
2. 각 프리팹 수동 생성 (위 명세 참고)
3. SO 에셋 생성 (Create > VS > ...)
4. 씬 구성 및 참조 연결

### 자동 설정 (선택)
1. `M1SetupEditor.cs` 구현
2. Tools > ProjectVS > Setup M1 실행

---

## 완료 기준

- [ ] Player.prefab 생성 (모든 컴포넌트 포함)
- [ ] Projectile.prefab 생성
- [ ] Enemy_Zombie.prefab 생성
- [ ] ExpGem.prefab 생성
- [ ] Character_Survivor.asset 생성
- [ ] Weapon_Pistol.asset 생성
- [ ] Enemy_Zombie.asset 생성
- [ ] Stage_City.asset 생성
- [ ] GameScene.unity 구성
- [ ] 레이어 및 충돌 설정
- [ ] 모든 참조 연결 완료
