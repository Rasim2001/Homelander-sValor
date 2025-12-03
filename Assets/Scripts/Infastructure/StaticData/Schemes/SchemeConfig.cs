using System;
using Infastructure.StaticData.Building;

namespace Infastructure.StaticData.Schemes
{
    [Serializable]
    public class SchemeConfig
    {
        public BuildingTypeId BuildingTypeId;
        public int Amount;
    }
}