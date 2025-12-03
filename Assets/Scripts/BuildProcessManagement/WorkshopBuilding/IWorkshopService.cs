using Infastructure.StaticData.Unit;
using UnityEngine;

namespace BuildProcessManagement.WorkshopBuilding
{
    public interface IWorkshopService
    {
        IWorkshopItemCreator Initialize(WorkshopItemId workshopItemId);
        GameObject SpawnUnitWithProfession(WorkshopItemId workshopItemId);
        GameObject CreateVendor(Vector3 position, UnitTypeId unitTypeId);
    }
}