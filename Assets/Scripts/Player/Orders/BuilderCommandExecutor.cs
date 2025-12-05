using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bonfire.Builds;
using BuildProcessManagement;
using BuildProcessManagement.Towers;
using Flag;
using FogOfWar;
using Infastructure;
using Infastructure.Data;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.BuildingRegistry;
using Infastructure.Services.Fence;
using Infastructure.Services.MarkerSignCoordinator;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.ResourceLimiter;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.StaticDataService;
using MinimapCore;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.SpeachBubleUI;
using Units;
using Units.UnitStates;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Player.Orders
{
    public class BuilderCommandExecutor : IBuilderCommandExecutor
    {
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly IPersistentProgressService _progressService;
        private readonly IStaticDataService _staticData;
        private readonly IResourceLimiterService _resourceLimiterService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IBuildingRegistryService _buildingRegistryService;
        private readonly IFogOfWarMinimap _fogOfWarMinimap;
        private readonly IFenceService _fenceService;
        private readonly IMarkerSignCoordinatorService _markerSignCoordinatorService;

        public Action OnBuildHappened { get; set; }

        private SpeachBuble _speachBuble;
        private SelectUnitArrow _selectUnitArrow;


        public BuilderCommandExecutor(
            ISafeBuildZone safeBuildZone,
            IPersistentProgressService progressService,
            IStaticDataService staticDataService,
            IResourceLimiterService resourceLimiterService,
            ICoroutineRunner coroutineRunner,
            IBuildingRegistryService buildingRegistryService,
            IFogOfWarMinimap fogOfWarMinimap,
            IFenceService fenceService,
            IMarkerSignCoordinatorService markerSignCoordinatorService)
        {
            _coroutineRunner = coroutineRunner;
            _buildingRegistryService = buildingRegistryService;
            _fogOfWarMinimap = fogOfWarMinimap;
            _fenceService = fenceService;
            _markerSignCoordinatorService = markerSignCoordinatorService;
            _safeBuildZone = safeBuildZone;
            _progressService = progressService;
            _staticData = staticDataService;
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

            if (!IsOrderExecutable(orderMarker))
                return;

            if (_safeBuildZone.IsNight && _safeBuildZone.IsSafeZone(orderMarker.transform.position.x))
                GiveOrderAfterBuildMode(orderMarker);
            else
                GiveOrderAfterBuildMode(orderMarker);

            OnBuildHappened?.Invoke();
        }


        public void StartBuildAfterBuildingMode(OrderMarker orderMarker)
        {
            MarkingBuild markingBuild = orderMarker.GetComponent<MarkingBuild>();
            markingBuild.PrepareBuild();

            if (_safeBuildZone.IsNight)
            {
                if (_safeBuildZone.IsSafeZone(orderMarker.transform.position.x))
                    _coroutineRunner.StartCoroutine(StartBuildCoroutine(orderMarker));
            }
            else
                _coroutineRunner.StartCoroutine(StartBuildCoroutine(orderMarker));
        }


        private void GiveOrderAfterBuildMode(OrderMarker orderMarker)
        {
            TryBuild(orderMarker, out bool canBuild);

            if (!canBuild)
                return;

            RegisterBuild(orderMarker);
            _coroutineRunner.StartCoroutine(StartBuildCoroutine(orderMarker));
        }

        private void TryBuild(OrderMarker orderMarker, out bool canBuild)
        {
            Debug.Log("TryBuild");

            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();

            BuildingUpgradeData buildingUpgradeNextData =
                _staticData.ForBuilding(buildInfo.BuildingTypeId, buildInfo.NextBuildingLevelId, buildInfo.CardKey);

            if (buildingUpgradeNextData == null && !orderMarker.IsStarted)
            {
                canBuild = false;
                return;
            }

            if (!orderMarker.IsStarted)
            {
                BuildingCoinsUI buildingCoinsUI = orderMarker.GetComponentInChildren<BuildingCoinsUI>();
                buildingCoinsUI.PlaySpendAnimation(buildingUpgradeNextData.CoinsValue, orderMarker.transform);

                _progressService.PlayerProgress.CoinData.Spend(buildingUpgradeNextData.CoinsValue);
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

            if (buildingUpgradeNextData == null)
                return false;

            if (orderMarker.OrderID == OrderID.Build &&
                !_progressService.PlayerProgress.CoinData.IsEnoughCoins(buildingUpgradeNextData.CoinsValue))
            {
                _speachBuble.UpdateSpeach(SpeachBubleId.Coins);
                return false;
            }

            return true;
        }

        private void RegisterBuild(OrderMarker orderMarker)
        {
            MarkingBuild markingBuild = orderMarker.GetComponent<MarkingBuild>();
            markingBuild.InitializeNextBuild();
            markingBuild.StartBuild();
        }

        private IEnumerator StartBuildCoroutine(OrderMarker orderMarker)
        {
            _markerSignCoordinatorService.AddMarker(orderMarker);

            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();
            BuildingProgress buildingProgress = orderMarker.GetComponent<BuildingProgress>();

            while (buildInfo.CurrentWoodsCount > 0)
            {
                buildingProgress.BuildWoods();

                yield return new WaitForSeconds(0.25f);
            }

            _markerSignCoordinatorService.RemoveMarker(orderMarker);

            ShowNewBuild(orderMarker);
        }

        private void ShowNewBuild(OrderMarker orderMarker)
        {
            orderMarker.IsStarted = false;

            BuildingProgress buildingProgress = orderMarker.GetComponent<BuildingProgress>();

            _fogOfWarMinimap.UpdateFogPosition(orderMarker.transform.position.x);

            ShowBarricadeFlag(orderMarker);
            ShowUpgradedBuild(orderMarker);
            ClearOldBuildingProgress(orderMarker);

            buildingProgress.ShakeAllWoods(5,
                () => _coroutineRunner.StartCoroutine(FinishBuildAnimationCoroutine(orderMarker)));
        }

        private void ClearOldBuildingProgress(OrderMarker orderMarker)
        {
            UniqueId uniqueId = orderMarker.GetComponent<UniqueId>();

            BuildingProgressData savedData =
                _progressService.PlayerProgress.WorldData.BuildingProgressData.FirstOrDefault(x =>
                    x.UniqueId == uniqueId.Id);

            _progressService.PlayerProgress.WorldData.BuildingProgressData.Remove(savedData);
        }

        private void ShowUpgradedBuild(OrderMarker orderMarker)
        {
            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();

            if (buildInfo.NextBuild != null)
            {
                DecorOnBuild decorOnBuild = buildInfo.GetComponent<DecorOnBuild>();
                decorOnBuild?.Hide();

                buildInfo.NextBuild.SetActive(true);
            }
        }

        private void ShowBarricadeFlag(OrderMarker orderMarker)
        {
            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();

            FlagActivator currentFlag = orderMarker.GetComponentInChildren<FlagActivator>();
            FlagActivator nextFlag = buildInfo.NextBuild.GetComponentInChildren<FlagActivator>();

            if (currentFlag == null && nextFlag == null)
                return;

            if (currentFlag == nextFlag)
                nextFlag.SpawnFlag();
            else if (currentFlag.HasFlag())
                nextFlag.Initialize(currentFlag.GetFlag());

            _fenceService.BuildFence((int)orderMarker.transform.position.x);
        }


        private IEnumerator FinishBuildAnimationCoroutine(OrderMarker orderMarker)
        {
            EnableFirstBuild(orderMarker);

            BuildInfo buildInfo = orderMarker.GetComponent<BuildInfo>();

            float centerOfWoodsX = buildInfo.GetComponentInChildren<WoodBuild>().transform.localPosition.x;
            float centerOfScaffoldsX = buildInfo.GetComponentInChildren<ScaffoldsBuild>().transform.localPosition.x;

            List<GameObject> woodsList = buildInfo.WoodsList.ToList();
            List<GameObject> scaffoldsList = buildInfo.ScaffoldsList.ToList();

            yield return AnimateBuildObjects(woodsList, centerOfWoodsX);
            yield return AnimateBuildObjects(scaffoldsList, centerOfScaffoldsX);

            RegisterNewBuild(orderMarker);
            DestroyPreviousBuild(orderMarker);
        }

        private void DestroyPreviousBuild(OrderMarker currentOrderMarker)
        {
            currentOrderMarker.GetComponent<BoxCollider2D>().enabled = true;

            BuildInfo buildInfo = currentOrderMarker.GetComponent<BuildInfo>();
            BuildingProgress buildingProgress = buildInfo.GetComponent<BuildingProgress>();

            if (currentOrderMarker.gameObject != buildInfo.NextBuild.gameObject)
            {
                _buildingRegistryService.RemoveBuild(buildInfo);
                Object.Destroy(currentOrderMarker.gameObject);
            }
            else
                DeleteMarkingBuild(buildInfo, buildingProgress);
        }

        private void DeleteMarkingBuild(BuildInfo buildInfo, BuildingProgress buildingProgress)
        {
            List<GameObject> currentWoodsList = new List<GameObject>(buildInfo.WoodsList);
            List<GameObject> currentScaffoldsList = new List<GameObject>(buildInfo.ScaffoldsList);

            foreach (GameObject woods in currentWoodsList)
                Object.Destroy(woods.gameObject);

            foreach (GameObject scaffold in currentScaffoldsList)
                Object.Destroy(scaffold.gameObject);

            buildInfo.WoodsList.Clear();
            buildInfo.ScaffoldsList.Clear();
            buildingProgress.Clear();

            buildInfo.CurrentWoodsCount = 0;
        }

        private void RegisterNewBuild(OrderMarker currentOrderMarker)
        {
            currentOrderMarker.GetComponent<BoxCollider2D>().enabled = false;

            Minimap minimap = currentOrderMarker.GetComponentInChildren<Minimap>();
            minimap.HideUpgradedProcess();

            BuildInfo buildInfo = currentOrderMarker.GetComponent<BuildInfo>();

            BuildInfo nextBuildInfo = buildInfo.NextBuild.GetComponent<BuildInfo>();
            _buildingRegistryService.AddBuild(nextBuildInfo);
        }

        private void EnableFirstBuild(OrderMarker currentOrderMarker)
        {
            BuildInfo buildInfo = currentOrderMarker.GetComponent<BuildInfo>();

            if (currentOrderMarker.gameObject == buildInfo.NextBuild.gameObject)
            {
                Minimap minimap = currentOrderMarker.GetComponentInChildren<Minimap>();
                minimap.Show();

                DecorOnBuild decorOnBuild = currentOrderMarker.GetComponent<DecorOnBuild>();
                decorOnBuild?.Show();
            }
        }

        private IEnumerator AnimateBuildObjects(IEnumerable<GameObject> objects, float centerX)
        {
            foreach (GameObject obj in objects)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                    AddRandomMovement(rb, centerX);
            }

            yield return new WaitForSeconds(0.75f);
        }

        private void AddRandomMovement(Rigidbody2D woodRb, float centerOfBuildX)
        {
            float deltaX = woodRb.transform.localPosition.x - centerOfBuildX;
            int randomHeight = Random.Range(3, 7);
            int randomX = Random.Range(-2, 2);

            woodRb.gravityScale = 1.5f;
            woodRb.AddRelativeForce(new Vector2(deltaX + randomX, randomHeight), ForceMode2D.Impulse);
        }
    }
}