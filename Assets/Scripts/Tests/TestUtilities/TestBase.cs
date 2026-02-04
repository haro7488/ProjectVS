using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Vs.Tests.Utilities
{
    /// <summary>
    /// 모든 테스트의 기반 클래스
    /// SetUp/TearDown에서 생성된 오브젝트 자동 정리
    /// </summary>
    public abstract class TestBase
    {
        protected List<GameObject> CreatedObjects { get; } = new();
        protected List<ScriptableObject> CreatedScriptableObjects { get; } = new();

        [SetUp]
        public virtual void SetUp()
        {
            CreatedObjects.Clear();
            CreatedScriptableObjects.Clear();
        }

        [TearDown]
        public virtual void TearDown()
        {
            // 생성된 GameObject 정리
            foreach (var obj in CreatedObjects)
            {
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }
            CreatedObjects.Clear();

            // 생성된 ScriptableObject 정리
            foreach (var so in CreatedScriptableObjects)
            {
                if (so != null)
                {
                    Object.DestroyImmediate(so);
                }
            }
            CreatedScriptableObjects.Clear();
        }

        /// <summary>
        /// 컴포넌트를 가진 새 GameObject 생성
        /// </summary>
        protected T CreateComponent<T>(string name = null) where T : Component
        {
            var go = new GameObject(name ?? typeof(T).Name);
            CreatedObjects.Add(go);
            return go.AddComponent<T>();
        }

        /// <summary>
        /// 여러 컴포넌트를 가진 새 GameObject 생성
        /// </summary>
        protected GameObject CreateGameObject(string name, params System.Type[] components)
        {
            var go = new GameObject(name, components);
            CreatedObjects.Add(go);
            return go;
        }

        /// <summary>
        /// 빈 GameObject 생성
        /// </summary>
        protected GameObject CreateEmptyGameObject(string name = "TestObject")
        {
            var go = new GameObject(name);
            CreatedObjects.Add(go);
            return go;
        }

        /// <summary>
        /// ScriptableObject 인스턴스 생성
        /// </summary>
        protected T CreateScriptableObject<T>() where T : ScriptableObject
        {
            var so = ScriptableObject.CreateInstance<T>();
            CreatedScriptableObjects.Add(so);
            return so;
        }

        /// <summary>
        /// 지정된 위치에 GameObject 생성
        /// </summary>
        protected GameObject CreateGameObjectAtPosition(string name, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            CreatedObjects.Add(go);
            return go;
        }

        /// <summary>
        /// 태그가 있는 GameObject 생성
        /// </summary>
        protected GameObject CreateGameObjectWithTag(string name, string tag)
        {
            var go = new GameObject(name);
            go.tag = tag;
            CreatedObjects.Add(go);
            return go;
        }

        /// <summary>
        /// 레이어가 있는 GameObject 생성
        /// </summary>
        protected GameObject CreateGameObjectWithLayer(string name, int layer)
        {
            var go = new GameObject(name);
            go.layer = layer;
            CreatedObjects.Add(go);
            return go;
        }
    }
}
