using UnityEngine;

namespace FCBH
{
    [CreateAssetMenu(menuName = "FCBH/Events/Create BoolEventSender")]
    public class BoolEventSender<T> : EventSenderOfType<bool>
    {
    }
}