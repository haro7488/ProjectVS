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

        /// <summary>
        /// Enter Play Mode Options로 Domain Reload가 비활성화된 경우
        /// static 변수 초기화를 위해 필요.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFields()
        {
            _instance = null;
            _isQuitting = false;
        }

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    // 기존 인스턴스가 유효하면 반환
                    if (_instance != null)
                    {
                        return _instance;
                    }

                    // 씬에서 인스턴스 찾기
                    _instance = FindFirstObjectByType<T>();

                    if (_instance != null)
                    {
                        // 인스턴스를 찾았으면 이전 세션의 _isQuitting 상태 리셋
                        _isQuitting = false;
                        return _instance;
                    }

                    // 종료 중이면 새로 생성하지 않음
                    if (_isQuitting)
                    {
                        return null;
                    }

                    // 새로 생성
                    var go = new GameObject($"[{typeof(T).Name}]");
                    _instance = go.AddComponent<T>();
                    return _instance;
                }
            }
        }

        public static bool HasInstance => _instance != null && !_isQuitting;

        protected virtual void Awake()
        {
            // 에디터 Play 모드 재시작 시 이전 세션의 _isQuitting 상태 리셋
            _isQuitting = false;

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