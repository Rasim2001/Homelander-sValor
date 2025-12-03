using System;

namespace Infastructure.Services.AutomatizationService.Homeless
{
    public interface IHomelessBehavior
    {
        void StopAction();
        void DoAction(IHomelessOrder order, float targetFreePositionX, Action onCompleted);
    }
}