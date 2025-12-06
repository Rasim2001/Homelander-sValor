using System.Collections.Generic;
using DayCycle;
using Enemy;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.BuildingCatalog;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.Flag;
using Infastructure.Services.HudFader;
using Infastructure.Services.PlayerProgressService;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.Unit;
using Player;
using UI.GameplayUI.HudUI;
using Units.UnitStatusManagement;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Editor.CheatCodes
{
    public class CheatCodesTool
    {
        [MenuItem("Cheats/Get Schemes")]
        [MenuItem("Cheats/", false, 1)]
        public static void GetSchemes()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();

            if (sceneContext == null)
                return;

            DiContainer container = sceneContext.Container;

            IBuildingModeConfigurationService _buildingModeConfigurationService =
                container.Resolve<IBuildingModeConfigurationService>();
            IBuildingCatalogService _buildingCatalogService = container.Resolve<IBuildingCatalogService>();

            IHudFaderService _hudFaderService = container.Resolve<IHudFaderService>();


            for (int i = 0; i < 5; i++)
            {
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.Baricade);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.BowWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.HammerWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.ShielderWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.ResetWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.TowerBow);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.TowerTar);

                _buildingCatalogService.CreateCatalog(BuildingTypeId.Baricade);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.BowWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.HammerWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.ShielderWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.ResetWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.TowerBow);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.TowerTar);
            }

            _hudFaderService.Show(HudId.Buildings);
            _hudFaderService.DoFade(HudId.Buildings);
        }

        [MenuItem("Cheats/Destroy All Enemies")]
        public static void DestroyAllEnemies()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DiContainer container = sceneContext.Container;

            IWaveEnemiesCountService waveEnemiesCountService = container.Resolve<IWaveEnemiesCountService>();

            List<GameObject> enemies = new List<GameObject>(waveEnemiesCountService.Enemies);

            for (int i = 0; i < waveEnemiesCountService.Enemies.Count; i++)
            {
                Object.DestroyImmediate(enemies[i]);
                waveEnemiesCountService.NumberOfEnemiesOnWave--;
            }

            waveEnemiesCountService.Enemies.Clear();
        }

        [MenuItem("Cheats/Cutscene/Skip Cutscene")]
        public static void SkipCutscene()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DiContainer container = sceneContext.Container;

            IPersistentProgressService persistentProgressService = container.Resolve<IPersistentProgressService>();
            persistentProgressService.PlayerProgress.CutSceneData.Active = false;

            Debug.Log("Cutscene Skipped");
        }


        [MenuItem("Cheats/Coins/Get All")]
        [MenuItem("Cheats/Coins/", false, 5)]
        public static void GetCoinsAll() =>
            CollectCoins(10000);

        [MenuItem("Cheats/Coins/Delete All")]
        public static void DeleteCoinsAll() =>
            DeleteCoins();

        [MenuItem("Cheats/Coins/Get 10")]
        public static void GetCoins10() =>
            CollectCoins(10);

        [MenuItem("Cheats/Coins/Delete 10")]
        public static void DeleteCoins10() =>
            DeleteCoins(10);


        [MenuItem("Cheats/Spawn Enemy/Marauder")]
        [MenuItem("Cheats/Spawn Enemy/", false, 3)]
        public static void SpawnEnemyMarauder() =>
            SpawnAndRegistry(EnemyTypeId.Marauder);

        [MenuItem("Cheats/Spawn Enemy/Bonecrusher")]
        public static void SpawnEnemyBonecrasher() =>
            SpawnAndRegistry(EnemyTypeId.Bonecrusher);

        [MenuItem("Cheats/Spawn Enemy/Bomber")]
        public static void SpawnEnemyBomber() =>
            SpawnAndRegistry(EnemyTypeId.Bomber);

        [MenuItem("Cheats/Spawn Enemy/Tank")]
        public static void SpawnEnemyTank() =>
            SpawnAndRegistry(EnemyTypeId.Tank);


        [MenuItem("Cheats/Spawn Unit/Homeless")]
        [MenuItem("Cheats/Spawn Unit/", false, 4)]
        public static void SpawnHomeless() =>
            SpawnUnit(UnitTypeId.Homeless);

        [MenuItem("Cheats/Spawn Unit/Shielder")]
        public static void SpawnShielder() =>
            SpawnUnit(UnitTypeId.Shielder);

        [MenuItem("Cheats/Spawn Unit/Builder")]
        public static void SpawnBuilder() =>
            SpawnUnit(UnitTypeId.Builder);

        [MenuItem("Cheats/Spawn Unit/Archer")]
        public static void SpawnArcher() =>
            SpawnUnit(UnitTypeId.Archer);


        [MenuItem("Cheats/Time/Freez Time")]
        [MenuItem("Cheats/Time/", false, 2)]
        public static void FreezTime()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DayCycleUpdater cycleUpdater = Object.FindObjectOfType<DayCycleUpdater>();

            if (sceneContext == null)
                return;

            DiContainer container = sceneContext.Container;

            IEnemyWavesService wavesService = container.Resolve<IEnemyWavesService>();
            wavesService.FreezTimeEditor();
            cycleUpdater.FreezTime();
        }

        [MenuItem("Cheats/Time/Unfreez Time")]
        public static void UnFreezTime()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DayCycleUpdater cycleUpdater = Object.FindObjectOfType<DayCycleUpdater>();

            if (sceneContext == null)
                return;

            DiContainer container = sceneContext.Container;

            IEnemyWavesService wavesService = container.Resolve<IEnemyWavesService>();
            wavesService.UnFreezTimeEditor();
            cycleUpdater.UnFreezTime();
        }

        [MenuItem("Cheats/Time/Force Night")]
        public static void ForceNight()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();

            if (sceneContext == null)
                return;

            DiContainer container = sceneContext.Container;

            IEnemyWavesService wavesService = container.Resolve<IEnemyWavesService>();
            wavesService.ForceNight();
        }


        [MenuItem("Cheats/Time/Force Day")]
        public static void ForceDay()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();

            if (sceneContext == null)
                return;

            DiContainer container = sceneContext.Container;

            IEnemyWavesService wavesService = container.Resolve<IEnemyWavesService>();
            wavesService.ForceDay();
        }

        private static void CollectCoins(int count)
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DiContainer container = sceneContext.Container;

            IPersistentProgressService persistentProgressService = container.Resolve<IPersistentProgressService>();
            persistentProgressService.PlayerProgress.CoinData.Collect(count);
        }

        private static void DeleteCoins()
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DiContainer container = sceneContext.Container;

            IPersistentProgressService persistentProgressService = container.Resolve<IPersistentProgressService>();
            persistentProgressService.PlayerProgress.CoinData.Spend(persistentProgressService.PlayerProgress.CoinData
                .NumberOfCoins);
        }

        private static void DeleteCoins(int count)
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DiContainer container = sceneContext.Container;

            IPersistentProgressService persistentProgressService = container.Resolve<IPersistentProgressService>();
            persistentProgressService.PlayerProgress.CoinData.Spend(count);
        }

        private static void SpawnAndRegistry(EnemyTypeId enemyType)
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            PlayerMove playerMove = Object.FindObjectOfType<PlayerMove>();

            if (sceneContext == null)
                return;

            DiContainer container = sceneContext.Container;

            IGameFactory gameFactory = container.Resolve<IGameFactory>();
            IFlagTrackerService flagTrackerService = container.Resolve<IFlagTrackerService>();


            float savedSpawnPointX = 0;

            bool isRight = playerMove.transform.position.x > 0;
            int signDirection = isRight ? 1 : -1;
            float spawnPointX = 20 * signDirection;

            Transform lastFlagTransform = flagTrackerService.GetLastFlag(isRight);
            float referencePositionX = isRight
                ? Mathf.Max(playerMove.transform.position.x, lastFlagTransform.position.x)
                : Mathf.Min(playerMove.transform.position.x, lastFlagTransform.position.x);

            spawnPointX += referencePositionX;

            savedSpawnPointX = isRight
                ? Mathf.Max(savedSpawnPointX, spawnPointX)
                : Mathf.Min(savedSpawnPointX, spawnPointX);


            gameFactory.CreateEnemy(enemyType, savedSpawnPointX);
        }

        private static void SpawnUnit(UnitTypeId unitTypeId)
        {
            SceneContext sceneContext = Object.FindObjectOfType<SceneContext>();
            DiContainer container = sceneContext.Container;

            IFutureOrdersService futureOrdersService = container.Resolve<IFutureOrdersService>();
            IGameFactory gameFactory = container.Resolve<IGameFactory>();

            GameObject unit = gameFactory.CreateUnit(unitTypeId);
            UnitStatus unitStatus = unit.GetComponent<UnitStatus>();

            if (unitStatus.UnitTypeId == UnitTypeId.Builder)
                futureOrdersService.AddBuilder(unitStatus);
        }
    }
}