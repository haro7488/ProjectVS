# Unity Editor Log 확인

Unity 에디터의 최근 로그를 확인하고 분석합니다.

## 사용법

```
/unity-log [필터] [라인수]
```

- `필터`: 검색할 키워드 (예: Error, LobbyScreen, TabGroup)
- `라인수`: 확인할 라인 수 (기본: 100)

## 실행 절차

1. **플랫폼 감지 및 로그 파일 경로 결정**
   - Windows: `%LOCALAPPDATA%\Unity\Editor\Editor.log`
   - Mac: `~/Library/Logs/Unity/Editor.log`

2. **로그 읽기 명령 실행**

### Windows
```bash
powershell -Command "Get-Content \"$env:LOCALAPPDATA\Unity\Editor\Editor.log\" -Tail $ARG2 | Select-String -Pattern '$ARG1' | Select-Object -Last 50"
```

### Mac
```bash
grep -E "$ARG1" ~/Library/Logs/Unity/Editor.log | tail -$ARG2
```

3. **필터 없이 최근 로그만 확인할 경우**

### Windows
```bash
powershell -Command "Get-Content \"$env:LOCALAPPDATA\Unity\Editor\Editor.log\" -Tail $ARG2"
```

### Mac
```bash
tail -$ARG2 ~/Library/Logs/Unity/Editor.log
```

## 자주 사용하는 필터 예시

| 명령 | 설명 |
|------|------|
| `/unity-log Error` | 에러 로그만 |
| `/unity-log Exception` | 예외 로그만 |
| `/unity-log LobbyScreen` | LobbyScreen 관련 로그 |
| `/unity-log "Error\|Warning"` | 에러와 경고 |
| `/unity-log` | 최근 100줄 전체 |

## 출력 형식

로그 분석 후 다음 형식으로 보고:

```
## Unity Editor Log 분석

**필터**: {사용된 필터}
**확인 라인**: {라인 수}

### 주요 로그
- [시간] 로그 내용...

### 발견된 문제
- 에러/경고 요약

### 권장 조치
- 문제 해결을 위한 제안
```
