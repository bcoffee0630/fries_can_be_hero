using System.Collections;
using UnityEngine;

namespace FCBH
{
    public class EnemyCreator : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private int initialPoolSize = 20;
        
        private bool _processing;
        private const string ENEMY_POOL_TAG = "Enemy";

        #region Unity methods

        private void Awake()
        {
            // Initialize the enemy pool when the game starts
            InitializeEnemyPool();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!config)
                return;

            Rect enemyArea = config.EnemyAreaRect;
            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(enemyArea.xMin, enemyArea.yMin, 10));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(enemyArea.xMax, enemyArea.yMax, 10));
            Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);
            Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
#endif

        #endregion

        private void InitializeEnemyPool()
        {
            if (config != null && config.EnemyPrefab != null)
            {
                ObjectPoolManager.Instance.CreatePool(ENEMY_POOL_TAG, config.EnemyPrefab.gameObject, initialPoolSize);
            }
            else
            {
                Debug.LogError("Enemy prefab is not assigned in GameConfig!");
            }
        }

        public void StartProcess()
        {
            _processing = true;
            StopCoroutine(CreationRoutine());
            StartCoroutine(CreationRoutine());
        }

        public void StopProcess()
        {
            _processing = false;
            StopCoroutine(CreationRoutine());
        }
        
        private IEnumerator CreationRoutine()
        {
            while (_processing)
            {
                yield return new WaitForSeconds(config.EnemyCreationInterval);
                if (!_processing)
                    break;
                
                // Get enemy from pool instead of instantiating
                GameObject enemyObj = ObjectPoolManager.Instance.GetObjectFromPool(
                    ENEMY_POOL_TAG, 
                    config.EnemyCreationRandomPosition, 
                    Quaternion.identity
                );
                
                yield return null; // thread safety.
            }
            
            // end of the loop.
        }
    }
}