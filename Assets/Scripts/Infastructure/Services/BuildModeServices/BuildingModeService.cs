using System;
using BuildProcessManagement;
using Grid;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.BuildingCatalog;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.ResourceLimiter;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.StaticDataService;
using MinimapCore;
using Player.Orders;
using UI.GameplayUI;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.SpeachBubleUI;
using UI.GameplayUI.TowerSelectionUI.MoveItems;
using Units;
using UnityEngine;

namespace Infastructure.Services.BuildModeServices
{
    public class BuildingModeService : IBuildingModeService
    {
        public bool IsBuildingState { get; private set; }

        public event Action OnBuildingStateChanged;

        private readonly IGridMap _gridMap;
        private readonly IStaticDataService _staticData;
        private readonly IPersistentProgressService _progressService;
        private readonly IGameFactory _gameFactory;
        private readonly IBuilderCommandExecutor _builderCommandExecutor;
        private readonly IBuildingModeConfigurationService _configurationService;
        private readonly IBuildingCatalogService _buildingCatalogService;
        private readonly IResourceLimiterService _resourceLimiterService;

        private MoveBuildingUI _moveBuildingUI;
        private SpriteRenderer _ghostSpriteRender;
        private Transform _buildingGhostTransform;
        private BuildingCoinsUI _buildingCoinsUI;
        private BuildHintsUI _buildHintsUI;

        private Transform _rightResource;
        private Transform _leftResource;
        private SpeachBuble _speachBuble;


        public BuildingModeService(
            IGridMap gridMap,
            IStaticDataService staticData,
            IPersistentProgressService progressService,
            IGameFactory gameFactory,
            IBuilderCommandExecutor builderCommandExecutor,
            IBuildingModeConfigurationService configurationService,
            IBuildingCatalogService buildingCatalogService,
            IResourceLimiterService resourceLimiterService)
        {
            _gridMap = gridMap;
            _staticData = staticData;
            _progressService = progressService;
            _gameFactory = gameFactory;
            _builderCommandExecutor = builderCommandExecutor;
            _configurationService = configurationService;
            _buildingCatalogService = buildingCatalogService;
            _resourceLimiterService = resourceLimiterService;
        }


        public void SubscribeUpdates() =>
            _resourceLimiterService.OnResourceChanged += ChangeEnteredResource;

        public void Cleanup() =>
            _resourceLimiterService.OnResourceChanged -= ChangeEnteredResource;

        public void Initialize(
            MoveBuildingUI moveBuilding,
            BuildHintsUI buildHints,
            SpriteRenderer ghostSpriteRender,
            BuildingCoinsUI buildingCoinsUI,
            SpeachBuble speachBuble)
        {
            _speachBuble = speachBuble;
            _moveBuildingUI = moveBuilding;
            _buildHintsUI = buildHints;
            _ghostSpriteRender = ghostSpriteRender;
            _buildingCoinsUI = buildingCoinsUI;

            _buildingGhostTransform = _ghostSpriteRender.transform;
        }


        public bool CanOccupyCells(int currentPosition)
        {
            BuildingTypeId buildingTypeId =
                _configurationService.GetItem();

            if (buildingTypeId == BuildingTypeId.Unknow)
            {
                Debug.Log("buildingTypeId is BuildingTypeId.Unknow");

                return false;
            }


            BuildingUpgradeData buildingUpgradeData =
                _staticData.ForBuilding(buildingTypeId, BuildingLevelId.Level1, CardId.Default);

            if (buildingUpgradeData == null)
            {
                Debug.Log("buildingUpgradeData is null");

                return false;
            }


            bool canOccupy = _gridMap.AreCellsFree(currentPosition) &&
                             IsEnoughCoins(buildingUpgradeData);

            if (canOccupy)
            {
                _gridMap.OccupyCells(currentPosition);

                CreateBuild(currentPosition, buildingUpgradeData, buildingTypeId);
                SpendCoins(buildingUpgradeData);
                StopBuildingState();
            }

            return canOccupy;
        }


