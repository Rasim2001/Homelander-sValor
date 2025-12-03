using System;
using Player.Orders;

namespace Units.StrategyBehaviour.ChopManagement
{
    public interface IChopWood
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