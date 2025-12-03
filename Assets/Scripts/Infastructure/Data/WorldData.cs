using System;
using System.Collections.Generic;

namespace Infastructure.Data
{
    [Serializable]
    public class WorldData
    {
        public Vector2Data Position;
        public bool FlipX;

        public List<BuildingData> BuildingData = new List<BuildingData>();
        public List<BuildingProgressData> BuildingProgressData = new List<BuildingProgressData>();
        public List<EnvironmentResouceData> EnvironmentResoucesData = new List<EnvironmentResouceData>();
    }
}