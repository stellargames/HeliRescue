using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class Pool : MonoBehaviour
    {
        private static readonly Dictionary<PooledMonoBehaviour, Pool> Pools =
            new Dictionary<PooledMonoBehaviour, Pool>();

        private readonly Queue<PooledMonoBehaviour> _objects =
            new Queue<PooledMonoBehaviour>();

        private PooledMonoBehaviour _prefab;

        private void OnDestroy()
        {
            Pools.Clear();
        }

        public static Pool GetPool(PooledMonoBehaviour prefab)
        {
            if (Pools.ContainsKey(prefab)) return Pools[prefab];

            var pool = new GameObject("Pool-" + prefab.name).AddComponent<Pool>();
            pool._prefab = prefab;
            Pools.Add(prefab, pool);

            return pool;
        }

        public T Get<T>() where T : PooledMonoBehaviour
        {
            if (_objects.Count == 0) GrowPool();

            var pooledObject = _objects.Dequeue();
            return pooledObject as T;
        }

        private void GrowPool()
        {
            for (var i = 0; i < _prefab.InitialPoolSize; i++)
            {
                var pooledObject = Instantiate(_prefab, transform);
                pooledObject.gameObject.name += " " + i;
                pooledObject.Expired += AddObjectToAvailableQueue;

                pooledObject.gameObject.SetActive(false);
            }
        }

        private void AddObjectToAvailableQueue(PooledMonoBehaviour pooledObject)
        {
            pooledObject.transform.SetParent(transform);
            _objects.Enqueue(pooledObject);
        }
    }
}
