# Unity MCP UI 작업 가이드라인

Unity MCP를 사용한 UI 프리팹 작업 시 참조하는 가이드라인입니다.

## 핵심 체크리스트

UI 프리팹 인스턴스화 후 **반드시 확인**:

| 배치 | anchorMin | anchorMax | pivot | anchoredPosition |
|------|-----------|-----------|-------|------------------|
| **중앙** | (0.5, 0.5) | (0.5, 0.5) | (0.5, 0.5) | **(0, 0)** |
| 전체 화면 | (0, 0) | (1, 1) | (0.5, 0.5) | (0, 0) |
| 좌상단 | (0, 1) | (0, 1) | (0, 1) | (x, y) |
| 우하단 | (1, 0) | (1, 0) | (1, 0) | (-x, y) |

> **중요**: 중앙 배치 시 `anchoredPosition = (0, 0)` 필수!

---

## 일반적인 문제와 해결

| 증상 | 원인 | 해결 |
|------|------|------|
| 좌하단에 배치됨 | anchoredPosition이 음수 오프셋 | (0, 0)으로 수정 |
| 화면 밖에 배치됨 | 부모/자식 앵커 불일치 | 앵커 기준 통일 |
| 크기가 0 | stretch 앵커에서 sizeDelta 설정 | sizeDelta (0, 0) 사용 |
| 위치가 프리팹과 다름 | 씬 인스턴스 오버라이드 | RectTransform 재설정 |

---

## Unity MCP 작업 순서

### UI 프리팹 인스턴스화

```
1. manage_prefabs (instantiate)
   └─ 프리팹을 Canvas 하위에 인스턴스화

2. find_gameobjects (by_name)
   └─ 인스턴스 ID 획득

3. manage_components (get) - RectTransform 검증
   └─ anchoredPosition 확인
   └─ 예상값과 다르면 4단계로

4. manage_components (set_property) - 필요시 수정
   └─ anchoredPosition = {"x": 0, "y": 0}

5. manage_scene (save)
   └─ 씬 저장
```

### 검증 명령 예시

```
// RectTransform 속성 조회
manage_components:
  action: get
  target: {instanceID}
  component_type: RectTransform

// anchoredPosition 수정
manage_components:
  action: set_property
  target: {instanceID}
  component_type: RectTransform
  property: anchoredPosition
  value: {"x": 0, "y": 0}
```

---

## 앵커 프리셋 가이드

### 중앙 배치 (팝업, 모달)

```
anchorMin: (0.5, 0.5)
anchorMax: (0.5, 0.5)
pivot: (0.5, 0.5)
anchoredPosition: (0, 0)  ← 반드시 (0, 0)
sizeDelta: (width, height)
```

### 전체 화면 (오버레이)

```
anchorMin: (0, 0)
anchorMax: (1, 1)
pivot: (0.5, 0.5)
anchoredPosition: (0, 0)
sizeDelta: (0, 0)  ← stretch이므로 (0, 0)
offsetMin: (padding, padding)
offsetMax: (-padding, -padding)
```

### 모서리 배치 (HUD 요소)

```
// 좌상단
anchorMin: (0, 1), anchorMax: (0, 1), pivot: (0, 1)

// 우상단
anchorMin: (1, 1), anchorMax: (1, 1), pivot: (1, 1)

// 좌하단
anchorMin: (0, 0), anchorMax: (0, 0), pivot: (0, 0)

// 우하단
anchorMin: (1, 0), anchorMax: (1, 0), pivot: (1, 0)
```

---

## 문제 사례: DebugPanel 좌하단 배치

### 상황

- 프리팹: anchoredPosition (0, 0) ✓
- 씬 인스턴스: anchoredPosition (-960, -540) ✗

### 원인

Unity MCP로 프리팹 인스턴스화 시 위치 값이 의도치 않게 설정됨.

### 해결

```
manage_components:
  action: set_property
  target: {DebugPanel instanceID}
  component_type: RectTransform
  property: anchoredPosition
  value: {"x": 0, "y": 0}
```

### 교훈

**모든 UI 프리팹 인스턴스화 후 RectTransform 검증 필수**

---

## 참고

- PrefabBuilder 코드 작성: `Docs/Design/PREFAB_BUILDER_GUIDELINES.md`
- Unity UI 공식 문서: [RectTransform](https://docs.unity3d.com/ScriptReference/RectTransform.html)
