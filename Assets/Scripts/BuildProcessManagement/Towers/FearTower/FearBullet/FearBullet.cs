using System;
using System.Collections;
using Enemy.Effects;
using Enemy.Effects.Fear;
using Enemy.Effects.Stun;
using Infastructure.Services.Pool;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.Towers
{
    public class FearBullet : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private readonly Vector3 _deltaTarget = new Vector3(0, 0.5f, 0);
        private Transform _target;
        private PoolObjects<FearBullet> _fearPoolObjects;
        private Coroutine _coroutine;

        [Inject]
        public void Construct(PoolObjects<FearBullet> fearPoolObjects) =>
            _fearPoolObjects = fearPoolObjects;

        private void OnEnable() =>
            _coroutine = StartCoroutine(StartDestroyBullet());

        private void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator StartDestroyBullet()
        {
            yield return new WaitForSeconds(5);

            _target = null;
            _fearPoolObjects.ReturnObjectToPool(this);
        }


        public void SetTarget(Transform target) =>
            _target = target;

        public void Update()
        {
            if (_target != null)
            {
                if (Vector3.Distance(transform.position, _target.position + _deltaTarget) < 0.01f)
                {
                    EnemyEffectSystem enemyEffectSystem = _target.GetComponentInParent<EnemyEffectSystem>();
                    enemyEffectSystem.AddEffect<FearEffect>();

                    _target = null;
                    _fearPoolObjects.ReturnObjectToPool(this);
                }
                else
                    FollowTo(_target);
            }
        }

        private void FollowTo(Transform target) =>
            transform.position =
                Vector3.Lerp(transform.position, target.position + _deltaTarget, _speed * Time.deltaTime);
    }
}