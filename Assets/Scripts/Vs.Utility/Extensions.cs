using UnityEngine;

namespace Vs.Utility
{
    public static class VectorExtensions
    {
        public static Vector2 WithX(this Vector2 v, float x) => new(x, v.y);
        public static Vector2 WithY(this Vector2 v, float y) => new(v.x, y);

        public static Vector3 WithX(this Vector3 v, float x) => new(x, v.y, v.z);
        public static Vector3 WithY(this Vector3 v, float y) => new(v.x, y, v.z);
        public static Vector3 WithZ(this Vector3 v, float z) => new(v.x, v.y, z);

        public static Vector2 ToVector2(this Vector3 v) => new(v.x, v.y);
        public static Vector3 ToVector3(this Vector2 v, float z = 0f) => new(v.x, v.y, z);

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        public static Vector2 RandomDirection()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    public static class TransformExtensions
    {
        public static void SetPositionX(this Transform t, float x)
        {
            t.position = t.position.WithX(x);
        }

        public static void SetPositionY(this Transform t, float y)
        {
            t.position = t.position.WithY(y);
        }

        public static void SetLocalPositionX(this Transform t, float x)
        {
            t.localPosition = t.localPosition.WithX(x);
        }

        public static void SetLocalPositionY(this Transform t, float y)
        {
            t.localPosition = t.localPosition.WithY(y);
        }

        public static void DestroyChildren(this Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(t.GetChild(i).gameObject);
            }
        }
    }

    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.TryGetComponent<T>(out var component) ? component : go.AddComponent<T>();
        }
    }

    public static class CollectionExtensions
    {
        public static T RandomElement<T>(this T[] array)
        {
            if (array == null || array.Length == 0) return default;
            return array[Random.Range(0, array.Length)];
        }

        public static T RandomElement<T>(this System.Collections.Generic.IList<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }

        public static void Shuffle<T>(this System.Collections.Generic.IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
