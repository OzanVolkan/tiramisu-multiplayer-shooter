using System;
using System.Collections.Generic;
using System.Data;
using Photon.Pun;
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
        private Dictionary<string, Queue<GameObject>> _poolDictionary = new();

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnJoinedRoom, new Action(InitializePools));
            EventManager.AddHandler(GameEvent.OnRematch, new Action(ClearPools));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnJoinedRoom, new Action(InitializePools));
            EventManager.RemoveHandler(GameEvent.OnRematch, new Action(ClearPools));
        }

        // Havuzları başlatır ve her nesne tipi için kuyruğu oluşturur.
        private void InitializePools()
        {
            ClearPools();

            foreach (var pool in _pools)
            {
                if (_poolDictionary.ContainsKey(pool.PoolName))
                {
                    Debug.LogWarning($"Pool with name '{pool.PoolName}' already exists. Skipping.");
                    continue;
                }
                
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
                    // objectPool.Enqueue(obj);
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

            _poolDictionary[poolName].Enqueue(obj);
            obj.SetActive(false);
        }

        private GameObject CreateNewObject(GameObject prefab)
        {
            var newObj = PhotonNetwork.Instantiate(prefab.name, Vector3.up * 100f, Quaternion.identity);
            newObj.SetActive(false);
            return newObj;
        }

        private void ClearPools()
        {
            foreach (var poolName in _poolDictionary.Keys)
            {
                var objectQueue = _poolDictionary[poolName];

                while (objectQueue.Count > 0)
                {
                    var obj = objectQueue.Dequeue();
                    Destroy(obj);
                }
            }

            _poolDictionary.Clear();
        }
    }
}
