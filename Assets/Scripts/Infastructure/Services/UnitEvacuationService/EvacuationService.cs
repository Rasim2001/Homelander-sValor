using System.Collections.Generic;
using System.Linq;
using Flag;
using Infastructure.Services.AutomatizationService;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.Flag;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.StaticData.SpeachBuble.Units;
using Infastructure.StaticData.Unit;
using ModestTree;
using Player.Orders;
using Units;
using Units.StrategyBehaviour;
using Units.UnitStates.BuilderStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Infastructure.Services.UnitEvacuationService
{
    public class EvacuationService : IEvacuationService
    {
        private readonly List<UnitStatus> _allUnitsInScene = new List<UnitStatus>();

        private readonly ISafeBuildZone _safeBuildZone;
        private readonly IFutureOrdersService _futureOrdersService;
        private readonly IFlagTrackerService _flagTrackerService;


        public EvacuationService(
            ISafeBuildZone safeBuildZone,
            IFutureOrdersService futureOrdersService,
            IFlagTrackerService flagTrackerService)
        {
            _safeBuildZone = safeBuildZone;
            _futureOrdersService = futureOrdersService;
            _flagTrackerService = flagTrackerService;
        }

        public void AddUnit(UnitStatus unitStatus)
        {
            if (!_allUnitsInScene.Contains(unitStatus))
                _allUnitsInScene.Add(unitStatus);
        }

        public void RemoveUnit(UnitStatus unitStatus)
        {
            if (_allUnitsInScene.Contains(unitStatus))
                _allUnitsInScene.Remove(unitStatus);
        }

        public void EvacuateAllUnits()
        {
            Debug.Log("StartEvacuation");

            if (_allUnitsInScene == null || _allUnitsInScene.Count == 0)
                return;

            List<UnitStatus> _archers = _allUnitsInScene.FindAll(x => x.UnitTypeId == UnitTypeId.Archer);
            List<UnitStatus> _shielders = _allUnitsInScene.FindAll(x => x.UnitTypeId == UnitTypeId.Shielder);

            Transform lastRightSide = _flagTrackerService.GetLastFlag(true);

            List<UnitStatus> nearestArchers = _archers
                .OrderBy(x => lastRightSide.position.x - x.transform.position.x)
                .ToList();

            List<UnitStatus> nearestShielders = _shielders
                .OrderBy(x => lastRightSide.position.x - x.transform.position.x)
                .ToList();

            HandleBindingWarriors(nearestArchers, lastRightSide);
            HandleBindingWarriors(nearestShielders, lastRightSide);

            foreach (UnitStatus unitStatus in _allUnitsInScene)
            {
                if (IsBusy(unitStatus))
                    continue;

                if (unitStatus.UnitTypeId == UnitTypeId.Builder)
                {
                    OrderMarker currentOrderMarker =
                        _futureOrdersService.GetOrderByOrderUniqueId(unitStatus.OrderUniqueId);

                    if (currentOrderMarker != null && unitStatus.IsWorked &&
                        _safeBuildZone.IsSafeZone(currentOrderMarker.transform.position.x))
                        continue;

                    if (currentOrderMarker == null && unitStatus.IsWorked &&
                        _safeBuildZone.IsSafeZone(unitStatus.transform.position.x))
                        continue;

                    UnitStrategyBehaviour unitStrategyBehaviour =
                        unitStatus.GetComponentInChildren<UnitStrategyBehaviour>();
                    unitStrategyBehaviour.StopAllActions();

                    /*unitStatus.GetComponent<UnitMove>().ChangeTargetPosition();
                    unitStatus.Release();*/

                    BuilderStateMachineView builderStateMachineView =
                        unitStatus.GetComponent<BuilderStateMachineView>();
                    builderStateMachineView.ChangeState<BuilderScaryRunState>();
                }
            }
        }

        public void ReleaseOfDefenseUnits()
        {
            foreach (UnitStatus unitStatus in _allUnitsInScene)
            {
                if (unitStatus.IsBindedToPlayer())
                    continue;

                if (IsFightingUnit(unitStatus) || IsBuilderBindedToFlag(unitStatus))
                {
                    UnitStrategyBehaviour unitStrategyBehaviour =
                        unitStatus.GetComponentInChildren<UnitStrategyBehaviour>();
                    unitStrategyBehaviour.StopAllActions();

                    unitStatus.GetComponent<UnitMove>().ChangeTargetPosition();
                    unitStatus.Release();
                }
            }

            _futureOrdersService.ContinueExecuteOrders();
        }

        private void HandleBindingWarriors(List<UnitStatus> currentWarriors, Transform lastRightSide)
        {
            for (int i = 0; i < currentWarriors.Count; i++)
            {
                if (i < currentWarriors.Count / 2)
                {
                    if (lastRightSide.TryGetComponent(out FlagSlotCoordinator flagSlotCoordinator))
                        BindToLastFlag(flagSlotCoordinator, currentWarriors[i]);
                }
                else
                {
                    Transform lastLeftOnSide = _flagTrackerService.GetLastFlag(false);

                    if (lastLeftOnSide.TryGetComponent(out FlagSlotCoordinator flagSlotCoordinator))
                        BindToLastFlag(flagSlotCoordinator, currentWarriors[i]);
                }
            }
        }

        private void BindToLastFlag(FlagSlotCoordinator flagSlotCoordinator, UnitStatus unitStatus)
        {
            flagSlotCoordinator.BindUnitToSlot(unitStatus.transform, unitStatus.UnitTypeId);
            flagSlotCoordinator.RelocateUnits();

            if (flagSlotCoordinator.HasEnemiesAroundBarricade)
                flagSlotCoordinator.PrepareForDefense();
        }

        private bool IsBusy(UnitStatus unitStatus)
        {
            return unitStatus.IsAutomaticBindedToPlayer || unitStatus.PlayerMove != null ||
                   unitStatus.IsDefensedFlag || unitStatus.BindedToFlagUniqueId.IsEmpty() == false;
        }

        private bool IsBuilderBindedToFlag(UnitStatus unitStatus) =>
            unitStatus.UnitTypeId == UnitTypeId.Builder && unitStatus.BindedToFlagUniqueId.IsEmpty() == false;

        private bool IsFightingUnit(UnitStatus unitStatus) =>
            unitStatus.UnitTypeId == UnitTypeId.Archer || unitStatus.UnitTypeId == UnitTypeId.Shielder;
    }
}