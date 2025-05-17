using UnityEngine;

namespace FCBH
{
    [CreateAssetMenu(menuName = "FCBH/Events/Create IntEventSender")]
    public class IntEventSender<T> : EventSenderOfType<int>
    {
    }
}