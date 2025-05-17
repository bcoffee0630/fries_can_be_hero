using UnityEngine;

namespace FCBH
{
    public abstract class BaseState : MonoBehaviour
    {
        [SerializeField] protected string stateId;

        /// <summary>
        /// To define what state this is.
        /// </summary>
        public string StateId => stateId;

        public abstract void OnInitialize();
        public abstract void OnEnter();
        public abstract void OnExit();
    }
}
