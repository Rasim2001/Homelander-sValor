using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class CurtainWindow : MonoBehaviour, ICurtainWindow
    {
        [SerializeField] private GameObject _container;
        [SerializeField] private Ease _ease;
        [SerializeField] private Image _fillImage;
        private Tween _fillAmountTween;

        public void Show()
        {
            int randomTime = Random.Range(2, 5);
            _container.SetActive(true);

            Time.timeScale = 0;

            _fillImage.fillAmount = 0;
            _fillAmountTween?.Kill();
            _fillAmountTween = _fillImage.DOFillAmount(1, randomTime)
                .SetEase(_ease)
                .SetUpdate(true)
                .OnComplete(Hide);
        }

        public void Hide()
        {
            Time.timeScale = 1;

            _container.SetActive(false);
        }
    }
}