using System.Collections.Generic;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.Forest;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.VagabondCampManagement;
using UnityEngine;

namespace Infastructure.StaticData
{
    [CreateAssetMenu(fileName = "GameData", menuName = "StaticData/GameData")]
    public class GameStaticData : ScriptableObject
    {
        public List<EnemyCampData> EnemyCamps;
        public List<BuildingSpawnerData> BuildingSpawners;
        public List<ResourceSpawnerData> ResourceSpawners;

        public List<ForestData> ForestSides;
        public List<VagabondCampData> VagabondCampDatas;

        public List<EnemyCristalConfig> EnemyCristalConfigs;
    }
}