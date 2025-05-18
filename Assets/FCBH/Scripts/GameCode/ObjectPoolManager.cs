using System.Collections.Generic;
using UnityEngine;

namespace FCBH
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private static ObjectPoolManager _instance;
        public static ObjectPoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ObjectPoolManager");
                    _instance = obj.AddComponent<ObjectPoolManager>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();
        private Dictionary<GameObject, string> _activeObjectsPool = new Dictionary<GameObject, string>();
        private Transform _poolContainer;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            _poolContainer = new GameObject("PoolContainer").transform;
            _poolContainer.parent = transform;
        }

        public void CreatePool(string poolTag, GameObject prefab, int initialSize = 10)
        {
            if (_poolDictionary.ContainsKey(poolTag))
            {
                Debug.LogWarning($"Pool with tag {poolTag} already exists!");
                return;
            }

            _prefabDictionary[poolTag] = prefab;
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            Transform poolGroupTransform = new GameObject(poolTag + "Pool").transform;
            poolGroupTransform.parent = _poolContainer;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, poolGroupTransform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary[poolTag] = objectPool;
        }

        public GameObject GetObjectFromPool(string poolTag, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(poolTag))
            {
                Debug.LogWarning($"Pool with tag {poolTag} doesn't exist! Creating new pool.");
                
                if (!_prefabDictionary.ContainsKey(poolTag))
                {
                    Debug.LogError($"No prefab registered for pool {poolTag}!");
                    return null;
                }
                
                CreatePool(poolTag, _prefabDictionary[poolTag]);
            }

            // If pool is empty, expand it
            if (_poolDictionary[poolTag].Count == 0)
            {
                GameObject newObj = Instantiate(_prefabDictionary[poolTag], 
                    _poolContainer.Find(poolTag + "Pool"));
                newObj.SetActive(false);
                _poolDictionary[poolTag].Enqueue(newObj);
            }

            // Get object from pool
            GameObject obj = _poolDictionary[poolTag].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            
            // Track this object
            _activeObjectsPool[obj] = poolTag;

            return obj;
        }

        public void ReturnObjectToPool(GameObject obj)
        {
            if (_activeObjectsPool.TryGetValue(obj, out string poolTag))
            {
                ReturnObjectToPool(obj, poolTag);
            }
            else
            {
                Debug.LogWarning("Trying to return an object that is not tracked by the pool manager!");
                Destroy(obj);
            }
        }

        public void ReturnObjectToPool(GameObject obj, string poolTag)
        {
            if (!_poolDictionary.ContainsKey(poolTag))
            {
                Debug.LogWarning($"Pool with tag {poolTag} doesn't exist! Destroying object instead.");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            _poolDictionary[poolTag].Enqueue(obj);
            
            // Remove from active tracking
            if (_activeObjectsPool.ContainsKey(obj))
            {
                _activeObjectsPool.Remove(obj);
            }
        }
        
        public void ClearPool(string poolTag)
        {
            if (!_poolDictionary.ContainsKey(poolTag))
                return;
                
            // First, identify all active objects from this pool and deactivate them
            List<GameObject> objectsToReturn = new List<GameObject>();
            foreach (var pair in _activeObjectsPool)
            {
                if (pair.Value == poolTag)
                {
                    objectsToReturn.Add(pair.Key);
                }
            }
            
            // Return all objects to pool
            foreach (var obj in objectsToReturn)
            {
                ReturnObjectToPool(obj, poolTag);
            }
        }
    }
}