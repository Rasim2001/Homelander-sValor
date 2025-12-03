using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Unit;
using Units;
using Units.RangeUnits;
using Units.RangeUnits.AttackOnTower;
using UnityEngine;

namespace BuildProcessManagement.Towers.UnitSpawnStrategies
{
    public class MarksmanSpawnStrategy : IUnitSpawnStrategy
    {
        private readonly int _rangeX = 6;

        public GameObject SpawnUnit(IGameFactory gameFactory, TowerUnitSpawnInfo spawnInfo, Transform towerTransform)
        {
            if (spawnInfo == null)
                return null;

            BoxCollider2D rangeCollider = spawnInfo.UnitObserverTrigger.GetComponent<BoxCollider2D>();
            rangeCollider.size = new Vector2(_rangeX, 1);
            rangeCollider.offset = new Vector2(_rangeX * 0.75f * spawnInfo.DirectionX, 1);

            GameObject unit = gameFactory.CreateUnit(UnitTypeId.Marksman);
            unit.transform.position = spawnInfo.Point.position;
            unit.transform.SetParent(towerTransform);

            MarksmenAttackOnTower attackOnTower = unit.GetComponent<MarksmenAttackOnTower>();
            attackOnTower.Initialize(spawnInfo.UnitObserverTrigger);
            attackOnTower.Enter();

            UnitFlip unitFlip = unit.GetComponent<UnitFlip>();
            unitFlip.SetFlip(spawnInfo.DirectionX != 1);

            return unit;
        }
    }
}