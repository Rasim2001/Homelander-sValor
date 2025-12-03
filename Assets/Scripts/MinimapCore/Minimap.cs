using DG.Tweening;
using UnityEngine;

namespace MinimapCore
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float blinkDuration = 0.1f;

        private Sequence _sequence;

        public void ShowHit()
        {
            if (_spriteRenderer == null || !_spriteRenderer.enabled)
                return;

            if (_sequence != null && _sequence.IsActive())
                _sequence.Kill();

            _sequence = DOTween.Sequence()
                .Append(_spriteRenderer.DOColor(Color.red, blinkDuration))
                .Append(_spriteRenderer.DOColor(Color.white, blinkDuration))
                .SetAutoKill(true);
        }

        public void ShowUpgradeProcess()
        {
            Color color = new Color(1, 1, 1, 0.25f);
            _spriteRenderer.color = color;
        }

        public void HideUpgradedProcess() =>
            _spriteRenderer.color = Color.white;

        public void Hide() =>
            _spriteRenderer.enabled = false;

        public void Show() =>
            _spriteRenderer.enabled = true;

        private void OnDestroy()
        {
            if (_sequence != null && _sequence.IsActive())
                _sequence.Kill();

            _sequence = null;
        }
    }
}