# TASK_M3: 콘텐츠 제작 (M2 독립 작업)

**세션 ID**: m3-content
**상태**: ✅ 완료
**시작일**: 2026-02-04
**완료일**: 2026-02-04

---

## 목표

GDD 섹션 5-7에 따른 콘텐츠 제작 (M2 진화 시스템과 독립적인 작업)

---

## 세부 작업

### M3-a: 적 4종 구현 ✅

**상태**: ✅ 완료
**의존성**: 없음 (M2 독립)
**완료일**: 2026-02-04

**목표**: GDD 7.1 도시 외곽 적 구성에 따른 적 타입 추가

**스펙** (GDD 기준):
| 적 | 출현 시간 | 특징 |
|----|----------|------|
| 좀비 (기존) | 0~3분 | 기본, 느림 |
| 뛰는 좀비 | 3~7분 | 빠름, 약함 |
| 대형 좀비 | 7~12분 | 느림, 강함, 체력↑ |
| 갱단원 | 12~17분 | 원거리 공격 |
| 변이체 (보스) | 17분 | 돌진, 범위 공격, 소환 |

**생성 파일**:
| 파일 | 경로 |
|------|------|
| `Enemy_FastZombie.asset` | `Assets/_Project/Data/Enemy_FastZombie.asset` |
| `Enemy_BigZombie.asset` | `Assets/_Project/Data/Enemy_BigZombie.asset` |
| `Enemy_Gangster.asset` | `Assets/_Project/Data/Enemy_Gangster.asset` |
| `Enemy_Boss.asset` | `Assets/_Project/Data/Enemy_Boss.asset` |
| `Enemy_FastZombie.prefab` | `Assets/_Project/Prefabs/Enemy_FastZombie.prefab` |
| `Enemy_BigZombie.prefab` | `Assets/_Project/Prefabs/Enemy_BigZombie.prefab` |
| `Enemy_Gangster.prefab` | `Assets/_Project/Prefabs/Enemy_Gangster.prefab` |
| `Enemy_Boss.prefab` | `Assets/_Project/Prefabs/Enemy_Boss.prefab` |
| `FastZombieGreen.mat` | `Assets/_Project/Materials/FastZombieGreen.mat` |
| `BigZombiePurple.mat` | `Assets/_Project/Materials/BigZombiePurple.mat` |
| `GangsterOrange.mat` | `Assets/_Project/Materials/GangsterOrange.mat` |
| `BossDarkRed.mat` | `Assets/_Project/Materials/BossDarkRed.mat` |

**적 스탯**:
| 적 | 체력 | 속도 | 데미지 | 행동 |
|----|------|------|--------|------|
| FastZombie | 5 | 4 | 8 | ChasePlayer |
| BigZombie | 50 | 1.2 | 25 | ChasePlayer |
| Gangster | 20 | 2.5 | 15 | Ranged |
| Boss | 100 | 2 | 30 | Boss |

**완료 기준**:
- [x] 4종 EnemyData SO 생성
- [x] 4종 프리팹 생성 (3D 컴포넌트)
- [ ] EnemySpawner에서 스폰 가능 (M3-c에서 처리)

---

### M3-b: 기본 무기 5종 구현 ✅

**상태**: ✅ 완료
**의존성**: 없음 (M2 독립)
**완료일**: 2026-02-04

**목표**: GDD 5.1 기본 무기 중 권총 외 5종 구현

**스펙** (GDD 기준):
| 무기 | 타입 | 공격 방식 |
|------|------|----------|
| 나이프 | 투사체 | 이동 방향으로 발사 |
| 야구 방망이 | 근접 | 전방 휘두르기 (MeleeWeapon) |
| 화염병 | 범위 | 지면에 화염 지대 생성 (AreaWeapon) |
| 드론 | 궤도 | 플레이어 주변 회전 (OrbitWeapon) |
| 번개 | 마법 | 랜덤 적에게 낙뢰 (LightningWeapon) |

