# ProjectVS - 진행 상황

## 현재 단계: M1 프로토타입

### 완료
- [x] 프로젝트 초기 설정 (Unity URP)
- [x] Claude Code 템플릿 적용
- [x] GDD 작성
- [x] 아키텍처 설계
  - [x] 폴더 구조 생성
  - [x] Assembly Definition 설정 (9개)
  - [x] 코어 시스템 (GameManager, TimeManager, PoolManager)
  - [x] 데이터 레이어 (WeaponData, PassiveData, CharacterData, EnemyData, StageData)
  - [x] 전투 인터페이스 (IDamageable, DamageInfo)
  - [x] JSON 밸런스 시스템
- [x] 코어 시스템 구현 (Round 1 완료)
  - [x] 플레이어 이동 (PlayerController, PlayerHealth, PlayerStats)
  - [x] 무기 시스템 (WeaponBase, ProjectileWeapon, Projectile)
  - [x] 적 스폰 시스템 (EnemyBase, EnemyAI, EnemySpawner)
  - [x] 경험치/레벨업 (ExperienceManager, LevelUpManager, ExpPickup)
- [x] Unity 에디터 설정 (Round 2 완료)
  - [x] ScriptableObject 데이터 생성 (Character_Survivor, Weapon_Pistol, Enemy_Zombie, Stage_City)
  - [x] 프리팹 생성 (Player, Projectile, Enemy_Zombie, ExpGem, Weapon_Pistol)
  - [x] GameScene 씬 구성
- [x] 통합 및 테스트 (Round 3 완료)
  - [x] PlayerInitializer 추가 - 플레이어 초기화 로직
  - [x] ExpPickupSpawner 추가 - 적 사망 시 경험치 젬 스폰
  - [x] TempLevelUpUI 추가 - 임시 키보드 레벨업 선택
  - [x] 컴파일 오류 수정 (TimeManager override, Assembly 참조)
  - [x] 컴포넌트 연결 (CharacterData, StageData, 프리팹 참조)
- [x] **2D → 3D 쿼터뷰 전환 완료**
  - [x] Singleton.cs - Domain Reload 비활성화 환경 대응
  - [x] 스크립트 12개 수정 (Rigidbody2D → Rigidbody, Vector2 → Vector3)
  - [x] 프리팹 3D 컴포넌트로 교체 (Player, Enemy, Projectile, ExpGem)
  - [x] CameraFollow 쿼터뷰 카메라 구현 (오프셋 10,15,-10 / 회전 45,-45,0)
  - [x] Ground Plane 추가, 머티리얼 생성
- [x] **M1 마무리 완료**
  - [x] 플레이어 머티리얼 설정 (PlayerBlue.mat)
  - [x] Physics 충돌 매트릭스 설정

- [x] **UI 시스템 구현**
  - [x] Core: UIPanel, UIManager (GameState 기반 패널 제어)
  - [x] HUD: HealthBar, ExperienceBar, TimeDisplay, ItemSlotDisplay, ItemSlotsPanel, HUDPanel
  - [x] Popup: LevelUpPanel, LevelUpChoiceButton, GameOverPanel, VictoryPanel
  - [x] Canvas 계층 구성 (HUD/Popup 레이어)
  - [x] TempLevelUpUI 제거
  - [x] SerializeField 참조 자동 연결 (32개)

### 예정
- [ ] 콘텐츠 제작
  - [ ] 무기 6종
  - [ ] 적 4종
  - [ ] 스테이지 1개

---

## 마일스톤

| 단계 | 목표 | 상태 |
|------|------|------|
| M0 | 설계 완료 | **완료** |
| M1 | 프로토타입 (이동 + 공격 + 적) | **완료** |
| M2 | 핵심 루프 (레벨업 + 진화) | 예정 |
| M3 | 콘텐츠 (무기/적/스테이지) | 예정 |
| M4 | 폴리싱 (UI/밸런스) | 예정 |

---

## 생성된 파일

### 코어 시스템
- `Assets/Scripts/Vs.Core/GameManager.cs`
- `Assets/Scripts/Vs.Core/TimeManager.cs`
- `Assets/Scripts/Vs.Core/PoolManager.cs`

### 유틸리티
- `Assets/Scripts/Vs.Utility/Singleton.cs`
- `Assets/Scripts/Vs.Utility/Extensions.cs`
- `Assets/Scripts/Vs.Utility/Constants.cs`

