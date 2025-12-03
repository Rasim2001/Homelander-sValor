using System;
using System.Collections.Generic;

namespace Infastructure.States
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _registeredStates;

        private IExitableState currentState;

        public StateMachine(
            BootstrapState.Factory bootstrapStateFactory,
            LoadProgressState.Factory loadGameSaveStateFactory,
            LoadLevelState.Factory loadLevelStateFactory,
            MainMenuState.Factory mainMenuStateFactory)
        {
            _registeredStates = new Dictionary<Type, IExitableState>();

            RegisterState(bootstrapStateFactory.Create(this));
            RegisterState(loadGameSaveStateFactory.Create(this));
            RegisterState(loadLevelStateFactory.Create(this));
            RegisterState(mainMenuStateFactory.Create());
        }

        private void RegisterState<TState>(TState state) where TState : IExitableState =>
            _registeredStates.Add(typeof(TState), state);

        public void Enter<TState>() where TState : class, IState
        {
            TState newState = ChangeState<TState>();
            newState.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPaylodedState<TPayload>
        {
            TState newState = ChangeState<TState>();
            newState.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            currentState?.Exit();

            TState state = GetState<TState>();
            currentState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _registeredStates[typeof(TState)] as TState;
    }
}