using System;
using BuildProcessManagement;
using BuildProcessManagement.Towers;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.Cards;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.ResourceLimiter;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.SpeachBubleUI;
using Units;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Player.Orders
{
    public class BuilderCommandExecutor : IBuilderCommandExecutor
    {
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly IExecuteOrdersService _executeOrdersService;
        private readonly IPersistentProgressService _progressService;
        private readonly IFutureOrdersService _futureOrdersService;
        private readonly IStaticDataService _staticData;
        private readonly IUnitsRecruiterService _unitsRecruiterService;
        private readonly ICardTrackerService _cardTrackerService;
        private readonly IResourceLimiterService _resourceLimiterService;

        public Action OnBuildHappened { get; set; }

        private SpeachBuble _speachBuble;
        private SelectUnitArrow _selectUnitArrow;


        public BuilderCommandExecutor(
            ISafeBuildZone safeBuildZone,
            IExecuteOrdersService executeOrdersService,
            IPersistentProgressService progressService,
            IFutureOrdersService futureOrdersService,
            IStaticDataService staticDataService,
            IUnitsRecruiterService unitsRecruiterService,
            ICardTrackerService cardTrackerService,
            IResourceLimiterService resourceLimiterService)
        {
            _safeBuildZone = safeBuildZone;
            _executeOrdersService = executeOrdersService;
            _progressService = progressService;
            _futureOrdersService = futureOrdersService;
            _staticData = staticDataService;
            _unitsRecruiterService = unitsRecruiterService;
            _cardTrackerService = cardTrackerService;
            _resourceLimiterService = resourceLimiterService;
        }

        public void Initialize(SpeachBuble speachBuble, SelectUnitArrow selectUnitArrow)
        {
            _speachBuble = speachBuble;
            _selectUnitArrow = selectUnitArrow;
        }

        public void StartBuild(OrderMarker orderMarker)
        {
            if (orderMarker == null)
                return;

            int freePlaceIndex = _executeOrdersService.FreePlaceIndex(orderMarker);
            if (freePlaceIndex == -1 || !IsOrderExecutable(orderMarker))
                return;

            if (_safeBuildZone.IsNight)
            {
                if (_safeBuildZone.IsSafeZone(orderMarker.transform.position.x))
                    GiveOrderToBuilders(orderMarker, freePlaceIndex);
                else
                {
                    TryBuild(orderMarker, out bool canBuild);

                    if (canBuild)
                        _futureOrdersService.AddOrder(orderMarker);
                }
            }
            else
                GiveOrderToBuilders(orderMarker, freePlaceIndex);

            OnBuildHappened?.Invoke();
            _unitsRecruiterService.RelocateRemainingUnitsToPlayer();
        }

        public void StartHarvest(OrderMarker orderMarker)
        {
            if (orderMarker == null)
                return;

            int freePlaceIndex = _executeOrdersService.FreePlaceIndex(orderMarker);
            if (freePlaceIndex == -1 || !IsOrderExecutable(orderMarker))
                return;

            if (_safeBuildZone.IsNight)
            {
                if (_safeBuildZone.IsSafeZone(orderMarker.transform.position.x))
                    GiveHarvestOrder(orderMarker, freePlaceIndex);
                else
                    _futureOrdersService.AddOrder(orderMarker);
            }
            else
                GiveHarvestOrder(orderMarker, freePlaceIndex);
        }

        public void StartBuildAfterBuildingMode(OrderMarker orderMarker)
        {
            MarkingBuild markingBuild = orderMarker.GetComponent<MarkingBuild>();
            markingBuild.PrepareBuild();

            if (_safeBuildZone.IsNight)
            {
                if (_safeBuildZone.IsSafeZone(orderMarker.transform.position.x))
                    GiveOrderAfterBuildMode(orderMarker);
            }
            else
                GiveOrderAfterBuildMode(orderMarker);

            _futureOrdersService.AddOrder(orderMarker);
            _unitsRecruiterService.RelocateRemainingUnitsToPlayer();
        }

        private void GiveHarvestOrder(OrderMarker orderMarker, int freePlaceIndex)
        {
            bool isSelectingUnit = false;

            if (_selectUnitArrow.IsActive())
            {
                int correctIndex = _selectUnitArrow.SelectableUnitIndex - 1;
                UnitTypeId unitType = _unitsRecruiterService.GetUnitType(correctIndex);

                if (unitType == UnitTypeId.Builder)
                {
                    isSelectingUnit = true;
                    _selectUnitArrow.UnSelectUnit();
                }
            }

            UnitStatus unitStatus = isSelectingUnit
                ? _unitsRecruiterService.ReleaseUnit(UnitTypeId.Anyone, _selectUnitArrow.SelectableUnitIndex - 1)
                : _unitsRecruiterService.ReleaseUnit(UnitTypeId.Builder);

            if (!orderMarker.IsStarted)
                orderMarker.IsStarted = true;

            if (unitStatus == null)
                _futureOrdersService.AddOrder(orderMarker);
            else
            {
                UnitMove unitMove = unitStatus.GetComponent<UnitMove>();
                GiveOrderToBuilder(unitMove, orderMarker, freePlaceIndex);
            }
        }

        private void GiveOrderAfterBuildMode(OrderMarker orderMarker)
        {
            int placesCount = orderMarker.Places.Count;
            for (int i = 0; i < placesCount; i++)
            {
                int freePlaceIndex = _executeOrdersService.FreePlaceIndex(orderMarker);
                if (freePlaceIndex != -1)
                    ExecuteOrderForBuilders(orderMarker, freePlaceIndex);
            }
        }

        private void GiveOrderToBuilders(OrderMarker orderMarker, int freePlaceIndex)
        {
            if (_selectUnitArrow.IsActive())
            {
                int correctIndex = _selectUnitArrow.SelectableUnitIndex - 1;
                UnitTypeId unitType = _unitsRecruiterService.GetUnitType(correctIndex);

                if (unitType == UnitTypeId.Builder)
                {
                    ExecuteOrderForBuilders(orderMarker, freePlaceIndex, true);
                    _selectUnitArrow.UnSelectUnit();
                }
            }
            else
            {
                ExecuteOrderForBuilders(orderMarker, freePlaceIndex);
            }
        }

        private void ExecuteOrderForBuilders(OrderMarker orderMarker, int freePlaceIndex, bool isSelectingUnit = false)
        {
            UnitStatus unitStatus = isSelectingUnit
                ? _unitsRecruiterService.ReleaseUnit(UnitTypeId.Anyone, _selectUnitArrow.SelectableUnitIndex - 1)
                : _unitsRecruiterService.ReleaseUnit(UnitTypeId.Builder);

            TryBuild(orderMarker, out bool canBuild);

            if (!canBuild)
                return;

            if (unitStatus == null)
                _futureOrdersService.AddOrder(orderMarker);
            else
            {
                UnitMove unitMove = unitStatus.GetComponent<UnitMove>();
                GiveOrderToBuilder(unitMove, orderMarker, freePlaceIndex);
            }
        }

        private void TryBuild(OrderMarker orderMarker, out bool canBuild)
        {
            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();

            BuildingUpgradeData buildingUpgradeNextData =
                _staticData.ForBuilding(buildInfo.BuildingTypeId, buildInfo.NextBuildingLevelId, buildInfo.CardKey);

            if (buildingUpgradeNextData == null && (!orderMarker.IsMarkered || !orderMarker.IsStarted))
            {
                canBuild = false;
                return;
            }

            if (!orderMarker.IsStarted && orderMarker.OrderID == OrderID.Build)
            {
                BuildingCoinsUI buildingCoinsUI = orderMarker.GetComponentInChildren<BuildingCoinsUI>();
                buildingCoinsUI.PlaySpendAnimation(buildingUpgradeNextData.CoinsValue, orderMarker.transform);

                _progressService.PlayerProgress.CoinData.Spend(buildingUpgradeNextData.CoinsValue);

                MarkingBuild markingBuild = orderMarker.GetComponent<MarkingBuild>();
                markingBuild.InitializeNextBuild();
                markingBuild.StartBuild();
            }

            canBuild = true;
        }

        private bool IsOrderExecutable(OrderMarker orderMarker)
        {
            if (orderMarker.OrderID == OrderID.Chop || orderMarker.OrderID == OrderID.Dig)
                return _resourceLimiterService.IsActive(orderMarker);

            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();

            BuildingUpgradeData buildingUpgradeNextData =
                _staticData.ForBuilding(buildInfo.BuildingTypeId, buildInfo.NextBuildingLevelId, buildInfo.CardKey);

            BarricadeVisibilityZone visibilityZone = orderMarker.GetComponentInChildren<BarricadeVisibilityZone>();
            if (visibilityZone != null && visibilityZone.HasVisiableEnemy)
                return false;

            if (!orderMarker.IsMarkered && buildingUpgradeNextData == null)
                return false;

            if (!orderMarker.IsMarkered && orderMarker.OrderID == OrderID.Build &&
                !_progressService.PlayerProgress.CoinData.IsEnoughCoins(buildingUpgradeNextData.CoinsValue))
            {
                _speachBuble.UpdateSpeach(SpeachBubleId.Coins);
                return false;
            }

            return true;
        }

        private void GiveOrderToBuilder(UnitMove unitMove, OrderMarker orderMarker, int freePlaceIndex)
        {
            BuilderBehaviour builderBehaviour =
                unitMove.GetComponentInChildren<BuilderBehaviour>();
            _executeOrdersService.ExecuteOrder(builderBehaviour, orderMarker, freePlaceIndex,
                _futureOrdersService.RemoveCompletedOrder, _futureOrdersService.ContinueExecuteOrders);
        }
    }
}