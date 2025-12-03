using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.UnitStates
{
    public class UnitStateMachine : IUnitStateMachine
    {
        private List<IUnitState> _states;
        private IUnitState _currentUnitState;

        public void Initialize(List<IUnitState> states)
        {
            _states = states;

            _currentUnitState = _states[0];
            _currentUnitState.Enter();
        }

        public void AddToCurrentStates(IUnitState unitState) =>
            _states.Add(unitState);

        public void ChangeState<TState>() where TState : IUnitState
        {
            IUnitState newState = _states.FirstOrDefault(newState => newState is TState);

            if (_currentUnitState.GetType() == newState.GetType())
                return;

            //Debug.Log($"Previous : {_currentUnitState.GetType().Name} / NewState : {newState.GetType().Name}");

            _currentUnitState.Exit();
            _currentUnitState = newState;
            _currentUnitState.Enter();
        }

        public void Update() =>
            _currentUnitState.Update();
    }
}