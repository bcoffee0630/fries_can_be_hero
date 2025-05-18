using UnityEngine;

namespace FCBH
{
    public class GameState : BaseState
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject tips;

        [Header("Gameplay")]
        [SerializeField] private GameConfig config;
        [SerializeField] private RuntimeGestureRecognizer gestureRecognizer;
        [SerializeField] private EnemyCreator enemyCreator;
        [SerializeField] private Timer timer;
        [SerializeField] private ScoreManager scoreManager;

        #region Unity methods

        private void OnEnable()
        {
            Timer.OnTimerStarted += enemyCreator.StartProcess;
            Timer.OnTimerStopped += enemyCreator.StopProcess;
        }

        private void OnDisable()
        {
            Timer.OnTimerStarted -= enemyCreator.StartProcess;
            Timer.OnTimerStopped -= enemyCreator.StopProcess;
        }

        #endregion
        
        public override void OnInitialize()
        {
            gestureRecognizer.IsActive = false;
            root.SetActive(false);
            hud.SetActive(false);
            tips.SetActive(false);
        }

        public override void OnEnter()
        {
            root.SetActive(true);
            hud.SetActive(true);
            tips.SetActive(true);
            scoreManager.ResetScore();
        }

        public override void OnExit()
        {
            gestureRecognizer.IsActive = false;
            timer.StopTimer();
            Enemy.KillAllEnemies();
            root.SetActive(false);
            hud.SetActive(false);
        }

        // to button event.
        public void PressToStart()
        {
            tips.SetActive(false);
            gestureRecognizer.IsActive = true;
            timer.StartTimer();
        }
    }
}
