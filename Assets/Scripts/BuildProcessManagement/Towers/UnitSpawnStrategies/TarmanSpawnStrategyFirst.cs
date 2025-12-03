using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Unit;
using Units;
using Units.RangeUnits.AttackOnTower;
using UnityEngine;

namespace BuildProcessManagement.Towers.UnitSpawnStrategies
{
    public class TarmanSpawnStrategyFirst : IUnitSpawnStrategy
    {
        private readonly int _rangeX = 3;

        public GameObject SpawnUnit(IGameFactory gameFactory, TowerUnitSpawnInfo spawnInfo, Transform towerTransform)
        {
            BoxCollider2D rangeCollider = spawnInfo.UnitObserverTrigger.GetComponent<BoxCollider2D>();
            rangeCollider.size = new Vector2(_rangeX, 1);

            BuildInfo buildInfo = towerTransform.GetComponent<BuildInfo>();

            GameObject unit = gameFactory.CreateUnit(UnitTypeId.Tarman01);
            unit.transform.SetParent(buildInfo.VisualBuilding.transform);
            unit.transform.position = spawnInfo.Point.position;
            unit.transform.localScale = Vector3.one;

            TarmanAttackOnTowerFirst tarmanAttackOnTower = unit.GetComponent<TarmanAttackOnTowerFirst>();
            tarmanAttackOnTower.Initialize(spawnInfo.UnitObserverTrigger);

            return unit;
        }
    }
}