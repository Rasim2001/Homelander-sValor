using System;
using System.Collections.Generic;
using System.Linq;
using _Tutorial;
using CameraManagement;
using CutScenes;
using DayCycle;
using DG.Tweening;
using Flag;
using Grid;
using Infastructure.Data;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.CallNight;
using Infastructure.Services.CameraFocus;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.Forest;
using Infastructure.Services.HudFader;
using Infastructure.Services.NearestBuildFind;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.Pool;
using Infastructure.Services.ProgressWatchers;
using Infastructure.Services.ResourceLimiter;
using Infastructure.Services.Tutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.UnitRecruiter;
using Infastructure.Services.VagabondCampManagement;
using Infastructure.StaticData;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using Infastructure.StaticData.Cheats;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Infastructure.StaticData.VagabondCampManagement;
using Loots;
using Player;
using Player.Orders;
using UI.GameplayUI;
using UI.GameplayUI.CristalUI;
using UI.GameplayUI.HudUI;
using Units;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using Units.Vagabond;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;
using BuildingData = Infastructure.Data.BuildingData;

namespace Infastructure.States
{
    public class BuildLevelState : IInitializable, IDisposable
    {
        private readonly IGameFactory _gameFactory;
        private readonly IStaticDataService _staticData;
        private readonly IGameUIFactory _uiFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly IFutureOrdersService _futureOrdersService;
        private readonly IEnemyWavesService _enemyWavesService;
        private readonly IEnemySpawnService _enemySpawnService;
        private readonly IExecuteOrdersService _executeOrdersService;
        private readonly IPoolObjects<CoinLoot> _poolLoots;
        private readonly ITutorialSpawnService _tutorialSpawnService;
        private readonly ITutorialService _tutorialService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGridMap _gridMap;
        private readonly IUnitsRecruiterService _unitsRecruiterService;
        private readonly IForestTransitionService _forestTransitionService;
        private readonly IBuildingModeService _buildingModeService;
        private readonly ICristalTimeline _cristalTimeline;
        private readonly ICameraFocusService _cameraFocusService;
        private readonly IVagabondCampService _vagabondCampService;
        private readonly IResourceLimiterService _resourceLimiterService;
        private readonly IHudFaderService _hudFaderService;
        private readonly ICallNightService _callNightService;
        private readonly ITutorialProgressService _tutorialProgressService;

        private readonly List<OrderInfo> _orderInfos = new List<OrderInfo>();
        private readonly List<FlagSlotCoordinator> _buildingFlags = new List<FlagSlotCoordinator>();
        private readonly List<OrderMarker> _allOrderMarkers = new List<OrderMarker>();

        private Coroutine _coroutine;
        private Camera _camera;

        public BuildLevelState(
            IGameFactory gameFactory,
            IStaticDataService staticData,
            IGameUIFactory uiFactory,
            IPersistentProgressService progressService,
            IFutureOrdersService futureOrdersService,
            IEnemyWavesService enemyWavesService,
            IEnemySpawnService enemySpawnService,
            IProgressWatchersService progressWatchersService,
            IExecuteOrdersService executeOrdersService,
            IPoolObjects<CoinLoot> poolLoots,
            ITutorialSpawnService tutorialSpawnService,
            INearestBuildFindService nearestBuildFindService,
            ICoroutineRunner coroutineRunner,
            IGridMap gridMap,
            IUnitsRecruiterService unitsRecruiterService,
            IForestTransitionService forestTransitionService,
            IBuildingModeService buildingModeService,
            ICristalTimeline cristalTimeline,
            ICameraFocusService cameraFocusService,
            IVagabondCampService vagabondCampService,
            IResourceLimiterService resourceLimiterService,
            IHudFaderService hudFaderService,
            ICallNightService callNightService,
            ITutorialProgressService tutorialProgressService,
            ITutorialService tutorialService
        )
        {
            _gameFactory = gameFactory;
            _staticData = staticData;
            _uiFactory = uiFactory;
            _progressService = progressService;
            _futureOrdersService = futureOrdersService;
            _enemyWavesService = enemyWavesService;
            _enemySpawnService = enemySpawnService;
            _executeOrdersService = executeOrdersService;
            _poolLoots = poolLoots;
            _tutorialSpawnService = tutorialSpawnService;
            _coroutineRunner = coroutineRunner;
            _gridMap = gridMap;
            _unitsRecruiterService = unitsRecruiterService;
            _forestTransitionService = forestTransitionService;
            _buildingModeService = buildingModeService;
            _cristalTimeline = cristalTimeline;
            _cameraFocusService = cameraFocusService;
            _vagabondCampService = vagabondCampService;
            _resourceLimiterService = resourceLimiterService;
            _hudFaderService = hudFaderService;
            _callNightService = callNightService;
            _tutorialProgressService = tutorialProgressService;
            _tutorialService = tutorialService;
        }

