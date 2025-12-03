using BuildProcessManagement.Towers.CatapultTower;
using Infastructure.Services.Pool;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits.ItemShooter
{
    public class CatapultBulletShooter : ItemShooterBase
    {
        private PoolObjects<CatapultBullet> _catapultPoolObjects;
        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(PoolObjects<CatapultBullet> catapultPoolObjects, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _catapultPoolObjects = catapultPoolObjects;
        }

        protected override void CreateBullet(float horizontalVelocity, float verticalVelocity, float gravity)
        {
            UnitStaticData unitStaticData = _staticDataService.ForUnit(UnitTypeId.Catapultman);

            CatapultBullet catapultBullet = _catapultPoolObjects.GetObjectFromPool();
            catapultBullet.transform.SetParent(null);
            catapultBullet.transform.position = ShootPoint.position;
            catapultBullet.Damage = unitStaticData.Damage;

            Rigidbody2D arrowRigidbody = catapultBullet.GetComponent<Rigidbody2D>();
            arrowRigidbody.gravityScale = gravity / 9.81f;
            arrowRigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }
    }
}