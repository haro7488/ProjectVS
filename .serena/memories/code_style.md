# 코드 스타일 및 컨벤션

## C# 코딩 컨벤션
- **네이밍**:
  - 클래스/메서드: PascalCase
  - 변수/파라미터: camelCase
  - private 필드: _camelCase 또는 camelCase
  - 상수: UPPER_SNAKE_CASE 또는 PascalCase

## Unity 스크립트 패턴
- MonoBehaviour 상속 클래스 사용
- Start(), Update() 등 Unity 라이프사이클 메서드 활용
- public 필드로 Inspector 노출

## 파일 구성
- 스크립트: Assets/Scripts/ 하위
- 씬: Assets/Scenes/ 하위
- 각 에셋에 .meta 파일 동반

## 기본 템플릿
```csharp
using UnityEngine;

public class ClassName : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }
}
```
