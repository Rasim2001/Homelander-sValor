using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class TirednessProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;

        private Color _defaultColor;

        public float CurrentAlpha => _fillImage.color.a;

        private void Awake() =>
            _defaultColor = _fillImage.color;

        public void UpdateProgressBar(float percentage) =>
            _fillImage.fillAmount = percentage;

        public void SetAlpha(float alpha)
        {
            Color newColor = _defaultColor;
            newColor.a = alpha;

            _fillImage.color = newColor;
        }
    }
}