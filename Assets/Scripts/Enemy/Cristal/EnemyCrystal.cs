using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Pool;
using Infastructure.StaticData.EnemyCristal;
using Infastructure.StaticData.StaticDataService;
using Player;
using Units;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Enemy.Cristal
{
    public class EnemyCrystal : MonoBehaviour
    {
        [SerializeField] private EnemyObserverTrigger _observerTrigger;
        [SerializeField] private EnemyObserverTrigger _aggressionTrigger;
        [SerializeField] private Health _health;
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private Levitate _levitate;
        [SerializeField] private Transform _spriteTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Light2D _light2D;

        private EnemyCrystalCommandExecutor _commandExecutor;
        private IGameFactory _gameFactory;
        private IStaticDataService _staticDataService;
        private IPoolObjects<EnemySpawnPoint> _spawnPointPool;

        [Inject]
        public void Construct(
            IGameFactory gameFactory,
            IStaticDataService staticDataService,
            IPoolObjects<EnemySpawnPoint> spawnPointPool)
        {
            _spawnPointPool = spawnPointPool;
            _staticDataService = staticDataService;
            _gameFactory = gameFactory;
        }

        private void Awake()
        {
            _commandExecutor =
                new EnemyCrystalCommandExecutor(
                    _gameFactory,
                    _spawnPointPool,
                    _observerTrigger,
                    _aggressionTrigger,
                    _health,
                    _levitate,
                    _spriteRenderer,
                    _light2D,
                    _spriteTransform);
        }

        private void Start()
        {
            List<EnemyCristalConfig> enemyCristalConfigs = _staticDataService.GameStaticData.EnemyCristalConfigs;
            EnemyCristalConfig cristalConfig = enemyCristalConfigs.FirstOrDefault(x => x.Id == _uniqueId.Id);

            _commandExecutor.Initialize(cristalConfig);
        }

        private void OnDestroy() =>
            _commandExecutor.Dispose();
    }
}