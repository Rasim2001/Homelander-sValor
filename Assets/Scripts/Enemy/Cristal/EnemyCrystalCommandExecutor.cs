using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Pool;
using Infastructure.StaticData.EnemyCristal;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Enemy.Cristal
{
    public class EnemyCrystalCommandExecutor
    {
        private readonly IGameFactory _gameFactory;
        private readonly IPoolObjects<EnemySpawnPoint> _spawnPointPool;
        private readonly EnemyObserverTrigger _enemyObserverTrigger;
        private readonly EnemyObserverTrigger _aggressionObserverTrigger;
        private readonly Health _health;
        private readonly Levitate _levitate;
        private readonly SpriteRenderer _cristalSpriteRenderer;
        private readonly Light2D _light2D;
        private readonly Transform _cristalTransform;
        private readonly Transform _cristalSpriteTransform;

        private List<ICommand> _cristalCommands;
        private List<IPayloadCommand> _payloadCrystalCommands;

        private bool _hasEnemies;
        private bool _isExecuted;

        private CancellationTokenSource _cts;

        public EnemyCrystalCommandExecutor(
            IGameFactory gameFactory,
            IPoolObjects<EnemySpawnPoint> spawnPointPool,
            EnemyObserverTrigger enemyObserverTrigger,
            EnemyObserverTrigger aggressionObserverTrigger,
            Health health,
            Levitate levitate,
            SpriteRenderer cristalSpriteRenderer,
            Light2D light2D,
            Transform cristalSpriteTransform)
        {
            _gameFactory = gameFactory;
            _spawnPointPool = spawnPointPool;
            _enemyObserverTrigger = enemyObserverTrigger;
            _aggressionObserverTrigger = aggressionObserverTrigger;
            _health = health;
            _levitate = levitate;
            _cristalSpriteRenderer = cristalSpriteRenderer;
            _light2D = light2D;

            _cristalSpriteTransform = cristalSpriteTransform;
        }

        public void Initialize(EnemyCristalConfig cristalConfig)
        {
            CristalCommandInitialize(cristalConfig);
            DefaultInitialize();
            SubscribeUpdates();
        }

        public void Dispose()
        {
            UnSubscribeUpdates();
            ClearAllCommands();
        }


        private void SubscribeUpdates()
        {
            _enemyObserverTrigger.OnTriggerEnter += TriggerEnter;
            _enemyObserverTrigger.OnTriggerExit += TriggerExit;
        }

        private void UnSubscribeUpdates()
        {
            _enemyObserverTrigger.OnTriggerEnter -= TriggerEnter;
            _enemyObserverTrigger.OnTriggerExit -= TriggerExit;
        }

        private void DefaultInitialize()
        {
            _payloadCrystalCommands.ForEach(x => x.Initialize());

            _cts = new CancellationTokenSource();
            _isExecuted = true;
        }

        private void ClearAllCommands()
        {
            _payloadCrystalCommands.ForEach(x => x.Clear());

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        private void CancelAllCommands()
        {
            ClearAllCommands();
            DefaultInitialize();
        }

        private void CristalCommandInitialize(EnemyCristalConfig cristalConfig)
        {
            _cristalCommands = new List<ICommand>()
            {
                new CrystalLaunchCommand(_cristalSpriteTransform, _aggressionObserverTrigger, _cristalSpriteRenderer,
                    _light2D,
                    CancelAllCommands),
                new CrystalMoveCommand(_cristalSpriteTransform, 1),
                new CrystalSpawnEnemyCommand(_gameFactory, _spawnPointPool, _health, cristalConfig, _levitate,
                    _cristalSpriteTransform),
                new CrystalMoveCommand(_cristalSpriteTransform, -1),
                new CrystalFinishCommand(_cristalSpriteTransform, _cristalSpriteRenderer, _light2D,
                    _aggressionObserverTrigger)
            };

            _payloadCrystalCommands = _cristalCommands.OfType<IPayloadCommand>().ToList();
        }

        private void TriggerEnter()
        {
            _hasEnemies = true;

            if (_isExecuted)
                ExecuteAllCommands().Forget();
        }

        private void TriggerExit() =>
            _hasEnemies = false;


        private async UniTask ExecuteAllCommands()
        {
            _isExecuted = false;

            while (!_health.IsDeath && _hasEnemies)
            {
                foreach (ICommand command in _cristalCommands)
                    await command.Execute(_cts.Token);
            }

            _isExecuted = true;
        }
    }
}