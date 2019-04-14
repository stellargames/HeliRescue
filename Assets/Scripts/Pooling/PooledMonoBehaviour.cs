using System;
using System.Collections;
using UnityEngine;

namespace Pooling
{
    public class PooledMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private int initialPoolSize = 5;

        // ReSharper disable once ConvertToAutoProperty
        public int InitialPoolSize => initialPoolSize;

        public event Action<PooledMonoBehaviour> Expired;

        private T Get<T>(bool enable = true) where T : PooledMonoBehaviour
        {
            var pool = Pool.GetPool(this);
            var pooledObject = pool.Get<T>();
            if (enable) pooledObject.gameObject.SetActive(true);

            return pooledObject;
        }

        public T Get<T>(Vector3 position, Quaternion rotation)
            where T : PooledMonoBehaviour
        {
            var pooledObject = Get<T>();
            var objectTransform = pooledObject.transform;
            objectTransform.position = position;
            objectTransform.rotation = rotation;

            return pooledObject;
        }

        protected virtual void OnDisable()
        {
            Expired?.Invoke(this);
        }

        public void ReturnToPool(float delay = 0f)
        {
            StartCoroutine(ReturnToPoolAfterSeconds(delay));
        }

        private IEnumerator ReturnToPoolAfterSeconds(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
