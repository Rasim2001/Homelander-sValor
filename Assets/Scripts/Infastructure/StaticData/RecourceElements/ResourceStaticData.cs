using UnityEngine;

namespace Infastructure.StaticData.RecourceElements
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "StaticData/Resource")]
    public class ResourceStaticData : ScriptableObject
    {
        public ResourceId ResourceId;
        public ResourceData ResourceData;
    }
}