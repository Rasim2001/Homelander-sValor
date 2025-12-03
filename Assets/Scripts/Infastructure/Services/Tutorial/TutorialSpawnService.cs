using System.Collections.Generic;
using _Tutorial;
using Enemy;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Pool;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Unit;
using Loots;
using Units;
using UnityEngine;

namespace Infastructure.Services.Tutorial
{
    public class TutorialSpawnService : ITutorialSpawnService
    {
        private readonly IStaticDataService _staticData;
        private readonly IGameFactory _gameFactory;
        private readonly IPoolObjects<CoinLoot> _pool;


        public GameObject FightPointObject { get; private set; }
        public GameObject EnemyObject { get; private set; }
        public GameObject ChestObject { get; private set; }
        public List<GameObject> HomelessList { get; } = new List<GameObject>();
        public List<GameObject> WarriorsList { get; } = new List<GameObject>();

        public TutorialSpawnService(IStaticDataService staticData, IGameFactory gameFactory,
            IPoolObjects<CoinLoot> pool)
        {
            _staticData = staticData;
            _gameFactory = gameFactory;
            _pool = pool;
        }


        public void StartSpawn()
        {
            List<TutorialData> tutorialObjects = _staticData.TutorialStaticData.TutorialObjects;

            foreach (TutorialData tutorialObject in tutorialObjects)
                Spawn(tutorialObject);
        }


        private void Spawn(TutorialData tutorialObject)
        {
            switch (tutorialObject.TypeId)
            {
                case TutorialObjectTypeId.Coin:
                    SpawnCoins(tutorialObject);
                    break;
                case TutorialObjectTypeId.Chest:
                    SpawnChest(tutorialObject);
                    break;
                case TutorialObjectTypeId.Homeless:
                    SpawnHomeless(tutorialObject);
                    break;
                case TutorialObjectTypeId.Shielder:
                case TutorialObjectTypeId.Archer:
                    SpawnWarriors(tutorialObject, tutorialObject.TypeId);
                    break;
                case TutorialObjectTypeId.FightPoint:
                    SpawnFightPoint(tutorialObject);
                    break;
                case TutorialObjectTypeId.Enemy:
                    SpawnEnemy(tutorialObject);
                    break;
            }
        }

        private void SpawnChest(TutorialData tutorialObject)
        {
            ChestObject = Object.Instantiate(Resources.Load<GameObject>(AssetsPath.ChestPath));
            ChestObject.transform.position = tutorialObject.Position;
        }

        private void SpawnHomeless(TutorialData tutorialObject)
        {
            GameObject unitObject = _gameFactory.CreateUnit(UnitTypeId.Homeless);
            unitObject.transform.position = tutorialObject.Position;

            HomelessList.Add(unitObject);
        }

        private void SpawnCoins(TutorialData tutorialObject)
        {
            CoinLoot coinLoot = _pool.GetObjectFromPool();
            coinLoot.transform.position = tutorialObject.Position;
        }

        private void SpawnWarriors(TutorialData tutorialObject, TutorialObjectTypeId typeId)
        {
            GameObject unitObject =
                _gameFactory.CreateUnit(typeId == TutorialObjectTypeId.Shielder
                    ? UnitTypeId.Shielder
                    : UnitTypeId.Archer);


            unitObject.transform.position = tutorialObject.Position;
            unitObject.GetComponentInChildren<UnitAggressionZoneBase>().enabled = false;

            WarriorsList.Add(unitObject);
        }

        private void SpawnFightPoint(TutorialData tutorialObject)
        {
            FightPointObject = Object.Instantiate(Resources.Load<GameObject>(AssetsPath.FightPoint));
            FightPointObject.transform.position = tutorialObject.Position;
        }

        private void SpawnEnemy(TutorialData tutorialObject)
        {
            EnemyObject = _gameFactory.CreateEnemy(EnemyTypeId.Marauder, tutorialObject.Position.x);

            EnemyObject.GetComponent<EnemyMove>().enabled = false;
            EnemyObject.GetComponent<EnemyAttack>().enabled = false;
        }
    }
}