        public void Initialize()
        {
            //_tutorialProgressService.TutorialStarted = true;

            _camera = Camera.main;

            _callNightService.SubscribeUpdates();
            _coroutineRunner.StopAllCoroutines();
            _forestTransitionService.SubscribeUpdates();
            _buildingModeService.SubscribeUpdates();


            InitializeGridMap();
            InitGameWorld();
            InitProgressReaders();
            GenerateLootMap();
        }

        private void InitializeGridMap() =>
            _gridMap.Initialize();

        private void InitTutorial()
        {
            if (_tutorialProgressService.TutorialStarted)
                _tutorialSpawnService.StartSpawn();
        }

        private void InitUI() =>
            _uiFactory.CreateMenuRootUI();

        private void GenerateLootMap()
        {
            List<LootData> lootDatas = _progressService.PlayerProgress.CoinData.LootDatas;

            if (lootDatas.Count == 0)
                return;

            foreach (LootData lootData in lootDatas)
            {
                CoinLoot coinLoot = _poolLoots.GetObjectFromPool();
                coinLoot.UniqueId = lootData.UniqueId;
                coinLoot.transform.position = lootData.Position.AsUnityVector();
            }
        }


        public void Dispose()
        {
            _forestTransitionService.Cleanup();
            _buildingModeService.Cleanup();
            _vagabondCampService.CleanUp();
            _resourceLimiterService.CleanUp();
            _hudFaderService.CleanUp();
        }


        private void InitProgressReaders()
        {
            /*List<ISavedProgressReader> readersCopy =
                new List<ISavedProgressReader>(_progressWatchersService.ProgressReaders);

            foreach (ISavedProgressReader progressReader in readersCopy)
                progressReader.LoadProgress(_progressService.PlayerProgress);*/
        }

        private void ContinueExecuteOrders() =>
            _futureOrdersService.ContinueExecuteOrders();

        private void InitGameWorld()
        {
            InitUI();
            InitTutorial();
            GameObject playerObject = InitPlayer();

            if (_staticData.CheatStaticData.SkipCutScene)
                InitCristalUI(playerObject);

            InitBuildings();
            InitResources();
            InitUnits();
            InitCamera(playerObject);
            GameObject hudObject = InitHud();
            InitEnemyWaves(hudObject, playerObject);
            InitEnemyCamps();
            InitVagabondCamps();
            InitEnemyCrystals();

            _cristalTimeline.Initialize(playerObject.transform);
            //_tutorialService.Initialize();
        }

        private void InitEnemyCrystals()
        {
            List<EnemyCristalConfig> enemyCristalConfigs = _staticData.GameStaticData.EnemyCristalConfigs;

            foreach (EnemyCristalConfig cristalConfig in enemyCristalConfigs)
                _gameFactory.CreateEnemyCrystal(cristalConfig.Position, cristalConfig.Id);
        }


        private void InitCristalUI(GameObject playerObject)
        {
            if (_tutorialProgressService.TutorialStarted)
                return;

            DayCycleUpdater dayCycleUpdater = _camera.GetComponentInChildren<DayCycleUpdater>();
            GameObject cristalObject = _gameFactory.CreateCristalUI();

            Cristal cristal = cristalObject.GetComponent<Cristal>();
            Light2D cristalLight = cristalObject.GetComponent<Light2D>();

            PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
            PlayerInputOrders playerInputOrders = playerObject.GetComponent<PlayerInputOrders>();

            cristal.Initialize(playerMove, _cristalTimeline);
            playerInputOrders.InitCristal(cristal);
            dayCycleUpdater.InitializeCristalLight(cristalLight);
        }

