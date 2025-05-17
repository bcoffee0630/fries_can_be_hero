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

        private void Awake()
        {
            foreach (var state in states)
            {
                state.OnInitialize();
            }
        }

        private void Start()
        {
            if (defaultState != null)
                ChangeState(defaultState);
        }

        #endregion

        private void EnterState(BaseState state)
        {
            state.OnEnter();
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