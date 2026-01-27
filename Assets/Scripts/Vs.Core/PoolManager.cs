using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Vs.Utility;

namespace Vs.Core
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }

    public class PoolManager : Singleton<PoolManager>
    {
        private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToKey = new();

        [Header("Settings")]
        [SerializeField] private int _defaultCapacity = Constants.DefaultPoolSize;
        [SerializeField] private int _maxSize = Constants.MaxPoolSize;

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = CreatePool(prefab);
                _pools[prefab] = pool;
            }

            var instance = pool.Get();
            var t = instance.transform;
            t.SetParent(parent);
            t.position = position;
            t.rotation = rotation;

            _instanceToKey[instance] = prefab;

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnSpawn();
            }

            return instance;
        }

        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var instance = Spawn(prefab.gameObject, position, rotation, parent);
            return instance.GetComponent<T>();
        }

        public void Despawn(GameObject instance)
        {
            if (!_instanceToKey.TryGetValue(instance, out var prefab))
            {
                Debug.LogWarning($"[PoolManager] Instance not found in pool: {instance.name}");
                Destroy(instance);
                return;
            }

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnDespawn();
            }

            if (_pools.TryGetValue(prefab, out var pool))
            {
                pool.Release(instance);
            }

            _instanceToKey.Remove(instance);
        }

        public void Despawn(GameObject instance, float delay)
        {
            StartCoroutine(DespawnDelayed(instance, delay));
        }

        private System.Collections.IEnumerator DespawnDelayed(GameObject instance, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (instance != null)
            {
                Despawn(instance);
            }
        }

        public void ClearPool(GameObject prefab)
        {
            if (_pools.TryGetValue(prefab, out var pool))
            {
                pool.Clear();
                _pools.Remove(prefab);
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
            _instanceToKey.Clear();
        }

        private ObjectPool<GameObject> CreatePool(GameObject prefab)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var instance = Instantiate(prefab);
                    instance.name = prefab.name;
                    return instance;
                },
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: false,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );
        }
    }
}
