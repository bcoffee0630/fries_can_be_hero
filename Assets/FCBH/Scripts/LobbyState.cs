using UnityEngine;

namespace FCBH
{
    public class LobbyState : BaseState
    {
        [SerializeField] private GameObject lobbyCanvas; // todo animate if have time
        
        public override void OnInitialize()
        {
            lobbyCanvas.SetActive(false);
        }

        public override void OnEnter()
        {
            lobbyCanvas.SetActive(true);
        }

        public override void OnExit()
        {
            lobbyCanvas.SetActive(false);
        }
    }
}
