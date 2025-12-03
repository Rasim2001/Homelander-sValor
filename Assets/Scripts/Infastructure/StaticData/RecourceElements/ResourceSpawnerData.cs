using System;
using UnityEngine;

namespace Infastructure.StaticData.RecourceElements
{
    [Serializable]
    public class ResourceSpawnerData
    {
        public string UniqueId;
        public Vector3 Position;
        public ResourceId ResourceId;

        public ResourceSpawnerData(string uniqueId, Vector3 position, ResourceId resourceId)
        {
            UniqueId = uniqueId;
            Position = position;
            ResourceId = resourceId;
        }
    }
}