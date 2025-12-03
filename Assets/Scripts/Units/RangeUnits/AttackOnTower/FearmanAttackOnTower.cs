using System.Collections.Generic;
using BuildProcessManagement.Towers;
using BuildProcessManagement.Towers.FearTower.Samovar;
using DG.Tweening;
using Infastructure.Services.Pool;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Units.RangeUnits.AttackOnTower
{
    public class FearmanAttackOnTower : UnitAttackOnTowerBase
    {
        private static readonly int ShootHash = Animator.StringToHash("Shoot");

        [SerializeField] private FearBullet _fearBulletPrefab;
        [SerializeField] private float _cooldown;

        private PoolObjects<FearBullet> _fearPoolObjects;
        private ISamovar _samovar;

        private bool _canShoot;

        private float _defaultCooldown;
        private bool _canUpdate;

        [Inject]
        public void Construct(PoolObjects<FearBullet> fearPoolObjects) =>
            _fearPoolObjects = fearPoolObjects;


        public void Initialize(ISamovar samovar) =>
            _samovar = samovar;

        private void Start()
        {
            _canShoot = true;

            _defaultCooldown = _cooldown;
            _cooldown = 0;
        }


        private void Update()
        {
            UpdateCooldown();

            if (!_canUpdate)
                return;

            if (CooldownFinished() && _canShoot)
                Enter();
        }


        protected override void Enter()
        {
            _canUpdate = true;

            if (!CooldownFinished())
                return;

            Shoot();
        }

        protected override void Exit()
        {
            _target = _unitObserverTrigger.GetNearestHit();

            if (_target == null)
                _canUpdate = false;
        }


        protected override void Shoot()
        {
            List<Collider2D> enemyColliders = _unitObserverTrigger.GetAllHits();

            if (enemyColliders == null || enemyColliders.Count == 0)
                return;

            _animator.SetBool(ShootHash, true);
            _samovar.PlayShootAnimation();

            _canShoot = false;
            _cooldown = _defaultCooldown;
        }


        public void OnReloadEnded() //calling from unity
        {
            _canShoot = true;
            _samovar.SetCharge();
        }

        public void OnReloadStarted() //calling from unity
            => _animator.SetBool(ShootHash, false);

        public void OnStartFearAttacked() //calling from unity
        {
            List<Collider2D> enemyColliders = _unitObserverTrigger.GetAllHits();

            for (int i = 0; i < enemyColliders.Count; i++)
            {
                FearBullet fearBullet = _fearPoolObjects.GetObjectFromPool();
                fearBullet.transform.SetParent(null);
                fearBullet.transform.position = _samovar.SpawnPoint.position;

                Transform targetEnemy = enemyColliders[i].transform;

                float randomX = Random.Range(-2f, 2f);
                float randomY = Random.Range(1f, 2f);

                float randonY2 = Random.Range(1.5f, 3);

                Vector3 randomOffset1 =
                    _samovar.SpawnPoint.position + new Vector3(randomX, randomY, 0f);
                Vector3 randomOffset2 =
                    _samovar.SpawnPoint.position + new Vector3(randomX, randonY2, 0f);

                fearBullet.transform
                    .DOPath(
                        new[] { randomOffset1, randomOffset2 },
                        1,
                        PathType.CatmullRom
                    )
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => fearBullet.SetTarget(targetEnemy));
            }
        }

        private void UpdateCooldown()
        {
            if (!CooldownFinished())
                _cooldown -= Time.deltaTime;
        }

        private bool CooldownFinished() =>
            _cooldown <= 0;
    }
}