        private void InitEnemyWaves(GameObject hudObject, GameObject playerObject)
        {
            WavesProgressBar wavesProgressBar = hudObject.GetComponent<WavesProgressBar>();
            WaveNotificatorUI waveNotificatorUI = hudObject.GetComponent<WaveNotificatorUI>();
            DayCycleUpdater dayCycleUpdater = _camera.GetComponentInChildren<DayCycleUpdater>();

            _enemyWavesService.Initialize(wavesProgressBar, dayCycleUpdater);
            _enemyWavesService.StartWaveCycle();

            _enemySpawnService.Initialize(playerObject.transform, waveNotificatorUI);
        }


        private GameObject InitHud() =>
            _gameFactory.CreateHUD();

        private void InitCamera(GameObject playerObject)
        {
            CinemachineFollow cinemachineFollow = _camera.GetComponent<CinemachineFollow>();

            cinemachineFollow.Initialize(playerObject);
            _cameraFocusService.Initialize(cinemachineFollow);
        }

        private void InitUnits()
        {
            if (IsNewProgress())
            {
                CheatStaticData cheatStaticData = _staticData.CheatStaticData;

                foreach (UnitCheatInfo unitCheatInfo in cheatStaticData.UnitCheatInfos)
                {
                    for (int i = 0; i < unitCheatInfo.Amount; i++)
                    {
                        GameObject unit = _gameFactory.CreateUnit(unitCheatInfo.UnitTypeId);

                        if (unitCheatInfo.UnitTypeId == UnitTypeId.Builder)
                            _futureOrdersService.AddBuilder(unit.GetComponent<UnitStatus>());
                    }
                }

                /*for (int i = 0; i < 10; i++)
                    _gameFactory.CreateUnit(UnitTypeId.Homeless);*/
                /*for (int i = 0; i < 2; i++)
                    _gameFactory.CreateUnit(UnitTypeId.Shielder);*/
                /*
                for (int i = 0; i < 1; i++)
                    _gameFactory.CreateUnit(UnitTypeId.Archer);
                */
                /*for (int i = 0; i < 2; i++)
                    _gameFactory.CreateUnit(UnitTypeId.Archer);*/
                /*for (int i = 0; i < 2; i++)
                {
                    UnitStatus builder = _gameFactory.CreateUnit(UnitTypeId.Builder).GetComponent<UnitStatus>();
                    _futureOrdersService.AddBuilder(builder);
                }*/
            }
            else
                LoadProgressUnits();
        }

        private void LoadProgressUnits()
        {
            List<UnitData> unitDatas = _progressService.PlayerProgress.UnitDataListWrapper.Units;

            List<UnitStatus> bindedUnitsToPlayer = new List<UnitStatus>();
            List<UnitStatus> bindedToFlagUnits = new List<UnitStatus>();
            List<UnitStatus> currentOrderUnits = new List<UnitStatus>();

            foreach (UnitData unitData in unitDatas)
            {
                GameObject unitObject = _gameFactory.CreateUnit(unitData.UnitTypeId);
                unitObject.transform.position = unitData.Position;

                UnitStatus unitStatus = unitObject.GetComponent<UnitStatus>();
                unitStatus.BindedToFlagUniqueId = unitData.BindedToFlagUniqueId;

                if (unitData.UnitTypeId == UnitTypeId.Builder)
                    _futureOrdersService.AddBuilder(unitStatus);

                if (unitData.IsBindedToPlayer)
                    bindedUnitsToPlayer.Add(unitStatus);
                else if (!string.IsNullOrEmpty(unitData.BindedToFlagUniqueId))
                    bindedToFlagUnits.Add(unitStatus);
                else if (unitData.FreePlaceIndex != -1 && !string.IsNullOrEmpty(unitData.OrderUniqueId))
                {
                    unitStatus.OrderUniqueId = unitData.OrderUniqueId;
                    unitStatus.FreePlaceIndex = unitData.FreePlaceIndex;

                    currentOrderUnits.Add(unitStatus);
                }
            }

            InitializeBindedToFlagUnits(bindedToFlagUnits);
            InitializeBindedToPlayerUnits(bindedUnitsToPlayer);
            InitializeRealtimeExecutionOrders(currentOrderUnits);
        }

