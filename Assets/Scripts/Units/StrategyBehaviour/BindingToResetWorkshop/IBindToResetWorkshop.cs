using System;

namespace Units.StrategyBehaviour.BindingToResetWorkshop
{
    public interface IBindToResetWorkshop
    {
        void StopAction();
        void DoAction(float targetFreePositionX, Action onCompleted);
    }
}