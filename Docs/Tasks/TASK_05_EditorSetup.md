# Task 05: 에디터 설정 (Unity MCP)

## 메타 정보
- **폴더**: `Assets/_Project/`
- **blockedBy**: Task 01~04 완료
- **실행 방법**: 서브에이전트 위임 (Unity MCP 도구 사용)
- **실행자**: `general-purpose` 서브에이전트 (model: opus)

## 목표
Unity MCP를 통해 테스트 가능한 환경 자동 구성

## 서브에이전트 지침

이 문서를 참조하여 Unity MCP 도구로 에셋을 생성하고, 완료 후 다음 형식으로 보고:

```markdown
## 완료 보고

### 수행 작업
- [x] 완료된 항목

### 결과
- 생성된 에셋 경로

### 확인 필요
- 수동 설정 필요 항목
```

---

## 사전 조건

1. Unity Editor 실행 중
2. Unity MCP 연결 확인
3. Round 1 스크립트 컴파일 완료 (`read_console`로 확인)

---

## 실행 순서

### Step 1: 폴더 구조 생성

```
manage_asset(action="create_folder", path="Assets/_Project/Prefabs")
manage_asset(action="create_folder", path="Assets/_Project/Data")
manage_asset(action="create_folder", path="Assets/_Project/Scenes")
```

### Step 2: 레이어/태그 설정

```
manage_editor(action="add_layer", layer_name="Player")      # Layer 6
manage_editor(action="add_layer", layer_name="Enemy")       # Layer 7
manage_editor(action="add_layer", layer_name="Projectile")  # Layer 8
manage_editor(action="add_layer", layer_name="Pickup")      # Layer 9

manage_editor(action="add_tag", tag_name="Player")
manage_editor(action="add_tag", tag_name="Enemy")
```

### Step 3: 프리팹 생성

#### 3-1. Player.prefab
```
# GameObject 생성
manage_gameobject(
    action="create",
    name="Player",
    components_to_add=["SpriteRenderer", "Rigidbody2D", "CircleCollider2D",
                       "Vs.Player.PlayerController", "Vs.Player.PlayerHealth",
                       "Vs.Player.PlayerStats", "Vs.Combat.WeaponController"],
    tag="Player",
    layer="Player",
    save_as_prefab=true,
    prefab_path="Assets/_Project/Prefabs/Player.prefab"
)

# Rigidbody2D 설정
manage_components(
    action="set_property",
    target="Player",
    component_type="Rigidbody2D",
    property="constraints",
    value="FreezeRotation"
)

# CircleCollider2D 설정
manage_components(
    action="set_property",
    target="Player",
    component_type="CircleCollider2D",
    property="radius",
    value=0.3
)
```

#### 3-2. Projectile.prefab
```
manage_gameobject(
    action="create",
    name="Projectile",
    components_to_add=["SpriteRenderer", "CircleCollider2D", "Vs.Combat.Projectile"],
    layer="Projectile",
    save_as_prefab=true,
    prefab_path="Assets/_Project/Prefabs/Projectile.prefab"
)

# Collider를 Trigger로 설정
manage_components(
    action="set_property",
    target="Projectile",
    component_type="CircleCollider2D",
    property="isTrigger",
    value=true
)
```

#### 3-3. Enemy_Zombie.prefab
```
manage_gameobject(
    action="create",
    name="Enemy_Zombie",
    components_to_add=["SpriteRenderer", "Rigidbody2D", "CircleCollider2D",
                       "Vs.Enemy.EnemyBase", "Vs.Enemy.EnemyAI"],
    layer="Enemy",
    save_as_prefab=true,
    prefab_path="Assets/_Project/Prefabs/Enemy_Zombie.prefab"
)

manage_components(
    action="set_property",
    target="Enemy_Zombie",
    component_type="Rigidbody2D",
    property="constraints",
    value="FreezeRotation"
)
```

#### 3-4. ExpGem.prefab
```
manage_gameobject(
    action="create",
    name="ExpGem",
    components_to_add=["SpriteRenderer", "CircleCollider2D", "Vs.Progression.ExpPickup"],
    layer="Pickup",
    save_as_prefab=true,
    prefab_path="Assets/_Project/Prefabs/ExpGem.prefab"
)

manage_components(
    action="set_property",
    target="ExpGem",
    component_type="CircleCollider2D",
    property="isTrigger",
    value=true
)
```

### Step 4: ScriptableObject 에셋 생성

#### 4-1. Character_Survivor.asset
```
manage_scriptable_object(
    action="create",
    type_name="Vs.Data.CharacterData",
    folder_path="Assets/_Project/Data",
    asset_name="Character_Survivor",
    patches=[
        {"path": "_id", "value": "survivor"},
        {"path": "_displayName", "value": "생존자"},
        {"path": "_baseMaxHealth", "value": 100},
        {"path": "_baseMoveSpeed", "value": 5},
        {"path": "_isUnlockedByDefault", "value": true}
    ]
)
```

#### 4-2. Weapon_Pistol.asset
```
manage_scriptable_object(
    action="create",
    type_name="Vs.Data.WeaponData",
    folder_path="Assets/_Project/Data",
    asset_name="Weapon_Pistol",
    patches=[
        {"path": "_id", "value": "pistol"},
        {"path": "_displayName", "value": "권총"},
        {"path": "_weaponType", "value": 0},  # Projectile
        {"path": "_baseDamage", "value": 10},
        {"path": "_baseInterval", "value": 1.0},
        {"path": "_baseProjectileCount", "value": 1},
        {"path": "_baseSpeed", "value": 15}
    ]
)
```

