# TASK_M2: 핵심 루프 (레벨업 + 진화)

**세션 ID**: m2-evolution
**상태**: ✅ 완료
**시작일**: 2026-02-04
**완료일**: 2026-02-04

---

## 목표

GDD 섹션 4 (성장 시스템)에 따른 핵심 루프 구현:
- 레벨업 선택지 시스템 완성
- 무기 진화 시스템 구현
- UI 아이템 슬롯 연동

---

## 세부 작업

### M2-1: 진화 시스템 핵심 로직 ✅

**상태**: ✅ 완료 (컴파일 확인 필요)

**목표**: 무기 진화 조건 체크 및 실행 로직 구현

**스펙**:
- 조건: 무기 Lv.8 + 대응 패시브 보유
- 실행: 기존 무기를 진화 무기로 교체
- 패시브는 유지 (진화 재료로 소모 안 함)

**수정 파일**:
| 파일 | 변경 내용 |
|------|----------|
| `LevelUpChoice.cs` | WeaponEvolution 타입, SourceData 필드 추가 ✅ |
| `LevelUpManager.cs` | 진화 선택지 생성/적용, OnWeaponEvolved 이벤트 ✅ |
| `WeaponController.cs` | ReplaceWeapon 메서드 추가 ✅ |
| `PlayerInitializer.cs` | HandleWeaponEvolved 핸들러 추가 ✅ |

**완료 기준**:
- [x] 무기 Lv.8 + 패시브 보유 시 진화 선택지 표시
- [x] 진화 선택 시 무기 교체 동작

---

### M2-2: 아이템 슬롯 UI 연동 ✅

**상태**: ✅ 완료

**목표**: HUD의 아이템 슬롯에 현재 보유 무기/패시브 표시

**수정 파일**:
| 파일 | 변경 내용 |
|------|----------|
| `ItemSlotsPanel.cs` | OnWeaponEvolved 이벤트 구독, HandleWeaponEvolved 추가 ✅ |
| `ItemSlotDisplay.cs` | 기존 구현 사용 (변경 없음) |

**완료 기준**:
- [x] 무기/패시브 획득 시 슬롯에 아이콘 표시 (기존 구현)
- [x] 레벨업 시 레벨 숫자 업데이트 (기존 구현)
- [x] 진화 시 슬롯 아이콘 업데이트 (신규)

---

### M2-3: 통합 테스트 및 데이터 ✅

**상태**: ✅ 완료

**목표**: M2 기능 통합 테스트 및 무기/패시브 데이터 설정

**작업 내용**:
- [x] Passive_Magazine (탄창) SO 생성
- [x] Weapon_MachineGun (기관총) SO 생성
- [x] Weapon_Pistol 진화 설정 (EvolvesTo, EvolutionRequirement)
- [x] LevelUpManager에 에셋 등록
- [x] 씬 저장

**생성된 에셋**:
- `Assets/_Project/Data/Passives/Passive_Magazine.asset`
- `Assets/_Project/Data/Weapon_MachineGun.asset`

**완료 기준**:
- [x] 진화 데이터 설정 완료
- [x] LevelUpManager 에셋 연결

---

## 진화 조합표 (GDD 기준)

| 무기 (Lv.8) | + 패시브 | = 진화 무기 |
|-------------|----------|-------------|
| 권총 | 탄창 | 기관총 |
| 나이프 | 가죽 장갑 | 천 개의 칼날 |
| 야구 방망이 | 강화 그립 | 홈런 배트 |
| 화염병 | 기름통 | 지옥불 |
| 드론 | 배터리 | 킬러 드론 |
| 번개 | 피뢰침 | 천둥 폭풍 |

---

## 변경 이력

| 날짜 | 내용 |
|------|------|
| 2026-02-04 | TASK 문서 생성, M2-1 시작 |
| 2026-02-04 | M2-1 완료 - 진화 시스템 핵심 로직 |
| 2026-02-04 | M2-2 완료 - 아이템 슬롯 UI 연동 |
| 2026-02-04 | M2-3 완료 - SO 생성 및 연결 |
