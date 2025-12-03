using Player.Orders;
using Units.UnitStatusManagement;

namespace Infastructure.Services.AutomatizationService.Builders
{
    public interface IFutureOrdersService
    {
        void AddBuilder(UnitStatus builder);
        void AddOrder(OrderMarker orderMarker);
        void RemoveCompletedOrder(OrderMarker orderMarker);
        void ContinueExecuteOrders();
        void RemoveBuilder(UnitStatus builder);
        void FilterNightOrders();
        void ClearNightOrders();
        OrderMarker GetOrderByOrderUniqueId(string uniqueId);
    }
}