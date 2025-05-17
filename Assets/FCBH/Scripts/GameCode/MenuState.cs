using UnityEngine;

namespace FCBH
{
    public class MenuState : BaseState
    {
        [SerializeField] private GameObject menuCanvas; // todo animate if have time
        
        public override void OnInitialize()
        {
            menuCanvas.SetActive(false);
        }

        public override void OnEnter()
        {
            menuCanvas.SetActive(true);
        }

        public override void OnExit()
        {
            menuCanvas.SetActive(false);
        }
    }
}
