using System;
using System.Collections.Generic;

namespace Infastructure.Data
{
    [Serializable]
    public class FutureOrdersData
    {
        public List<OrderData> OrderDatas = new List<OrderData>();
    }


    [Serializable]
    public class OrderData
    {
        public string UniqueId;
        public int IndexOrder;

        public OrderData(string uniqueId, int indexOrder)
        {
            UniqueId = uniqueId;
            IndexOrder = indexOrder;
        }
    }
}