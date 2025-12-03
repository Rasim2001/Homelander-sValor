using System;
using UnityEngine;

namespace Infastructure.StaticData.Forest
{
    [Serializable]
    public class ForestData
    {
        public Vector2 Position;
        public Vector2 ColliderOffset;
        public Vector2 ColliderSize;

        public ForestData(Vector2 position, Vector2 colliderOffset, Vector2 colliderSize)
        {
            Position = position;
            ColliderOffset = colliderOffset;
            ColliderSize = colliderSize;
        }
    }
}