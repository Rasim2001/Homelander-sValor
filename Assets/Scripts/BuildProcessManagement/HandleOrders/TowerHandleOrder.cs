using BuildProcessManagement.Towers;
using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.Cards;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Player;
using Player.Orders;
using UI.GameplayUI.SpeachBubleUI;
using UI.GameplayUI.TowerSelectionUI.Tower;
using Units;
using Units.HomelessUnits;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class TowerHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private TowerUnitSpawner _towerUnitSpawner;
        [SerializeField] private OrderMarker _orderMarker;
        [SerializeField] private OrderSelectionUI _orderSelectionUI;
        [SerializeField] private BuildInfo _buildInfo;

        private IPlayerRegistryService _playerRegistryService;
        private IUnitsRecruiterService _unitsRecruiterService;
        private IHomelessOrdersService _homelessOrdersService;
        private IStaticDataService _staticDataService;
        private IPersistentProgressService _persistentProgressService;
        private ICardSpawnService _cardSpawnService;
        private IDestroyCommandExecutor _destroyCommandExecutor;

        private SpeachBuble _speachBuble;
        private SelectUnitArrow _selectUnitArrow;

        [Inject]
        public void Construct(
            IPlayerRegistryService playerRegistryService,
            IUnitsRecruiterService unitsRecruiterService,
            IHomelessOrdersService homelessOrdersService,
            IStaticDataService staticDataService,
            IPersistentProgressService persistentProgressService,
            ICardSpawnService cardSpawnService,
            IDestroyCommandExecutor destroyCommandExecutor)
        {
            _playerRegistryService = playerRegistryService;
            _unitsRecruiterService = unitsRecruiterService;
            _homelessOrdersService = homelessOrdersService;
            _staticDataService = staticDataService;
            _persistentProgressService = persistentProgressService;
            _cardSpawnService = cardSpawnService;
            _destroyCommandExecutor = destroyCommandExecutor;
        }


        public void Awake()
        {
            _speachBuble = _playerRegistryService.Player.GetComponentInChildren<SpeachBuble>();
            _selectUnitArrow = _playerRegistryService.Player.GetComponentInChildren<SelectUnitArrow>();
        }

        public void Handle()
        {
            if (_orderMarker.IsStarted)
                return;

            switch (_orderSelectionUI.OrderSelectionId)
            {
                case OrderSelectionId.OnTower:
                    OnTowerExecute();
                    break;
                case OrderSelectionId.OnGround:
                    OnGroundExecute();
                    break;
                case OrderSelectionId.LevelUp:
                    ShowCardWindowTower();
                    break;
                case OrderSelectionId.Destroy:
                    DestroyCommandExecute();
                    break;
            }
        }

        private void OnTowerExecute()
        {
            BindToTower();
            _unitsRecruiterService.RelocateRemainingUnitsToPlayer();
        }

        private void OnGroundExecute()
        {
            GameObject homelessObject = _towerUnitSpawner.SpawnHomeless();
            if (homelessObject != null)
            {
                UnitStatus unitStatus = homelessObject.GetComponent<UnitStatus>();
                _unitsRecruiterService.AddUnitToList(unitStatus);
                _unitsRecruiterService.BindUnitToPlayer(unitStatus);
            }
        }

        private void ShowCardWindowTower()
        {
            if (IsEnoughtCoins())
                _cardSpawnService.ShowCardsWindow(_orderMarker, onCardSelected: ReleaseAllUnitsOnTower);
            else
                _speachBuble.UpdateSpeach(SpeachBubleId.Coins);
        }

        private void BindToTower()
        {
            if (!_towerUnitSpawner.HasAvailableSlot())
            {
                _speachBuble.UpdateSpeach(SpeachBubleId.TowerCapacityFull);
                return;
            }

            int correctSelectableUnitIndex = _selectUnitArrow.IsActive()
                ? _selectUnitArrow.SelectableUnitIndex - 1
                : _selectUnitArrow.SelectableUnitIndex;

            UnitTypeId unitType = _unitsRecruiterService.GetUnitType(correctSelectableUnitIndex);

            UnitStatus unitStatus = _selectUnitArrow.IsActive()
                ? unitType != UnitTypeId.Homeless
                    ? null
                    : _unitsRecruiterService.ReleaseUnit(UnitTypeId.Anyone, correctSelectableUnitIndex)
                : _unitsRecruiterService.ReleaseUnit(UnitTypeId.Homeless);

            if (_selectUnitArrow.IsActive())
                _selectUnitArrow.UnSelectUnit();

            if (unitStatus != null)
            {
                HomelessBehaviour homelessBehaviour =
                    unitStatus.GetComponentInChildren<HomelessBehaviour>();

                float targetFreePosition = transform.position.x;
                homelessBehaviour.PlayHomelessOrderBehavior(_towerUnitSpawner, targetFreePosition,
                    () => _homelessOrdersService.CompleteOrder(_towerUnitSpawner, unitStatus));
            }
            else
                _speachBuble.UpdateSpeach(SpeachBubleId.Homeless);
        }

        private bool IsEnoughtCoins()
        {
            BuildInfo buildInfo = _orderMarker.GetComponent<BuildInfo>();
            BuildingUpgradeData buildingUpgradeData =
                _staticDataService.ForBuilding(buildInfo.BuildingTypeId, buildInfo.NextBuildingLevelId,
                    buildInfo.CardKey);

            return _persistentProgressService.PlayerProgress.CoinData.IsEnoughCoins(buildingUpgradeData.CoinsValue);
        }

        private void ReleaseAllUnitsOnTower()
        {
            _homelessOrdersService.ReleaseOtherUnits(_towerUnitSpawner);

            int amountOfUnitsOnTower = _towerUnitSpawner.GetAmountOfUnits();

            for (int i = 0; i < amountOfUnitsOnTower; i++)
            {
                GameObject spawnHomeless = _towerUnitSpawner.SpawnHomeless();
                UniqueId uniqueId = _orderMarker.GetComponent<UniqueId>();

                HomelessMove homelessMove = spawnHomeless.GetComponent<HomelessMove>();
                homelessMove.ChangeTargetPosition(_orderMarker.transform.position.x);

                UnitStrategyBehaviour unitStrategyBehaviour =
                    spawnHomeless.GetComponentInChildren<UnitStrategyBehaviour>();
                unitStrategyBehaviour.StopAllActions();

                UnitStatus unitStatus = spawnHomeless.GetComponent<UnitStatus>();
                unitStatus.IsWorked = true;
                unitStatus.OrderUniqueId = uniqueId.Id;
                unitStatus.Release();

                _homelessOrdersService.AddHomelessTemp(unitStatus);
            }
        }

        private void DestroyCommandExecute()
        {
            _homelessOrdersService.ReleaseOtherUnits(_towerUnitSpawner);
            _homelessOrdersService.RemoveOrder(_towerUnitSpawner);
            
            int amountOfUnitsOnTower = _towerUnitSpawner.GetAmountOfUnits();

            for (int i = 0; i < amountOfUnitsOnTower; i++)
                _towerUnitSpawner.SpawnHomeless();

            _destroyCommandExecutor.DestroyBuild(_buildInfo);
        }
    }
}