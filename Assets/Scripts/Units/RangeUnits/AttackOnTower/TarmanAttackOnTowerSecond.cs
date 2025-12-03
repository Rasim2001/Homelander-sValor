using System.Collections;
using BuildProcessManagement.Towers;
using BuildProcessManagement.Towers.BallistaTar;
using Units.UnitStates;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits.AttackOnTower
{
    public class TarmanAttackOnTowerSecond : UnitAttackOnTowerBase
    {
        [SerializeField] private GameObject _bulletPrefab;

        private DiContainer _diContainer;

        private IBallistaRotation _ballistaRotation;
        private ICarriage _carriage;
        private ITarStonesAnimator _tarStonesAnimator;

        private bool _canShoot;

        private Coroutine _waitAimCoroutime;
        private JugBulletSecond _jugBulletSecond;

        [Inject]
        public void Construct(DiContainer diContainer) => //TODO:
            _diContainer = diContainer;

        public void Initialize(
            IBallistaRotation ballistaRotation,
            ICarriage carriage,
            ITarStonesAnimator tarStonesAnimator)
        {
            _ballistaRotation = ballistaRotation;
            _carriage = carriage;
            _tarStonesAnimator = tarStonesAnimator;
        }

        private void Start()
        {
            _canShoot = true;

            OnJugBulletCreated();
        }

        public void OnRealodEnded() //calling from unity
        {
            _canShoot = true;

            Enter();
        }

        public void OnReloadStarted() //calling from unity
        {
            _carriage.Reload();
            _tarStonesAnimator.PlayStonesUp();
            _ballistaRotation.UnlockTarget();
        }

        public void OnJugBulletCreated() //calling from unity
        {
            _jugBulletSecond = _diContainer
                .InstantiatePrefabForComponent<JugBulletSecond>(_bulletPrefab, _carriage.CarriageTransform.position,
                    _carriage.CarriageTransform.rotation, _carriage.CarriageTransform);
        }


        protected override void Enter()
        {
            if (!_canShoot)
                return;

            _target = _unitObserverTrigger.GetNearestHit();
            _ballistaRotation.SetTarget(_target);
            _animator.SetBool(UnitStatesPath.ShootHash, false);


            if (_target != null)
            {
                StopWaitCoroutine();

                _waitAimCoroutime = StartCoroutine(WaitCoroutine());
            }
            else
            {
                StopWaitCoroutine();
            }
        }

        private void StopWaitCoroutine()
        {
            if (_waitAimCoroutime != null)
            {
                StopCoroutine(_waitAimCoroutime);
                _waitAimCoroutime = null;
            }
        }


        protected override void Shoot()
        {
            _carriage.Shoot();
            _tarStonesAnimator.PlayStonesDown();
            _jugBulletSecond.Shoot();

            _canShoot = false;
        }

        private IEnumerator WaitCoroutine()
        {
            while (!_ballistaRotation.HasReachedTargetRotation())
                yield return null;

            _ballistaRotation.SetTargetLock();

            yield return new WaitForSeconds(0.1f);

            _animator.SetBool(UnitStatesPath.ShootHash, true);
            Shoot();
        }
    }
}