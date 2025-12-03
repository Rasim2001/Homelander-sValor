using System;
using UnityEngine;

namespace Infastructure.StaticData.VagabondCampManagement
{
    [Serializable]
    public class VagabondCampData
    {
        public string UniqueId;

        public Vector3 Position;

        public VagabondCampData(Vector3 position, string uniqueId)
        {
            Position = position;
            UniqueId = uniqueId;
        }
    }
}