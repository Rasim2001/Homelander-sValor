using BuildProcessManagement.Towers.FearTower.Samovar;
using BuildProcessManagement.Towers.SpawnOnTowers;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Unit;
using Units.RangeUnits.AttackOnTower;
using UnityEngine;

namespace BuildProcessManagement.Towers.UnitSpawnStrategies
{
    public class FearmanSpawnStrategy : IUnitSpawnStrategy
    {
        private readonly int _rangeX = 8;

        public GameObject SpawnUnit(IGameFactory gameFactory, TowerUnitSpawnInfo spawnInfo, Transform towerTransform)
        {
            if (spawnInfo == null)
                return null;

            BoxCollider2D rangeCollider = spawnInfo.UnitObserverTrigger.GetComponent<BoxCollider2D>();
            rangeCollider.size = new Vector2(_rangeX, 1);

            BuildInfo buildInfo = towerTransform.GetComponent<BuildInfo>();

            GameObject unit = gameFactory.CreateUnit(UnitTypeId.Fearman);
            unit.transform.position = spawnInfo.Point.position;
            unit.transform.SetParent(buildInfo.VisualBuilding.transform);
            unit.transform.localScale = Vector3.one;

            ISamovar samovar = towerTransform.GetComponentInChildren<ISamovar>();

            FearmanAttackOnTower fearmanAttackOnTower = unit.GetComponent<FearmanAttackOnTower>();
            fearmanAttackOnTower.Initialize(spawnInfo.UnitObserverTrigger);
            fearmanAttackOnTower.Initialize(samovar);

            return unit;
        }
    }
}