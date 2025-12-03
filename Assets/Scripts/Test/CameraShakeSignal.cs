using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Test
{
    public class CameraShakeSignal : MonoBehaviour
    {
        [SerializeField] private Ease _ease;
        [SerializeField] private CinemachineVirtualCamera _nearCamera;

        [SerializeField] private float duration = 0.2f;
        [SerializeField] private float strength = 0.1f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float randomness = 90;

        private Tween _shakeTween;


        public void Shake()
        {
            Debug.Log("Shake");

            _shakeTween = _nearCamera.transform
                .DOShakePosition(duration, strength, vibrato, randomness).SetEase(_ease)
                .SetLoops(-1, LoopType.Yoyo).OnStepComplete(() => strength += 0.05f);
        }

        public void CancelShake()
        {
            Tween _orthoTween = DOTween.To(
                () => _nearCamera.m_Lens.OrthographicSize,
                x => _nearCamera.m_Lens.OrthographicSize = x,
                5.25f,
                0.2f
            ).SetEase(_ease);

            Tween _orthoTween2 = DOTween.To(
                () => _nearCamera.m_Lens.OrthographicSize,
                x => _nearCamera.m_Lens.OrthographicSize = x,
                5.5f,
                0.05f
            ).SetEase(_ease);


            DOTween.Sequence().Append(_orthoTween).Append(_orthoTween2);

            _shakeTween.Kill();
        }
    }
}