using Player.Orders;
using Units.UnitStatusManagement;

namespace Infastructure.Services.AutomatizationService.Homeless
{
    public interface IHomelessOrdersService
    {
        void AddHomeless(UnitStatus unitStatus);
        void RemoveHomeless(UnitStatus unitStatus);
        void AddOrder(IHomelessOrder order, OrderMarker orderMarker, float positionX, string uniqueId);
        void RemoveOrder(IHomelessOrder order);
        void ContinueExecuteOrders();
        void CompleteOrder(IHomelessOrder order, UnitStatus currentUnit);
        void ReleaseOtherUnits(IHomelessOrder order);
        void AddHomelessTemp(UnitStatus unitStatus);
        void RemoveHomelessTemp(UnitStatus unitStatus);
    }
}