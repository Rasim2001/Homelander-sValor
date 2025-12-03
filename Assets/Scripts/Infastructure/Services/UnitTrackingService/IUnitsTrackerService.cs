using System;
using Infastructure.StaticData.Unit;

namespace Infastructure.Services.UnitTrackingService
{
    public interface IUnitsTrackerService
    {
        void AddUnit(UnitTypeId unitTypeId);
        void RemoveUnit(UnitTypeId unitTypeId);
        Action<UnitTypeId, int> OnUnitCountChanged { get; set; }
        int GetUnitsCount(UnitTypeId unitTypeId);
    }
}