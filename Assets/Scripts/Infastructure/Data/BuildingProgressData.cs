using System;

namespace Infastructure.Data
{
    [Serializable]
    public class BuildingProgressData
    {
        public string UniqueId;
        public float Progress;
        public int AmountOfUpdates;

        public BuildingProgressData(string uniqueId, float progress, int amountOfUpdates)
        {
            UniqueId = uniqueId;
            Progress = progress;
            AmountOfUpdates = amountOfUpdates;
        }
    }
}