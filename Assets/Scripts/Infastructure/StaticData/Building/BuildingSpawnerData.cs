using System;
using UnityEngine;

namespace Infastructure.StaticData.Building
{
    [Serializable]
    public class BuildingSpawnerData
    {
        public string UniqueId;
        public Vector3 Position;
        public BuildingTypeId BuildingTypeId;

        public BuildingSpawnerData(Vector3 position, BuildingTypeId buildingTypeId, string uniqueId)
        {
            Position = position;
            BuildingTypeId = buildingTypeId;
            UniqueId = uniqueId;
        }
    }
}