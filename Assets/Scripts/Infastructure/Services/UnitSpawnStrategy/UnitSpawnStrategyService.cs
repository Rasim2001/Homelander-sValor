using System.Collections.Generic;
using BuildProcessManagement.Towers.UnitSpawnStrategies;
using Infastructure.StaticData.Unit;

namespace Infastructure.Services.UnitSpawnStrategy
{
    public class UnitSpawnStrategyService : IUnitSpawnStrategyService
    {
        private readonly Dictionary<UnitTypeId, IUnitSpawnStrategy> _spawnStrategies =
            new Dictionary<UnitTypeId, IUnitSpawnStrategy>
            {
                { UnitTypeId.Marksman, new MarksmanSpawnStrategy() },
                { UnitTypeId.Tarman01, new TarmanSpawnStrategyFirst() },
                { UnitTypeId.Tarman02, new TarmanSpawnStrategySecond() },
                { UnitTypeId.Ballistaman, new BalistamanSpawnStrategy() },
                { UnitTypeId.Fearman, new FearmanSpawnStrategy() },
                { UnitTypeId.Catapultman, new CatapultSpawnStrategy() },
            };

        public IUnitSpawnStrategy GetStrategy(UnitTypeId unitType) =>
            _spawnStrategies.GetValueOrDefault(unitType);
    }
}