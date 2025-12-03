using System;
using System.Collections;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Bonfire;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.Schemes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Infastructure.Services.SchemeSpawner
{
    public class SchemesSpawnerService : ISchemesSpawnerService
    {
        private readonly IGameFactory _gameFactory;
        private readonly ICoroutineRunner _coroutineRunner;

        public SchemesSpawnerService(IGameFactory gameFactory, ICoroutineRunner coroutineRunner)
        {
            _gameFactory = gameFactory;
            _coroutineRunner = coroutineRunner;
        }

        public void CreateShemesByMainflag(BonfireLevelData bonfireLevelData, Vector3 position, Action onCompleted) =>
            _coroutineRunner.StartCoroutine(StartCreateSchemesCoroutine(bonfireLevelData, position, onCompleted));

        public void CreateShemesByType(BuildingTypeId buildingTypeId, Vector3 position, Vector2 randomDirection)
        {
            GameObject scheme = _gameFactory.CreateScheme(buildingTypeId, position);
            Rigidbody2D rb = scheme.GetComponent<Rigidbody2D>();

            rb.velocity = randomDirection;
        }

        private IEnumerator StartCreateSchemesCoroutine(BonfireLevelData bonfireLevelData, Vector3 position,
            Action onCompleted)
        {
            foreach (SchemeConfig schemeConfig in bonfireLevelData.SchemeConfigs)
            {
                for (int i = 0; i < schemeConfig.Amount; i++)
                {
                    Vector2 randomDirection = new Vector2(Random.Range(-3f, 3f), Random.Range(5, 10));

                    CreateShemesByType(schemeConfig.BuildingTypeId, position, randomDirection);

                    yield return new WaitForSeconds(0.25f);
                }
            }

            onCompleted?.Invoke();
        }
    }
}