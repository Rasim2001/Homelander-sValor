using BuildProcessManagement.Towers.CatapultTower;
using UnityEngine;

namespace Units.RangeUnits.AttackOnTower
{
    public class CatapultmanAttackOnTower : UnitAttackOnTowerBase
    {
        [SerializeField] private float _cooldown;
        [SerializeField] private bool _canShoot;

        private static readonly int ShootHash = Animator.StringToHash("Shoot");

        private IBowl _bowl;

        private float _defaultCooldown;

        public void Initialize(IBowl bowl) =>
            _bowl = bowl;

        private void Start()
        {
            _canShoot = true;

            _defaultCooldown = _cooldown;
            _cooldown = 0;
        }

        private void Update()
        {
            UpdateCooldown();

            if (CooldownFinished() && _canShoot && _target != null)
                Shoot();
        }

        public void OnReloadEnded()
            => _canShoot = true;


        public void OnReloadStarted() =>
            _animator.SetBool(ShootHash, false);


        protected override void Enter() =>
            _target = _unitObserverTrigger.GetNearestHit();


        protected override void Shoot()
        {
            if (!_canShoot)
                return;

            _animator.SetBool(ShootHash, true);
            _bowl.SetTarget(_target.transform);
            _bowl.PlayShootAnimation();

            _canShoot = false;
            _cooldown = _defaultCooldown;
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