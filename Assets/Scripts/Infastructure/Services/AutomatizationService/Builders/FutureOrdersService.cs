using System.Collections.Generic;
using System.Linq;
using Enviroment;
using Flag;
using Infastructure.Services.Flag;
using Infastructure.Services.MarkerSignCoordinator;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.StaticData.SpeachBuble.Units;
using Infastructure.StaticData.StaticDataService;
using Player.Orders;
using UI.GameplayUI;
using Units;
using Units.StrategyBehaviour;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Infastructure.Services.AutomatizationService.Builders
{
    public class FutureOrdersService : IFutureOrdersService
    {
        private readonly List<OrderMarker> _orders = new List<OrderMarker>();
        private readonly List<OrderMarker> _nightOrders = new List<OrderMarker>();

        private readonly List<UnitStatus> _builders = new List<UnitStatus>();

        private readonly IExecuteOrdersService _executeOrdersService;
        private readonly IMarkerSignCoordinatorService _markerCoodinatorService;
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly IFlagTrackerService _flagTrackerService;
        private readonly IStaticDataService _staticDataService;

        public FutureOrdersService(
            IExecuteOrdersService executeOrdersService,
            IMarkerSignCoordinatorService markerCoodinatorService,
            ISafeBuildZone safeBuildZone,
            IFlagTrackerService flagTrackerService,
            IStaticDataService staticDataService)
        {
            _executeOrdersService = executeOrdersService;
            _markerCoodinatorService = markerCoodinatorService;
            _safeBuildZone = safeBuildZone;
            _flagTrackerService = flagTrackerService;
            _staticDataService = staticDataService;
        }

        public void AddBuilder(UnitStatus builder) =>
            _builders.Add(builder);

        public void RemoveBuilder(UnitStatus builder) =>
            _builders.Remove(builder);

        public void AddOrder(OrderMarker orderMarker)
        {
            if (!_orders.Contains(orderMarker))
            {
                if (_safeBuildZone.IsNight)
                    _nightOrders.Add(orderMarker);

                _orders.Add(orderMarker);
                _markerCoodinatorService.AddMarker(orderMarker);

                orderMarker.IsMarkered = true;

                if (orderMarker.TryGetComponent(out IlluminateObject illuminateObject))
                    illuminateObject.Release();

                if (orderMarker.TryGetComponent(out HintsDisplayBase hintsDisplayBase))
                    hintsDisplayBase.HideHints();

                ExecuteOrder();
            }
        }


        public void ContinueExecuteOrders() =>
            ExecuteOrder();

        public void RemoveCompletedOrder(OrderMarker orderMarker)
        {
            if (_nightOrders.Contains(orderMarker))
                _nightOrders.Remove(orderMarker);

            foreach (UnitStatus unitStatus in _builders)
            {
                UniqueId uniqueId = orderMarker.GetComponent<UniqueId>();

                if (unitStatus.OrderUniqueId == uniqueId.Id)
                {
                    BuilderBehaviour unitStrategyBehaviour =
                        unitStatus.GetComponentInChildren<BuilderBehaviour>();

                    unitStrategyBehaviour.StopAllActions();
                    unitStatus.Release();
                }
            }

            if (_orders.Contains(orderMarker))
            {
                _orders.Remove(orderMarker);
                _markerCoodinatorService.RemoveMarker(orderMarker);
                orderMarker.IsMarkered = false;
            }
        }


        public void FilterNightOrders()
        {
            /*foreach (OrderMarker order in _orders)
            {
                if (_safeBuildZone.IsSafeZone(order.transform.position.x))
                    _nightOrders.Add(order);
            }*/
        }

        public void ClearNightOrders()
        {
            FlagSlotCoordinator rightFlag = _flagTrackerService.GetLastFlag(true).GetComponent<FlagSlotCoordinator>();
            FlagSlotCoordinator leftFlag = _flagTrackerService.GetLastFlag(false).GetComponent<FlagSlotCoordinator>();

            if (rightFlag != null && rightFlag.IsBarricadeDamaged())
                AddBarricadeHealOrder(rightFlag.GetBarricadeOrderMarker());

            if (leftFlag != null && leftFlag.IsBarricadeDamaged())
                AddBarricadeHealOrder(leftFlag.GetBarricadeOrderMarker());

            _nightOrders.Clear();
        }

        private bool IsSafeZoneInNight(OrderMarker orderMarker) =>
            _safeBuildZone.IsNight && _safeBuildZone.IsSafeZone(orderMarker.transform.position.x);

        public OrderMarker GetOrderByOrderUniqueId(string uniqueId) =>
            _orders.FirstOrDefault(x => x.GetComponent<UniqueId>().Id.Contains(uniqueId));

        private void AddBarricadeHealOrder(OrderMarker orderMarker)
        {
            if (_orders.Contains(orderMarker))
                _orders.Remove(orderMarker);

            _orders.Insert(0, orderMarker);
            _markerCoodinatorService.AddMarker(orderMarker);

            orderMarker.IsMarkered = true;

            ExecuteOrder();
        }

        private void ExecuteOrder() // должна вызываться после каждого конца выполнения приказа с каждого юнита
        {
            //if (!_safeBuildZone.IsNight || IsSafeZoneInNight(orderMarker))

            if (_builders.Count == 0)
                return;

            List<UnitStatus> freeBuilders = new List<UnitStatus>();

            SelectFreeBuilders(freeBuilders);
            GiveOrdersToSelectedBuilders(freeBuilders);
        }

        private void SelectFreeBuilders(List<UnitStatus> freeBuilders)
        {
            foreach (UnitStatus builder in _builders)
            {
                if (!builder.IsBusy())
                    freeBuilders.Add(builder);
            }
        }

        private void GiveOrdersToSelectedBuilders(List<UnitStatus> freeBuilders)
        {
            int amountOfFreeUnits = freeBuilders.Count;

            bool isNight = _safeBuildZone.IsNight;

            List<OrderMarker> currentOrders =
                isNight
                    ? new List<OrderMarker>(_nightOrders).Where(x => _safeBuildZone.IsSafeZone(x.transform.position.x)).ToList()
                    : new List<OrderMarker>(_orders);

            for (int i = 0; i < amountOfFreeUnits; i++)
            {
                OrderMarker closestOrder =
                    GetClosestOrder(freeBuilders, currentOrders, out int freePlaceIndex, out UnitStatus closestUnit);

                if (closestOrder == null)
                    break;

                freeBuilders.Remove(closestUnit);
                BlockMoveAndRelease();
                GiveOrder(closestUnit, closestOrder, freePlaceIndex);
            }
        }


        private OrderMarker GetClosestOrder(List<UnitStatus> freeBuilders, List<OrderMarker> currentOrders,
            out int freePlaceIndex, out UnitStatus closestUnit)
        {
            float minimalDistance = Mathf.Infinity;
            OrderMarker orderMarkerMain = null;
            closestUnit = null;
            freePlaceIndex = -1;

            foreach (UnitStatus freeBuilder in freeBuilders)
            {
                foreach (OrderMarker orderMarker in currentOrders.ToList())
                {
                    float distance = Mathf.Abs(freeBuilder.transform.position.x - orderMarker.transform.position.x);
                    int freePlaceIndexCurrent = _executeOrdersService.FreePlaceIndex(orderMarker);

                    if (DoesNotFreePlace(freePlaceIndexCurrent))
                    {
                        ReleaseRemainingUnits();
                        currentOrders.Remove(orderMarker);
                    }

                    else
                    {
                        if (distance < minimalDistance)
                        {
                            minimalDistance = distance;
                            orderMarkerMain = orderMarker;
                            freePlaceIndex = freePlaceIndexCurrent;
                            closestUnit = freeBuilder;
                        }
                    }
                }
            }


            return orderMarkerMain;
        }


        private void ReleaseRemainingUnits()
        {
            foreach (UnitStatus builder in _builders)
            {
                if (!builder.IsBusy())
                    builder.GetComponent<UnitMove>().enabled = true;
            }
        }

        private void BlockMoveAndRelease()
        {
            foreach (UnitStatus builder in _builders)
            {
                if (!builder.IsBusy())
                    builder.GetComponent<UnitMove>().enabled = false;
            }
        }

        private bool DoesNotFreePlace(int freePlaceindex) =>
            freePlaceindex == -1;


        private void GiveOrder(UnitStatus freeBuilder, OrderMarker currentOrder, int freePlaceIndex)
        {
            BuilderBehaviour unitStrategyBehaviour =
                freeBuilder.GetComponentInChildren<BuilderBehaviour>();

            UnitStateMachineView stateMachineView = freeBuilder.GetComponent<UnitStateMachineView>();
            stateMachineView.ChangeState<UnknowState>();

            SpeachBubleOrderConfig orderConfig = _staticDataService.ForSpeachBuble(currentOrder.OrderID);
            SpeachBubleOrderUpdater speachBubleUpdater =
                freeBuilder.GetComponentInChildren<SpeachBubleOrderUpdater>();
            speachBubleUpdater.UpdateSpeachBuble(orderConfig.Sprite);

            _executeOrdersService.ExecuteOrder(unitStrategyBehaviour, currentOrder, freePlaceIndex,
                RemoveCompletedOrder, ContinueExecuteOrders);
        }
    }
}