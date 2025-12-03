using System;
using Player.Orders;

namespace Infastructure.Services.AutomatizationService.Homeless
{
    [Serializable]
    public class HomelessOrderInfo
    {
        public IHomelessOrder HomelessOrder;
        public float PositionX;
        public string UniqueId;
        public OrderMarker OrderMarker;

        public HomelessOrderInfo(IHomelessOrder homelessOrder, float positionX, string uniqueId, OrderMarker orderMarker)
        {
            HomelessOrder = homelessOrder;
            PositionX = positionX;
            UniqueId = uniqueId;
            OrderMarker = orderMarker;
        }
    }
}