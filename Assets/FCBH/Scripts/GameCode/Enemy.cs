using System;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;
using UnityEngine.UI;

namespace FCBH
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private GameConfig config;
        
        private string _gesture;
        private float _autoKillDelayId;
        
        private static List<Enemy> CreatedEnemies = new();
        private const string ENEMY_POOL_TAG = "Enemy";

        public static event Action OnRunAway;
        public static event Action OnKill;

        #region Unity methods

        private void OnEnable()
        {
            if (!CreatedEnemies.Contains(this))
                CreatedEnemies.Add(this);
            
            Initialize();
        }
        
        private void OnDisable()
        {
            CancelInvoke(nameof(RunAway));
            
            if (CreatedEnemies.Contains(this))
                CreatedEnemies.Remove(this);
        }

        #endregion

        public void Initialize()
        {
            _gesture = GestureUtility.GetRandomGesture();
            var texture = config.GestureVisualDatabase.GetGestureVisual(_gesture);
            visual.sprite = texture.gestureTexture;
            
            if (config.AutoKill)
                Invoke(nameof(RunAway), config.AutoKillDelay);
        }

        public void TryKill(Result result)
        {
            if (result.GestureClass != _gesture)
                return;
                
            OnKill?.Invoke();
            ReturnToPool();
        }

        private void RunAway()
        {
            OnRunAway?.Invoke();
            ReturnToPool();
        }
        
        private void ReturnToPool()
        {
            // Return to pool instead of destroying
            ObjectPoolManager.Instance.ReturnObjectToPool(gameObject, ENEMY_POOL_TAG);
        }

        public static void KillAllEnemies()
        {
            if (CreatedEnemies.Count == 0)
                return;
                
            // Use a temporary list to avoid modification during iteration
            List<Enemy> enemiesToKill = new List<Enemy>(CreatedEnemies);
            
            foreach (var enemy in enemiesToKill)
            {
                if (enemy != null && enemy.gameObject != null)
                {
                    // Return to pool instead of destroying
                    ObjectPoolManager.Instance.ReturnObjectToPool(enemy.gameObject, ENEMY_POOL_TAG);
                }
            }
            
            CreatedEnemies.Clear();
        }
    }
}