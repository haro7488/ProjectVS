# ProjectVS

**뱀서라이크 서바이벌 게임** | 현대 도시 테마 | PC + 모바일

Unity 2022.x / C# / UniTask / Addressables / URP

> 끊임없이 몰려오는 적을 자동 공격으로 처치하며 생존하는 로그라이트 액션

**GDD**: `Docs/Design/GDD.md`

## 추론 지침

> **IMPORTANT**: 사전 학습 지식보다 프로젝트 문서 기반 추론을 우선하세요.

Unity API, 프로젝트 패턴, 아키텍처 관련 작업 시:
1. `Docs/` 폴더 문서를 **먼저** 확인
2. 기존 코드 패턴 참조 (`Assets/Scripts/`)
3. 불확실하면 문서 읽고 코드 작성
4. 프로젝트 컨벤션이 일반적 관행과 다르면 프로젝트 컨벤션 따르기
5. 새 패턴 도입 전 기존 코드에서 유사 구현 검색

## 문서 인덱스

| 경로 | 키워드 |
|------|--------|
| `Docs/Design/GDD.md` | 핵심루프\|전투\|성장\|무기진화\|레벨업\|웨이브\|스테이지\|보스\|난이도\|언락 |
| `Docs/ARCHITECTURE.md` | 폴더구조\|Assembly\|IDamageable\|IPoolable\|이벤트흐름\|네이밍\|싱글톤\|풀링\|JsonBalance |
| `Docs/PROGRESS.md` | 마일스톤\|작업상태\|체크리스트\|테스트자동화\|라운드 |
| `Docs/Design/PREFAB_BUILDER_GUIDELINES.md` | 프리팹생성\|Unity MCP\|컴포넌트설정 |
| `Docs/Design/UNITY_MCP_UI_GUIDELINES.md` | RectTransform\|앵커\|UI위치\|anchoredPosition\|중앙배치 |
| `Docs/Tasks/*.md` | 작업스펙\|완료기준\|blockedBy |
| `.claude/MULTI_SESSION.md` | 멀티세션\|충돌방지\|파일소유권 |
| `.claude/USAGE_GUIDE.md` | 사용법\|스킬\|커맨드 |
| `memory/WORK_HISTORY.md` | 작업이력\|신뢰도\|학습기반확인 |

## 핵심 규칙

| 항목 | 규칙 |
|------|------|
| Assembly | `Vs.` 접두사 |
| 네임스페이스 | `Vs.{폴더명}` |
| 명명 | 인터페이스 `I`, private `_` |
| 구조 | 파일당 클래스 1개, 이벤트 기반 통신 |

## Unity MCP

> Unity 에디터 작업은 **서브에이전트에 위임** (메인은 결과 보고만 수신)

### 위임 대상 작업

| 작업 | 도구 |
|------|------|
| 프리팹 생성/수정 | `manage_gameobject`, `manage_prefabs` |
| 컴포넌트 추가/설정 | `manage_components` |
| SO 에셋 생성/수정 | `manage_scriptable_object` |
| 씬 구성 | `manage_scene`, `manage_gameobject` |
| 레이어/태그 설정 | `manage_editor` |
| 에셋 관리 | `manage_asset` |
| 플레이 테스트 | `manage_editor(action="play/stop")` |
| 콘솔 확인 | `read_console` |

### 서브에이전트 호출 방식

```
Task tool 호출:
- subagent_type: "general-purpose"
- model: "opus"
- prompt: TASK 문서 경로 + Unity MCP 사용 지시
```

### 서브에이전트 보고 형식

```markdown
## 완료 보고

### 수행 작업
- [x] 생성/수정된 에셋 목록

### 결과
- 성공/실패 여부
- 생성된 에셋 경로

### 확인 필요
- 수동 설정 항목 (충돌 매트릭스 등)
- 발견된 이슈
```

### 주의사항

- 스크립트 컴파일 완료 후 컴포넌트 추가 가능
- 프리팹 GUID는 `manage_asset(action="get_info")`로 조회
- Physics2D 충돌 매트릭스는 수동 설정 필요
- **UI 프리팹 인스턴스화 후 RectTransform 검증 필수**
  - 중앙 배치: anchoredPosition이 (0, 0)인지 확인
  - 상세: `Docs/Design/UNITY_MCP_UI_GUIDELINES.md`

## 서브에이전트 위임

> 메인 = 검토/의사결정, 서브 = 탐색/문서화/구현

| 작업 | 위임 | 모델 |
|------|------|------|
| 3+ 파일 탐색 | `Explore` | sonnet |
| 아키텍처 분석 | `code-explorer` | **opus** |
| 설계/문서 | `code-architect` | **opus** |
| 기능 구현 | `general-purpose` | **opus** |
| 코드 리뷰 | `code-reviewer` | **opus** |
| **Unity 에디터 작업** | `general-purpose` | **opus** |

**메인 직접**: 단일 파일, Serena 심볼 조회, 최종 승인

**결과 포맷**: `## 완료 보고` → 수행/결과/확인필요

**컨텍스트**: 완료 보고만 보존, 상세는 파일 경로 참조

## Skills (온디맨드)

| 스킬 | 용도 |
|------|------|
| `/explore` | 탐색 프로토콜 |
| `/implement` | 구현 프로토콜 |
| `/document` | 문서화 프로토콜 |

## Commands

