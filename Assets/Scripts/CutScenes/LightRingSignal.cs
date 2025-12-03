using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CutScenes
{
    public class LightRingSignal
    {
        private readonly Light2D _lightRing;

        public LightRingSignal(Light2D lightRing) =>
            _lightRing = lightRing;

        public void PlayLightRing()
        {
            DOTween.To(() => _lightRing.intensity, x => _lightRing.intensity = x, 20, 4f);
            _lightRing.transform.DOScale(20, 4);
        }

        public void StopLightRing() =>
            _lightRing.transform.localScale = new Vector3(0, 0, 0);
    }
}