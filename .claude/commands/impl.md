---
name: impl
description: 기능 구현 (분석 → 설계 → 구현 → 검증)
---

# 구현 프로토콜

$ARGUMENTS 기능을 구현합니다.

## 실행 단계

1. **분석**: Task(Explore)로 관련 코드 파악
2. **설계**: Task(code-architect, model=opus)로 구현 청사진 작성
3. **승인**: 메인이 설계안 검토 후 사용자 확인
4. **구현**: Task(general-purpose)로 코드 작성
5. **검증**: 빌드 확인

## 필수 참조

- 스펙: `Docs/Specs/{Assembly}.md`
- 컨벤션: `Docs/CONVENTIONS.md`
- 유사 코드: 기존 패턴 검색

## 결과 포맷

```
## 구현 완료: {기능}
- 생성/수정: (파일 목록)
- 빌드: 성공/실패
- 확인 필요: (있으면 기술)
```
