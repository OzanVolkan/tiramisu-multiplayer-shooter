using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class PoolingManager : SingletonManager<PoolingManager>
    {
        [System.Serializable]
        public class Pool
        {
            [SerializeField] private string _poolName;
            [SerializeField] private GameObject _prefab;
            [SerializeField] private int _initialPoolSize;

            #region Properties

            public string PoolName => _poolName;
            public GameObject Prefab => _prefab;
            public int InitialPoolSize => _initialPoolSize;

            #endregion
        }

        [SerializeField] private List<Pool> _pools;
        private Dictionary<string, Queue<GameObject>> _poolDictionary;

        private void Awake()
        {
            InitializePools();
        }

        // Havuzları başlatır ve her nesne tipi için kuyruğu oluşturur.
        private void InitializePools()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (var pool in _pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.InitialPoolSize; i++)
                {
                    GameObject obj = CreateNewObject(pool.Prefab);
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.PoolName, objectPool);
            }
        }

        public GameObject GetObject(string poolName)
        {
            if (!_poolDictionary.ContainsKey(poolName))
            {
                Debug.LogError($"'{poolName}' doesn't exist!");
                return null;
            }

            var objectPool = _poolDictionary[poolName];

            if (objectPool.Count > 0)
            {
                var obj = objectPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                // Havuz boşsa yeni bir nesne oluştur
                Pool pool = _pools.Find(p => p.PoolName == poolName);
                if (pool != null)
                {
                    var obj = CreateNewObject(pool.Prefab);
                    obj.SetActive(true);
                    return obj;
                }
                else
                {
                    Debug.LogError($"'{poolName}' doesn't exist!");
                    return null;
                }
            }
        }

        public void ReturnObject(string poolName, GameObject obj)
        {
            if (!_poolDictionary.ContainsKey(poolName))
            {
                Debug.LogError($"'{poolName}' doesn't exist!");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            _poolDictionary[poolName].Enqueue(obj);
        }

        private GameObject CreateNewObject(GameObject prefab)
        {
            var newObj = Instantiate(prefab);
            newObj.SetActive(false);
            return newObj;
        }
    }
}