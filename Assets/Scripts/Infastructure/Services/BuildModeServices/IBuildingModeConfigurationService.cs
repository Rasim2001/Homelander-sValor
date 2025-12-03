using System.Collections.Generic;
using Infastructure.StaticData.Building;
using UI.GameplayUI.TowerSelectionUI;
using UI.GameplayUI.TowerSelectionUI.MoveItems;
using UnityEngine;

namespace Infastructure.Services.BuildModeServices
{
    public interface IBuildingModeConfigurationService
    {
        void Initialize(Transform container, MoveBuildingUI moveBuildingUI);
        void CreateItemUI(BuildingTypeId typeId);
        void RemoveItemUI(BuildingTypeId typeId);
        List<BuildingTypeInfo> BuildingTypeInfos { get; }
        int CorrectIndex { get; set; }
        BuildingTypeId GetItem();
        bool HasBuilding();
    }
}