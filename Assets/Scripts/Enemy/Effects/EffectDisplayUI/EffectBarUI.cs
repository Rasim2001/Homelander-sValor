using UnityEngine;
using UnityEngine.UI;

namespace Enemy.Effects.EffectDisplayUI
{
    public class EffectBarUI : MonoBehaviour
    {
        [SerializeField] private Image _effectImage;
        [SerializeField] private Image _bgBarImage;

        private float _progressTime;

        public void SetEffectSprite(Sprite sprite) =>
            _effectImage.sprite = sprite;

        public void SetEffectBarTime(float progressTime)
        {
            _bgBarImage.fillAmount = 1;
            _progressTime = progressTime;
        }

        public void UpdateEffectBarUI()
        {
            if (_bgBarImage.fillAmount > 0)
                _bgBarImage.fillAmount -= Time.deltaTime / _progressTime;
        }
    }
}