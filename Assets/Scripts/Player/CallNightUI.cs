using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CallNightUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;

        private CanvasGroup _canvasGroup;
        private PlayerFlip _playerFlip;
        private RectTransform _rectTransform;

        private bool _isShowing;

        private void Awake()
        {
            _playerFlip = GetComponentInParent<PlayerFlip>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            /*_rectTransform.anchoredPosition = _playerFlip.FlipX
                ? new Vector2(1, _rectTransform.anchoredPosition.y)
                : new Vector2(-1, _rectTransform.anchoredPosition.y);*/
        }

        public void SetValue(float normalizedValue)
        {
            Show(normalizedValue > 0.15f);

            _fillImage.fillAmount = normalizedValue;
        }


        private void Show(bool value)
        {
            if (_isShowing == value)
                return;

            _isShowing = value;
            _canvasGroup.alpha = value ? 1 : 0;
        }
    }
}