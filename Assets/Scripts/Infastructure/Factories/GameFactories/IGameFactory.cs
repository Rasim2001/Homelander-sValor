using Enemy;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.CardsData;
using Infastructure.StaticData.Matryoshka;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.Unit;
using Infastructure.StaticData.WaveOfEnemies;
using Units.Vagabond;
using UnityEngine;

namespace Infastructure.Factories.GameFactories
{
    public interface IGameFactory
    {
        GameObject CreateBuilding(
            BuildingTypeId buildingTypeId,
            Vector3 position,
            string spawnerDataUniqueId,
            BuildingLevelId levelId = BuildingLevelId.Level1,
            CardId cardId = CardId.Default,
            CardId cardKey = CardId.Default);

        GameObject CreatePlayer();
        GameObject CreateUnit(UnitTypeId unitTypeId, float positionX = 0);
        GameObject CreateHUD();
        GameObject CreateEnemy(EnemyTypeId enemyTypeId, float savedSpawnPointX);
        void CreateEnemyCamp(MicroWavesInfo microWavesInfo, Vector3 position, int hp, string enemyCampUniqueId);
        GameObject CreateCristalUI();

        GameObject CreateFirstBuilding(BuildingUpgradeData buildingUpgradeData, BuildingTypeId buildingTypeId,
            Vector3 position);

        GameObject CreateResource(ResourceId resourceId, Vector3 position, string uniqueId);
        GameObject CreateScheme(BuildingTypeId buildingTypeId, Vector3 position);
        GameObject CreateBarricadeFlag(Vector3 position, string uniqueId);
        void CreateEnemyMatryoshka(EnemyTypeId enemyTypeId, MatryoshkaId previous, Vector3 position);
        VagabondCamp CreateVagabondCamp(Vector3 position);
        void CreateEnemyCrystal(Vector3 position, string uniqueId);
    }
}