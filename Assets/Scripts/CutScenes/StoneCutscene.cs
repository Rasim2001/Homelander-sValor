using UnityEngine;

namespace CutScenes
{
    public class StoneCutscene : MonoBehaviour
    {
        private float _followSpeed = 4f;
        private float _waveAmplitude = 0.1f;
        private float _waveTimer = 0f;
        private float _waveFrequency = 4f;

        private float _speed;

        private void Start()
        {
            _waveTimer = 0;
            _followSpeed = Random.Range(4, 10);
            _waveAmplitude = Random.Range(0.1f, 0.3f);
            _waveFrequency = Random.Range(3, 7);

            _speed = Random.Range(2, 6);
        }

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