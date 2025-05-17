using UnityEngine;
using UnityEngine.Events;

namespace FCBH
{
    public class EventListener : MonoBehaviour
    {
        [SerializeField] private EventSender eventSender;
        [SerializeField] private UnityEvent onRaise;

        #region Unity methods

        private void OnEnable()
        {
            eventSender.RegisterListener(this);
        }

        private void OnDisable()
        {
            eventSender.UnregisterListener(this);
        }

        #endregion

        public void Raise()
        {
            onRaise.Invoke();
        }
    }
}