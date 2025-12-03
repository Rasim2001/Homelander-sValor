using UnityEngine;

namespace Enemy.Cristal
{
    public class Levitate : MonoBehaviour
    {
        private readonly float _followSpeed = 5f;
        private readonly float _waveAmplitude = 0.1f;
        private readonly float _speed = 4;
        private readonly float _waveFrequency = 2f;

        private float _waveTimer = 0f;

        public void Reset() =>
            _waveTimer = 0;

        public void UpdateCustom(Vector2 targetPosition)
        {
            _waveTimer += Time.deltaTime * _waveFrequency;

            float waveOffsetY = Mathf.Sin(_waveTimer) * _waveAmplitude;
            targetPosition.y += waveOffsetY;

            transform.position =
                Vector2.Lerp(transform.position, targetPosition,
                    (_followSpeed + _speed) * Time.deltaTime);
        }
    }
}