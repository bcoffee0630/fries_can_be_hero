using System.Collections.Generic;
using UnityEngine;

namespace FCBH
{
    public abstract class EventSenderOfType<T> : ScriptableObject
    {
        protected List<EventListenerOfType<T>>  _listeners = new ();

        public virtual void RegisterListener(EventListenerOfType<T> listener)
        {
            if (_listeners.Contains(listener))
                return;
            _listeners.Add(listener);
        }

        public virtual void UnregisterListener(EventListenerOfType<T> listener)
        {
            if (!_listeners.Contains(listener))
                return;
            _listeners.Remove(listener);
        }

        public virtual void RaiseEvent(T value)
        {
            foreach (var listener in _listeners)
            {
                listener.Raise(value);
            }
        }
    }
}