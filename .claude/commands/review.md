---
name: review
description: 코드 리뷰 (변경사항 분석)
---

# 리뷰 프로토콜

$ARGUMENTS 에 대해 코드 리뷰를 수행합니다.

## 실행 단계

1. **변경 파악**: git diff 또는 지정 파일 확인
2. **리뷰**: Task(code-reviewer, model=opus)로 이슈 분석
3. **보고**: high confidence 이슈만 보고

## 리뷰 기준

- 버그/로직 오류
- 보안 취약점
- 프로젝트 컨벤션 위반
- 성능 이슈

## 결과 포맷

```
## 리뷰 완료
- 검토 파일: (개수)
- 발견 이슈: (개수)

### Issues (High Confidence)
1. [파일:라인] 설명
```
