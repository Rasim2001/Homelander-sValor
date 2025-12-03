using System;
using System.Collections.Generic;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using Infastructure.StaticData.Schemes;
using UnityEngine;

namespace Infastructure.StaticData.Bonfire
{
    [CreateAssetMenu(fileName = "BonfireData", menuName = "StaticData/Bonfire")]
    public class BonfireStaticData : ScriptableObject
    {
        public List<BonfireLevelData> Levels;
    }


    [Serializable]
    public class BonfireLevelData
    {
        public int LevelId;
        public int CoinsValue;
        public int Hp;

        public GameObject UpgradedBonfireObject;

        public List<SchemeConfig> SchemeConfigs;
        public List<RequiredBuildData> RequiredBuildings;
    }

    [Serializable]
    public class RequiredBuildData
    {
        public BuildingTypeId BuildingTypeId;
        public BuildingLevelId LevelId;
        public CardId CardKey;

        public int Amount;
    }
}