        public void StartBuildingState()
        {
            IsBuildingState = true;
            OnBuildingStateChanged?.Invoke();

            _buildingCoinsUI.Show();

            _moveBuildingUI.gameObject.SetActive(true);
            _moveBuildingUI.ReInitialize();

            _buildHintsUI.gameObject.SetActive(true);
            _ghostSpriteRender.enabled = true;
        }

        public void StopBuildingState()
        {
            IsBuildingState = false;
            OnBuildingStateChanged?.Invoke();

            _buildingCoinsUI.Hide();

            _moveBuildingUI.gameObject.SetActive(false);
            _buildHintsUI.gameObject.SetActive(false);
            _ghostSpriteRender.enabled = false;
        }

        public void RegistBuild()
        {
            if (_configurationService.BuildingTypeInfos.Count == 0)
                return;

            BuildingTypeId buildingTypeId =
                _configurationService.BuildingTypeInfos[_configurationService.CorrectIndex].BuildingTypeId;

            BuildingUpgradeData buildingUpgradeData =
                _staticData.ForBuilding(buildingTypeId, BuildingLevelId.Level1, CardId.Default);
            BuildingStaticData buildingStaticData = _staticData.ForBuilding(buildingTypeId);

            _buildingCoinsUI.UpdateCoinsUI(buildingUpgradeData.CoinsValue);
            _buildHintsUI.UpdateText(buildingStaticData.KeyboardHint);
            _gridMap.RegisterBuild(buildingUpgradeData, buildingStaticData);

            _ghostSpriteRender.sprite = buildingStaticData.GhostSprite;
        }

        public void MoveGhost(Vector3 position) =>
            _buildingGhostTransform.position = position;

        public void PaintGhost(int currentPosition) =>
            _ghostSpriteRender.color = _gridMap.AreCellsFree(currentPosition) ? Color.white : Color.red;

        private void ChangeEnteredResource(bool isRight, Transform resource)
        {
            if (isRight)
                _rightResource = resource;
            else
                _leftResource = resource;
        }


        private void CreateBuild(int currentPosition, BuildingUpgradeData buildingUpgradeData,
            BuildingTypeId buildingTypeId)
        {
            GameObject building =
                _gameFactory.CreateFirstBuilding(buildingUpgradeData, buildingTypeId,
                    new Vector3(currentPosition, -2.75f, 0));

            BuildingCoinsUI buildingCoinsUI = building.GetComponentInChildren<BuildingCoinsUI>();
            if (buildingCoinsUI == null)
                return;

            DecorOnBuild decorOnBuild = building.GetComponent<DecorOnBuild>();
            decorOnBuild?.Hide();

            OrderMarker orderMarker = building.GetComponent<OrderMarker>();
            orderMarker.IsStarted = true;

            BuildInfo buildingInfo = building.GetComponent<BuildInfo>();
            buildingInfo.NextBuild = building;

            UniqueId uniqueId = building.GetComponent<UniqueId>();
            uniqueId.Id = GetUniqueId();

            Minimap minimap = building.GetComponentInChildren<Minimap>();
            minimap.Hide();

            _builderCommandExecutor.StartBuildAfterBuildingMode(orderMarker);
            _buildingCoinsUI.PlaySpendAnimation(buildingUpgradeData.CoinsValue, building.transform);
            _configurationService.RemoveItemUI(buildingInfo.BuildingTypeId);
            _buildingCatalogService.RemoveCatalogItem(buildingInfo.BuildingTypeId);
        }

        private void SpendCoins(BuildingUpgradeData buildingUpgradeData) =>
            _progressService.PlayerProgress.CoinData.Spend(buildingUpgradeData.CoinsValue);

        private bool IsEnoughCoins(BuildingUpgradeData buildingUpgradeData)
        {
            bool isEnoughCoins = _progressService.PlayerProgress.CoinData.IsEnoughCoins(buildingUpgradeData.CoinsValue);

            if (!isEnoughCoins)
                _speachBuble.UpdateSpeach(SpeachBubleId.Coins);

            return isEnoughCoins;
        }


        private string GetUniqueId() =>
            Guid.NewGuid().ToString();
    }
}