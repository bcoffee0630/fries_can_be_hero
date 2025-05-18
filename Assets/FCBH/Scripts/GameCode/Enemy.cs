using System;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FCBH
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private GameConfig config;
        
        private string _gesture;

        private static List<Enemy> CreatedEnemies = new();

        public static event Action OnRunAway;
        public static event Action OnKill;

        #region Unity methods

        private void Start()
        {
            if (!CreatedEnemies.Contains(this))
                CreatedEnemies.Add(this);
            
            _gesture = GestureUtility.GetRandomGesture();
            var texture = config.GestureVisualDatabase.GetGestureVisual(_gesture);
            visual.sprite = texture.gestureTexture;
            
            if (config.AutoKill)
                Invoke(nameof(RunAway), config.AutoKillDelay); // todo set randomize time
        }

        #endregion

        public void TryKill(Result result)
        {
            if (result.GestureClass != _gesture)
                return;
            OnKill?.Invoke();
            Destroy(gameObject); // todo pooling, animate, effect
        }

        private void RunAway()
        {
            OnRunAway?.Invoke();
            Destroy(gameObject); // todo pooling, animate, effect
        }

        public static void KillAllEnemies()
        {
            if (CreatedEnemies.Count == 0)
                return;
            for (var x = CreatedEnemies.Count - 1; x >= 0; x--)
            {
                var createdEnemy = CreatedEnemies[x];
                Destroy(createdEnemy.gameObject);
                CreatedEnemies.RemoveAt(x);
            }
        }
    }
}