### 데이터
- `Assets/Scripts/Vs.Data/SO/WeaponData.cs`
- `Assets/Scripts/Vs.Data/SO/PassiveData.cs`
- `Assets/Scripts/Vs.Data/SO/CharacterData.cs`
- `Assets/Scripts/Vs.Data/SO/EnemyData.cs`
- `Assets/Scripts/Vs.Data/SO/StageData.cs`
- `Assets/Scripts/Vs.Data/Json/BalanceLoader.cs`

### 전투
- `Assets/Scripts/Vs.Combat/Damage/IDamageable.cs`
- `Assets/Scripts/Vs.Combat/Damage/DamageInfo.cs`
- `Assets/Scripts/Vs.Combat/Weapons/WeaponBase.cs`
- `Assets/Scripts/Vs.Combat/Weapons/ProjectileWeapon.cs`
- `Assets/Scripts/Vs.Combat/Weapons/Projectile.cs`
- `Assets/Scripts/Vs.Combat/Weapons/WeaponController.cs`

### 플레이어
- `Assets/Scripts/Vs.Player/PlayerController.cs`
- `Assets/Scripts/Vs.Player/PlayerHealth.cs`
- `Assets/Scripts/Vs.Player/PlayerStats.cs`
- `Assets/Scripts/Vs.Player/PlayerInitializer.cs` (신규)

### 적
- `Assets/Scripts/Vs.Enemy/EnemyBase.cs`
- `Assets/Scripts/Vs.Enemy/EnemyAI.cs`
- `Assets/Scripts/Vs.Enemy/EnemySpawner.cs`
- `Assets/Scripts/Vs.Enemy/EnemyDeathEvent.cs`

### 진행
- `Assets/Scripts/Vs.Progression/ExperienceManager.cs`
- `Assets/Scripts/Vs.Progression/LevelUpManager.cs`
- `Assets/Scripts/Vs.Progression/LevelUpChoice.cs`
- `Assets/Scripts/Vs.Progression/ExpPickup.cs`
- `Assets/Scripts/Vs.Progression/PickupMagnet.cs`
- `Assets/Scripts/Vs.Progression/ExpPickupSpawner.cs`

### UI
- `Assets/Scripts/Vs.UI/Core/UIPanel.cs`
- `Assets/Scripts/Vs.UI/Core/UIManager.cs`
- `Assets/Scripts/Vs.UI/HUD/HealthBar.cs`
- `Assets/Scripts/Vs.UI/HUD/ExperienceBar.cs`
- `Assets/Scripts/Vs.UI/HUD/TimeDisplay.cs`
- `Assets/Scripts/Vs.UI/HUD/ItemSlotDisplay.cs`
- `Assets/Scripts/Vs.UI/HUD/ItemSlotsPanel.cs`
- `Assets/Scripts/Vs.UI/HUD/HUDPanel.cs`
- `Assets/Scripts/Vs.UI/Popup/LevelUpPanel.cs`
- `Assets/Scripts/Vs.UI/Popup/LevelUpChoiceButton.cs`
- `Assets/Scripts/Vs.UI/Popup/GameOverPanel.cs`
- `Assets/Scripts/Vs.UI/Popup/VictoryPanel.cs`

### 머티리얼
- `Assets/_Project/Materials/PlayerBlue.mat`
- `Assets/_Project/Materials/EnemyRed.mat`
- `Assets/_Project/Materials/ProjectileYellow.mat`
- `Assets/_Project/Materials/ExpGemGreen.mat`
- `Assets/_Project/Materials/GroundGray.mat`

### Assembly Definition (9개)
- `Vs.Utility`, `Vs.Core`, `Vs.Data`, `Vs.Combat`
- `Vs.Player`, `Vs.Enemy`, `Vs.Progression`, `Vs.Meta`, `Vs.UI`

---

## 변경 이력

| 날짜 | 내용 |
|------|------|
| 2026-01-27 | 프로젝트 생성, GDD 작성 |
| 2026-01-27 | 아키텍처 설계 완료, 기반 클래스 구현 |
| 2026-01-28 | M1 Round 1 완료 - 플레이어/무기/적/경험치 시스템 |
| 2026-01-28 | M1 Round 2 완료 - SO/프리팹/씬 설정 |
| 2026-01-28 | M1 Round 3 완료 - 통합 테스트 (TASK_06) |
| 2026-01-28 | 2D → 3D 쿼터뷰 전환 완료 (스크립트 12개, 프리팹/씬 설정) |
| 2026-01-28 | M1 마무리 - 머티리얼 설정, Physics 충돌 매트릭스 설정 |
| 2026-01-28 | UI 시스템 구현 - Core/HUD/Popup 컴포넌트 12개, Canvas 구조 |
