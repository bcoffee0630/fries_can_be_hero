using UnityEngine;

namespace FCBH
{
    [CreateAssetMenu(menuName = "FCBH/Create GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private float time;
        [SerializeField] private float reduceTimeOnDamaged = 1;
        
        [Header("Settings/Gesture")]
        [SerializeField] private Transform gesturePrefab;
        [Range(0f, 1f), SerializeField] private float drawAreaX = 0.1f;
        [Range(0f, 1f), SerializeField] private float drawAreaY = 0.1f;
        [Range(0f, 1f), SerializeField] private float drawAreaWidth = 0.8f;
        [Range(0f, 1f), SerializeField] private float drawAreaHeight = 0.8f;
        [SerializeField] private float recognizeDelay = 1f;
        
        [Header("Settings/Enemy")]
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private GestureVisualDatabase gestureVisualDatabase;
        
        [SerializeField] private float enemyCreationInterval = 1;
        [SerializeField] private float enemyCreationRandomSeed = 1;
        
        [SerializeField] private bool autoKill = true;
        [SerializeField] private float autoKillDelay = 2;
        [Tooltip("If delay = 3, randomize = 1, the auto-kill delay will between 2~4 (3-1=2 ~ 3+1=4).")]
        [SerializeField] private float autoKillRandomize = 2;
        
        [Range(0f, 1f), SerializeField] private float enemyAreaX = 0.1f;
        [Range(0f, 1f), SerializeField] private float enemyAreaY = 0.1f;
        [Range(0f, 1f), SerializeField] private float enemyAreaWidth = 0.8f;
        [Range(0f, 1f), SerializeField] private float enemyAreaHeight = 0.8f;

        [Header("Settings/Score")]
        [SerializeField] private int baseScore;
        [SerializeField] private int scoreRandomSeed = 1;
        
        #region Read-only properties

        public float Time => time;
        public float ReduceTimeOnDamaged => reduceTimeOnDamaged;

        #region Gesture settings

        public Transform GesturePrefab => gesturePrefab;

        #region Draw area

        public float DrawAreaX => drawAreaX;

        public float DrawAreaY => drawAreaY;

        public float DrawAreaWidth => drawAreaWidth;

        public float DrawAreaHeight => drawAreaHeight;

        #endregion

        public float RecognizeDelay => recognizeDelay;

        #endregion

        #region Enemy settings

        public Enemy EnemyPrefab => enemyPrefab;

        public GestureVisualDatabase GestureVisualDatabase => gestureVisualDatabase;

        #region Enemy creation

        public float EnemyCreationInterval
        {
            get
            {
                var min = enemyCreationInterval - enemyCreationRandomSeed;
                if (min < 0)
                    min = 0;
                return Random.Range(min, enemyCreationInterval + enemyCreationRandomSeed);
            }
        }
        
        public Vector3 EnemyCreationRandomPosition =>
            Camera.main != null
                ? Camera.main.ScreenToWorldPoint(new Vector3(
                    Random.Range(EnemyAreaRect.xMin, EnemyAreaRect.xMax),
                    Random.Range(EnemyAreaRect.yMin, EnemyAreaRect.yMax),
                    10
                ))
                : Vector3.zero;
        
        public Rect EnemyAreaRect =>
            new Rect(
                Screen.width * enemyAreaX,
                Screen.height * enemyAreaY,
                Screen.width * enemyAreaWidth,
                Screen.height * enemyAreaHeight
            );

        public HorizontalDirection EnemyHorizontalDirection(Vector3 position)
        {
            if (Camera.main == null)
                return HorizontalDirection.Middle;
            
            var min = Camera.main.ScreenToWorldPoint(new Vector3(EnemyAreaRect.xMin, EnemyAreaRect.y / 2, 10));
            var max = Camera.main.ScreenToWorldPoint(new Vector3(EnemyAreaRect.xMax, EnemyAreaRect.y / 2, 10));
            var delta = (max - min) / 3;
            delta.y = 0;
            delta.z = 0;
            
            var firstSection = min + delta;
            var secondSection = firstSection + delta;

            if (position.x >= min.x && position.x < firstSection.x)
                return HorizontalDirection.Left;
            else if (position.x >= firstSection.x && position.x < secondSection.x)
                return HorizontalDirection.Middle;
            else
                return HorizontalDirection.Right;
        }

        #endregion
        
        #region Enemy auto-kill

        public bool AutoKill => autoKill;

        public float AutoKillDelay
        {
            get
            {
                var min = autoKillDelay - autoKillRandomize;
                if (min < 0)
                    min = 0;
                return Random.Range(min, autoKillDelay + autoKillRandomize);
            }
        }

        #endregion

        #region Enemy area

        public float EnemyAreaX => enemyAreaX;

        public float EnemyAreaY => enemyAreaY;

        public float EnemyAreaWidth => enemyAreaWidth;

        public float EnemyAreaHeight => enemyAreaHeight;

        #endregion

        #endregion

        #region Score settings

        public int GetScore
        {
            get
            {
                var min = baseScore - scoreRandomSeed;
                if (min < 1)
                    min = 1;
                return Random.Range(min, baseScore + scoreRandomSeed);
            }
        }

        #endregion

        #endregion
    }

    public enum HorizontalDirection
    {
        Left,
        Middle,
        Right
    }
}