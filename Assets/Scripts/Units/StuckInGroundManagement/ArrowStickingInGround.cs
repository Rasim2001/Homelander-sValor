using System.Collections;
using DG.Tweening;
using Infastructure.Services.Pool;
using Units.RangeUnits;
using UnityEngine;
using Zenject;

namespace Units.StuckInGroundManagement
{
    public class ArrowStickingInGround : StuckInGround<ArrowDamage>
    {
        private IPoolObjects<ArrowDamage> _arrowPoolObjects;

        [Inject]
        public void Construct(IPoolObjects<ArrowDamage> arrowPoolObjects) =>
            _arrowPoolObjects = arrowPoolObjects;

        protected override void OnGroundHitLogic(ArrowDamage arrow) =>
            DestroyCoroutine = StartCoroutine(DestroyArrow(arrow));

        private IEnumerator DestroyArrow(ArrowDamage arrow)
        {
            arrow.IsHittingToGround = true;
            arrow.ArrowRotation.enabled = false;
            arrow.Rigidbody.velocity = Vector2.zero;
            arrow.Rigidbody.angularVelocity = 0f;
            arrow.Rigidbody.isKinematic = true;

            yield return new WaitForSeconds(2);

            ShakeTween.Kill();

            arrow.ArrowRotation.enabled = true;
            arrow.IsHittingToGround = false;
            arrow.Rigidbody.isKinematic = false;

            _arrowPoolObjects.ReturnObjectToPool(arrow);
        }
    }
}