**생성 파일**:
| 파일 | 경로 |
|------|------|
| `MeleeWeapon.cs` | `Assets/Scripts/Vs.Combat/Weapons/MeleeWeapon.cs` |
| `AreaWeapon.cs` | `Assets/Scripts/Vs.Combat/Weapons/AreaWeapon.cs` |
| `OrbitWeapon.cs` | `Assets/Scripts/Vs.Combat/Weapons/OrbitWeapon.cs` |
| `LightningWeapon.cs` | `Assets/Scripts/Vs.Combat/Weapons/LightningWeapon.cs` |
| `FireZone.cs` | `Assets/Scripts/Vs.Combat/Weapons/FireZone.cs` |
| `Orbiter.cs` | `Assets/Scripts/Vs.Combat/Weapons/Orbiter.cs` |
| `Weapon_Knife.asset` | `Assets/_Project/Data/Weapon_Knife.asset` |
| `Weapon_Bat.asset` | `Assets/_Project/Data/Weapon_Bat.asset` |
| `Weapon_Molotov.asset` | `Assets/_Project/Data/Weapon_Molotov.asset` |
| `Weapon_Drone.asset` | `Assets/_Project/Data/Weapon_Drone.asset` |
| `Weapon_Lightning.asset` | `Assets/_Project/Data/Weapon_Lightning.asset` |
| `Weapon_Knife.prefab` | `Assets/_Project/Prefabs/Weapons/Weapon_Knife.prefab` |
| `Weapon_Bat.prefab` | `Assets/_Project/Prefabs/Weapons/Weapon_Bat.prefab` |
| `Weapon_Molotov.prefab` | `Assets/_Project/Prefabs/Weapons/Weapon_Molotov.prefab` |
| `Weapon_Drone.prefab` | `Assets/_Project/Prefabs/Weapons/Weapon_Drone.prefab` |
| `Weapon_Lightning.prefab` | `Assets/_Project/Prefabs/Weapons/Weapon_Lightning.prefab` |
| `Projectile_Knife.prefab` | `Assets/_Project/Prefabs/Projectiles/Projectile_Knife.prefab` |
| `FireZone.prefab` | `Assets/_Project/Prefabs/Effects/FireZone.prefab` |
| `Orbiter_Drone.prefab` | `Assets/_Project/Prefabs/Effects/Orbiter_Drone.prefab` |
| `LightningEffect.prefab` | `Assets/_Project/Prefabs/Effects/LightningEffect.prefab` |
| `weapons.json` | `Assets/StreamingAssets/Balance/weapons.json` (밸런스 데이터 추가) |

**완료 기준**:
- [x] 4종 무기 클래스 구현 (Melee, Area, Orbit, Lightning)
- [x] 5종 WeaponData SO 생성
- [x] 레벨업 선택지에서 선택 가능 (WeaponPool 연동 완료)

**설정 완료**:
- [x] WeaponData SO에 WeaponPrefab, ProjectilePrefab 참조 연결
- [x] 레벨업 시스템의 WeaponPool에 무기 7종 등록
- [x] Collider Trigger 설정 (FireZone, Orbiter_Drone, Projectile_Knife)

---

### M3-c: 스테이지 웨이브 구성 ✅

**상태**: ✅ 완료
**의존성**: M3-a 적 구현 완료 후
**완료일**: 2026-02-04

**목표**: GDD 7.1 도시 외곽 웨이브 구성

**스펙**:
- StageData에 웨이브별 적 스폰 설정
- EnemySpawner 시간 기반 웨이브 로직

**웨이브 구성** (GDD 기준):
| 웨이브 | 적 | 시작~종료 | 간격 | 성장률 |
|--------|-----|---------|------|--------|
| 1 | 좀비 | 0~20분 | 2초 | 0.1 |
| 2 | 뛰는 좀비 | 3~20분 | 3초 | 0.15 |
| 3 | 대형 좀비 | 7~20분 | 5초 | 0.08 |
| 4 | 갱단원 | 12~20분 | 4초 | 0.1 |

**보스**: 17분(1020초)에 변이체 스폰

**완료 기준**:
- [x] Stage_City 웨이브 데이터 설정
- [x] 17분 보스 스폰 동작

---

## 제외 (M2 완료 후)

- 진화 무기 6종 (기관총, 천 개의 칼날, 홈런 배트, 지옥불, 킬러 드론, 천둥 폭풍)
- 패시브 아이템 6종 (탄창, 가죽 장갑, 강화 그립, 기름통, 배터리, 피뢰침)

---

## 변경 이력

| 날짜 | 내용 |
|------|------|
| 2026-02-04 | TASK 문서 생성, M3-a/b 병렬 시작 |
| 2026-02-04 | M3-a 적 4종 구현 완료 (SO, 프리팹, 머티리얼) |
| 2026-02-04 | M3-b 무기 5종 구현 완료 (클래스, SO, 프리팹) |
| 2026-02-04 | M3-c 스테이지 웨이브 구성 완료 |
| 2026-02-04 | 프리팹 참조 연결, WeaponPool 등록, Trigger 설정 완료 |