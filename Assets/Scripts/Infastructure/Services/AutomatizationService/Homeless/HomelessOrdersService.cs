using System.Collections.Generic;
using System.Linq;
using BuildProcessManagement;
using Infastructure.Services.UnitTrackingService;
using Infastructure.States;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.SpeachBuble.Units;
using Infastructure.StaticData.StaticDataService;
using Player.Orders;
using Sirenix.Utilities;
using Units;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Infastructure.Services.AutomatizationService.Homeless
{
    public class HomelessOrdersService : IHomelessOrdersService
    {
        private readonly List<HomelessOrderInfo> _orders = new List<HomelessOrderInfo>();
        private readonly List<UnitStatus> _homeless = new List<UnitStatus>();

        private readonly List<UnitStatus> _homelessTemp = new List<UnitStatus>();

        private readonly IUnitsTrackerService _unitsTrackerService;
        private readonly IStaticDataService _staticDataService;


        public HomelessOrdersService(IUnitsTrackerService unitsTrackerService, IStaticDataService staticDataService)
        {
            _unitsTrackerService = unitsTrackerService;
            _staticDataService = staticDataService;
        }

        public void AddHomeless(UnitStatus unitStatus)
        {
            _homeless.Add(unitStatus);

            ExecuteNewOrder();
        }

        public void RemoveHomeless(UnitStatus unitStatus)
        {
            _homeless.Remove(unitStatus);

            RemoveHomelessTemp(unitStatus);
        }

        public void AddHomelessTemp(UnitStatus unitStatus) =>
            _homelessTemp.Add(unitStatus);

        public void RemoveHomelessTemp(UnitStatus unitStatus)
        {
            if (_homelessTemp.Contains(unitStatus))
                _homelessTemp.Remove(unitStatus);
        }


        public void AddOrder(IHomelessOrder order, OrderMarker orderMarker, float positionX, string uniqueId)
        {
            if (_orders.All(x => x.HomelessOrder != order))
                _orders.Add(new HomelessOrderInfo(order, positionX, uniqueId, orderMarker));

            ExecuteAllOrders();
        }


        public void RemoveOrder(IHomelessOrder order)
        {
            HomelessOrderInfo homelessOrderInfo = _orders.FirstOrDefault(x => x.HomelessOrder == order);

            if (_orders.Contains(homelessOrderInfo))
            {
                _orders.Remove(homelessOrderInfo);

                ReleaseUnitsFromRemovedOrder(homelessOrderInfo);
                ExecuteNewOrder();
            }
        }


        public void ContinueExecuteOrders() =>
            ExecuteNewOrder();

        public void CompleteOrder(IHomelessOrder order, UnitStatus currentUnit)
        {
            _unitsTrackerService.RemoveUnit(currentUnit.UnitTypeId);

            RemoveHomelessTemp(currentUnit);
            RemoveHomeless(currentUnit);
            ReleaseAndDestroyUnit(currentUnit);
            ReleaseOtherUnits(order);
            ExecuteNewOrder();
        }

        private void ReleaseAndDestroyUnit(UnitStatus currentUnit)
        {
            HomelessBehaviour homelessBehaviour = currentUnit.GetComponentInChildren<HomelessBehaviour>();
            homelessBehaviour.StopAllActions();

            Object.Destroy(currentUnit.gameObject);
        }

        public void ReleaseOtherUnits(IHomelessOrder order)
        {
            HomelessOrderInfo homelessOrderInfo = _orders.FirstOrDefault(x => x.HomelessOrder == order);
            if (homelessOrderInfo == null)
                return;

            foreach (UnitStatus unitStatus in _homeless)
            {
                if (homelessOrderInfo.UniqueId == unitStatus.OrderUniqueId)
                {
                    HomelessBehaviour homelessBehaviour = unitStatus.GetComponentInChildren<HomelessBehaviour>();
                    homelessBehaviour.StopAllActions();

                    unitStatus.Release();
                }
            }
        }

        private void ReleaseUnitsFromRemovedOrder(HomelessOrderInfo homelessOrderInfo)
        {
            if (homelessOrderInfo == null)
                return;

            foreach (UnitStatus unitStatus in _homeless)
            {
                if (homelessOrderInfo.UniqueId == unitStatus.OrderUniqueId && !_homelessTemp.Contains(unitStatus))
                {
                    HomelessBehaviour homelessBehaviour = unitStatus.GetComponentInChildren<HomelessBehaviour>();
                    homelessBehaviour.StopAllActions();

                    unitStatus.OrderUniqueId = string.Empty;
                    unitStatus.IsWorked = false;
                    unitStatus.Release();
                }
            }
        }


        private void ExecuteAllOrders()
        {
            ExecuteTempOrder();
            ExecuteNewOrder();
        }

        private void ExecuteTempOrder()
        {
            foreach (UnitStatus unitStatus in _homelessTemp.ToList())
            {
                HomelessOrderInfo homelessOrderInfo = _orders.Find(x => x.UniqueId == unitStatus.OrderUniqueId);

                if (CanExecute(homelessOrderInfo))
                {
                    ExecutePendingOrder(homelessOrderInfo, unitStatus);
                    _homelessTemp.Remove(unitStatus);
                }
            }
        }

        private void ExecuteNewOrder()
        {
            foreach (HomelessOrderInfo orderInfo in _orders)
            {
                if (CanExecute(orderInfo))
                    ExecuteCurrentOrder(orderInfo);
            }
        }

        private bool CanExecute(HomelessOrderInfo orderInfo)
        {
            return orderInfo != null &&
                   orderInfo.HomelessOrder.HasAvailableSlot() &&
                   orderInfo.OrderMarker != null &&
                   !orderInfo.OrderMarker.IsStarted;
        }

        private void ExecutePendingOrder(HomelessOrderInfo orderInfo, UnitStatus unitStatus)
        {
            HomelessBehaviour homelessBehaviour =
                unitStatus.GetComponentInChildren<HomelessBehaviour>();

            UpdateSpeachBuble(orderInfo, unitStatus);

            homelessBehaviour.PlayHomelessOrderBehavior(orderInfo.HomelessOrder, orderInfo.PositionX,
                () => CompleteOrder(orderInfo.HomelessOrder, unitStatus));
        }

        private void ExecuteCurrentOrder(HomelessOrderInfo orderInfo)
        {
            int workedUnitsOnOrder = NumberOfWorkedUnits(orderInfo.UniqueId);

            for (int i = 0; i < orderInfo.HomelessOrder.NumberOfOrders() - workedUnitsOnOrder; i++)
            {
                UnitStatus nearestHomeless = GetNearestHomeless(orderInfo.PositionX);

                if (nearestHomeless == null)
                    return;

                nearestHomeless.IsWorked = true;
                HomelessBehaviour homelessBehaviour =
                    nearestHomeless.GetComponentInChildren<HomelessBehaviour>();

                UpdateSpeachBuble(orderInfo, nearestHomeless);

                homelessBehaviour.PlayHomelessOrderBehavior(orderInfo.HomelessOrder, orderInfo.PositionX,
                    () => CompleteOrder(orderInfo.HomelessOrder, nearestHomeless));
            }
        }

        private void UpdateSpeachBuble(HomelessOrderInfo orderInfo, UnitStatus nearestHomeless)
        {
            BuildingTypeId buildingTypeId = orderInfo.OrderMarker.GetComponent<BuildInfo>().BuildingTypeId;
            SpeachBubleOrderUpdater speachBubleOrderUpdater =
                nearestHomeless.GetComponentInChildren<SpeachBubleOrderUpdater>();

            SpeachBubleHomelessOrderConfig speachBubleData =
                _staticDataService.ForSpeachBuble(buildingTypeId);
            speachBubleOrderUpdater.UpdateSpeachBuble(speachBubleData.Sprite);
        }

        private UnitStatus GetNearestHomeless(float targetX)
        {
            UnitStatus nearestUnit = _homeless
                .Where(unit => unit != null && !unit.IsBusy())
                .OrderBy(unit => Mathf.Abs(unit.transform.position.x - targetX))
                .FirstOrDefault();

            return nearestUnit;
        }

        private int NumberOfWorkedUnits(string uniqueId) =>
            _homeless.Count(unitStatus => unitStatus.OrderUniqueId == uniqueId);
    }
}