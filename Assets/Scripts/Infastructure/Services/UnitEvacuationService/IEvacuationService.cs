using Units.UnitStatusManagement;

namespace Infastructure.Services.UnitEvacuationService
{
    public interface IEvacuationService
    {
        void AddUnit(UnitStatus unitStatus);
        void RemoveUnit(UnitStatus unitStatus);
        void EvacuateAllUnits();
        void ReleaseOfDefenseUnits();
    }
}