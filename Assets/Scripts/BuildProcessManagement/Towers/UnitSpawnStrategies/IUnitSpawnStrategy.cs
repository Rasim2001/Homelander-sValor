using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Factories.GameFactories;
using UnityEngine;

namespace BuildProcessManagement.Towers.UnitSpawnStrategies
{
    public interface IUnitSpawnStrategy
    {
        GameObject SpawnUnit(IGameFactory gameFactory, TowerUnitSpawnInfo spawnInfo, Transform towerTransform);
    }
}