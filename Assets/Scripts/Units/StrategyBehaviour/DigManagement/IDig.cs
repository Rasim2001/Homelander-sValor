using System;
using Player.Orders;

namespace Units.StrategyBehaviour.DigManagement
{
    public interface IDig
    {
        void DoAction(
            OrderMarker orderMarker,
            float speed,
            int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted,
            Action onContinueOrderHappened);

        void StopAction();
    }
}