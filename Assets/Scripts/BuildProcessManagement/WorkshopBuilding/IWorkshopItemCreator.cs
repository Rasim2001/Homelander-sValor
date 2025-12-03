using UnityEngine;

namespace BuildProcessManagement.WorkshopBuilding
{
    public interface IWorkshopItemCreator
    {
        void CreateItem(Vector3 positoin);
        void Reduce();
    }
}