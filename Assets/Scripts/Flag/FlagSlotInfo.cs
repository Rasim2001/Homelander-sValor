using System;
using System.Collections.Generic;
using Infastructure.StaticData.Unit;
using UnityEngine;

namespace Flag
{
    [Serializable]
    public class FlagSlotInfo
    {
        public float OffsetBetweenUnits;
        public float OffsetNextGroups;
        public UnitTypeId UnitTypeId;
        public List<Transform> BindedUnits = new List<Transform>();
    }
}