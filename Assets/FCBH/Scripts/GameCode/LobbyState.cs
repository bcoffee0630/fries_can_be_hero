using TMPro;
using UnityEngine;

namespace FCBH
{
    public class LobbyState : BaseState
    {
        [SerializeField] private GameObject lobbyCanvas; // todo animate if have time
        [SerializeField] private TextMeshProUGUI rankVisual;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioClip bgmLobby;
        [SerializeField] private AudioClip bgmGame;

        #region Unity methods

        private void OnEnable()
        {
            RankSystem.OnRankingChanged += UpdateRanking;
        }

        private void OnDisable()
        {
            RankSystem.OnRankingChanged -= UpdateRanking;
        }

        #endregion
        
        public override void OnInitialize()
        {
            lobbyCanvas.SetActive(false);
        }

        public override void OnEnter()
        {
            lobbyCanvas.SetActive(true);
            if (bgmSource.isPlaying)
                bgmSource.Stop();
            bgmSource.clip = bgmLobby;
            bgmSource.Play();
        }

        public override void OnExit()
        {
            lobbyCanvas.SetActive(false);
            if (bgmSource.isPlaying)
                bgmSource.Stop();
            bgmSource.clip = bgmGame;
            bgmSource.Play();
        }

        private void UpdateRanking(string rank)
        {
            if (rankVisual)
                rankVisual.SetText(rank);
        }
    }
}
