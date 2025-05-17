using System.Collections;
using UnityEngine;

namespace FCBH
{
    public class GameState : BaseState
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject hud;

        [Header("Gameplay")]
        [SerializeField] private RuntimeGestureRecognizer gestureRecognizer;
        [SerializeField] private GameConfig config;

        private bool _isPlaying; // is game playing?

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
        
        public override void OnInitialize()
        {
            gestureRecognizer.IsActive = false;
            _isPlaying = false;
            root.SetActive(false);
            hud.SetActive(false);
        }

        public override void OnEnter()
        {
            // todo reset game systems
            gestureRecognizer.IsActive = true;
            _isPlaying = true;
            StartCoroutine(EnemyCreationRoutine());
            root.SetActive(true);
            hud.SetActive(true);
        }

        public override void OnExit()
        {
            // todo stop running game systems
            gestureRecognizer.IsActive = false;
            _isPlaying = false;
            StopCoroutine(EnemyCreationRoutine());
            root.SetActive(false);
            hud.SetActive(false);
        }

        private IEnumerator EnemyCreationRoutine()
        {
            while (_isPlaying)
            {
                yield return new WaitForSeconds(config.EnemyCreationInterval);
                Instantiate(config.EnemyPrefab, config.EnemyCreationRandomPosition, Quaternion.identity);
                yield return null; // thread safety.
            }
            
            // end of the loop.
        }
    }
}
