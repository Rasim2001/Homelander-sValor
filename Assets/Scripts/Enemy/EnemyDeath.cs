using Bag;
using DG.Tweening;
using Infastructure.Data;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.Pool;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.StaticData.StaticDataService;
using Player;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyDeath : MonoBehaviour
    {
        private static readonly int FullDistortionFade = Shader.PropertyToID("_FullDistortionFade");

        [SerializeField] private EnemyInfo _enemyInfo;
        [SerializeField] private EnemyAgressionZone _agressionZone;
        [SerializeField] private EnemyMove _enemyMove;
        [SerializeField] private EnemyAttack _enemyAttack;
        [SerializeField] private EnemyAnimator _animator;
        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _healthCollider;

        private IWaveEnemiesCountService _waveEnemiesCountService;
        private IGameFactory _gameFactory;
        private IPoolObjects<BagOfMoney> _bagOfMoneyPool;
        private IEnemySpawnService _enemySpawnService;
        private IStaticDataService _staticDataService;

        private Material _deathMaterialInstance;
        private ISafeBuildZone _safeBuildZone;


        [Inject]
        public void Construct(
            IWaveEnemiesCountService waveEnemiesCountService, IGameFactory gameFactory,
            IEnemySpawnService enemySpawnService,
            IPoolObjects<BagOfMoney> bagOfMoneyPool,
            IStaticDataService staticDataService,
            ISafeBuildZone safeBuildZone)
        {
            _safeBuildZone = safeBuildZone;
            _staticDataService = staticDataService;
            _enemySpawnService = enemySpawnService;
            _bagOfMoneyPool = bagOfMoneyPool;
            _waveEnemiesCountService = waveEnemiesCountService;
            _gameFactory = gameFactory;
        }

        private void Awake()
        {
            Material deathMaterial = _staticDataService.DefaultMaterialStaticData.EnemyDeathMaterial;
            _deathMaterialInstance = new Material(deathMaterial);
        }

        private void Start()
        {
            _health.OnDeathHappened += SpawnMatryoshka;
            _health.OnDeathHappened += Death;
        }

        private void OnDestroy()
        {
            _health.OnDeathHappened -= SpawnMatryoshka;
            _health.OnDeathHappened -= Death;
        }

        private void SpawnMatryoshka() =>
            _gameFactory.CreateEnemyMatryoshka(_enemyInfo.EnemyTypeId, _enemyInfo.MatryoshkaId, transform.position);

        private void Death()
        {
            _enemyMove.enabled = false;
            _enemyAttack.enabled = false;
            _agressionZone.enabled = false;

            _waveEnemiesCountService.NumberOfEnemiesOnWave--;
            _waveEnemiesCountService.Enemies.Remove(gameObject);


            StartDeath();
        }


        private void StartDeath()
        {
            _animator.PlayDeathAnimation();

            float animationLenght = _animator.GetDeathAnimationClipLength();
            float randomDeltaDeath = Random.Range(0.7f, 1);

            float randomDeathTime = animationLenght * randomDeltaDeath;
            float currentSpeed = _animator.GetAnimatorSpeed();

            Tween animTween = DOTween.To(
                () => currentSpeed,
                x =>
                {
                    currentSpeed = x;
                    _animator.SetAnimatorSpeed(x);
                },
                0,
                randomDeathTime
            ).SetEase(Ease.Linear);

            Tween colorTween = DOTween.To(
                () => _spriteRenderer.color,
                x => _spriteRenderer.color = x,
                Color.black,
                randomDeathTime
            ).SetEase(Ease.Linear);

            DOTween.Sequence()
                .Append(animTween)
                .Join(colorTween)
                .OnComplete(DeathAfterAnimation);
        }


        public void DeathAfterAnimation()
        {
            _spriteRenderer.material = _deathMaterialInstance;
            float time = 1;
            float distortionFadeValue = 1;

            DOTween.To(
                    () => distortionFadeValue,
                    x =>
                    {
                        distortionFadeValue = x;
                        _deathMaterialInstance.SetFloat(FullDistortionFade, distortionFadeValue);
                    },
                    0,
                    time
                )
                .SetEase(Ease.Linear)
                .OnComplete(DeathAfterEffects);
        }

        private void DeathAfterEffects()
        {
            if (CanSpawnBagOfMoney())
                SpawnBagOfMoney();

            Destroy(gameObject);
        }

        private void SpawnBagOfMoney() =>
            _bagOfMoneyPool.GetObjectFromPool().WithSetPosition(transform.position);

        private bool CanSpawnBagOfMoney()
        {
            return _enemySpawnService.EnemyWaveFinished() &&
                   _waveEnemiesCountService.NumberOfEnemiesOnWave == 0 &&
                   _safeBuildZone.IsNight;
        }
    }
}