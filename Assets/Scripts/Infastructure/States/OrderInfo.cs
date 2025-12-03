using Player.Orders;

namespace Infastructure.States
{
    public class OrderInfo
    {
        public OrderMarker OrderMarker;
        public int IndexOrder;

        public OrderInfo(OrderMarker orderMarker, int indexOrder)
        {
            OrderMarker = orderMarker;
            IndexOrder = indexOrder;
        }
    }
}