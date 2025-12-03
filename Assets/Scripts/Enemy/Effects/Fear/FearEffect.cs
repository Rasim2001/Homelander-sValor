using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Effects.Fear
{
    public class FearEffect : IEnemyEffect, IEffectUpdater
    {
        private readonly EnemyMove _enemyMove;
        private readonly EnemyFlip _enemyFlip;
        private readonly EnemyAttack _enemyAttack;
        private readonly EnemyAnimator _enemyAnimator;
        private readonly Transform _enemyTransform;

        private Coroutine _fearCoroutine;

        public bool IsWorked { get; private set; }

        public FearEffect(
            EnemyMove enemyMove,
            EnemyFlip enemyFlip,
            EnemyAttack enemyAttack,
            EnemyAnimator enemyAnimator)
        {
            _enemyMove = enemyMove;
            _enemyFlip = enemyFlip;
            _enemyAttack = enemyAttack;
            _enemyAnimator = enemyAnimator;

            _enemyTransform = _enemyMove.transform;
        }


        public List<Type> GetOverridableEffects() =>
            null;

        public List<EnemyTypeId> GetUnaffectedUnits() =>
            null;

        public void ApplyEffect()
        {
            IsWorked = true;

            ApplyEffectLocal();
        }

        public void RemoveEffect()
        {
            IsWorked = false;

            RemoveEffectLocal();
        }

        public void Update()
        {
            if (!IsWorked)
                return;

            int directionX = _enemyTransform.position.x < 0 ? -1 : 1;
            Vector3 moveVector = new Vector3(directionX * (_enemyMove.Speed * Time.deltaTime), 0);
            _enemyTransform.Translate(moveVector);
        }


        private void ApplyEffectLocal()
        {
            _enemyMove.enabled = false;
            _enemyMove.IsActiveMove = true;

            _enemyAttack.enabled = false;
            _enemyAttack.IsAttackingProcess = false;

            _enemyAnimator.PlayRunAway(true);
            _enemyFlip.SetFlip(_enemyTransform.position.x < 0);
        }

        private void RemoveEffectLocal()
        {
            _enemyMove.enabled = true;
            _enemyMove.IsActiveMove = true;

            _enemyAttack.enabled = true;
            _enemyAnimator.PlayRunAway(false);
        }
    }
}