using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace FCBH
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private TextMeshProUGUI visual;

        private float _timer;
        private WaitForSeconds _timerTick = new(1);
        
        public static event Action<float> OnTimerTick;
        public static event Action OnTimerStarted;
        public static event Action OnTimerStopped;

        #region Unity methods

        private void OnEnable()
        {
            OnTimerTick += UpdateVisual;
            Enemy.OnRunAway += ReduceTimer;
        }

        private void OnDisable()
        {
            OnTimerTick -= UpdateVisual;
            Enemy.OnRunAway -= ReduceTimer;
        }

        #endregion
        
        public void StartTimer()
        {
            _timer = config.Time;
            StopCoroutine(TimerRoutine());
            StartCoroutine(TimerRoutine());
        }

        public void StopTimer()
        {
            _timer = 0;
            StopCoroutine(TimerRoutine());
        }

        private IEnumerator TimerRoutine()
        {
            OnTimerStarted?.Invoke();
            OnTimerTick?.Invoke(_timer); // initial value
            
            while (!Mathf.Approximately(_timer, 0))
            {
                _timer -= 1;
                OnTimerTick?.Invoke(_timer); // updated value
                yield return _timerTick;
            }
            
            OnTimerTick?.Invoke(0); // ended value
            OnTimerStopped?.Invoke();
        }

        private void UpdateVisual(float time)
        {
            if (visual)
            {
                visual.SetText($"{time:0}");
            }
        }

        private void ReduceTimer()
        {
            _timer = Mathf.Clamp(_timer - config.ReduceTimeOnDamaged, 0, config.Time);
        }
    }
}