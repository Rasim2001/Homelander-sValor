using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Bonfire
{
    public class FireFlicker : MonoBehaviour
    {
        [SerializeField] private float _baseIntensity = 3;
        [SerializeField] private float _flickerIntensity = 5;
        [SerializeField] private float _flickerSpeed = 5f;

        private Light2D _fireLight;

        private void Awake() =>
            _fireLight = GetComponent<Light2D>();

        private void Update()
        {
            float noise1 = Mathf.PerlinNoise(Time.time * _flickerSpeed, 0f);
            float noise2 = Mathf.PerlinNoise(Time.time * _flickerSpeed * 2.3f + 50f, 0f);
            float noise3 = Mathf.PerlinNoise(Time.time * _flickerSpeed * 0.7f + 100f, 0f);

            float combinedNoise = (noise1 + noise2 * 0.5f + noise3 * 0.3f) / 1.8f;

            _fireLight.intensity = _baseIntensity + (combinedNoise - 0.5f) * _flickerIntensity;
        }
    }
}