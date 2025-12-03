using DG.Tweening;
using UnityEngine;

namespace BuildProcessManagement.ResourceElements
{
    public class RepairIconDisplay : MonoBehaviour
    {
        [SerializeField] private Transform _iconTransform;

        private Sequence _sequence;

        public void Show()
        {
            _sequence?.Kill();

            _sequence = DOTween.Sequence();
            _sequence.Append(_iconTransform.DOScale(Vector3.one, 0.35f));
            _sequence.Append(_iconTransform.DOScale(Vector3.zero, 0.35f));
        }

        private void OnDestroy() =>
            _sequence.Kill();
    }
}