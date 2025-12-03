using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Windows.Mainflag
{
    public class TaskItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Image _fillImage;

        private Coroutine _shakeCoroutine;
        private Sequence _sequence;

        [SerializeField] private float duration = 1f;
        [SerializeField] private Vector3 strength = new Vector3(0, 0, 15f);
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float randomness = 90;
        [SerializeField] private bool fadeOut = true;
        [SerializeField] private Ease ease = Ease.Linear;

        private void Start() => 
            _shakeCoroutine = StartCoroutine(StartShakeCoroutine());

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_shakeCoroutine == null)
                return;

            StopCoroutine(_shakeCoroutine);
            _sequence?.Kill();
            _shakeCoroutine = null;
        }

        public void OnPointerExit(PointerEventData eventData) =>
            _shakeCoroutine = StartCoroutine(StartShakeCoroutine());

        private IEnumerator StartShakeCoroutine()
        {
            yield return new WaitForSecondsRealtime(3);

            if (Mathf.Approximately(_fillImage.fillAmount, 1))
                yield break;

            while (true)
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence();

                Tween shakeTween =
                    _iconImage.transform.DOShakeRotation(
                            duration: duration,
                            strength: strength,
                            vibrato: vibrato,
                            randomness: randomness,
                            fadeOut: fadeOut,
                            ShakeRandomnessMode.Harmonic
                        )
                        .SetUpdate(UpdateType.Normal, true)
                        .SetEase(ease);

                Tween rotateTween = _iconImage.transform.DORotate(Vector3.zero, duration);

                _sequence
                    .Append(shakeTween)
                    .Append(rotateTween)
                    .SetUpdate(UpdateType.Normal, true);

                yield return new WaitForSecondsRealtime(duration * 2);
            }
        }

        private void OnDestroy() =>
            _sequence?.Kill();


        public void SetProgressFill(float value) =>
            _fillImage.fillAmount = value;

        public void SetAmountText(int amount) =>
            _amountText.text = $"x{amount}";

        public void SetIcon(Sprite sprite) =>
            _iconImage.sprite = sprite;
    }
}