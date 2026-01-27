---
name: analyze
description: 기능 분석 후 설계안 작성
---

# 분석 프로토콜

$ARGUMENTS 에 대해 분석을 수행합니다.

## 실행 단계

1. **탐색**: Task(Explore)로 관련 파일 및 구조 파악
2. **분석**: Task(code-explorer)로 의존성 및 패턴 분석
3. **문서화**: 결과를 `Docs/Analysis/` 에 저장
4. **보고**: 핵심 요약만 메인 컨텍스트에 보고

## 결과 포맷

```
## 분석 완료: {대상}
- 핵심 파일: (3개 이내)
- 주요 패턴: (bullet 3개)
- 권장 접근: (1줄)
- 상세: Docs/Analysis/{feature}.md
```
