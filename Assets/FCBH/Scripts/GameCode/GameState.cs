using UnityEngine;

namespace FCBH
{
    public class GameState : BaseState
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject hud;

        [Header("Gameplay")]
        [SerializeField] private GameConfig config;
        [SerializeField] private RuntimeGestureRecognizer gestureRecognizer;
        [SerializeField] private EnemyCreator enemyCreator;

        public override void OnInitialize()
        {
            gestureRecognizer.IsActive = false;
            root.SetActive(false);
            hud.SetActive(false);
        }

        public override void OnEnter()
        {
            // todo reset game systems
            gestureRecognizer.IsActive = true;
            enemyCreator.StartProcess();
            root.SetActive(true);
            hud.SetActive(true);
        }

        public override void OnExit()
        {
            // todo stop running game systems
            gestureRecognizer.IsActive = false;
            enemyCreator.StopProcess();
            root.SetActive(false);
            hud.SetActive(false);
        }
    }
}
