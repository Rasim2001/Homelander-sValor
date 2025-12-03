using System.Collections.Generic;

namespace Infastructure.Services.Tutorial
{
    public interface ITutorialStateMachine
    {
        void Initialize(List<ITutorialState> states);
        void ChangeState<TState>() where TState : ITutorialState;
        void Update();
    }
}