        private void InitializeRealtimeExecutionOrders(List<UnitStatus> currentOrderUnits)
        {
            foreach (OrderMarker orderMarker in _allOrderMarkers)
            {
                UniqueId uniqueId = orderMarker.GetComponent<UniqueId>();

                List<UnitStatus> findAllUnits = currentOrderUnits.FindAll(x => x.OrderUniqueId.Contains(uniqueId.Id));

                if (findAllUnits.Count == 0)
                    continue;

                GiveOrderToBuilders(findAllUnits, orderMarker);
            }
        }

        private void GiveOrderToBuilders(List<UnitStatus> findAll, OrderMarker orderMarker)
        {
            foreach (UnitStatus unitStatus in findAll)
            {
                UnitMove unitMove = unitStatus.GetComponent<UnitMove>();
                unitMove.enabled = false;

                BuilderBehaviour unitStrategyBehaviour =
                    unitStatus.GetComponentInChildren<BuilderBehaviour>();

                _executeOrdersService.ExecuteOrder(unitStrategyBehaviour, orderMarker, unitStatus.FreePlaceIndex,
                    _futureOrdersService.RemoveCompletedOrder, _futureOrdersService.ContinueExecuteOrders);
            }
        }

        private void InitializeBindedToFlagUnits(List<UnitStatus> bindedToFlagUnits)
        {
            foreach (FlagSlotCoordinator flagSlotCoordinator in _buildingFlags)
            {
                List<UnitStatus> unitStatusList =
                    bindedToFlagUnits.FindAll(x => x.BindedToFlagUniqueId.Contains(flagSlotCoordinator.UniqueId.Id));

                foreach (UnitStatus unitStatus in unitStatusList)
                {
                    UnitMove unitMove = unitStatus.GetComponent<UnitMove>();
                    unitMove.enabled = false;

                    flagSlotCoordinator.BindUnitToSlot(unitStatus.transform, unitStatus.UnitTypeId);
                    flagSlotCoordinator.RelocateUnits();
                }
            }
        }

        private void InitBuildings()
        {
            GameStaticData gameStaticData = _staticData.GameStaticData;

            foreach (BuildingSpawnerData spawnerData in gameStaticData.BuildingSpawners)
            {
                bool isCleared =
                    _progressService.PlayerProgress.KillData.ClearedEnviromentResources.Contains(spawnerData.UniqueId);

                if (isCleared)
                    continue;

                GameObject building = RegisterBuildingProgress(spawnerData);

                if (building.TryGetComponent(out OrderMarker orderMarker))
                {
                    _allOrderMarkers.Add(orderMarker);

                    InitMarkingSign(spawnerData, orderMarker);
                }
            }

            InitializeSortedMarkingSign();
        }

        private void InitResources()
        {
            GameStaticData gameStaticData = _staticData.GameStaticData;

            foreach (ResourceSpawnerData resourceSpawnerData in gameStaticData.ResourceSpawners)
            {
                GameObject resourceObject = _gameFactory.CreateResource(
                    resourceSpawnerData.ResourceId,
                    resourceSpawnerData.Position,
                    resourceSpawnerData.UniqueId);

                ResourceData buildingData = _staticData.ForResource(resourceSpawnerData.ResourceId);
                _gridMap.OccupyCells((int)resourceObject.transform.position.x, buildingData);
            }
        }

        private void InitMarkingSign(BuildingSpawnerData spawnerData, OrderMarker orderMarker)
        {
            FutureOrdersData futureOrdersData = _progressService.PlayerProgress.FutureOrdersData;

            OrderData savedData =
                futureOrdersData.OrderDatas.FirstOrDefault(x =>
                    x.UniqueId.Contains(spawnerData.UniqueId));

            if (savedData != null)
                _orderInfos.Add(new OrderInfo(orderMarker, savedData.IndexOrder));
        }

