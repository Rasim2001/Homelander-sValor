using System;
using Player.Orders;

namespace Units.StrategyBehaviour.HealManagement
{
    public interface IHealBuilding
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