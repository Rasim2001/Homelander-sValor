using System;
using System.Collections;
using Infastructure;
using UnityEngine;

namespace CutScenes
{
    public class CristalEmbersParticlesSignal
    {
        private readonly ParticleSystem _particleSystem;
        private readonly ICoroutineRunner _coroutineRunner;

        public CristalEmbersParticlesSignal(ParticleSystem particleSystem, ICoroutineRunner coroutineRunner)
        {
            _particleSystem = particleSystem;
            _coroutineRunner = coroutineRunner;
        }

        public void PlayParticles(Action onComplete) =>
            _coroutineRunner.StartCoroutine(StartParticleAnimationCoroutine(onComplete));

        private IEnumerator StartParticleAnimationCoroutine(Action onComplete)
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.0f);
            curve.AddKey(1.0f, 1.0f);
            curve.AddKey(-20, -20);
            curve.AddKey(15, 15);
            curve.AddKey(-40, -40);

            var velocityOverLifetime = _particleSystem.velocityOverLifetime;
            velocityOverLifetime.xMultiplier = -20;
            velocityOverLifetime.yMultiplier = 15f;
            velocityOverLifetime.radialMultiplier = 1;

            _particleSystem.Play();

            yield return new WaitForSeconds(4);

            velocityOverLifetime.xMultiplier = 0;
            velocityOverLifetime.yMultiplier = 0;
            velocityOverLifetime.radialMultiplier = -40;

            yield return new WaitForSeconds(2.2f);

            onComplete?.Invoke();
        }
    }
}