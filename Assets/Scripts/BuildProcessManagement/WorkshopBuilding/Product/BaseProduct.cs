using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BuildProcessManagement.WorkshopBuilding.Product
{
    public class BaseProduct : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        [SerializeField] private Material _unlitMaterial;
        [SerializeField] private Material _litMaterial;

        private CancellationTokenSource _cts;

        private void Awake() =>
            _spriteRenderer = GetComponent<SpriteRenderer>();

        private void OnDisable() =>
            StopIlluminate();

        public void Initialize(Material unlitMaterial, Material litMaterial)
        {
            _litMaterial = litMaterial;
            _unlitMaterial = unlitMaterial;
        }

        public void Illuminate()
        {
            _cts = new CancellationTokenSource();

            IlluminateTask().Forget();
        }

        private async UniTaskVoid IlluminateTask()
        {
            float blinkDuration = 0.1f;
            int blinkCount = 3;

            for (int i = 0; i < blinkCount; i++)
            {
                _spriteRenderer.material = _unlitMaterial;
                await UniTask.Delay(TimeSpan.FromSeconds(blinkDuration), cancellationToken: _cts.Token);

                _spriteRenderer.material = _litMaterial;
                await UniTask.Delay(TimeSpan.FromSeconds(blinkDuration), cancellationToken: _cts.Token);
            }
        }


        private void StopIlluminate()
        {
            if (_cts == null)
                return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;

            _spriteRenderer.material = _litMaterial;
        }
    }
}