using Infastructure.StaticData.Building;
using UnityEngine;

namespace Infastructure.StaticData.Schemes
{
    [CreateAssetMenu(fileName = "SchemeData", menuName = "StaticData/Scheme")]
    public class SchemeStaticData : ScriptableObject
    {
        public BuildingTypeId BuildingTypeId;
        public GameObject Prefab;
    }
}