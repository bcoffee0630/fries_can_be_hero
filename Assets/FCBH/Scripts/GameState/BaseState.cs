using UnityEngine;

namespace FCBH
{
    public abstract class BaseState : MonoBehaviour
    {
        #region Structures

        public enum Status
        {
            IsEnter,
            IsUpdate,
            IsExit,
        }

        #endregion
        
        [SerializeField] protected string stateId;

        protected Status _status;
        
        /// <summary>
        /// To define what state this is.
        /// </summary>
        public string StateId => stateId;
        public Status StateStatus => _status;

        public virtual void OnEnter()
        {
            _status = Status.IsEnter;
        }

        public virtual void OnUpdate()
        {
            _status = Status.IsUpdate;
        }

        public virtual void OnExit()
        {
            _status = Status.IsExit;
        }
    }
}
