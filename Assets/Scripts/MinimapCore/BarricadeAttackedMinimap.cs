using System;
using DG.Tweening;
using UnityEngine;

namespace MinimapCore
{
    public class BarricadeAttackedMinimap : MonoBehaviour
    {
        private Sequence _loopSequence;
        private Tween _showTween;
        private Tween _hideTween;

        private readonly float baseScale = 2f;
        private readonly float pulseScale = 2.5f;

        private void Awake() =>
            transform.localScale = Vector3.zero;


        public void EnableObject()
        {
            ClearTweens();

            _showTween = transform.DOScale(baseScale, 1f)
                .OnComplete(StartPulse);
        }


        public void DisableObject(Action onComplete)
        {
            ClearTweens();

            _hideTween = transform.DOScale(0f, 1f)
                .OnComplete(onComplete.Invoke);
        }

        private void StartPulse()
        {
            _loopSequence = DOTween.Sequence()
                .Append(transform.DOScale(pulseScale, 0.5f).SetEase(Ease.InOutSine))
                .Append(transform.DOScale(baseScale, 0.5f).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Restart);
        }

        private void ClearTweens()
        {
            _showTween?.Kill();
            _hideTween?.Kill();
            _loopSequence?.Kill();
        }
    }
}