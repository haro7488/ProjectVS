# PrefabBuilder 작성 가이드라인

## 개요

이 문서는 `*PrefabBuilder.cs` 파일 작성 시 따라야 할 가이드라인을 정의합니다.

## RectTransform 추가 시 주의사항

### 문제: Unity Object와 `??` 연산자

Unity의 `==` null 체크는 C#의 기본 null 체크와 다르게 동작합니다:
- Unity는 destroyed 오브젝트도 null로 취급 (fake null)
- `??` 연산자는 C#의 reference null만 체크

**안티패턴** (사용 금지):
```csharp
// GetComponent가 "Unity null"을 반환해도 ??가 이를 null로 인식하지 못함
var rect = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
```

**올바른 패턴**:
```csharp
// 명시적 if 체크 사용
var rect = go.GetComponent<RectTransform>();
if (rect == null)
    rect = go.AddComponent<RectTransform>();
```

### 권장: 헬퍼 메서드 사용

```csharp
private static RectTransform AddRectTransform(GameObject go)
{
    var rect = go.GetComponent<RectTransform>();
    if (rect == null)
        rect = go.AddComponent<RectTransform>();
    return rect;
}

// 사용
var rect = AddRectTransform(header);
rect.anchorMin = new Vector2(0f, 1f);
```

## 표준 헬퍼 메서드

### CreateChild
```csharp
private static GameObject CreateChild(GameObject parent, string name)
{
    var child = new GameObject(name);
    child.transform.SetParent(parent.transform, false);
    return child;
}
```

### SetStretch
```csharp
private static RectTransform SetStretch(GameObject go)
{
    var rect = AddRectTransform(go);
    rect.anchorMin = Vector2.zero;
    rect.anchorMax = Vector2.one;
    rect.offsetMin = Vector2.zero;
    rect.offsetMax = Vector2.zero;
    return rect;
}
```

### Anchor 헬퍼
```csharp
private static void SetAnchorTopLeft(RectTransform rect, Vector2 size, Vector2 position);
private static void SetAnchorTopRight(RectTransform rect, Vector2 size, Vector2 position);
private static void SetAnchorBottomRight(RectTransform rect, Vector2 size, Vector2 position);
```

## 파일 구조

```csharp
public static class {ScreenName}PrefabBuilder
{
    #region Colors (Theme)
    // 색상 상수 정의
    #endregion

    #region Constants
    // 크기/레이아웃 상수
    #endregion

    #region Config Data
    // 버튼/위젯 설정 배열
    #endregion

    #region Font Helper
    private static void ApplyFont(TextMeshProUGUI tmp);
    #endregion

    // Entry Point
    public static GameObject Build() { }

    #region Create Methods
    // 각 영역별 Create 메서드
    #endregion

    #region SerializeField Connections
    // 필드 연결 메서드
    #endregion

    #region Helpers
    // CreateChild, SetStretch, AddRectTransform 등
    #endregion
}
```

## SerializeField 연결

```csharp
private static void ConnectSerializedFields(GameObject root, ...)
{
    var screen = root.GetComponent<ScreenType>();
    if (screen == null) return;

    var so = new SerializedObject(screen);

    // 단일 필드
    so.FindProperty("_fieldName").objectReferenceValue = component;

    // 배열 필드
    var arrayProp = so.FindProperty("_arrayField");
    arrayProp.arraySize = items.Length;
    for (int i = 0; i < items.Length; i++)
    {
        arrayProp.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
    }

    // string 필드
    so.FindProperty("_stringField").stringValue = "value";

    so.ApplyModifiedPropertiesWithoutUndo();
}
```
