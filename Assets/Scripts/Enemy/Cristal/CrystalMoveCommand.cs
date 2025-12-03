using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HealthSystem;
using UnityEngine;

namespace Enemy.Cristal
{
    public class CrystalMoveCommand : ICommand
    {
        private readonly IHealth _health;
        private readonly Transform _cristalSpriteTransform;
        private readonly float _directionY;

        public CrystalMoveCommand(Transform cristalSpriteTransform, float directionY)
        {
            _cristalSpriteTransform = cristalSpriteTransform;
            _directionY = directionY;
        }

        public async UniTask Execute(CancellationToken cancellationToken) =>
            await _cristalSpriteTransform.DOMoveY(_cristalSpriteTransform.position.y + _directionY * 0.7f, 1f)
                .WithCancellation(cancellationToken);
    }
}