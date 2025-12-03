using BuildProcessManagement.Towers;
using BuildProcessManagement.Towers.UnitSpawnStrategies;
using Infastructure.StaticData.Unit;

namespace Infastructure.Services.UnitSpawnStrategy
{
    public interface IUnitSpawnStrategyService
    {
        IUnitSpawnStrategy GetStrategy(UnitTypeId unitType);
    }
}