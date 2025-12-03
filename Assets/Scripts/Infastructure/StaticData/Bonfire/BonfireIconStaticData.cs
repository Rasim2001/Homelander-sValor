using System;
using System.Collections.Generic;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using UnityEngine;

namespace Infastructure.StaticData.Bonfire
{
    [CreateAssetMenu(fileName = "BonfireIconData", menuName = "StaticData/BonfireIconsData")]
    public class BonfireIconStaticData : ScriptableObject
    {
        public CoinsIconData CoinsIconData;
        public List<SchemeIconData> SchemeIconsData;
        public List<RequiredBuildIconData> RequiredBuildIconsData;
        public List<MainFlagIconData> MainFlagIconsDatas;
    }


    [Serializable]
    public class SchemeIconData
    {
        public BuildingTypeId BuildingTypeId;
        public Sprite Icon;
    }

    [Serializable]
    public class RequiredBuildIconData
    {
        public BuildingTypeId BuildingTypeId;
        public BuildingLevelId LevelId;
        public CardId CardKey;

        public Sprite Icon;
        public Sprite DisabledIcon;
    }

    [Serializable]
    public class CoinsIconData
    {
        public Sprite Icon;
        public Sprite DisabledIcon;
    }

    [Serializable]
    public class MainFlagIconData
    {
        public int Level;
        public Sprite Icon;
    }
}