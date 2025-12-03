using Infastructure.Services.Cards;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.PlayerRegistry;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.StaticDataService;
using Player.Orders;
using UI.GameplayUI.SpeachBubleUI;
using UI.GameplayUI.TowerSelectionUI.Tower;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class BarricadeHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private OrderSelectionUI _orderSelectionUI;
        [SerializeField] private OrderMarker _orderMarker;
        [SerializeField] private BuildInfo _buildInfo;

        private IPlayerRegistryService _playerRegistryService;
        private ICardSpawnService _cardSpawnService;
        private IStaticDataService _staticDataService;
        private IPersistentProgressService _persistentProgressService;

        private SpeachBuble _speachBuble;
        private IDestroyCommandExecutor _destroyCommandExecutor;

        [Inject]
        public void Construct(
            IPlayerRegistryService playerRegistryService,
            ICardSpawnService cardSpawnService,
            IStaticDataService staticDataService,
            IPersistentProgressService persistentProgressService,
            IDestroyCommandExecutor destroyCommandExecutor)
        {
            _destroyCommandExecutor = destroyCommandExecutor;
            _persistentProgressService = persistentProgressService;
            _staticDataService = staticDataService;
            _cardSpawnService = cardSpawnService;
            _playerRegistryService = playerRegistryService;
        }

        private void Awake() => 
            _speachBuble = _playerRegistryService.Player.GetComponentInChildren<SpeachBuble>();

        public void Handle()
        {
            if (!_orderMarker.IsStarted && _orderMarker.OrderID != OrderID.Heal)
            {
                switch (_orderSelectionUI.OrderSelectionId)
                {
                    case OrderSelectionId.LevelUp:
                        ShowCardWindow();
                        break;
                    case OrderSelectionId.Destroy:
                        DestroyCommandExecute();
                        break;
                }
            }
        }

        private void ShowCardWindow()
        {
            if (IsEnoughtCoins(_orderMarker))
                _cardSpawnService.ShowCardsWindow(_orderMarker);
            else
                _speachBuble.UpdateSpeach(SpeachBubleId.Coins);
        }

        private void DestroyCommandExecute() =>
            _destroyCommandExecutor.DestroyBuild(_buildInfo);

        private bool IsEnoughtCoins(OrderMarker orderMarker)
        {
            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();
            BuildingUpgradeData buildingUpgradeData =
                _staticDataService.ForBuilding(buildInfo.BuildingTypeId, buildInfo.NextBuildingLevelId,
                    buildInfo.CardKey);

            return _persistentProgressService.PlayerProgress.CoinData.IsEnoughCoins(buildingUpgradeData.CoinsValue);
        }
    }
}