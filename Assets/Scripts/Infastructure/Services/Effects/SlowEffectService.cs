using System.Collections;
using Enemy;
using UnityEngine;

namespace Infastructure.Services.Effects
{
    public class SlowEffectService : ISlowEffectService
    {
        private const string EnemyLayer = "EnemyDamage";

        private readonly ICoroutineRunner _coroutineRunner;

        private readonly Collider2D[] _results = new Collider2D[10];

        public SlowEffectService(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        public void CastEffect(Vector2 position)
        {
            LayerMask enemyLayerMask = 1 << LayerMask.NameToLayer(EnemyLayer);

            int count = Physics2D.OverlapCircleNonAlloc(position, 5, _results, enemyLayerMask);

            _coroutineRunner.StartCoroutine(CastEffectCoroutine(count));
        }

        private IEnumerator CastEffectCoroutine(int count)
        {
            Collider2D[] currentResults = (Collider2D[])_results.Clone();

            for (int i = 0; i < count; i++)
            {
                EnemyMove enemyMove = currentResults[i].GetComponentInParent<EnemyMove>();
                enemyMove.Speed = 1.5f;

                Animator animator = enemyMove.GetComponent<Animator>();
                animator.speed = 0.5f;
            }

            yield return new WaitForSeconds(3f);

            for (int i = 0; i < count; i++)
            {
                EnemyMove enemyMove = currentResults[i].GetComponentInParent<EnemyMove>();
                enemyMove.Speed = 3;

                Animator animator = enemyMove.GetComponent<Animator>();
                animator.speed = 1;
            }
        }
    }
}