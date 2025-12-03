using System;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Player.Orders;
using Units.StrategyBehaviour;

namespace Infastructure.Services.AutomatizationService.Builders
{
    public class ExecuteOrdersService : IExecuteOrdersService
    {
        private readonly float _speed;

        public ExecuteOrdersService(IStaticDataService staticDataService)
        {
            UnitStaticData unitStaticData = staticDataService.ForUnit(UnitTypeId.Builder);
            _speed = unitStaticData.RunSpeed;
        }

        public void ExecuteOrder(
            BuilderBehaviour behaviour,
            OrderMarker orderMarker,
            int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted,
            Action onContinueOrderHappened)
        {
            switch (orderMarker.OrderID)
            {
                case OrderID.Chop:
                    behaviour
                        .PlayChopWoodBehaviour(orderMarker, _speed, freePlaceIndex, onOrderCompleted,
                            onContinueOrderHappened);
                    break;
                case OrderID.Build:
                    behaviour
                        .PlayBuildBehaviour(orderMarker, _speed, freePlaceIndex, onOrderCompleted,
                            onContinueOrderHappened);
                    break;
                case OrderID.Dig:
                    behaviour
                        .PlayDigBehaviour(orderMarker, _speed, freePlaceIndex, onOrderCompleted,
                            onContinueOrderHappened);
                    break;
                case OrderID.Heal:
                    behaviour
                        .PlayHealBehaviour(orderMarker, _speed, freePlaceIndex, onOrderCompleted,
                            onContinueOrderHappened);
                    break;
            }
        }

        public int FreePlaceIndex(OrderMarker orderMarker)
        {
            for (int i = 0; i < orderMarker.Places.Count; i++)
            {
                if (!orderMarker.Places[i].IsBusy)
                    return i;
            }

            return -1;
        }
    }
}