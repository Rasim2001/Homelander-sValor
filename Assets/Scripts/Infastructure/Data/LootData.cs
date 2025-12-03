using System;

namespace Infastructure.Data
{
    [Serializable]
    public class LootData
    {
        public Vector2Data Position;
        public string UniqueId;

        public LootData(Vector2Data position, string uniqueId)
        {
            Position = position;
            UniqueId = uniqueId;
        }
    }
}