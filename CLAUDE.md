# ProjectVS

**뱀서라이크 서바이벌 게임** | 현대 도시 테마 | PC + 모바일

Unity 2022.x / C# / UniTask / Addressables / URP

> 끊임없이 몰려오는 적을 자동 공격으로 처치하며 생존하는 로그라이트 액션

**GDD**: `Docs/Design/GDD.md`

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

## 작업 절차

### 1단계: 계획 수립

```
요청 → 탐색 → 종속성 분석 → 작업 분해 → 문서화
```

| 단계 | 수행 | 산출물 |
|------|------|--------|
| 탐색 | `code-explorer` | 영향 범위 파악 |
| 분석 | `code-architect` | 종속성 그래프 |
| 분해 | 메인 | 독립 작업 단위 |
| 문서 | 메인 | `Docs/Design/Tasks/TASK_XX.md` |

### 2단계: 작업 문서 작성

각 독립 작업별 TASK 문서:
- 목표 / 스펙 참조
- 수정 파일 목록
- 선행 조건 (blockedBy)
- 완료 기준

### ⚠️ 작업 시작 체크리스트 (필수)

서브에이전트 실행 **전** 반드시 수행:

```
1. [ ] PROGRESS.md 마일스톤 상태 → "🟡 진행 중" 변경
2. [ ] TASK 문서 생성/갱신
3. [ ] 서브에이전트 실행
```

### 3단계: 멀티에이전트 실행

```
[독립 작업] → 병렬 실행 (Task tool)
[종속 작업] → 순차 실행 (blockedBy 해소 후)
```

**실행 방식**:
- 독립 작업: 단일 메시지에 여러 Task 호출
- 종속 작업: 선행 완료 확인 후 다음 Task 호출
- 각 에이전트: TASK 문서 기반 자율 수행

### 4단계: 통합 및 검증

1. 각 에이전트 완료 보고 수집
2. 충돌/통합 이슈 해결
3. 커밋 (필요시)

### ⚠️ 작업 완료 체크리스트 (필수)

모든 서브에이전트 완료 **후** 반드시 수행:

```
1. [ ] 서브에이전트 결과 확인
2. [ ] PROGRESS.md 마일스톤 상태 → "✅ 완료" 변경
3. [ ] PROGRESS.md 변경 이력 추가
4. [ ] TASK 문서 상태 갱신
```

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

## 참조

| 문서 | 용도 |
|------|------|
| `Docs/Design/GDD.md` | **게임 디자인** |
| `.claude/USAGE_GUIDE.md` | 사용법 가이드 |
| `Docs/PROGRESS.md` | 작업 상태 |
| `Docs/ARCHITECTURE.md` | 폴더/의존성 |
| `Docs/Design/PREFAB_BUILDER_GUIDELINES.md` | 프리팹 생성 지침 |
| `.claude/MULTI_SESSION.md` | 멀티세션 |
