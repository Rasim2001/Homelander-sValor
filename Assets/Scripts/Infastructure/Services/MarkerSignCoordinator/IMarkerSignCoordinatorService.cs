using Player.Orders;

namespace Infastructure.Services.MarkerSignCoordinator
{
    public interface IMarkerSignCoordinatorService
    {
        void AddMarker(OrderMarker orderMarker);
        void RemoveMarker(OrderMarker orderMarker);
    }
}