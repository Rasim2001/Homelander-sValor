using System;
using Infastructure.StaticData.Unit;
using UnityEngine;

namespace Infastructure.Data
{
    [Serializable]
    public class UnitData
    {
        public UnitTypeId UnitTypeId;
        public Vector3 Position;
        public bool IsBindedToPlayer;

        public string BindedToFlagUniqueId;

        public string OrderUniqueId;
        public int FreePlaceIndex;


        public UnitData(
            UnitTypeId unitTypeId,
            Vector3 position,
            bool isBindedToPlayer,
            string bindedToFlagUniqueId,
            string orderUniqueId,
            int freePlaceIndex)
        {
            UnitTypeId = unitTypeId;
            Position = position;
            IsBindedToPlayer = isBindedToPlayer;
            BindedToFlagUniqueId = bindedToFlagUniqueId;
            OrderUniqueId = orderUniqueId;
            FreePlaceIndex = freePlaceIndex;
        }
    }
}