# ProjectVS - 진행 상황

## 현재 단계: 프로토타입 준비

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

### 진행 중
- [ ] 코어 시스템 구현
  - [ ] 플레이어 이동
  - [ ] 무기 시스템
  - [ ] 적 스폰 시스템
  - [ ] 경험치/레벨업

### 예정
- [ ] UI 구현
- [ ] 콘텐츠 제작
  - [ ] 무기 6종
  - [ ] 적 4종
  - [ ] 스테이지 1개

---

## 마일스톤

| 단계 | 목표 | 상태 |
|------|------|------|
| M0 | 설계 완료 | **완료** |
| M1 | 프로토타입 (이동 + 공격 + 적) | 진행 중 |
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

### Assembly Definition (9개)
- `Vs.Utility`, `Vs.Core`, `Vs.Data`, `Vs.Combat`
- `Vs.Player`, `Vs.Enemy`, `Vs.Progression`, `Vs.Meta`, `Vs.UI`

---

## 변경 이력

| 날짜 | 내용 |
|------|------|
| 2026-01-27 | 프로젝트 생성, GDD 작성 |
| 2026-01-27 | 아키텍처 설계 완료, 기반 클래스 구현 |
