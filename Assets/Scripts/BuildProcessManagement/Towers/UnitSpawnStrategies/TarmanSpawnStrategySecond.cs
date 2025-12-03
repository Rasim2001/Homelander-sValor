using BuildProcessManagement.Towers.BallistaTar;
using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Unit;
using Units.RangeUnits.AttackOnTower;
using UnityEngine;

namespace BuildProcessManagement.Towers.UnitSpawnStrategies
{
    public class TarmanSpawnStrategySecond : IUnitSpawnStrategy
    {
        private const int DeltaAngle = 0;
        private readonly int _rangeX = 3;

        public GameObject SpawnUnit(IGameFactory gameFactory, TowerUnitSpawnInfo spawnInfo, Transform towerTransform)
        {
            BoxCollider2D rangeCollider = spawnInfo.UnitObserverTrigger.GetComponent<BoxCollider2D>();
            rangeCollider.size = new Vector2(_rangeX, 1);

            BuildInfo buildInfo = towerTransform.GetComponent<BuildInfo>();

            GameObject unit = gameFactory.CreateUnit(UnitTypeId.Tarman02);
            unit.transform.position = spawnInfo.Point.position;
            unit.transform.SetParent(buildInfo.VisualBuilding.transform);
            unit.transform.localScale = Vector3.one;

            IBallistaRotation ballista = towerTransform.GetComponentInChildren<IBallistaRotation>();
            ballista.Initialize(DeltaAngle);

            ICarriage carriage = towerTransform.GetComponentInChildren<ICarriage>();
            ITarStonesAnimator tarStonesAnimator = towerTransform.GetComponentInChildren<ITarStonesAnimator>();

            TarmanAttackOnTowerSecond tarmanAttackOnTower = unit.GetComponent<TarmanAttackOnTowerSecond>();
            tarmanAttackOnTower.Initialize(spawnInfo.UnitObserverTrigger);
            tarmanAttackOnTower.Initialize(ballista, carriage, tarStonesAnimator);

            return unit;
        }
    }
}