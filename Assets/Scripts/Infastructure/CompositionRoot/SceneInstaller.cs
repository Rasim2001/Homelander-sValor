using Bonfire;
using Bonfire.Builds;
using BuildProcessManagement.WorkshopBuilding;
using BuildProcessManagement.WorkshopBuilding.Product;
using CutScenes;
using FogOfWar;
using Grid;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.BuildingCatalog;
using Infastructure.Services.BuildingRegistry;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.CameraFocus;
using Infastructure.Services.Cards;
using Infastructure.Services.CoinsCreator;
using Infastructure.Services.ECSInput;
using Infastructure.Services.Effects;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.Fence;
using Infastructure.Services.Flag;
using Infastructure.Services.Forest;
using Infastructure.Services.HudFader;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.MarkerSignCoordinator;
using Infastructure.Services.MinimapManagement;
using Infastructure.Services.NearestBuildFind;
using Infastructure.Services.PauseService;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.Pool;
using Infastructure.Services.PurchaseDelay;
using Infastructure.Services.ResourceLimiter;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.Services.SchemeSpawner;
using Infastructure.Services.StreetLight;
using Infastructure.Services.Taskbook;
using Infastructure.Services.Tooltip;
using Infastructure.Services.Trees;
using Infastructure.Services.Tutorial;
using Infastructure.Services.UnitEvacuationService;
using Infastructure.Services.UnitRecruiter;
using Infastructure.Services.UnitSpawnStrategy;
using Infastructure.Services.UnitTrackingService;
using Infastructure.Services.VagabondCampManagement;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.States;
using Loots;
using Player.Orders;
using Tooltip;
using Tooltip.UI;
using Tooltip.World;
using UI.Windows.Mainflag;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Infastructure.CompositionRoot
{
    public class SceneInstaller : MonoInstaller
    {
        public Volume Volume;

        public MarkerSign MarkerSignPrefab;
        public Transform MarkerSignContainer;

        public ArrowObject ArrowObject;
        public Transform ArrowsContainer;

        public ShieldObject ShieldObject;
        public Transform ShieldContainer;

        public HammerObject HammerObject;

        public Transform HammersContainer;

        public override void InstallBindings()
        {
            BindFlagTrackerService();

            BindMarkerSignPool();

            BindMarkerSignCoordinator();

            BindBuildLevelState();

            BindWindowService();

            BindGameFactory();

            BindUIFactory();

            BindInputPlayer();

            BindExecuteOrdersService();

            BindFutureOrdersService();

            BindFlagDefenseHandler();

            BindEnemyWavesService();

            BindEnemySpawnService();

            BindWorkshopService();

            BindArrowWorkshopPool();

            BindHammerWorkshopPool();

            BindWaveEnemiesCountService();

            BindTowerSelectionService();

            BindSortingLayerTrees();

            BindBonfireAnimations();

            BindPauseService();

            BindTutorialSpawnService();

            BindNearestBuildFindService();

            BindShieldWorkshopPool();

            BindSafeZoneBuildTracker();

            BindUnitEvacuationService();

            BindBuilderCommandExecutor();

            BindGridMap();

            BindBuildModeService();

            BindBuildModeUIService();

            BindDestroyCommandExecutor();

            BindUnitsUICounterService();

            BindUpgdadeMainFlag();

            BindHomelessOrdersService();

            BindUnitRecruiterService();

            BindBuildingModifyService();

            BindPurchaseDelayService();

            BindSchemeSpawnerService();

            BindBuildModeConfigurationService();

            BindCardSpawnService();

            BindCardTrackerService();

            BindTooltipUI();

            BindHudFaderService();

            BindBuildingCatalogService();

            BindCatalogOpenService();

            BindResourceLimiterService();

            BindUnitSpawnStrategyService();

            BindSlowEffectService();

            BindCameraFocusService();

            BindForestService();

            BindGlobalVolume();

            BindForestTransitionService();

            BindBuildingRegistryService();

            BindFogOfWar();

            BindTooltipWorldService();

            BindTooltipInputService();

            BindECSInputTrackerService();

            BindEcsWatchersService();

            BindFenceService();

            BindBuildBonfire();

            BindTaskBookService();

            BindCristalTimeline();

            BindVagabondCampService();

            BindPlayerRegistry();

            BindCoinsCreatorService();

            BindStreetLightsService();

            BindMinimapNotifierService();
        }

        private void BindMinimapNotifierService() =>
            Container.BindInterfacesAndSelfTo<MinimapNotifierService>().AsSingle();

        private void BindStreetLightsService() =>
            Container.BindInterfacesAndSelfTo<StreetLightsService>().AsSingle();

        private void BindCoinsCreatorService() =>
            Container.BindInterfacesAndSelfTo<CoinsCreatorService>().AsSingle();

        private void BindPlayerRegistry() =>
            Container.BindInterfacesAndSelfTo<PlayerRegistryService>().AsSingle();

        private void BindVagabondCampService() =>
            Container.BindInterfacesAndSelfTo<VagabondCampService>().AsSingle();

        private void BindCristalTimeline()
        {
            Container
                .Bind<ICristalTimeline>().To<CristalTimeline>()
                .FromComponentInNewPrefabResource(AssetsPath.CristalTimeline)
                .AsSingle();
        }

        private void BindTaskBookService() =>
            Container.BindInterfacesAndSelfTo<TaskBookInputService>().AsSingle();

        private void BindBuildBonfire() =>
            Container.BindInterfacesAndSelfTo<BuildBonfire>().AsSingle();

        private void BindFenceService() =>
            Container.BindInterfacesAndSelfTo<FenceService>().AsSingle();

        private void BindEcsWatchersService() =>
            Container.BindInterfacesAndSelfTo<EcsWatchersService>().AsSingle();

        private void BindECSInputTrackerService() =>
            Container.BindInterfacesAndSelfTo<ECSInputTrackerService>().AsSingle();

        private void BindTooltipInputService() =>
            Container.BindInterfacesAndSelfTo<TooltipInputService>().AsSingle();


        private void BindTooltipWorldService() =>
            Container.BindInterfacesAndSelfTo<TooltipWorldService>().AsSingle();

        private void BindBuildingRegistryService() =>
            Container.BindInterfacesAndSelfTo<BuildingRegistryService>().AsSingle();


        private void BindFogOfWar()
        {
            Container
                .Bind<IFogOfWarMinimap>()
                .To<FogOfWarMinimap>()
                .FromComponentInNewPrefabResource(AssetsPath.FogOfWarPath)
                .AsSingle();
        }

        private void BindForestTransitionService() =>
            Container.BindInterfacesAndSelfTo<ForestTransitionService>().AsSingle();

        private void BindForestService() =>
            Container.BindInterfacesAndSelfTo<ForestService>().AsSingle();

        private void BindGlobalVolume() =>
            Container.Bind<Volume>().FromInstance(Volume).AsSingle();

        private void BindCameraFocusService() =>
            Container.BindInterfacesAndSelfTo<CameraFocusService>().AsSingle();

        private void BindSlowEffectService() =>
            Container.BindInterfacesAndSelfTo<SlowEffectService>().AsSingle();

        private void BindUnitSpawnStrategyService() =>
            Container.BindInterfacesAndSelfTo<UnitSpawnStrategyService>().AsSingle();

        private void BindResourceLimiterService() =>
            Container.BindInterfacesAndSelfTo<ResourceLimiterService>().AsSingle();

        private void BindCatalogOpenService() =>
            Container.BindInterfacesAndSelfTo<CatalogOpenService>().AsSingle();

        private void BindBuildingCatalogService() =>
            Container.BindInterfacesAndSelfTo<BuildingCatalogService>().AsSingle();

        private void BindHudFaderService() =>
            Container.BindInterfacesAndSelfTo<HudFaderService>().AsSingle();


        private void BindTooltipUI()
        {
            Container
                .Bind<ITooltip>()
                .To<TooltipUI>()
                .FromComponentInNewPrefabResource(UIAssetPath.TooltipUI)
                .AsSingle();
        }

        private void BindCardTrackerService() =>
            Container.BindInterfacesAndSelfTo<CardTrackerService>().AsSingle();

        private void BindCardSpawnService() =>
            Container.BindInterfacesAndSelfTo<CardSpawnService>().AsSingle();

        private void BindBuildModeConfigurationService() =>
            Container.BindInterfacesAndSelfTo<BuildingModeConfigurationService>().AsSingle();

        private void BindSchemeSpawnerService() =>
            Container.BindInterfacesAndSelfTo<SchemesSpawnerService>().AsSingle();

        private void BindPurchaseDelayService() =>
            Container.BindInterfacesAndSelfTo<PurchaseDelayService>().AsSingle();

        private void BindBuildingModifyService() =>
            Container.BindInterfacesAndSelfTo<BuildingModifyService>().AsSingle();

        private void BindUnitRecruiterService() =>
            Container.BindInterfacesAndSelfTo<UnitsRecruiterService>().AsSingle();

        private void BindHomelessOrdersService() =>
            Container.BindInterfacesAndSelfTo<HomelessOrdersService>().AsSingle();

        private void BindUpgdadeMainFlag() =>
            Container.BindInterfacesAndSelfTo<UpgradeMainFlag>().AsSingle();

        private void BindUnitsUICounterService() =>
            Container.BindInterfacesAndSelfTo<UnitsTrackerService>().AsSingle();

        private void BindDestroyCommandExecutor() =>
            Container.BindInterfacesAndSelfTo<DestroyCommandExecutor>().AsSingle();

        private void BindBuildModeUIService() =>
            Container.BindInterfacesAndSelfTo<BuildingModeUIService>().AsSingle();

        private void BindBuildModeService() =>
            Container.BindInterfacesAndSelfTo<BuildingModeService>().AsSingle();

        private void BindGridMap() =>
            Container.BindInterfacesAndSelfTo<GridMap>().AsSingle();

        private void BindBuilderCommandExecutor() =>
            Container.BindInterfacesAndSelfTo<BuilderCommandExecutor>().AsSingle();

        private void BindUnitEvacuationService() =>
            Container.BindInterfacesAndSelfTo<EvacuationService>().AsSingle();

        private void BindSafeZoneBuildTracker() =>
            Container.BindInterfacesAndSelfTo<SafeBuildZone>().AsSingle();

        private void BindNearestBuildFindService() =>
            Container.BindInterfacesAndSelfTo<NearestBuildFindService>().AsSingle();

        private void BindTutorialSpawnService() =>
            Container.BindInterfacesAndSelfTo<TutorialSpawnService>().AsSingle();

        private void BindPauseService() =>
            Container.BindInterfacesAndSelfTo<PauseService>().AsSingle();


        private void BindBonfireAnimations() =>
            Container.BindInterfacesAndSelfTo<UpdateBonfireLevelAnimation>().AsSingle();


        private void BindSortingLayerTrees() =>
            Container.BindInterfacesAndSelfTo<SortingLayerTrees>().AsSingle();

        private void BindTowerSelectionService() =>
            Container.BindInterfacesAndSelfTo<OrderSelectionUIService>().AsSingle();

        private void BindWaveEnemiesCountService() =>
            Container.BindInterfacesAndSelfTo<WaveEnemiesCountService>().AsSingle();

        private void BindArrowWorkshopPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<ArrowObject>>()
                .AsSingle()
                .WithArguments(ArrowObject, ArrowsContainer);
        }

        private void BindHammerWorkshopPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<HammerObject>>()
                .AsSingle()
                .WithArguments(HammerObject, HammersContainer);
        }

        private void BindShieldWorkshopPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<ShieldObject>>()
                .AsSingle()
                .WithArguments(ShieldObject, ShieldContainer);
        }

        private void BindWorkshopService() =>
            Container.BindInterfacesAndSelfTo<WorkshopService>().AsSingle();

        private void BindEnemySpawnService() =>
            Container.BindInterfacesAndSelfTo<EnemySpawnService>().AsSingle();

        private void BindEnemyWavesService() =>
            Container.BindInterfacesAndSelfTo<EnemyWavesService>().AsSingle();

        private void BindFlagDefenseHandler() =>
            Container.BindInterfacesAndSelfTo<FlagDefenseHandler>().AsSingle();

        private void BindFlagTrackerService() =>
            Container.BindInterfacesAndSelfTo<FlagTrackerService>().AsSingle();

        private void BindMarkerSignCoordinator() =>
            Container.BindInterfacesAndSelfTo<MarkerSignCoordinatorService>().AsSingle();

        private void BindMarkerSignPool()
        {
            Container
                .BindInterfacesAndSelfTo<PoolObjects<MarkerSign>>()
                .AsSingle()
                .WithArguments(MarkerSignPrefab, MarkerSignContainer);
        }


        private void BindFutureOrdersService() =>
            Container.BindInterfacesAndSelfTo<FutureOrdersService>().AsSingle();

        private void BindExecuteOrdersService() =>
            Container.BindInterfacesAndSelfTo<ExecuteOrdersService>().AsSingle();


        private void BindInputPlayer() =>
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();

        private void BindBuildLevelState() =>
            Container.BindInterfacesAndSelfTo<BuildLevelState>().AsSingle();

        private void BindUIFactory() =>
            Container.BindInterfacesAndSelfTo<GameUIFactory>().AsSingle();


        private void BindGameFactory() =>
            Container.BindInterfacesAndSelfTo<GameFactory>().AsSingle();

        private void BindWindowService() =>
            Container.BindInterfacesAndSelfTo<GameWindowService>().AsSingle();
    }
}