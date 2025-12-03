using Infastructure.Services.AutomatizationService;
using Infastructure.Services.AutomatizationService.Homeless;

namespace BuildProcessManagement.WorkshopBuilding
{
    public interface IWorkshop : IHomelessOrder
    {
        void ReduceItemsAmount();
        bool HasVendor { get; set; }
        void StartSpawnItems();
        void CreateVendor();
        void ReduceIndex();
    }
}