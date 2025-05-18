using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FCBH
{
    public class RankSystem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI visual;

        #region Unity methods

        private void OnEnable()
        {
            ScoreManager.OnScoreChanged += UpdateRanking;
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreChanged -= UpdateRanking;
        }

        #endregion
        
        private Dictionary<int, string> _rankingMap = new()
        {
            { 0,   "N"   },
            { 20,  "R"   },
            { 50,  "SR"  },
            { 100, "SSR" },
            { 200, "GOD"  }
        };

        private string _rank;

        private string Rank
        {
            get => _rank;
            set
            {
                _rank = value;
                OnRankingChanged?.Invoke(value);
                if (visual)
                    visual.SetText(value);
            }
        }

        public static event Action<string> OnRankingChanged;

        private string GetRank(int score)
        {
            var scoreKeys = _rankingMap.Keys;
            var currentKey = 0;
            foreach (var key in scoreKeys)
            {
                if (score < key)
                    break;
                currentKey = key;
            }

            return _rankingMap[currentKey];
        }

        private void UpdateRanking(int score)
        {
            Rank =  GetRank(score);
        }
    }
}
