using UnityEngine;

namespace Vs.Utility
{
    /// <summary>
    /// MonoBehaviour 싱글톤 베이스 클래스.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new();
        private static bool _isQuitting;

        public static T Instance
        {
            get
            {
                if (_isQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType<T>();

                        if (_instance == null)
                        {
                            var go = new GameObject($"[{typeof(T).Name}]");
                            _instance = go.AddComponent<T>();
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool HasInstance => _instance != null;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }

    /// <summary>
    /// 씬 전환 시에도 유지되는 싱글톤.
    /// </summary>
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            if (Instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
