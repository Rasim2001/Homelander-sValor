using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DayCycle
{
    public class LightInterpolator : MonoBehaviour
    {
        [SerializeField] private float _dayTime;

        [SerializeField] private Light2D _shadowLight;
        [SerializeField] private LightFrame[] _lightFrames;

        private float _currentTime = 0;

        [SerializeField] private int _startFrame = 0;

        private void Update()
        {
            if (_lightFrames.Length == 0)
                return;

            _currentTime += Time.deltaTime;

            if (_startFrame < _lightFrames.Length - 1 &&
                _lightFrames[_startFrame + 1].NormalizedTime < _currentTime / _dayTime)
            {
                _startFrame += 1;
            }

            if (_startFrame == _lightFrames.Length)
                return;
            else
            {
                float frameLength = _lightFrames[_startFrame + 1].NormalizedTime -
                                    _lightFrames[_startFrame].NormalizedTime;
                float frameValue = _currentTime / _dayTime - _lightFrames[_startFrame].NormalizedTime;
                float normalizedFrame = frameValue / frameLength;

                Interpolate(_lightFrames[_startFrame].ReferenceLight, _lightFrames[_startFrame + 1].ReferenceLight,
                    normalizedFrame);
            }
        }


        private void Interpolate(Light2D start, Light2D end, float t)
        {
            _shadowLight.color = Color.Lerp(start.color, end.color, t);
            _shadowLight.intensity = Mathf.Lerp(start.intensity, end.intensity, t);

            Vector3[] startPath = start.shapePath;
            Vector3[] endPath = end.shapePath;

            Vector3[] newPath = new Vector3[startPath.Length];

            for (int i = 0; i < startPath.Length; ++i)
                newPath[i] = Vector3.Lerp(startPath[i], endPath[i], t);

            _shadowLight.SetShapePath(newPath);
        }
    }

    [Serializable]
    public class LightFrame
    {
        public Light2D ReferenceLight;
        public float NormalizedTime;
    }
}