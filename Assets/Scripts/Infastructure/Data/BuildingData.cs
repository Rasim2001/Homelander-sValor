using System;
using Infastructure.StaticData.Building;

namespace Infastructure.Data
{
    [Serializable]
    public class BuildingData
    {
        public string UniqueId;
        public BuildingTypeId BuildingTypeId;
        public BuildingLevelId CurrentBuildingLevelId;

        public BuildingData(string uniqueId, BuildingTypeId buildingTypeId,
            BuildingLevelId currentBuildingLevelId)
        {
            UniqueId = uniqueId;
            BuildingTypeId = buildingTypeId;
            CurrentBuildingLevelId = currentBuildingLevelId;
        }
    }
}