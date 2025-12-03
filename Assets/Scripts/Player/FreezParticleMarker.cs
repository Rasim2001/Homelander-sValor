using UnityEngine;

namespace Player
{
    public class FreezParticleMarker : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake() =>
            _particleSystem = GetComponent<ParticleSystem>();

        private void OnEnable() =>
            _particleSystem.Play();
    }
}