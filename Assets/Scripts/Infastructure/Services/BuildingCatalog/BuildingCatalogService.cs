using System.Collections.Generic;
using System.Linq;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.BuildingCatalog;
using Infastructure.StaticData.StaticDataService;
using Tooltip;
using Tooltip.UI;
using UI.GameplayUI.BuildingCatalog;
using UI.GameplayUI.TowerSelectionUI;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.BuildingCatalog
{
    public class BuildingCatalogService : IBuildingCatalogService
    {
        private readonly List<BuildingTypeId> _buildingTypeInfos = new List<BuildingTypeId>();
        private readonly Dictionary<Sprite, BuildingCatalogUI> _catalogs = new Dictionary<Sprite, BuildingCatalogUI>();

        private readonly IStaticDataService _staticDataService;

        private Transform _mainContainer;
        private readonly DiContainer _diContainer;

        public void Initialize(Transform mainContainer) =>
            _mainContainer = mainContainer;

        public BuildingCatalogService(DiContainer diContainer, IStaticDataService staticDataService)
        {
            _diContainer = diContainer;
            _staticDataService = staticDataService;
        }

        public void CreateCatalog(BuildingTypeId typeId)
        {
            BuildingCatalogStaticData catalogData = _staticDataService.ForCatalog(typeId);

            if (!_catalogs.ContainsKey(catalogData.CatalogSprite))
            {
                GameObject buildInfoPrefab =
                    _diContainer.InstantiatePrefabResource(AssetsPath.CatalogUIPath, _mainContainer);
                BuildingCatalogUI buildingCatalogUI = buildInfoPrefab.GetComponent<BuildingCatalogUI>();
                buildingCatalogUI.SetCatalogSprite(catalogData.CatalogSprite);

                TooltipTriggerRuntime tooltipTrigger = buildInfoPrefab.GetComponentInChildren<TooltipTriggerRuntime>();
                tooltipTrigger.Initialize(TooltipPositionId.Above, catalogData.TooltipText);

                _catalogs.Add(catalogData.CatalogSprite, buildingCatalogUI);
            }

            _catalogs[catalogData.CatalogSprite].AddCatalogItem();


            if (!_buildingTypeInfos.Contains(typeId))
            {
                BuildingStaticData buildingData = _staticDataService.ForBuilding(typeId);

                GameObject buildingItemObject =
                    _diContainer.InstantiatePrefabResource(AssetsPath.CatalogItemUIPath,
                        _catalogs[catalogData.CatalogSprite].SubContainer);

                TooltipTriggerRuntime tooltipTrigger = buildingItemObject.GetComponent<TooltipTriggerRuntime>();
                tooltipTrigger.Initialize(TooltipPositionId.Above, buildingData.TooltipText);

                UpdateIconItem(buildingData, buildingItemObject);

                _buildingTypeInfos.Add(typeId);
            }
        }


        public void RemoveCatalogItem(BuildingTypeId typeId)
        {
            BuildingCatalogStaticData catalogData = _staticDataService.ForCatalog(typeId);

            if (_buildingTypeInfos.Contains(typeId))
                _catalogs[catalogData.CatalogSprite].RemoveCatalogItem();
        }


        private void UpdateIconItem(BuildingStaticData buildingData, GameObject buildingItemObject)
        {
            CatalogItemUI catalogItemUI = buildingItemObject.GetComponent<CatalogItemUI>();
            catalogItemUI.UpdateIcon(buildingData.CatalogItemUI);
        }
    }
}