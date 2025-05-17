using System;
using System.Collections.Generic;
using UnityEngine;

namespace FCBH
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] private List<BaseState> states = new();
        [SerializeField] private BaseState defaultState;
        
        private BaseState _currentState;

        #region Unity methods

        private void Start()
        {
            if (defaultState != null)
                EnterState(defaultState);
        }

        private void Update()
        {
            if (_currentState != null && _currentState.StateStatus == BaseState.Status.IsUpdate)
                _currentState.OnUpdate();
        }

        #endregion

        private void EnterState(BaseState state)
        {
            state.OnEnter();
        }

        private void UpdateState(BaseState state)
        {
            state.OnUpdate();
        }

        private void ExitState(BaseState state)
        {
            state.OnExit();
        }

        public void ChangeState(BaseState state)
        {
            if (_currentState != null)
                ExitState(_currentState);
            _currentState = state;
            EnterState(_currentState);
        }

        public void ChangeState(string stateId)
        {
            var state = states.Find(s => s.StateId == stateId);
            if (!state)
                throw new NullReferenceException($"Missing state with id: {stateId}");
            ChangeState(state);
        }
    }
}