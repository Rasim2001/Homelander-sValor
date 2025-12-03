using DG.Tweening;
using UnityEngine;

namespace UI.GameplayUI
{
    public class KeyboardUI : MonoBehaviour
    {
        [SerializeField] private Transform[] _buttons;

        private Tween _clickTween;
        private Sequence _sequence;

        public void ClickOn(int index) =>
            PlayAnimation(index);


        private void PlayAnimation(int index)
        {
            Transform button = _buttons[index];

            if (button != null)
            {
                button.DOKill();

                if (_sequence != null && _sequence.IsActive())
                    _sequence.Kill();

                _sequence = DOTween.Sequence();
                _sequence.Append(button.DOScale(0.85f, 0.1f));
                _sequence.Append(button.DOScale(1, 0.1f));
            }
        }

        private void OnDisable() =>
            _sequence?.Kill();
    }
}