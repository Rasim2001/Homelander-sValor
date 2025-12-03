using System.Collections;
using Enemy.Effects;
using Enemy.Effects.Freez;
using Infastructure;
using Infastructure.Services.Pool;
using UnityEngine;

namespace Player.Skills
{
    public class FreezSkill : ISkill
    {
        private const string EnemyLayer = "EnemyDamage";

        private readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[30];
        private readonly PlayerFlip _playerFlip;
        private readonly Transform _castStartPoint;
        private readonly float _maxDistance = 4;
        private readonly int _enemyLayerMask;

        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IPoolObjects<FreezParticleMarker> _poolObjects;
        private Coroutine _freezCoroutine;
        private FreezParticleMarker _freezParticleMarker;

        public FreezSkill(
            PlayerFlip playerFlip,
            Transform castStartPoint,
            ICoroutineRunner coroutineRunner,
            IPoolObjects<FreezParticleMarker> poolObjects)
        {
            _playerFlip = playerFlip;
            _castStartPoint = castStartPoint;
            _coroutineRunner = coroutineRunner;
            _poolObjects = poolObjects;

            _enemyLayerMask = 1 << LayerMask.NameToLayer(EnemyLayer);
        }

        public void CastSkill()
        {
            StopAllCoroutines();

            Vector2 rayDirection = new Vector2(-_playerFlip.FlipValue(), 0);
            Vector2 startPointPosition = _castStartPoint.position;

            _freezParticleMarker = _poolObjects.GetObjectFromPool();
            _freezParticleMarker.transform.position = _playerFlip.transform.position;
            _freezParticleMarker.transform.rotation = Quaternion.Euler(0, 0, _playerFlip.FlipBoolValue() ? 180 : 0);

            int hitCount =
                Physics2D.RaycastNonAlloc(startPointPosition, rayDirection, _raycastHits, _maxDistance,
                    _enemyLayerMask);

            _freezCoroutine = _coroutineRunner.StartCoroutine(StartFreezCoroutine(hitCount));
        }

        private void StopAllCoroutines()
        {
            if (_freezCoroutine == null)
                return;

            _coroutineRunner.StopCoroutine(_freezCoroutine);
            _freezCoroutine = null;

            _poolObjects.ReturnObjectToPool(_freezParticleMarker);
        }

        private IEnumerator StartFreezCoroutine(int hitCount)
        {
            yield return new WaitForSeconds(0.25f);

            if (hitCount != 0)
                Freez();

            yield return new WaitForSeconds(5f);

            _poolObjects.ReturnObjectToPool(_freezParticleMarker);
        }


        private void Freez()
        {
            foreach (RaycastHit2D raycastHit2D in _raycastHits)
            {
                if (raycastHit2D.collider == null)
                    continue;

                IEnemyEffectSystem effectSystem = raycastHit2D.collider.GetComponentInParent<IEnemyEffectSystem>();
                effectSystem?.AddEffect<FreezEffect>();
            }
        }
    }
}