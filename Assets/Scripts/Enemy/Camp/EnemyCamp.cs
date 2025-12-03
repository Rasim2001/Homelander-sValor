using System.Collections;
using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.WaveOfEnemies;
using UnityEngine;
using Zenject;

namespace Enemy.Camp
{
    public class EnemyCamp : MonoBehaviour
    {
        [SerializeField] private EnemyObserverTrigger _observerTrigger;

        private IGameFactory _gameFactory;

        private MicroWavesInfo _microWavesInfo;
        private Coroutine _spawnEnemiesCoroutine;
        private int _savedWaveIndex;


        [Inject]
        public void Consturct(IGameFactory gameFactory) =>
            _gameFactory = gameFactory;

        private void Start() =>
            _observerTrigger.OnTriggerEnter += TriggerEnter;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= TriggerEnter;

        public void Initialize(MicroWavesInfo microWavesInfo) =>
            _microWavesInfo = microWavesInfo;

        private void TriggerEnter()
        {
            if (_spawnEnemiesCoroutine != null || _savedWaveIndex >= _microWavesInfo.Waves.Count)
                return;

            _spawnEnemiesCoroutine = StartCoroutine(SpawnEnemiesCoroutine());
        }


        private IEnumerator SpawnEnemiesCoroutine()
        {
            for (int i = 0; i < _microWavesInfo.Waves[_savedWaveIndex].Amount; i++)
            {
                EnemyWaveInfo enemyWaveInfo = _microWavesInfo.Waves[_savedWaveIndex];

                SpawnEnemy(enemyWaveInfo);
                yield return new WaitForSeconds(enemyWaveInfo.TimeBetweenSpawnEnemy);
            }

            _savedWaveIndex++;

            if (_spawnEnemiesCoroutine != null)
            {
                StopCoroutine(_spawnEnemiesCoroutine);
                _spawnEnemiesCoroutine = null;
            }

            if (_observerTrigger.HasAnyColliders())
                TriggerEnter();
        }

        private void SpawnEnemy(EnemyWaveInfo enemyWaveInfo) =>
            _gameFactory.CreateEnemy(enemyWaveInfo.EnemyTypeId, transform.position.x);
    }
}