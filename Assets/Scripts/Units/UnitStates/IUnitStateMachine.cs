using System.Collections.Generic;

namespace Units.UnitStates
{
    public interface IUnitStateMachine
    {
        void ChangeState<TState>() where TState : IUnitState;
        void Update();
        void Initialize(List<IUnitState> states);
        void AddToCurrentStates(IUnitState unitState);
    }
}