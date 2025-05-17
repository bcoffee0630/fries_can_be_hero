using UnityEngine;

namespace FCBH
{
    [CreateAssetMenu(menuName = "FCBH/Events/Create StringEventSender")]
    public class StringEventSender<T> : EventSenderOfType<string>
    {
    }
}