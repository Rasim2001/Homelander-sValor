using System;
using _Tutorial;
using UnityEngine;

namespace Infastructure.StaticData.Tutorial
{
    [Serializable]
    public class TutorialData
    {
        public TutorialObjectTypeId TypeId;
        public Vector3 Position;

        public TutorialData(Vector3 position, TutorialObjectTypeId typeId)
        {
            Position = position;
            TypeId = typeId;
        }
    }
}