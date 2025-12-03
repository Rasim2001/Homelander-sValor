using Infastructure.StaticData.Building;
using UnityEngine;

namespace UI.GameplayUI.TowerSelectionUI
{
    public class BuildingTypeInfo
    {
        public readonly BuildingTypeId BuildingTypeId;
        public readonly Transform MoveTransform;

        public int Amount;

        public BuildingTypeInfo(BuildingTypeId buildingTypeId, Transform moveTransform)
        {
            BuildingTypeId = buildingTypeId;
            MoveTransform = moveTransform;

            Amount = 1;
        }
    }
}