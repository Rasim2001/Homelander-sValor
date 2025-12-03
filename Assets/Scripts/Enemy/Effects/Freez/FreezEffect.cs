using System;
using System.Collections.Generic;
using DG.Tweening;
using Enemy.Effects.Convert;
using Enemy.Effects.Fear;
using Enemy.Effects.Knockback;
using Enemy.Effects.Miss;
using Enemy.Effects.Slowdown;
using Enemy.Effects.Stun;
using UnityEngine;

namespace Enemy.Effects.Freez
{
    public class FreezEffect : IEnemyEffect
    {
        private static readonly int Percent = Shader.PropertyToID("_Percent");

        private readonly EnemyAttack _enemyAttack;
        private readonly EnemyMove _enemyMove;
        private readonly Animator _animator;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Material _freezMaterial;

        private Coroutine _freezCoroutine;
        private Tween _freezTween;

        private Material _freezMaterialInstance;
        private Material _defaultMaterialInstance;

        private float _percent;

        public bool IsWorked { get; private set; }

        public FreezEffect(
            EnemyAttack enemyAttack,
            EnemyMove enemyMove,
            Animator animator,
            SpriteRenderer spriteRenderer,
            Material freezMaterial)
        {
            _enemyAttack = enemyAttack;
            _enemyMove = enemyMove;
            _animator = animator;
            _spriteRenderer = spriteRenderer;
            _freezMaterial = freezMaterial;

            Initialize();
        }


        public void ApplyEffect()
        {
            IsWorked = true;

            _animator.speed = 0;
            _enemyMove.enabled = false;
            _enemyAttack.enabled = false;

            _spriteRenderer.material = _freezMaterialInstance;
            _freezTween = DOTween.To(() => _percent, x => _percent = x, 1, 1).SetEase(Ease.Linear)
                .OnUpdate(() => _freezMaterialInstance.SetFloat(Percent, _percent));
        }

        public void RemoveEffect()
        {
            IsWorked = false;

            _animator.speed = 1;
            _enemyMove.enabled = true;
            _enemyAttack.enabled = true;

            _freezTween.Kill();
            _freezMaterialInstance.SetFloat(Percent, 0);
            _spriteRenderer.material = _defaultMaterialInstance;
        }


        public List<Type> GetOverridableEffects()
        {
            return new List<Type>()
            {
                typeof(SlowdownEffect),
                typeof(FearEffect),
                typeof(MissEffect),
                typeof(ConvertEffect),
                typeof(StunEffect),
                typeof(KnockbackEffect),
            };
        }

        public List<EnemyTypeId> GetUnaffectedUnits() =>
            null;


        private void Initialize()
        {
            _defaultMaterialInstance = new Material(_spriteRenderer.material);
            _freezMaterialInstance = new Material(_freezMaterial);
            _spriteRenderer.material = _defaultMaterialInstance;
        }
    }
}