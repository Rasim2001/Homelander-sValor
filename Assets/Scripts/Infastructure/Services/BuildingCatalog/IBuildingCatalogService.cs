using Infastructure.StaticData.Building;
using UnityEngine;

namespace Infastructure.Services.BuildingCatalog
{
    public interface IBuildingCatalogService
    {
        void Initialize(Transform mainContainer);
        void CreateCatalog(BuildingTypeId typeId);
        void RemoveCatalogItem(BuildingTypeId typeId);
    }
}