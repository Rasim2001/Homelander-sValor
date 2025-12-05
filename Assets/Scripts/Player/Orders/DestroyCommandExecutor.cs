using BuildProcessManagement;
using Flag;
using FogOfWar;
using Grid;
using Infastructure.Services.BuildingRegistry;
using Infastructure.Services.Fence;
using Infastructure.Services.MinimapManagement;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.SchemeSpawner;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;

namespace Player.Orders
{
    public class DestroyCommandExecutor : IDestroyCommandExecutor
    {
        private readonly IStaticDataService _staticData;
        private readonly IPersistentProgressService _persistentProgressService;
        private readonly IGridMap _gridMap;
        private readonly ISchemesSpawnerService _schemesSpawnerService;
        private readonly IFenceService _fenceService;
        private readonly IBuildingRegistryService _buildingRegistryService;
        private readonly IMinimapNotifierService _minimapNotifierService;
        private readonly IFogOfWarMinimap _fogOfWarMinimap;

        public DestroyCommandExecutor(
            IStaticDataService staticData,
            IPersistentProgressService persistentProgressService,
            IGridMap gridMap,
            ISchemesSpawnerService schemesSpawnerService,
            IFenceService fenceService,
            IBuildingRegistryService buildingRegistryService,
            IMinimapNotifierService minimapNotifierService,
            IFogOfWarMinimap fogOfWarMinimap)
        {
            _staticData = staticData;
            _persistentProgressService = persistentProgressService;
            _gridMap = gridMap;
            _schemesSpawnerService = schemesSpawnerService;
            _fenceService = fenceService;
            _buildingRegistryService = buildingRegistryService;
            _minimapNotifierService = minimapNotifierService;
            _fogOfWarMinimap = fogOfWarMinimap;
        }

        public void DestroyBuild(BuildInfo buildInfo)
        {
            BuildingStaticData buildingStaticData = _staticData.ForBuilding(buildInfo.BuildingTypeId);

            BuildingUpgradeData buildingUpgradeData =
                _staticData.ForBuilding(buildInfo.BuildingTypeId, buildInfo.CurrentLevelId, buildInfo.PreviousCardId);


            ClearOccupyCells(buildInfo, buildingStaticData, buildingUpgradeData);
            GetMoneyFromDestroyableBuild(buildingUpgradeData.CoinsValue);
            SpawnScheme(buildInfo);
            Destroy(buildInfo);
        }

        private void SpawnScheme(BuildInfo buildInfo)
        {
            Vector2 randomDirection = new Vector2(Random.Range(-3f, 3f), Random.Range(2, 5));

            _schemesSpawnerService.CreateShemesByType(buildInfo.BuildingTypeId,
                buildInfo.transform.position + new Vector3(0, 2, 0), randomDirection);
        }

        private void Destroy(BuildInfo buildInfo)
        {
            if (buildInfo.BuildingTypeId == BuildingTypeId.Baricade)
            {
                FlagActivator flagActivator = buildInfo.GetComponentInChildren<FlagActivator>();
                flagActivator.DestroyFlag();

                _fenceService.DestroyFence((int)buildInfo.transform.position.x);
                _minimapNotifierService.BarricadeDestroyedNotify(buildInfo.transform.position);
            }

            _buildingRegistryService.RemoveBuild(buildInfo);
            _fogOfWarMinimap.UpdateFogPositionAfterDestroyBuild(buildInfo.transform.position.x);

            Object.Destroy(buildInfo.gameObject);
        }

        private void ClearOccupyCells(BuildInfo buildInfo, BuildingStaticData buildingStaticData,
            BuildingUpgradeData buildingUpgradeData) =>
            _gridMap.ClearCells((int)buildInfo.transform.position.x, buildingStaticData, buildingUpgradeData);

        private void GetMoneyFromDestroyableBuild(int coinsValue)
        {
            int coinsValueAfterDestroy = coinsValue / 2;

            _persistentProgressService.PlayerProgress.CoinData.Collect(coinsValueAfterDestroy);
        }
    }
}