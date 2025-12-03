using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using UnityEngine;

namespace Test
{
    [CreateAssetMenu(fileName = "TestData", menuName = "StaticData/Test")]
    public class TestStaticData : ScriptableObject
    {
        public BuildingTypeId BuildingTypeId;
        public CardId CardId;
    }
}