using System;
using BuildProcessManagement;
using BuildProcessManagement.ResourceElements;
using Enemy;
using Enemy.Camp;
using HealthSystem;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.ECSInput;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.Flag;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.ProgressWatchers;
using Infastructure.Services.ResourceLimiter;
using Infastructure.Services.StreetLight;
using Infastructure.Services.UnitEvacuationService;
using Infastructure.Services.UnitTrackingService;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.Matryoshka;
using Infastructure.StaticData.Player;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.Schemes;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Infastructure.StaticData.WaveOfEnemies;
using Player;
using Player.Orders;
using Schemes;
using UI.GameplayUI.BuildingCoinsUIManagement;
using Units;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using Units.Vagabond;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Infastructure.Factories.GameFactories
{
    public class GameFactory : IGameFactory
    {
        private readonly DiContainer _diContainer;

        private readonly IStaticDataService _staticData;
        private readonly IFlagTrackerService _flagTrackerService;
        private readonly IWaveEnemiesCountService _waveEnemiesCountService;
        private readonly IProgressWatchersService _progressWatchersService;
        private readonly IEvacuationService _evacuationService;
        private readonly IUnitsTrackerService _unitsTrackerService;
        private readonly IHomelessOrdersService _homelessOrdersService;
        private readonly IResourceLimiterService _resourceLimiterService;
        private readonly IEcsWatchersService _ecsWatchersService;
        private readonly IPlayerRegistryService _playerRegistryService;
        private readonly IStreetLightsService _streetLightsService;

        private readonly Camera _camera = Camera.main;


        public GameFactory(
            DiContainer diContainer,
            IStaticDataService staticData,
            IFlagTrackerService flagTrackerService,
            IWaveEnemiesCountService waveEnemiesCountService,
            IProgressWatchersService progressWatchersService,
            IEvacuationService evacuationService,
            IUnitsTrackerService unitsTrackerService,
            IHomelessOrdersService homelessOrdersService,
            IResourceLimiterService resourceLimiterService,
            IEcsWatchersService ecsWatchersService,
            IPlayerRegistryService playerRegistryService,
            IStreetLightsService streetLightsService
        )
        {
            _diContainer = diContainer;
            _staticData = staticData;
            _flagTrackerService = flagTrackerService;
            _waveEnemiesCountService = waveEnemiesCountService;
            _progressWatchersService = progressWatchersService;
            _evacuationService = evacuationService;
            _unitsTrackerService = unitsTrackerService;
            _homelessOrdersService = homelessOrdersService;
            _resourceLimiterService = resourceLimiterService;
            _ecsWatchersService = ecsWatchersService;
            _playerRegistryService = playerRegistryService;
            _streetLightsService = streetLightsService;
        }

        public GameObject CreateBuilding(
            BuildingTypeId buildingTypeId,
            Vector3 position,
            string uniqueId,
            BuildingLevelId levelId = BuildingLevelId.Level1,
            CardId cardId = CardId.Default,
            CardId cardKey = CardId.Default)
        {
            BuildingUpgradeData buildingUpgradeData = _staticData.ForBuilding(buildingTypeId, levelId, cardKey);

            GameObject building =
                _diContainer.InstantiatePrefab(buildingUpgradeData.GetPrefabFrom(cardId), position, Quaternion.identity,
                    null);

            RegisterCanvas(building);

            if (buildingTypeId == BuildingTypeId.Bonfire)
                _flagTrackerService.RegisterFlag(building.transform, 0);

            building.GetComponentInChildren<IHealth>()?.Initialize(buildingUpgradeData.HP);
            building.GetComponent<UniqueId>().Id = uniqueId;

            if (building.TryGetComponent(out BuildInfo buildInfo))
            {
                buildInfo.CurrentLevelId = levelId;
                buildInfo.NextBuildingLevelId = GetNextLevelId(levelId);
                buildInfo.CardKey = cardId;
                buildInfo.PreviousCardId = cardKey;
                buildInfo.BuildingTypeId = buildingTypeId;

                float localScaleX = buildInfo.VisualBuilding.transform.localScale.x;
                float totalScaleX = position.x > 0 ? localScaleX : localScaleX * -1;
                buildInfo.VisualBuilding.transform.localScale = new Vector3(totalScaleX,
                    buildInfo.transform.localScale.y);

                buildInfo.GetComponent<BuildCoinsUIRegistrator>()?.Register();
            }

            _progressWatchersService.RegisterWatchers(building);
            _streetLightsService.RegisterLights(building);

            return building;
        }


        private void RegisterCanvas(GameObject building)
        {
            Canvas canvas = building.GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvas.worldCamera = _camera;
        }

        public GameObject CreateFirstBuilding(BuildingUpgradeData buildingUpgradeData, BuildingTypeId buildingTypeId,
            Vector3 position)
        {
            GameObject building =
                _diContainer.InstantiatePrefab(buildingUpgradeData.GetPrefabFrom(CardId.Default), position,
                    Quaternion.identity, null);

            Canvas canvas = building.GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvas.worldCamera = _camera;

            building.GetComponentInChildren<IHealth>()?.Initialize(buildingUpgradeData.HP);

            if (building.TryGetComponent(out BuildInfo buildInfo))
            {
                buildInfo.CurrentLevelId = BuildingLevelId.Level1;
                buildInfo.NextBuildingLevelId = BuildingLevelId.Level2;
                buildInfo.CardKey = CardId.Default;
                buildInfo.PreviousCardId = CardId.Default;
                buildInfo.BuildingTypeId = buildingTypeId;

                float localScaleX = buildInfo.VisualBuilding.transform.localScale.x;
                float totalScaleX = position.x > 0 ? localScaleX : localScaleX * -1;
                buildInfo.VisualBuilding.transform.localScale = new Vector3(totalScaleX,
                    buildInfo.transform.localScale.y);

                buildInfo.GetComponent<BuildCoinsUIRegistrator>()?.Register();
            }

            _progressWatchersService.RegisterWatchers(building);
            _streetLightsService.RegisterLights(building);

            return building;
        }

        public GameObject CreateBarricadeFlag(Vector3 position, string uniqueId)
        {
            GameObject barricadeFlag =
                _diContainer.InstantiatePrefabResource(AssetsPath.BuildingFlagPath, position, Quaternion.identity,
                    null);

            barricadeFlag.GetComponent<UniqueId>().Id = uniqueId;

            return barricadeFlag;
        }

        public GameObject CreateScheme(BuildingTypeId buildingTypeId, Vector3 position)
        {
            SchemeStaticData schemeData = _staticData.ForScheme(buildingTypeId);

            GameObject prefab =
                _diContainer.InstantiatePrefab(schemeData.Prefab, position, Quaternion.identity, null);

            Scheme scheme = prefab.GetComponent<Scheme>();
            scheme.BuildingTypeId = buildingTypeId;

            return prefab;
        }

        public GameObject CreateResource(ResourceId resourceId, Vector3 position, string uniqueId)
        {
            ResourceData buildingData = _staticData.ForResource(resourceId);

            GameObject randomPrefab = buildingData.ResoucePrefabs[Random.Range(0, buildingData.ResoucePrefabs.Count)];

            GameObject resourceObject =
                _diContainer.InstantiatePrefab(randomPrefab, position, Quaternion.identity, null);

            OrderMarker orderMarker = resourceObject.GetComponent<OrderMarker>();
            _resourceLimiterService.AddResource(orderMarker);

            ResourceInfo resourceInfo = resourceObject.GetComponent<ResourceInfo>();
            resourceInfo.ResourceId = resourceId;

            resourceObject.GetComponent<UniqueId>().Id = uniqueId;

            Canvas canvas = randomPrefab.GetComponentInChildren<Canvas>();
            canvas.worldCamera = _camera;

            _progressWatchersService.RegisterWatchers(randomPrefab);

            return resourceObject;
        }


        public void CreateEnemyCamp(MicroWavesInfo microWavesInfo, Vector3 position, int hp, string enemyCampUniqueId)
        {
            EnemyCamp enemyCamp =
                _diContainer.InstantiatePrefabResourceForComponent<EnemyCamp>(AssetsPath.EnemyCampPath);
            enemyCamp.transform.position = position;

            enemyCamp.Initialize(microWavesInfo);
            enemyCamp.GetComponent<UniqueId>().Id = enemyCampUniqueId;
            enemyCamp.GetComponentInChildren<IHealth>().Initialize(hp);

            _progressWatchersService.RegisterWatchers(enemyCamp.gameObject);
        }

        public VagabondCamp CreateVagabondCamp(Vector3 position)
        {
            VagabondCamp vagabondCamp =
                _diContainer.InstantiatePrefabResourceForComponent<VagabondCamp>(AssetsPath.VagabondCampPath);

            vagabondCamp.transform.position = position;

            return vagabondCamp;
        }


        public GameObject CreatePlayer()
        {
            bool skipCutScene = _staticData.CheatStaticData.SkipCutScene;
            float targetPositionX = skipCutScene ? -20f : -70;

            GameObject playerObject = _diContainer.InstantiatePrefabResource(AssetsPath.PlayerPath);
            playerObject.transform.position = new Vector3(targetPositionX, -2.75f, 0);

            Canvas canvas = playerObject.GetComponentInChildren<Canvas>();

            if (canvas != null)
                canvas.worldCamera = _camera;

            PlayerStaticData playerStaticData = _staticData.PlayerStaticData;

            PlayerMove playerMove = playerObject.GetComponent<PlayerMove>();
            playerMove.Speed = playerStaticData.Speed;
            playerMove.AccelerationTime = playerStaticData.AccelerationTime;

            IHealth health = playerObject.GetComponentInChildren<IHealth>();
            health.Initialize(playerStaticData.Hp);

            PlayerInputBuildingOrders playerInputBuildingOrders =
                playerObject.GetComponent<PlayerInputBuildingOrders>();
            playerInputBuildingOrders.BuildModeDelay = playerStaticData.BuildModeDelay;

            _progressWatchersService.RegisterWatchers(playerObject);
            _progressWatchersService.RegisterWatchers(_camera.gameObject);
            _ecsWatchersService.RegisterWatchers(playerObject);

            _playerRegistryService.Player = playerObject;

            return playerObject;
        }

        public void CreateEnemyCrystal(Vector3 position, string uniqueId)
        {
            GameObject cristal =
                _diContainer.InstantiatePrefabResource(AssetsPath.EnemyCristalPath, position, Quaternion.identity,
                    null);

            cristal.GetComponent<UniqueId>().Id = uniqueId;
        }

        public GameObject CreateEnemy(EnemyTypeId enemyTypeId, float savedSpawnPointX)
        {
            EnemyStaticData enemyData = _staticData.ForEnemy(enemyTypeId);
            GameObject prefab = _diContainer.InstantiatePrefab(enemyData.Prefab);
            prefab.transform.position = new Vector3(savedSpawnPointX, -2.75f, 0);

            prefab.GetComponentInChildren<IHealth>().Initialize(enemyData.Hp);
            prefab.GetComponent<EnemyInfo>().EnemyTypeId = enemyTypeId;

            _waveEnemiesCountService.NumberOfEnemiesOnWave++;
            _waveEnemiesCountService.Enemies.Add(prefab);

            _progressWatchersService.RegisterWatchers(prefab);

            return prefab;
        }

        public void CreateEnemyMatryoshka(EnemyTypeId enemyTypeId, MatryoshkaId previous, Vector3 position)
        {
            MatryoshkaData newMatryoshkaData = _staticData.ForNextMatryoshka(enemyTypeId, previous);
            if (newMatryoshkaData == null)
                return;

            EnemyStaticData enemyData = _staticData.ForEnemy(enemyTypeId);
            GameObject prefab = _diContainer.InstantiatePrefab(enemyData.Prefab);
            prefab.transform.position = position;
            prefab.transform.localScale = newMatryoshkaData.Scale;

            prefab.GetComponentInChildren<IHealth>().Initialize(newMatryoshkaData.Hp / 2);

            EnemyInfo enemyInfo = prefab.GetComponent<EnemyInfo>();
            enemyInfo.EnemyTypeId = enemyTypeId;
            enemyInfo.MatryoshkaId = newMatryoshkaData.MatryoshkaId;

            _waveEnemiesCountService.NumberOfEnemiesOnWave++;
            _waveEnemiesCountService.Enemies.Add(prefab);

            _progressWatchersService.RegisterWatchers(prefab);
        }


        public GameObject CreateUnit(UnitTypeId unitTypeId, float positionX = 0)
        {
            UnitStaticData unitData = _staticData.ForUnit(unitTypeId);
            GameObject unit = _diContainer.InstantiatePrefab(unitData.Prefab);
            unit.transform.position = new Vector3(positionX, unit.transform.position.y);

            RegisterUnit(unitTypeId, unit);

            unit.transform.localScale = new Vector3(Random.Range(0.95f, 1f), Random.Range(0.95f, 1f));
            unit.GetComponentInChildren<IHealth>()?.Initialize(unitData.Hp);

            _unitsTrackerService.AddUnit(unitTypeId);
            _progressWatchersService.RegisterWatchers(unit);

            return unit;
        }

        private void RegisterUnit(UnitTypeId unitTypeId, GameObject unit)
        {
            UnitStateMachineView unitStateMachineView = unit.GetComponent<UnitStateMachineView>();
            unitStateMachineView?.Initialize();

            VagabondStateMachineView vagabondStateMachineView = unit.GetComponent<VagabondStateMachineView>();
            vagabondStateMachineView?.Initialize();

            if (unit.TryGetComponent(out UnitStatus unitStatus))
            {
                unitStatus.UnitTypeId = unitTypeId;

                if (unitStatus.UnitTypeId == UnitTypeId.Homeless)
                    _homelessOrdersService.AddHomeless(unitStatus);
                else
                    _evacuationService.AddUnit(unitStatus);
            }
        }

        public GameObject CreateHUD()
        {
            GameObject prefab = _diContainer.InstantiatePrefabResource(AssetsPath.HUDPath);

            _progressWatchersService.RegisterWatchers(prefab);

            return prefab;
        }


        public GameObject CreateCristalUI()
        {
            GameObject prefab = _diContainer.InstantiatePrefabResource(AssetsPath.CristalUIPath);

            _progressWatchersService.RegisterWatchers(prefab);

            return prefab;
        }

        private BuildingLevelId GetNextLevelId(BuildingLevelId currentLevel)
        {
            Array levels = Enum.GetValues(typeof(BuildingLevelId));
            int currentIndex = Array.IndexOf(levels, currentLevel);

            if (currentIndex < levels.Length - 1)
                return (BuildingLevelId)levels.GetValue(currentIndex + 1);

            return BuildingLevelId.Unknow;
        }
    }
}