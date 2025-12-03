using System.Collections.Generic;
using Infastructure.StaticData.Building;
using UnityEngine;

namespace Infastructure.StaticData.CardsData
{
    [CreateAssetMenu(fileName = "CardData", menuName = "StaticData/CardData")]
    public class BuildingUpgradeStaticData : ScriptableObject
    {
        public BuildingTypeId BuildingTypeId;
        public BuildingLevelId BuildingLevelId;

        public List<UpgradeCardData> CardsData;
    }
}