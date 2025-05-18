using System.Collections;
using UnityEngine;

namespace FCBH
{
    public class EnemyCreator : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        
        private bool _processing;

        #region Unity methods

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
                Instantiate(config.EnemyPrefab, config.EnemyCreationRandomPosition, Quaternion.identity);
                yield return null; // thread safety.
            }
            
            // end of the loop.
        }
    }
}