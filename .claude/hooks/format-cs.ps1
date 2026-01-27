# Claude Code Hook: C# 파일 자동 포맷팅
# PostToolUse (Edit|Write) 후 실행

$ErrorActionPreference = "SilentlyContinue"

# stdin에서 JSON 읽기
$json = $input | Out-String
if (-not $json) { exit 0 }

try {
    $data = $json | ConvertFrom-Json
    $filePath = $data.tool_input.file_path

    # .cs 파일만 처리
    if ($filePath -and $filePath -match '\.cs$') {
        # 프로젝트 루트 찾기 (스크립트 위치 기준)
        $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
        $projectRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)

        $settingsFile = Join-Path $projectRoot "*.DotSettings"

        # DotSettings 파일 찾기
        $settingsPath = Get-ChildItem $settingsFile -ErrorAction SilentlyContinue | Select-Object -First 1

        if ($settingsPath) {
            & jb cleanupcode --profile="Built-in: Reformat Code" --settings="$($settingsPath.FullName)" "$filePath" 2>$null
        } else {
            # DotSettings 없으면 기본 프로필로 실행
            & jb cleanupcode --profile="Built-in: Reformat Code" "$filePath" 2>$null
        }
    }
} catch {
    # 오류 무시 (훅 실패가 작업을 중단시키지 않도록)
    exit 0
}
