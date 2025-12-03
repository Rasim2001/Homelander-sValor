using System;
using Player.Orders;

namespace Units.StrategyBehaviour.BuildManagement
{
    public interface IBuild
    {
        void DoAction(
            float speed,
            int freePlaceIndex,
            OrderMarker orderMarker,
            Action<OrderMarker> onOrderCompleted,
            Action onContinueOrderHappened);

        void StopAction();
    }
}