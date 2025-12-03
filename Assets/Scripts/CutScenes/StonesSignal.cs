using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Infastructure;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CutScenes
{
    public class StonesSignal
    {
        private static readonly int AddColorFade = Shader.PropertyToID("_AddColorFade");

        private readonly Vector3 _centerOfRuins;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly List<StoneSignalData> _stonesSignals;

        private readonly float _maxLightIntensity = 0.25f;
        private float _time = 10;
        private Coroutine _moveWaveCoroutine;

        public StonesSignal(
            List<StoneSignalData> stonesSignals,
            Vector3 centerOfRuins,
            ICoroutineRunner coroutineRunner)
        {
            _centerOfRuins = centerOfRuins;
            _coroutineRunner = coroutineRunner;
            _stonesSignals = new List<StoneSignalData>(stonesSignals);
        }

        public void MoveWaveStones()
        {
            foreach (StoneSignalData stonesSignal in _stonesSignals)
            {
                Rigidbody2D rigidbody2D = stonesSignal.StoneCutscene.GetComponent<Rigidbody2D>();

                AnimateLight(stonesSignal);
                AnimateGlowMask(stonesSignal);
                SetGravity(0, rigidbody2D);
            }

            _moveWaveCoroutine = _coroutineRunner.StartCoroutine(StartMoveWaveStonesCoroutine());
        }

        public void StopMoveStones()
        {
            foreach (StoneSignalData stonesSignal in _stonesSignals)
            {
                Rigidbody2D rigidbody2D = stonesSignal.StoneCutscene.GetComponent<Rigidbody2D>();

                SetGravity(1, rigidbody2D);
                AddRandomMovement(rigidbody2D);
                TurnOffLight(stonesSignal);
                TurnOffGlowMask(stonesSignal);
            }

            _coroutineRunner.StopCoroutine(_moveWaveCoroutine);
        }

        private void TurnOffGlowMask(StoneSignalData stonesSignal)
        {
            SpriteRenderer spriteRenderer = stonesSignal.StoneCutscene.GetComponent<SpriteRenderer>();
            Material material = spriteRenderer.material;
            material.SetFloat(AddColorFade, 0);
        }

        private void TurnOffLight(StoneSignalData stonesSignal)
        {
            Light2D light2D = stonesSignal.StoneCutscene.GetComponent<Light2D>();
            light2D.intensity = 0;
        }

        private void SetGravity(int gravityScale, Rigidbody2D rigidbody2D) =>
            rigidbody2D.gravityScale = gravityScale;

        private void AddRandomMovement(Rigidbody2D rigidbody2D)
        {
            float deltaX = rigidbody2D.transform.localPosition.x - _centerOfRuins.x;
            int randomHeight = Random.Range(3, 7);
            int randomX = Random.Range(-2, 2);

            rigidbody2D.AddRelativeForce(new Vector2(deltaX + randomX, randomHeight), ForceMode2D.Impulse);
        }


        private IEnumerator StartMoveWaveStonesCoroutine()
        {
            while (_time > 0)
            {
                foreach (StoneSignalData stonesSignal in _stonesSignals)
                {
                    stonesSignal.StoneCutscene.UpdateCustom(stonesSignal.MovePoint.position);
                    yield return null;
                }

                _time -= Time.deltaTime;
            }
        }

        private void AnimateLight(StoneSignalData stonesSignal)
        {
            Light2D light2D = stonesSignal.StoneCutscene.GetComponent<Light2D>();

            DOTween.To
                (() => light2D.intensity,
                    x => light2D.intensity = x,
                    _maxLightIntensity,
                    1)
                .SetDelay(1)
                .SetEase(Ease.Linear);
        }

        private void AnimateGlowMask(StoneSignalData stonesSignal)
        {
            SpriteRenderer spriteRenderer = stonesSignal.StoneCutscene.GetComponent<SpriteRenderer>();
            Material material = spriteRenderer.material;

            DOTween.To(
                () => material.GetFloat(AddColorFade),
                x => material.SetFloat(AddColorFade, x),
                1,
                2
            ).SetEase(Ease.Linear);
        }
    }
}