using System.Collections.Generic;
using System.Linq;
using Infastructure.Services.AssetProvider;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.StaticDataService;
using UI.GameplayUI.TowerSelectionUI;
using UI.GameplayUI.TowerSelectionUI.MoveItems;
using UnityEngine;
using UnityEngine.UI;

namespace Infastructure.Services.BuildModeServices
{
    public class BuildingModeConfigurationService : IBuildingModeConfigurationService
    {
        public List<BuildingTypeInfo> BuildingTypeInfos { get; private set; } = new List<BuildingTypeInfo>();
        public int CorrectIndex { get; set; }

        private readonly IStaticDataService _staticDataService;
        private readonly IAssetProviderService _assetProviderService;


        private Transform _container;
        private MoveBuildingUI _moveBuildingUI;

        private readonly List<BuildingTypeId> _sortTypes = new List<BuildingTypeId>
        {
            BuildingTypeId.HammerWorkshop,
            BuildingTypeId.BowWorkshop,
            BuildingTypeId.ShielderWorkshop,
            BuildingTypeId.ResetWorkshop,
            BuildingTypeId.Baricade,
            BuildingTypeId.TowerBow,
            BuildingTypeId.TowerFire,
            BuildingTypeId.TowerTar,
        };

        public BuildingModeConfigurationService(IStaticDataService staticDataService,
            IAssetProviderService assetProviderService)
        {
            _staticDataService = staticDataService;
            _assetProviderService = assetProviderService;
        }

        public void Initialize(Transform container, MoveBuildingUI moveBuildingUI)
        {
            _container = container;
            _moveBuildingUI = moveBuildingUI;
        }

        public void CreateItemUI(BuildingTypeId typeId)
        {
            BuildingStaticData buildingStaticData = _staticDataService.ForBuilding(typeId);

            BuildingTypeInfo targetInfo = BuildingTypeInfos.FirstOrDefault(info => info.BuildingTypeId == typeId);

            if (targetInfo != null)
            {
                targetInfo.Amount++;
                UpdateBuildingItem(targetInfo);
            }
            else
            {
                CreateNewBuildingItem(typeId, buildingStaticData);
                SortUI();
                UpdateNumberOfItems();
            }
        }

        public void RemoveItemUI(BuildingTypeId typeId)
        {
            BuildingTypeInfo targetInfo = BuildingTypeInfos.FirstOrDefault(info => info.BuildingTypeId == typeId);

            if (targetInfo.Amount == 1)
            {
                DestroyItemUI(targetInfo);
                UpdateNumberOfItems();
            }
            else
            {
                targetInfo.Amount--;
                UpdateBuildingItem(targetInfo);
            }
        }

        public BuildingTypeId GetItem()
        {
            if (CorrectIndex >= 0 && CorrectIndex < BuildingTypeInfos.Count)
                return BuildingTypeInfos[CorrectIndex].BuildingTypeId;

            return BuildingTypeId.Unknow;
        }


        public bool HasBuilding() =>
            BuildingTypeInfos.Count != 0;

        private void DestroyItemUI(BuildingTypeInfo targetInfo)
        {
            BuildingTypeInfos.Remove(targetInfo);

            Object.Destroy(targetInfo.MoveTransform.gameObject);
        }

        private void UpdateBuildingItem(BuildingTypeInfo targetInfo)
        {
            BuildingItemsUI buildingItemsUI = targetInfo.MoveTransform.GetComponent<BuildingItemsUI>();
            buildingItemsUI.UpdateAmout(targetInfo.Amount.ToString());
        }

        private void CreateNewBuildingItem(BuildingTypeId typeId, BuildingStaticData buildingStaticData)
        {
            GameObject buildInfoPrefab = _assetProviderService.Instantiate(AssetsPath.BuildItemUIPath, _container);

            Image image = buildInfoPrefab.GetComponent<Image>();
            image.sprite = buildingStaticData.HintSprite;

            BuildingTypeInfos.Add(new BuildingTypeInfo(typeId, buildInfoPrefab.transform));
        }

        private void SortUI()
        {
            BuildingTypeInfos =
                BuildingTypeInfos.OrderBy(x => _sortTypes.IndexOf(x.BuildingTypeId)).ToList();

            for (int i = 0; i < BuildingTypeInfos.Count; i++)
                BuildingTypeInfos[i].MoveTransform.SetSiblingIndex(i);
        }


        private void UpdateNumberOfItems() =>
            _moveBuildingUI.UpdateNumberOfItems();
    }
}