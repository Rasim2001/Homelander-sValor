using System.Collections;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.Pool;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.WaveOfEnemies;
using Loots;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Bag
{
    public class BagOfMoney : MonoBehaviour
    {
        private static readonly int OpenTriggerHash = Animator.StringToHash("OpenTrigger");

        private IPoolObjects<CoinLoot> _poolObjects;
        private IPoolObjects<BagOfMoney> _bagOfMoneyPool;
        private bool _isTriggered;

        private ObserverTrigger _observerTrigger;
        private Animator _animator;
        private Coroutine _destroyCoroutine;

        private IStaticDataService _staticData;
        private IEnemySpawnService _enemySpawnService;


        [Inject]
        public void Construct(
            IPoolObjects<CoinLoot> poolObjects,
            IPoolObjects<BagOfMoney> bagOfMoneyPool,
            IStaticDataService staticData, 
            IEnemySpawnService enemySpawnService)
        {
            _enemySpawnService = enemySpawnService;
            _staticData = staticData;
            _bagOfMoneyPool = bagOfMoneyPool;
            _poolObjects = poolObjects;
        }

        private void Awake()
        {
            _observerTrigger = GetComponent<ObserverTrigger>();
            _animator = GetComponent<Animator>();
        }

        private void Start() =>
            _observerTrigger.OnTriggerEnter += TriggerEnter;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= TriggerEnter;

        private void OnDisable() =>
            _isTriggered = false;

        private void TriggerEnter()
        {
            if (_isTriggered)
                return;

            _isTriggered = true;

            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }

            _destroyCoroutine = StartCoroutine(StartDestroyCoroutine());
        }

        private IEnumerator StartDestroyCoroutine()
        {
            int levelWaveId = _staticData.CheatStaticData.LevelWaveId;

            WaveStaticData waveStaticData = _staticData.ForWave(levelWaveId, _enemySpawnService.WavePassedWaveId);
            int passedWaveCoins = waveStaticData.PassedWaveCoins;

            _animator.SetTrigger(OpenTriggerHash);

            for (int i = 0; i < passedWaveCoins; i++)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-3f, 3f), Random.Range(4, 8));
                CreateCoins(transform.position, randomDirection);

                yield return new WaitForSeconds(0.25f);
            }


            yield return new WaitForSeconds(3f);

            _bagOfMoneyPool.ReturnObjectToPool(this);
        }

        private void CreateCoins(Vector3 position, Vector2 randomDirection)
        {
            CoinLoot coinLoot = _poolObjects.GetObjectFromPool();
            coinLoot.transform.position = position;

            Rigidbody2D rb = coinLoot.GetComponent<Rigidbody2D>();

            rb.velocity = randomDirection;
        }
    }
}