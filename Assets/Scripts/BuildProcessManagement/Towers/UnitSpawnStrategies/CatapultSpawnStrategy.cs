using BuildProcessManagement.Towers.CatapultTower;
using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Unit;
using Units.RangeUnits.AttackOnTower;
using UnityEngine;

namespace BuildProcessManagement.Towers.UnitSpawnStrategies
{
    public class CatapultSpawnStrategy : IUnitSpawnStrategy
    {
        private readonly int _rangeX = 10;

        public GameObject SpawnUnit(IGameFactory gameFactory, TowerUnitSpawnInfo spawnInfo, Transform towerTransform)
        {
            if (spawnInfo == null)
                return null;

            BoxCollider2D rangeCollider = spawnInfo.UnitObserverTrigger.GetComponent<BoxCollider2D>();
            rangeCollider.size = new Vector2(_rangeX, 1);
            rangeCollider.offset = new Vector2(15.72f * spawnInfo.DirectionX, 1);

            BuildInfo buildInfo = towerTransform.GetComponent<BuildInfo>();

            GameObject unit = gameFactory.CreateUnit(UnitTypeId.Catapultman);
            unit.transform.position = spawnInfo.Point.position;
            unit.transform.SetParent(buildInfo.VisualBuilding.transform);
            unit.transform.localScale = Vector3.one;

            IBowl bowl = towerTransform.GetComponentInChildren<IBowl>();

            CatapultmanAttackOnTower catapultmanAttackOnTower = unit.GetComponent<CatapultmanAttackOnTower>();
            catapultmanAttackOnTower.Initialize(spawnInfo.UnitObserverTrigger);
            catapultmanAttackOnTower.Initialize(bowl);

            return unit;
        }
    }
}