using System.Collections.Generic;
using UnityEngine;

namespace FCBH
{
    [CreateAssetMenu(menuName = "FCBH/Events/Create EventSender")]
    public class EventSender : ScriptableObject
    {
        private List<EventListener>  _listeners = new List<EventListener>();

        public void RegisterListener(EventListener listener)
        {
            if (_listeners.Contains(listener))
                return;
            _listeners.Add(listener);
        }

        public void UnregisterListener(EventListener listener)
        {
            if (!_listeners.Contains(listener))
                return;
            _listeners.Remove(listener);
        }

        public void RaiseEvent()
        {
            foreach (var listener in _listeners)
            {
                listener.Raise();
            }
        }
    }
}
