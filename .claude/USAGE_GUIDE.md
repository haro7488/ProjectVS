# Claude Code 사용 가이드

## 빠른 참조

### 세션 관리
| 명령 | 시점 |
|------|------|
| `/clear` | 작업 전환, 새 기능 시작 |
| `/compact` | 70% 용량, 20회+ 대화, 긴 디버깅 후 |

### 커맨드
| 커맨드 | 용도 |
|--------|------|
| `/analyze {대상}` | 탐색 + 분석 → 문서화 |
| `/impl {기능}` | 탐색 → 설계 → 승인 → 구현 |
| `/review` | 코드 리뷰 (high confidence) |

### 모델 전략
| 작업 | 에이전트 | 모델 |
|------|----------|------|
| 탐색 | `Explore` | sonnet |
| **분석** | `code-explorer` | **opus** |
| **설계/문서** | `code-architect` | **opus** |
| **구현** | `general-purpose` | **opus** |
| **리뷰** | `code-reviewer` | **opus** |

---

## 작업 유형별 사용법

### 1. 탐색/조사
```
"인증 시스템 어디있어?"
"Gacha 관련 코드 찾아줘"
```
→ `Explore`(sonnet) 또는 `code-explorer`(**opus**) 자동 위임 → 요약 반환

### 2. 기능 구현
```
/impl 천장 기능
```
파이프라인:
1. `Explore`(sonnet) - 탐색
2. `code-architect`(**opus**) - 설계
3. **사용자 승인**
4. `general-purpose`(**opus**) - 구현
5. 빌드 검증

### 3. 코드 리뷰
```
/review
/review ShopManager.cs
```
→ `code-reviewer`(**opus**) → 이슈 보고

### 4. 분석만
```
/analyze StageSystem
```
→ `Docs/Analysis/`에 저장 → 요약 보고

### 5. 단순 작업 (직접 요청)
```
"UserSaveData.cs에 _pityCount 필드 추가해줘"
"GachaManager의 Pull 메서드 보여줘"
```
→ 서브에이전트 없이 메인이 Serena로 직접 처리

---

## 워크플로우 예시

### 새 기능 개발
```
/clear
→ "Shop 시스템 구현해줘"
→ [설계 검토 → 승인]
→ /review
→ /commit
```

### 버그 수정
```
/clear
→ "GachaManager 확률 버그 고쳐줘"
→ /review
```

### 리팩토링
```
/clear
→ /analyze AuthSystem
→ "분석 기반으로 리팩토링해줘"
→ [설계 승인]
→ 구현
```

---

## 최적화 팁

### Do
| 상황 | 방법 |
|------|------|
| 기능 전환 | `/clear` 후 시작 |
| 구체적 요청 | "ShopManager의 Purchase에 에러 처리 추가" |
| 복잡한 기능 | `/impl` 사용 (설계 승인 단계) |
| 긴 세션 | `/compact` 로 정리 |

### Don't
| 피할 것 | 이유 |
|---------|------|
| "코드 개선해줘" | 모호 → 과도한 탐색 |
| 한 세션에 여러 기능 | 컨텍스트 오염 |
| 설계 없이 큰 기능 구현 | 재작업 위험 |
| `/clear` 없이 전환 | 불필요한 컨텍스트 누적 |

---

## 비용 절감 체크리스트

- [ ] 작업 전환 시 `/clear` 사용
- [ ] 단순 작업은 직접 요청 (서브에이전트 오버헤드 제거)
- [ ] 구체적으로 요청 (불필요한 탐색 방지)
- [ ] 설계 승인 후 구현 (재작업 방지)
- [ ] 20회 대화 후 `/compact` 또는 `/clear`

---

## 파일 구조

```
.claude/
├── USAGE_GUIDE.md          # 이 문서
├── MULTI_SESSION.md        # 멀티세션 규칙
├── settings.json           # hooks
├── skills/
│   ├── explore/SKILL.md    # 탐색 프로토콜
│   ├── implement/SKILL.md  # 구현 프로토콜
│   └── document/SKILL.md   # 문서화 프로토콜
└── commands/
    ├── analyze.md          # /analyze
    ├── impl.md             # /impl
    └── review.md           # /review
```
