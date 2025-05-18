using System;
using TMPro;
using UnityEngine;

namespace FCBH
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private TextMeshProUGUI visual;

        private int _score;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                if (visual)
                    visual.SetText($"{value:0}");
                OnScoreChanged?.Invoke(value);
            }
        }
        
        public static event Action<int> OnScoreChanged;

        #region Unity methods

        private void OnEnable()
        {
            Enemy.OnKill += AddScore;
        }

        private void OnDisable()
        {
            Enemy.OnKill -= AddScore;
        }

        #endregion

        public void ResetScore()
        {
            Score = 0;
        }
        
        private void AddScore()
        {
            Score += config.GetScore;
        }
    }
}