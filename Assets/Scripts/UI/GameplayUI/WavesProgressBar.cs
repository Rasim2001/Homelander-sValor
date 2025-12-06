using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayUI
{
    public class WavesProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;

        private int _allSeconds;
        private Tween _waveTween;
        private int _lastSeconds = -1;

        public void InitializeWaves(int allSeconds)
        {
            _allSeconds = allSeconds;
            _fillImage.fillAmount = 0f;
            _lastSeconds = -1;
        }

        public void UpdateWavesBar(int currentSeconds)
        {
            if (_allSeconds <= 0)
                return;

            if (currentSeconds == _lastSeconds)
                return;

            _lastSeconds = currentSeconds;

            _waveTween?.Kill();

            float target = Mathf.Clamp01((float)currentSeconds / _allSeconds);

            _waveTween = _fillImage
                .DOFillAmount(target, 0.2f)
                .SetEase(Ease.Linear);
        }

        private void OnDestroy() =>
            _waveTween?.Kill();
    }
}