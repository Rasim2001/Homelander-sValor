using System;
using Player.Orders;
using Units.StrategyBehaviour;

namespace Infastructure.Services.AutomatizationService.Builders
{
    public interface IExecuteOrdersService
    {
        void ExecuteOrder(BuilderBehaviour behaviour, OrderMarker orderMarker, int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted, Action onContinueOrderHappened);

        int FreePlaceIndex(OrderMarker orderMarker);
    }
}