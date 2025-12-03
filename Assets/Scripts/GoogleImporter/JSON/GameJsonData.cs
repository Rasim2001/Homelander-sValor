using System;
using System.Collections.Generic;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.VagabondCampManagement;

namespace GoogleImporter.JSON
{
    [Serializable]
    public class GameJsonData
    {
        public List<BuildingSpawnerData> BuildingSpawners;
        public List<ResourceSpawnerData> ResourceSpawners;
        public List<ForestData> ForestSides;
        public List<VagabondCampData> VagabondCampDatas;
        public List<EnemyCristalConfig> EnemyCristalConfigs;
    }
}