using BuildProcessManagement.Towers.BallistaTar;
using Units.UnitStates;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits.AttackOnTower
{
    public class TarmanAttackOnTowerFirst : UnitAttackOnTowerBase
    {
        [SerializeField] private GameObject _bulletPrefab;

        private bool _canShoot;

        private DiContainer _diContainer;
        private JugBulletFirst _jugBulletfirst;

        [Inject]
        public void Construct(DiContainer diContainer) => //TODO:
            _diContainer = diContainer;

        private void Start() =>
            _canShoot = true;

        public void OnRealodEnded() //calling from unity
        {
            _canShoot = true;

            Enter();
        }

        public void OnReloadStarted() //calling from unity
        {
        }

        public void OnJugBulletCreated() //calling from unity
        {
            _jugBulletfirst = _diContainer.InstantiatePrefabForComponent<JugBulletFirst>(_bulletPrefab, transform);
            _jugBulletfirst.transform.localPosition = new Vector3(-0.017f, -0.775f);

            Shoot();
        }


        protected override void Enter()
        {
            if (!_canShoot)
                return;

            _target = _unitObserverTrigger.GetNearestHit();
            _animator.SetBool(UnitStatesPath.ShootHash, _target != null);
        }


        protected override void Shoot()
        {
            _jugBulletfirst.Shoot();
            _canShoot = false;
        }
    }
}