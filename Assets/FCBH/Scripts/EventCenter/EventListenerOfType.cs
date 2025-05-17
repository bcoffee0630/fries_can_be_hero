using UnityEngine;
using UnityEngine.Events;

namespace FCBH
{
    public abstract class EventListenerOfType<T> : MonoBehaviour
    {
        [SerializeField] private EventSenderOfType<T> eventSender;
        [SerializeField] private UnityEvent<T> onRaise;
        
        #region Unity methods

        protected virtual void OnEnable()
        {
            eventSender.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            eventSender.UnregisterListener(this);
        }

        #endregion

        public virtual void Raise(T value)
        {
            onRaise.Invoke(value);
        }
    }
}