#### 4-3. Enemy_Zombie.asset
```
manage_scriptable_object(
    action="create",
    type_name="Vs.Data.EnemyData",
    folder_path="Assets/_Project/Data",
    asset_name="Enemy_Zombie",
    patches=[
        {"path": "_id", "value": "zombie"},
        {"path": "_displayName", "value": "좀비"},
        {"path": "_maxHealth", "value": 10},
        {"path": "_moveSpeed", "value": 2},
        {"path": "_contactDamage", "value": 10},
        {"path": "_expValue", "value": 1}
    ]
)
```

#### 4-4. Stage_City.asset
```
manage_scriptable_object(
    action="create",
    type_name="Vs.Data.StageData",
    folder_path="Assets/_Project/Data",
    asset_name="Stage_City",
    patches=[
        {"path": "_id", "value": "city"},
        {"path": "_displayName", "value": "도시 외곽"},
        {"path": "_duration", "value": 300}
    ]
)
```

### Step 5: 씬 구성

#### 5-1. GameScene 생성
```
manage_scene(action="create", name="GameScene", path="Assets/_Project/Scenes/GameScene.unity")
manage_scene(action="load", path="Assets/_Project/Scenes/GameScene.unity")
```

#### 5-2. Managers 오브젝트 생성
```
# 빈 부모 오브젝트
manage_gameobject(action="create", name="--- Managers ---")

# 매니저들
manage_gameobject(
    action="create",
    name="GameManager",
    parent="--- Managers ---",
    components_to_add=["Vs.Core.GameManager"]
)

manage_gameobject(
    action="create",
    name="TimeManager",
    parent="--- Managers ---",
    components_to_add=["Vs.Core.TimeManager"]
)

manage_gameobject(
    action="create",
    name="PoolManager",
    parent="--- Managers ---",
    components_to_add=["Vs.Core.PoolManager"]
)

manage_gameobject(
    action="create",
    name="ExperienceManager",
    parent="--- Managers ---",
    components_to_add=["Vs.Progression.ExperienceManager"]
)

manage_gameobject(
    action="create",
    name="LevelUpManager",
    parent="--- Managers ---",
    components_to_add=["Vs.Progression.LevelUpManager"]
)
```

#### 5-3. Spawner 생성
```
manage_gameobject(action="create", name="--- Spawners ---")

manage_gameobject(
    action="create",
    name="EnemySpawner",
    parent="--- Spawners ---",
    components_to_add=["Vs.Enemy.EnemySpawner"]
)
```

#### 5-4. Player 프리팹 인스턴스
```
manage_gameobject(action="create", name="--- Gameplay ---")

manage_gameobject(
    action="create",
    name="Player",
    parent="--- Gameplay ---",
    prefab_path="Assets/_Project/Prefabs/Player.prefab"
)
```

#### 5-5. Camera 설정
```
# Main Camera는 기본 존재, 플레이어 추적 스크립트 추가
manage_components(
    action="add",
    target="Main Camera",
    component_type="Vs.Core.CameraFollow"
)
```

#### 5-6. Background
```
manage_gameobject(action="create", name="--- Environment ---")

manage_gameobject(
    action="create",
    name="Background",
    parent="--- Environment ---",
    components_to_add=["SpriteRenderer"],
    position=[0, 0, 10]
)
```

### Step 6: 씬 저장 및 검증

```
manage_scene(action="save")
read_console()  # 오류 확인
```

---

## 참조 연결 (Step 7)

프리팹과 SO 간 참조는 `manage_components`의 `set_property`로 설정:

```
# WeaponData에 ProjectilePrefab 연결
manage_scriptable_object(
    action="modify",
    target={"path": "Assets/_Project/Data/Weapon_Pistol.asset"},
    patches=[
        {"path": "_projectilePrefab", "value": {"guid": "<Projectile.prefab GUID>"}}
    ]
)

# CharacterData에 StartingWeapon 연결
manage_scriptable_object(
    action="modify",
    target={"path": "Assets/_Project/Data/Character_Survivor.asset"},
    patches=[
        {"path": "_startingWeapon", "value": {"guid": "<Weapon_Pistol.asset GUID>"}}
    ]
)
```

> **Note**: GUID는 `manage_asset(action="get_info", path="...")` 로 조회

---

## 완료 기준

- [ ] 폴더 구조 생성
- [ ] 레이어/태그 설정
- [ ] Player.prefab 생성
- [ ] Projectile.prefab 생성
- [ ] Enemy_Zombie.prefab 생성
- [ ] ExpGem.prefab 생성
- [ ] Character_Survivor.asset 생성
- [ ] Weapon_Pistol.asset 생성
- [ ] Enemy_Zombie.asset 생성
- [ ] Stage_City.asset 생성
- [ ] GameScene.unity 구성
- [ ] 모든 참조 연결 완료
- [ ] `read_console()` 오류 없음

---

## 충돌 매트릭스 설정

> **Note**: Unity MCP에서 Physics2D 충돌 매트릭스 직접 설정 불가
> TASK_06 통합 단계에서 스크립트로 처리 또는 수동 설정

```
Layer 설정 (Edit > Project Settings > Physics 2D):

           Player  Enemy  Projectile  Pickup
Player       -       ✓        -         ✓
Enemy        ✓       -        ✓         -
Projectile   -       ✓        -         -
Pickup       ✓       -        -         -
```
