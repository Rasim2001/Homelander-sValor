using DG.Tweening;
using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public class Carriage : MonoBehaviour, ICarriage
    {
        [SerializeField] private float _shootDuration;
        [SerializeField] private Transform _ballistaTransform;

        public Transform CarriageTransform => transform;

        public void Shoot()
        {
            transform.DOKill();
            transform.DOLocalMoveY(-0.4f, _shootDuration).OnComplete(Shake);
        }

        public void Reload()
        {
            transform.DOKill();
            transform.DOLocalMoveY(0, 1.25f);
        }


        private void Shake()
        {
            DOTween.Sequence()
                .Append(_ballistaTransform.DOShakePosition(0.1f, new Vector3(0, 0.02f, 0), 25, 90, false, false))
                .Join(transform.DOShakePosition(0.2f, new Vector3(0, 0.02f, 0), 25, 90, false, false));
        }
    }
}