        private void InitializeSortedMarkingSign()
        {
            _orderInfos.Sort((a, b) => a.IndexOrder.CompareTo(b.IndexOrder));

            foreach (OrderInfo orderInfo in _orderInfos)
                _futureOrdersService.AddOrder(orderInfo.OrderMarker);
        }


        private GameObject RegisterBuildingProgress(BuildingSpawnerData spawnerData)
        {
            BuildingData savedBuilding =
                _progressService.PlayerProgress.WorldData.BuildingData.FirstOrDefault(x =>
                    x.UniqueId == spawnerData.UniqueId);


            //GameObject buildingObject;

            GameObject buildingObject = _gameFactory.CreateBuilding(spawnerData.BuildingTypeId, spawnerData.Position,
                spawnerData.UniqueId);

            if (spawnerData.BuildingTypeId == BuildingTypeId.Bonfire)
            {
                BuildingUpgradeData bonfireBuildingData =
                    _staticData.ForBuilding(spawnerData.BuildingTypeId, BuildingLevelId.Level1, CardId.Default);

                _gridMap.OccupyCells((int)buildingObject.transform.position.x, bonfireBuildingData);
            }

            /*if (savedBuilding == null)
            {
                buildingObject = _gameFactory.CreateBuilding(spawnerData.BuildingTypeId, spawnerData.Position,
                    spawnerData.UniqueId);
            }
            else
            {
                buildingObject = _gameFactory.CreateBuilding(savedBuilding.BuildingTypeId, spawnerData.Position,
                    savedBuilding.UniqueId, savedBuilding.CurrentBuildingLevelId);

                if (savedBuilding.BuildingTypeId == BuildingTypeId.Baricade &&
                    savedBuilding.CurrentBuildingLevelId != BuildingLevelId.Level0)
                {
                    FlagSlotCoordinator flagSlotCoordinator =
                        buildingObject.GetComponentInChildren<FlagSlotCoordinator>();

                    if (flagSlotCoordinator != null)
                        _buildingFlags.Add(flagSlotCoordinator);
                }
            }*/


            return buildingObject;
        }


        private void InitializeBindedToPlayerUnits(List<UnitStatus> bindedUnitsToPlayer) =>
            _unitsRecruiterService.InitializeSavedUnits(bindedUnitsToPlayer);

        private bool IsNewProgress() =>
            _progressService.PlayerProgress.UnitDataListWrapper.Units.Count == 0;

        private GameObject InitPlayer()
        {
            GameObject playerObject = _gameFactory.CreatePlayer();
            TutorialView tutorialView = playerObject.GetComponentInChildren<TutorialView>();

            _forestTransitionService.Initialize(playerObject.transform);

            /*if (!_tutorialCheckerService.TutorialStarted)
                tutorialView.gameObject.SetActive(false);
            else
                tutorialView.Initialize();*/

            return playerObject;
        }

        private void InitEnemyCamps()
        {
            GameStaticData gameStaticData = _staticData.GameStaticData;

            foreach (EnemyCampData enemyCamp in gameStaticData.EnemyCamps)
            {
                if (string.IsNullOrEmpty(enemyCamp.UniqueId))
                    Debug.LogError("Вы не создали UniqueID для всех элементов");

                bool isCleared =
                    _progressService.PlayerProgress.KillData.ClearedEnemyCamps.Contains(enemyCamp.UniqueId);

                if (isCleared == false)
                    _gameFactory.CreateEnemyCamp(enemyCamp.MicroWaveCamp, enemyCamp.Position, enemyCamp.Hp,
                        enemyCamp.UniqueId);
            }
        }

        private void InitVagabondCamps()
        {
            GameStaticData gameStaticData = _staticData.GameStaticData;

            foreach (VagabondCampData vagabondCampData in gameStaticData.VagabondCampDatas)
            {
                VagabondCamp vagabondCamp = _gameFactory.CreateVagabondCamp(vagabondCampData.Position);
                _vagabondCampService.AddCamp(vagabondCamp);
            }
        }
    }
}