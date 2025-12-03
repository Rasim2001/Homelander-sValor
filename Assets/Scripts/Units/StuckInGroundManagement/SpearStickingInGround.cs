using System.Collections;
using BuildProcessManagement.Towers.BallistaBow;
using DG.Tweening;
using UnityEngine;

namespace Units.RangeUnits
{
    public class SpearStickingInGround : StuckInGround<Spear>
    {
        protected override void OnGroundHitLogic(Spear spear) =>
            DestroyCoroutine = StartCoroutine(DestroySpear(spear));

        private IEnumerator DestroySpear(Spear spear)
        {
            spear.IsHittingToGround = true;

            yield return new WaitForSeconds(2);

            ShakeTween.Kill();
            Destroy(gameObject);
        }
    }
}