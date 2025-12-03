using System;
using Cysharp.Threading.Tasks;
using Infastructure.Services.Pool;
using Loots;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Infastructure.Services.CoinsCreator
{
    public class CoinsCreatorService : ICoinsCreatorService
    {
        private readonly IPoolObjects<CoinLoot> _pool;

        public CoinsCreatorService(IPoolObjects<CoinLoot> pool) =>
            _pool = pool;


        public async UniTask CreateCoinsAsync(Vector3 position, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-3f, 3f), Random.Range(4, 8));
                CreateCoins(position, randomDirection);

                await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
            }
        }

        private void CreateCoins(Vector3 position, Vector2 randomDirection)
        {
            CoinLoot coinLoot = _pool.GetObjectFromPool();
            coinLoot.transform.position = position;

            Rigidbody2D rb = coinLoot.GetComponent<Rigidbody2D>();

            rb.velocity = randomDirection;
        }
    }
}