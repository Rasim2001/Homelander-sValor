using System.Collections;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Flag;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.WaveOfEnemies;
using UI.GameplayUI;
using UnityEngine;

namespace Infastructure.Services.EnemyWaves
{
    public class EnemySpawnService : IEnemySpawnService
    {
        private readonly IFlagTrackerService _flagTrackerService;
        private readonly IGameFactory _gameFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IStaticDataService _staticDataService;

        private Transform _playerTransform;
        private WaveNotificatorUI _waveNotificatorUI;
        private Coroutine _spawnCoroutine;

        public int WavePassedWaveId { get; private set; }

        public EnemySpawnService(IFlagTrackerService flagTrackerService, IGameFactory gameFactory,
            ICoroutineRunner coroutineRunner, IStaticDataService staticDataService)
        {
            _flagTrackerService = flagTrackerService;
            _gameFactory = gameFactory;
            _coroutineRunner = coroutineRunner;
            _staticDataService = staticDataService;
        }

        public void Initialize(Transform playerTransform, WaveNotificatorUI waveNotificatorUI)
        {
            _playerTransform = playerTransform;
            _waveNotificatorUI = waveNotificatorUI;
        }

        public void StartSpawnEnemies(int levelId, int waveId)
        {
            _spawnCoroutine = _coroutineRunner.StartCoroutine(StartSpawnEnemiesCoroutine(levelId, waveId));

            WavePassedWaveId = waveId;
        }

        public bool EnemyWaveFinished() =>
            _spawnCoroutine == null;

        private IEnumerator StartSpawnEnemiesCoroutine(int levelId, int waveId)
        {
            float savedSpawnPointX = 0;

            WaveStaticData waveStaticData = _staticDataService.ForWave(levelId, waveId);

            for (int waveIndex = 0; waveIndex < waveStaticData.MicroWavesInfo.Waves.Count; waveIndex++)
            {
                EnemyWaveInfo waveInfo = waveStaticData.MicroWavesInfo.Waves[waveIndex];

                for (int i = 0; i < waveInfo.Amount; i++)
                {
                    bool isRight = Random.Range(0, 2) == 0;
                    int signDirection = isRight ? 1 : -1;
                    float spawnPointX = 20 * signDirection;

                    _waveNotificatorUI.Notify(signDirection);

                    Transform lastFlagTransform = _flagTrackerService.GetLastFlag(isRight);
                    float referencePositionX = isRight
                        ? Mathf.Max(_playerTransform.position.x, lastFlagTransform.position.x)
                        : Mathf.Min(_playerTransform.position.x, lastFlagTransform.position.x);

                    spawnPointX += referencePositionX;

                    savedSpawnPointX = isRight
                        ? Mathf.Max(savedSpawnPointX, spawnPointX)
                        : Mathf.Min(savedSpawnPointX, spawnPointX);

                    _gameFactory.CreateEnemy(waveInfo.EnemyTypeId, savedSpawnPointX);

                    yield return new WaitForSeconds(waveInfo.TimeBetweenSpawnEnemy);
                }

                if (waveIndex < waveStaticData.MicroWavesInfo.Waves.Count - 1)
                    yield return new WaitForSeconds(waveStaticData.TimeBetweenMicroWaves);
            }


            _spawnCoroutine = null;
        }
    }
}