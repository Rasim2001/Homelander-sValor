using UnityEngine;

namespace Player
{
    public class PlayerGiveOrderFx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _shaveParticle;
        [SerializeField] private PlayerFlip _playerFlip;


        public void PlayFx()
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = _shaveParticle.velocityOverLifetime;
            ParticleSystem.MinMaxCurve linearX = new ParticleSystem.MinMaxCurve(-_playerFlip.FlipValue());
            velocityOverLifetime.x = linearX;

            ParticleSystemRenderer render = _shaveParticle.GetComponent<ParticleSystemRenderer>();
            render.flip = new Vector3(-_playerFlip.FlipValue(), 0, 0);

            _shaveParticle.Play();
        }
    }
}