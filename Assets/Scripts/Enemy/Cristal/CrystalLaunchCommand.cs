using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Enemy.Cristal
{
    public class CrystalLaunchCommand : IPayloadCommand
    {
        private static readonly int AddColorFade = Shader.PropertyToID("_AddColorFade");

        private readonly Transform _crystalSpriteTransform;
        private readonly EnemyObserverTrigger _aggressionZoneTrigger;
        private readonly Light2D _light2D;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Material _material;
        private readonly Action _onCancelAllCommand;

        private CancellationToken _cancellationToken;
        private bool _isTriggered;
        private bool _operationIsReady;

        private Sequence _crystalUpSequence;
        private Sequence _crystalDownSequence;

        private bool _isExecuting;

        private readonly Vector3 _minScale = new Vector3(0.5f, 0.5f);
        private readonly Vector3 _maxScale = Vector3.one;
        private readonly float _targetTime = 3;

        public CrystalLaunchCommand(
            Transform crystalSpriteTransform,
            EnemyObserverTrigger aggressionZoneTrigger,
            SpriteRenderer spriteRenderer,
            Light2D light2D,
            Action OnCancelAllCommand)
        {
            _crystalSpriteTransform = crystalSpriteTransform;
            _aggressionZoneTrigger = aggressionZoneTrigger;
            _light2D = light2D;
            _onCancelAllCommand = OnCancelAllCommand;

            _material = spriteRenderer.material;
        }

        public void Initialize()
        {
            _aggressionZoneTrigger.OnTriggerEnter += TriggerEnter;
            _aggressionZoneTrigger.OnTriggerExit += TriggerExit;
        }

        public void Clear()
        {
            _aggressionZoneTrigger.OnTriggerEnter -= TriggerEnter;
            _aggressionZoneTrigger.OnTriggerExit -= TriggerExit;
        }

        public async UniTask Execute(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _operationIsReady = false;
            _isExecuting = true;

            TriggerEnter();

            await UniTask.WaitUntil(() => _operationIsReady, cancellationToken: cancellationToken);
        }

        private void TriggerEnter()
        {
            if (_isTriggered || !_isExecuting)
                return;

            _isTriggered = true;

            ActivateCrystal().Forget();
        }

        private void TriggerExit()
        {
            if (_aggressionZoneTrigger.HasAnyColliders() || !_isExecuting)
                return;

            _isTriggered = false;
            DeActivateCrystal().Forget();
        }

        private async UniTask ActivateCrystal()
        {
            _crystalDownSequence?.Kill();
            _crystalUpSequence = DOTween.Sequence();

            float currentLocalScaleX = _crystalSpriteTransform.localScale.x;
            float progress = (_maxScale.x - currentLocalScaleX) / (_maxScale.x - _minScale.x);
            float remainingTime = _targetTime * progress;

            Tween scaleTween = _crystalSpriteTransform.DOScale(_maxScale, remainingTime);
            Tween materialTween = _material.DOFloat(1, AddColorFade, remainingTime);
            Tween lightTween = DOTween.To(() => _light2D.intensity, x => _light2D.intensity = x, 1, remainingTime);

            _crystalUpSequence
                .Append(scaleTween)
                .Join(materialTween)
                .Join(lightTween)
                .OnComplete(CompleteCrystal);


            await _crystalUpSequence.WithCancellation(_cancellationToken);
        }


        private async UniTask DeActivateCrystal()
        {
            _crystalUpSequence?.Kill();
            _crystalDownSequence = DOTween.Sequence();

            float currentLocalScaleX = _crystalSpriteTransform.localScale.x;
            float progress = (currentLocalScaleX - _minScale.x) / (_maxScale.x - _minScale.x);
            float remainingTime = _targetTime * progress;

            Tween scaleTween = _crystalSpriteTransform.DOScale(_minScale, remainingTime);
            Tween materialTween = _material.DOFloat(0, AddColorFade, remainingTime);
            Tween lightTween = DOTween.To(() => _light2D.intensity, x => _light2D.intensity = x, 0, remainingTime);

            _crystalDownSequence
                .Append(scaleTween)
                .Join(materialTween)
                .Join(lightTween)
                .OnComplete(CancelAll);

            await _crystalDownSequence.WithCancellation(_cancellationToken);
        }

        private void CancelAll()
        {
            CompleteCrystal();
            _onCancelAllCommand?.Invoke();
        }

        private void CompleteCrystal()
        {
            _isExecuting = false;
            _isTriggered = false;

            _operationIsReady = true;
        }
    }
}