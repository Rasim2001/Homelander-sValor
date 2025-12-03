using UnityEngine;

namespace Infastructure.StaticData.Forest
{
    [CreateAssetMenu(fileName = "ForestData", menuName = "StaticData/Forest")]
    public class ForestStaticData : ScriptableObject
    {
        public GameObject[] TreePrefabs;
        public float MinDistanceBetweenTrees = 1f;
        public int TreeCount = 30;

        public GameObject Container;
    }
}