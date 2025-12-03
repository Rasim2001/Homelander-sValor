using Infastructure.Services.Pool;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits.ItemShooter
{
    public class MarksmanShooter : ItemShooterBase
    {
        private PoolObjects<ArrowDamage> _arrowPoolObjects;
        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(PoolObjects<ArrowDamage> arrowPoolObjects, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _arrowPoolObjects = arrowPoolObjects;
        }

        protected override void CreateBullet(float horizontalVelocity, float verticalVelocity, float gravity)
        {
            UnitStaticData unitStaticData = _staticDataService.ForUnit(UnitTypeId.Marksman);

            ArrowDamage arrowDamage = _arrowPoolObjects.GetObjectFromPool();
            arrowDamage.transform.SetParent(null);
            arrowDamage.transform.position = ShootPoint.position;
            arrowDamage.Damage = unitStaticData.Damage;

            Rigidbody2D arrowRigidbody = arrowDamage.GetComponent<Rigidbody2D>();
            arrowRigidbody.gravityScale = gravity / 9.81f;
            arrowRigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }
    }
}