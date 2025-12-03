using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Bonfire
{
    public class UpdateBonfireLevelAnimation : IUpdateBonfireLevelAnimation
    {
        public void PlayAnimation(List<Transform> Elements, float duration, Ease ease)
        {
            foreach (Transform element in Elements)
            {
                Initialize(element);
                Animate(element, duration, ease);
            }
        }

        private void Initialize(Transform element) =>
            element.transform.localScale = Vector3.zero;

        private void Animate(Transform element, float duration, Ease ease)
        {
            Sequence sequence = DOTween.Sequence();

            Tween scaleTween = element.DOScale(new Vector3(1.2f, 1.3f, 1), duration).SetEase(ease);
            Tween normalTween = element.DOScale(Vector3.one, 0.1f).SetEase(Ease.InCirc);

            sequence.Append(scaleTween);
            sequence.Append(normalTween);
        }
    }
}