| 커맨드 | 용도 |
|--------|------|
| `/analyze {대상}` | 분석 → 설계안 |
| `/impl {기능}` | 분석 → 구현 → 검증 |
| `/review {대상}` | 코드 리뷰 |
| `/unity-log` | Unity 콘솔 로그 확인 |
| `/progress` | 진행상황 확인 |

## 컨텍스트 관리

| 명령 | 시점 |
|------|------|
| `/clear` | 작업 전환 |
| `/compact` | 70% 용량 또는 20회 반복 |

## 작업 유형

| 유형 | 예시 |
|------|------|
| `bug-fix` | 버그 수정, 오류 해결 |
| `feature-add` | 새 기능, 컴포넌트 추가 |
| `refactor` | 코드 정리, 구조 개선 |
| `unity-mcp` | 프리팹, 씬, SO 작업 |
| `docs` | 문서 작성/수정 |
| `test` | 테스트 추가/수정 |

## 작업 절차

### 1단계: 복잡도 평가

| 복잡도 | 기준 | 예시 |
|--------|------|------|
| **Low** | 1-2 파일, 명확한 스펙 | 버그 수정, 단일 함수 추가 |
| **Medium** | 3-5 파일, 패턴 존재 | 새 컴포넌트, 기존 패턴 확장 |
| **High** | 6+ 파일, 탐색 필요 | 아키텍처 변경, 새 시스템 |
| **Unity MCP** | 에디터 작업 포함 | 프리팹/씬 구성 |

**평가 체크리스트**:
- [ ] 영향 파일 수 추정
- [ ] 기존 패턴/유틸리티 존재 여부
- [ ] Unity 에디터 작업 포함 여부
- [ ] 스펙 명확성 (모호하면 +1 등급)

### 2단계: 사용자 확인 (학습 기반)

`WORK_HISTORY.md`에서 작업 유형별 신뢰도 레벨 조회 후 분기:

| 레벨 | 조건 | 확인 정책 |
|------|------|-----------|
| **New** | 0회 | 항상 확인 |
| **Learning** | 1-4회 연속 성공 | 항상 확인 |
| **Trusted** | 5-9회 연속 성공 | 확인 선택적 |
| **Auto** | 10회+ 연속 성공 | 확인 생략 가능 |

**리셋**: 수정 필요 시 → Learning으로 강등

### 3단계: 계획 수립

```
요청 → 탐색 → 종속성 분석 → 작업 분해 → 문서화
```

| 단계 | 수행 | 산출물 |
|------|------|--------|
| 탐색 | `code-explorer` | 영향 범위 파악 |
| 분석 | `code-architect` | 종속성 그래프 |
| 분해 | 메인 | 독립 작업 단위 |
| 문서 | 메인 | `Docs/Design/Tasks/TASK_XX.md` |

### 4단계: 작업 시작 보고

서브에이전트 실행 **전** 사용자에게 보고:

```markdown
## 작업 시작 보고
- **작업**: {작업명}
- **유형**: {bug-fix/feature-add/...}
- **복잡도**: {Low/Medium/High/Unity MCP}
- **위임 계획**: {에이전트 수, 병렬/순차}
```

**시작 체크리스트**:
1. [ ] PROGRESS.md 마일스톤 상태 → "진행 중"
2. [ ] TASK 문서 생성/갱신
3. [ ] 시작 보고 출력

### 5단계: 멀티에이전트 실행

```
[독립 작업] → 병렬 실행 (Task tool)
[종속 작업] → 순차 실행 (blockedBy 해소 후)
```

**실행 방식**:
- 독립 작업: 단일 메시지에 여러 Task 호출
- 종속 작업: 선행 완료 확인 후 다음 Task 호출
- 각 에이전트: TASK 문서 기반 자율 수행

### 6단계: 완료 보고 + 이력 갱신

1. 각 에이전트 완료 보고 수집
2. 충돌/통합 이슈 해결
3. 커밋 (필요시)
4. `WORK_HISTORY.md` 업데이트

**완료 체크리스트**:
1. [ ] 서브에이전트 결과 확인
2. [ ] PROGRESS.md 마일스톤 상태 → "완료"
3. [ ] PROGRESS.md 변경 이력 추가
4. [ ] TASK 문서 상태 갱신
5. [ ] WORK_HISTORY.md 연속 성공 수 갱신

## 충돌 방지 (멀티에이전트)

### 3계층 분리

| 계층 | 방식 | 적용 |
|------|------|------|
| 시간 | Round 순차 | 종속 작업은 이전 Round 완료 후 |
| 공간 | 도메인 분리 | 각 에이전트 다른 폴더/파일 |
| 논리 | 파일 소유권 | `_active.json` 등록 |

### TASK 문서 필수 항목

| 항목 | 용도 |
|------|------|
| `생성 파일` | 새로 만드는 파일 (배타적 소유) |
| `수정 파일` | 기존 파일 변경 (소유권 확인 필요) |
| `blockedBy` | 선행 작업 ID |

### 공유 파일 규칙

> 여러 에이전트가 수정 가능한 파일

**규칙**: 공유 파일은 **필드/섹션 레벨**로 분할 소유

### 세션 프로토콜

```
시작: _active.json 확인 → 충돌 체크 → 등록
진행: 등록된 범위 내 작업
완료: 소유권 해제 → completed 이동
```

**상세**: `.claude/MULTI_SESSION.md` 참조

## 커밋

```
Add feature       ← 영어 (동사 원형)
- 설명 한국어     ← 본문
```
