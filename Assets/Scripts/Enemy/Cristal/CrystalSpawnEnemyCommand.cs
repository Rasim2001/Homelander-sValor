using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Pool;
using Infastructure.StaticData.EnemyCristal;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.Cristal
{
    public class CrystalSpawnEnemyCommand : ICommand
    {
        private readonly IGameFactory _gameFactory;
        private readonly IPoolObjects<EnemySpawnPoint> _spawnPointPool;
        private readonly Health _health;
        private readonly EnemyCristalConfig _enemyCristalConfig;
        private readonly Levitate _levitate;
        private readonly Transform _cristalTransform;

        private readonly HashSet<int> _usedGroupOffsets = new();


        public CrystalSpawnEnemyCommand(
            IGameFactory gameFactory,
            IPoolObjects<EnemySpawnPoint> spawnPointPool,
            Health health,
            EnemyCristalConfig enemyCristalConfig,
            Levitate levitate,
            Transform cristalTransform)
        {
            _gameFactory = gameFactory;
            _spawnPointPool = spawnPointPool;
            _health = health;
            _enemyCristalConfig = enemyCristalConfig;
            _levitate = levitate;
            _cristalTransform = cristalTransform;
        }

        public async UniTask Execute(CancellationToken cancellationToken)
        {
            _usedGroupOffsets.Clear();

            CancellationTokenSource updateCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            UpdateCustomAsync(updateCts.Token).Forget();

            List<EnemyCristalData> enemyCristalDatas = _enemyCristalConfig.Configs;
            float currentHpInPercentage = (float)_health.CurrentHP / _health.MaxHp;

            List<EnemyCristalData> sortedEnemyCristalDatas =
                enemyCristalDatas.OrderByDescending(x => x.PercentHealth).ToList();
            EnemyCristalData enemyCristalData =
                sortedEnemyCristalDatas.FindLast(x => x.PercentHealth >= currentHpInPercentage);

            await SpawnEnemiesAsync(enemyCristalData.EnemySpawnInfos, cancellationToken);

            updateCts.Cancel();
            updateCts.Dispose();
        }

        private async UniTask UpdateCustomAsync(CancellationToken cancellationToken)
        {
            _levitate.Reset();
            Vector2 position = _cristalTransform.position;

            while (!cancellationToken.IsCancellationRequested)
            {
                _levitate.UpdateCustom(position);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cancellationToken);
            }
        }

        private async UniTask SpawnEnemiesAsync(List<EnemyCristalSpawnInfo> enemySpawnInfos,
            CancellationToken cancellationToken)
        {
            List<UniTask> tasks = new List<UniTask>();

            foreach (EnemyCristalSpawnInfo enemySpawnInfo in enemySpawnInfos)
            {
                UniTask task = SpawnEnemiesInGroupAsync(cancellationToken, enemySpawnInfo);
                tasks.Add(task);
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask SpawnEnemiesInGroupAsync(CancellationToken cancellationToken,
            EnemyCristalSpawnInfo enemySpawnInfo)
        {
            float randomPositionX = GetRandomPosition();
            EnemySpawnPoint enemySpawnPoint = _spawnPointPool.GetObjectFromPool();
            enemySpawnPoint.transform.position = new Vector3(randomPositionX, -2.75f);

            try
            {
                await enemySpawnPoint.Show(cancellationToken);

                for (int i = 0; i < enemySpawnInfo.Amount; i++)
                {
                    _gameFactory.CreateEnemy(enemySpawnInfo.EnemyTypeId, randomPositionX);
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
                }

                await enemySpawnPoint.Hide(cancellationToken);
            }
            finally
            {
                if (enemySpawnPoint != null)
                    _spawnPointPool.ReturnObjectToPool(enemySpawnPoint);
            }
        }

        private float GetRandomPosition()
        {
            float cristalPositionX = _cristalTransform.position.x;

            int randomDeltaX;
            int attempts = 10;

            do
            {
                randomDeltaX = Random.Range(-6, 7);
                attempts--;
            } while (_usedGroupOffsets.Contains(randomDeltaX) && attempts > 0);

            _usedGroupOffsets.Add(randomDeltaX);

            return cristalPositionX + randomDeltaX;
        }
    }
}