using BuildProcessManagement.Towers.BallistaBow;
using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Unit;
using Units;
using Units.RangeUnits.AttackOnTower;
using UnityEngine;

namespace BuildProcessManagement.Towers.UnitSpawnStrategies
{
    public class BalistamanSpawnStrategy : IUnitSpawnStrategy
    {
        private const int DeltaAngleLeft = 90;
        private const int DeltaAngleRight = 270;

        private readonly int _rangeX = 8;
        private readonly int _distance = 3;

        public GameObject SpawnUnit(IGameFactory gameFactory, TowerUnitSpawnInfo spawnInfo, Transform towerTransform)
        {
            if (spawnInfo == null)
                return null;

            BoxCollider2D rangeCollider = spawnInfo.UnitObserverTrigger.GetComponent<BoxCollider2D>();
            rangeCollider.size = new Vector2(_rangeX, 1);
            rangeCollider.offset = new Vector2(_rangeX / 2 * spawnInfo.DirectionX * _distance, 1);

            GameObject unit = gameFactory.CreateUnit(UnitTypeId.Ballistaman);
            unit.transform.position = spawnInfo.Point.position;
            unit.transform.SetParent(towerTransform);

            UnitFlip unitFlip = unit.GetComponent<UnitFlip>();
            unitFlip.SetFlip(spawnInfo.DirectionX == 1);

            IBallistaBowAnimator ballistaBowAnimator = unit.GetComponentInChildren<IBallistaBowAnimator>();
            IBallistaRotation ballistaRotation = unit.GetComponentInChildren<IBallistaRotation>();
            ballistaRotation.Initialize(spawnInfo.Point.position.x < 0 ? DeltaAngleLeft : DeltaAngleRight);

            BallistaAttackOnTower ballistaAttackOnTower = unit.GetComponent<BallistaAttackOnTower>();
            ballistaAttackOnTower.Initialize(spawnInfo.UnitObserverTrigger);
            ballistaAttackOnTower.Initialize(ballistaBowAnimator, ballistaRotation);

            return unit;
        }
    }
}