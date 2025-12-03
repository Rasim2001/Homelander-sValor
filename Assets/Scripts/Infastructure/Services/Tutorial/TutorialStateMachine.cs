using System.Collections.Generic;
using System.Linq;

namespace Infastructure.Services.Tutorial
{
    public class TutorialStateMachine : ITutorialStateMachine
    {
        private List<ITutorialState> _states;

        private ITutorialState _currentUnitState;

        public void Initialize(List<ITutorialState> states)
        {
            _states = new List<ITutorialState>(states);

            _currentUnitState = _states[0];
            _currentUnitState.Enter();
        }

        public void ChangeState<TState>() where TState : ITutorialState
        {
            ITutorialState newState = _states.FirstOrDefault(newState => newState is TState);

            _currentUnitState.Exit();
            _currentUnitState = newState;
            _currentUnitState.Enter();
        }

        public bool ActiveStateIs<TState>() where TState : ITutorialState =>
            _currentUnitState is TState;

        public void Update() =>
            _currentUnitState.Update();
    }
}