using UnityEngine;

namespace FCBH
{
    public class GameState : BaseState
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject tips;
        [SerializeField] private GameObject rank;

        [Header("Gameplay")]
        [SerializeField] private GameConfig config;
        [SerializeField] private RuntimeGestureRecognizer gestureRecognizer;
        [SerializeField] private EnemyCreator enemyCreator;
        [SerializeField] private Timer timer;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private StateManager stateManager;

        private const string LOBBY_STATE = "Lobby";
        private const string ENEMY_POOL_TAG = "Enemy";
        
        #region Unity methods

        private void OnEnable()
        {
            Timer.OnTimerStarted += enemyCreator.StartProcess;
            Timer.OnTimerStopped += enemyCreator.StopProcess;
            Timer.OnTimerStopped += DisplayRank;
        }

        private void OnDisable()
        {
            Timer.OnTimerStarted -= enemyCreator.StartProcess;
            Timer.OnTimerStopped -= enemyCreator.StopProcess;
            Timer.OnTimerStopped -= DisplayRank;
        }

        #endregion
        
        public override void OnInitialize()
        {
            gestureRecognizer.IsActive = false;
            root.SetActive(false);
            hud.SetActive(false);
            tips.SetActive(false);
            rank.SetActive(false);
        }

        public override void OnEnter()
        {
            root.SetActive(true);
            hud.SetActive(true);
            tips.SetActive(true);
            rank.SetActive(false);
            scoreManager.ResetScore();
        }

        public override void OnExit()
        {
            gestureRecognizer.IsActive = false;
            timer.StopTimer();
            root.SetActive(false);
            hud.SetActive(false);
            rank.SetActive(false);
            Enemy.KillAllEnemies();
            
            // Clear the enemy pool when exiting the game state
            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ClearPool(ENEMY_POOL_TAG);
            }
        }

        private void DisplayRank()
        {
            gestureRecognizer.IsActive = false;
            timer.StopTimer();
            rank.SetActive(true);
            Enemy.KillAllEnemies();
        }

        // start game when button pressed.
        public void PressToStart()
        {
            tips.SetActive(false);
            gestureRecognizer.IsActive = true;
            timer.StartTimer();
        }

        // end game and close ranking when button pressed.
        public void PressToEnd()
        {
            stateManager.ChangeState(LOBBY_STATE);
        }
    }
}