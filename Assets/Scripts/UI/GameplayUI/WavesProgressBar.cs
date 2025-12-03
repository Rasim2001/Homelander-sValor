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

        public void InitializeWaves(int allSeconds) =>
            _allSeconds = allSeconds;

        public void UpdateWavesBar(int currentSeconds)
        {
            _waveTween = _fillImage.DOFillAmount((float)currentSeconds / _allSeconds, 1)
                .SetEase(Ease.Linear);
        }

        private void OnDestroy() =>
            _waveTween?.Kill();
    }
}