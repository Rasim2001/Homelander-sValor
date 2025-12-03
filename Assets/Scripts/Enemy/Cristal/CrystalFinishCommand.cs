using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Enemy.Cristal
{
    public class CrystalFinishCommand : IPayloadCommand
    {
        private static readonly int AddColorFade = Shader.PropertyToID("_AddColorFade");

        private readonly Transform _crystalSpriteTransform;
        private readonly Light2D _light2d;
        private readonly EnemyObserverTrigger _aggressionObserverTrigger;

        private bool _operationIsReady;
        private bool _isExecuting;
        private bool _isTriggered;

        private Sequence _crystalSequence;

        private CancellationToken _cancellationToken;

        private readonly Vector3 _minScale = new Vector3(0.5f, 0.5f);
        private readonly Vector3 _maxScale = Vector3.one;
        private readonly float _targetTime = 3;
        private readonly Material _material;


        public CrystalFinishCommand(Transform crystalSpriteTransform, SpriteRenderer spriteRenderer, Light2D light2d,
            EnemyObserverTrigger aggressionObserverTrigger)
        {
            _crystalSpriteTransform = crystalSpriteTransform;
            _light2d = light2d;
            _aggressionObserverTrigger = aggressionObserverTrigger;

            _material = spriteRenderer.material;
        }

        public async UniTask Execute(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _operationIsReady = false;
            _isExecuting = true;

            TryWaitTime().Forget();
            TriggerExit();

            await UniTask.WaitUntil(() => _operationIsReady, cancellationToken: cancellationToken);
        }

        public void Initialize()
        {
            _aggressionObserverTrigger.OnTriggerEnter += TriggerEnter;
            _aggressionObserverTrigger.OnTriggerExit += TriggerExit;
        }

        public void Clear()
        {
            _aggressionObserverTrigger.OnTriggerEnter -= TriggerEnter;
            _aggressionObserverTrigger.OnTriggerExit -= TriggerExit;
        }

        private void TriggerEnter()
        {
            if (_isTriggered || !_isExecuting)
                return;

            _isTriggered = true;
            _crystalSequence?.Kill();
        }

        private void TriggerExit()
        {
            if (_aggressionObserverTrigger.HasAnyColliders() || !_isExecuting)
                return;

            _isTriggered = false;

            DeActivateCrystal().Forget();
        }


        private async UniTask DeActivateCrystal()
        {
            _crystalSequence = DOTween.Sequence();

            float currentLocalScaleX = _crystalSpriteTransform.localScale.x;
            float progress = (currentLocalScaleX - _minScale.x) / (_maxScale.x - _minScale.x);
            float remainingTime = _targetTime * progress;

            Tween scaleTween = _crystalSpriteTransform.DOScale(_minScale, remainingTime);
            Tween materialTween = _material.DOFloat(0, AddColorFade, remainingTime);
            Tween lightTween = DOTween.To(() => _light2d.intensity, x => _light2d.intensity = x, 0, remainingTime);

            _crystalSequence
                .Append(scaleTween)
                .Join(materialTween)
                .Join(lightTween);

            await _crystalSequence.WithCancellation(_cancellationToken);
        }

        private async UniTask TryWaitTime() =>
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: _cancellationToken).ContinueWith(Finish);

        private void Finish()
        {
            _isExecuting = false;
            _isTriggered = false;

            _operationIsReady = true;
        }
    }
}