# 멀티 세션 작업 규칙

## 파일 구조
```
.claude/tasks/
├── _active.json        # 세션 현황 + 파일 소유권
└── {task-id}.json      # 개별 작업 상세 (선택적)
```

## _active.json 구조

```json
{
  "sessions": {
    "{session-id}": {
      "area": "작업 영역명",
      "spec_doc": "스펙 문서 경로",
      "status": "in_progress | paused | completed",
      "started": "ISO 시간",
      "scope": ["작업 대상 경로/패턴"],
      "modified_files": ["실제 변경된 파일"],
      "last_updated": "ISO 시간"
    }
  },
  "file_owners": {
    "파일경로": "session-id"
  },
  "completed": []
}
```

## 작업 흐름

### 세션 시작
1. `_active.json` 확인 → 충돌 영역 체크
2. 세션 등록 (area, scope 선언)
3. 작업 시작

### 작업 중
1. 파일 수정 시 `modified_files` 업데이트
2. `file_owners`에 소유권 등록
3. 커밋 단위로 `last_updated` 갱신

### 다른 세션 시작 시
1. `file_owners` 확인 → 이미 소유된 파일은 수정 금지
2. `sessions[].scope` 확인 → 겹치는 영역 피하기
3. 불가피하게 겹치면 해당 세션과 조율 필요

### 세션 완료
1. status → "completed"
2. `completed` 배열로 이동
3. `file_owners`에서 해당 파일 제거

## 충돌 판단 기준

| 상황 | 판단 |
|------|------|
| 다른 세션의 `modified_files`에 있는 파일 수정 | 충돌 |
| 다른 세션의 `scope` 내 새 파일 생성 | 주의 (가능하면 피하기) |
| 완료된 세션의 파일 수정 | 가능 |

## 공유 파일 (항상 주의)

아래 파일은 여러 영역에서 수정 가능성 높음:
- `LocalGameServer.cs` - 핸들러 등록
- `DataManager.cs` - Database 주입
- `UserSaveData.cs` - 유저 데이터 필드
- `PROGRESS.md` - 진행 상황

→ 수정 전 반드시 `_active.json` 확인
