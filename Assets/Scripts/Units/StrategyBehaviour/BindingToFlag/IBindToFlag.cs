using System;
using Flag;

namespace Units.StrategyBehaviour.BindingToFlag
{
    public interface IBindToFlag
    {
        void StopAction();

        void DoAction(FlagSlotCoordinator flagSlotCoordinator, float targetFreePositionX, int flipSideValue,
            Action onCompleted);
    }
}