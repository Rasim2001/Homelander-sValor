using Infastructure.Services.Pool;
using Infastructure.StaticData.Player;
using Infastructure.StaticData.StaticDataService;
using Units.RangeUnits;
using UnityEngine;
using Zenject;

namespace Player.Shoot
{
    public class PlayerShooter : ItemShooterBase
    {
        private IPoolObjects<PlayerArrowDamage> _arrowPoolObjects;
        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(IPoolObjects<PlayerArrowDamage> arrowPoolObjects, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _arrowPoolObjects = arrowPoolObjects;
        }

        protected override void CreateBullet(float horizontalVelocity, float verticalVelocity, float gravity)
        {
            PlayerStaticData playerStaticData = _staticDataService.PlayerStaticData;

            PlayerArrowDamage arrowDamage = _arrowPoolObjects.GetObjectFromPool();
            arrowDamage.transform.SetParent(null);
            arrowDamage.transform.position = ShootPoint.position;
            arrowDamage.Damage = playerStaticData.ShootDamage;

            Rigidbody2D arrowRigidbody = arrowDamage.Rigidbody;
            arrowRigidbody.gravityScale = gravity / 9.81f;
            arrowRigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }
    }
}