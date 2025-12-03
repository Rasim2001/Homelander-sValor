namespace Infastructure.Services.AutomatizationService.Homeless
{
    public interface IHomelessOrder
    {
        bool HasAvailableSlot();
        int NumberOfOrders();
    }
}