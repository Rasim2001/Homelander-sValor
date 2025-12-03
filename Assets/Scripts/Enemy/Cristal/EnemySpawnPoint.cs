using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.Cristal
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Tween _alphaChangerTween;

        private Color _defaultColor;

        private void Awake() =>
            _spriteRenderer = GetComponent<SpriteRenderer>();

        private void Start() =>
            _defaultColor = _spriteRenderer.color;

        public async UniTask Show(CancellationToken cancellationToken)
        {
            float randomTime = Random.Range(1, 4);

            Color color = _spriteRenderer.color;
            Color newColor = new Color(color.r, color.g, color.b, 1);

            await DOTween.To(
                    () => _spriteRenderer.color,
                    x => _spriteRenderer.color = x,
                    newColor,
                    randomTime)
                .WithCancellation(cancellationToken: cancellationToken);
        }

        public async UniTask Hide(CancellationToken cancellationToken)
        {
            await DOTween.To(
                    () => _spriteRenderer.color,
                    x => _spriteRenderer.color = x,
                    _defaultColor,
                    0.5f)
                .WithCancellation(cancellationToken: cancellationToken);
        }
    }
}