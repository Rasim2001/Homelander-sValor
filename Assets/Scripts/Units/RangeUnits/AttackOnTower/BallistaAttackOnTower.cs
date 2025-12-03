using System;
using System.Collections;
using BuildProcessManagement.Towers;
using BuildProcessManagement.Towers.BallistaBow;
using Units.UnitStates;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits.AttackOnTower
{
    public class BallistaAttackOnTower : UnitAttackOnTowerBase
    {
        [SerializeField] private Transform _bulletContainer;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Spear _spear;
        [SerializeField] private float _cooldown;

        private IBallistaBowAnimator _ballistaBowAnimator;
        private IBallistaRotation _ballistaRotation;

        private Coroutine _waitAimCoroutine;
        private DiContainer _diContainer;
        private bool _canShoot;
        private float _defaultCooldown;


        [Inject]
        public void Construct(DiContainer diContainer) => //TODO:
            _diContainer = diContainer;

        public void Initialize(IBallistaBowAnimator ballistaBowAnimator, IBallistaRotation ballistaRotation)
        {
            _ballistaBowAnimator = ballistaBowAnimator;
            _ballistaRotation = ballistaRotation;

            _ballistaRotation.OnTargetDeath += UpdateTarget;
        }

        protected override void OnDestroy()
        {
            _ballistaRotation.OnTargetDeath -= UpdateTarget;

            base.OnDestroy();
        }


        private void Start()
        {
            _canShoot = true;

            OnSpearBulletCreated();

            _defaultCooldown = _cooldown;
            _cooldown = 0;
        }


        private void Update()
        {
            UpdateCooldown();

            if (CooldownFinished() && _canShoot && _target != null)
                TryShoot();
        }

        public void OnReloadStarted() //calling from unity
        {
            _ballistaBowAnimator.Reload();
            _ballistaRotation.UnlockTarget();

            _animator.SetBool(UnitStatesPath.ShootHash, false);
        }

        public void OnSpearBulletCreated() //calling from unity
        {
            _spear = _diContainer
                .InstantiatePrefabForComponent<Spear>(_bulletPrefab, _bulletContainer.position,
                    _bulletContainer.rotation, _bulletContainer);
        }

        public void OnReloadEnded() //calling from unity
            => _canShoot = true;


        protected override void Enter()
        {
            _target = _unitObserverTrigger.GetNearestHit();

            if (_target == null)
                _ballistaRotation.SetTarget(null);
        }

        protected override void Shoot()
        {
            _ballistaBowAnimator.Shoot();
            _spear.Shoot();

            _cooldown = _defaultCooldown;
        }

        private void UpdateTarget()
        {
            _canShoot = true;

            Enter();
        }

        private void TryShoot()
        {
            if (!_canShoot)
                return;

            _canShoot = false;

            if (_target != null)
                TryShoot();
            else
                _ballistaRotation.SetTarget(null);

            StopWaitCoroutine();
            _waitAimCoroutine = StartCoroutine(WaitCoroutine());
        }

        private void StopWaitCoroutine()
        {
            if (_waitAimCoroutine != null)
            {
                StopCoroutine(_waitAimCoroutine);
                _waitAimCoroutine = null;
            }
        }


        private IEnumerator WaitCoroutine()
        {
            _ballistaRotation.SetTarget(_target);

            while (!_ballistaRotation.HasReachedTargetRotation())
                yield return null;

            _ballistaRotation.SetTargetLock();

            yield return new WaitForSeconds(0.1f);

            _animator.SetBool(UnitStatesPath.ShootHash, true);

            Shoot();
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