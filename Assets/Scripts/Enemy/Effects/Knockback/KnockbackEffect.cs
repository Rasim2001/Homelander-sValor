using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Effects.Knockback
{
    public class KnockbackEffect : IEnemyEffectWithSender
    {
        public Transform SenderTransform { get; set; }
        public bool IsWorked { get; private set; }

        private readonly EnemyMove _enemyMove;
        private readonly EnemyAttack _enemyAttack;
        private readonly EnemyAnimator _enemyAnimator;
        private readonly Rigidbody2D _rigidbody2D;

        private readonly Vector2 _knockbackImpulseDirection = new Vector3(3, 3);

        public KnockbackEffect(
            EnemyMove enemyMove,
            EnemyAttack enemyAttack,
            EnemyAnimator enemyAnimator,
            Rigidbody2D rigidbody2D)
        {
            _enemyMove = enemyMove;
            _enemyAttack = enemyAttack;
            _enemyAnimator = enemyAnimator;
            _rigidbody2D = rigidbody2D;
        }

        public void ApplyEffect()
        {
            if (SenderTransform == null)
                return;

            IsWorked = true;

            int signDirection = SenderTransform.position.x - _enemyMove.transform.position.x > 0 ? -1 : 1;
            Vector2 targetImpulse =
                new Vector2(signDirection * _knockbackImpulseDirection.x, _knockbackImpulseDirection.y);

            _rigidbody2D.AddForce(targetImpulse, ForceMode2D.Impulse);

            ApplyLocalEffect();
        }

        public void RemoveEffect()
        {
            IsWorked = false;
            SenderTransform = null;

            RemoveLocalEffect();
        }

        public List<Type> GetOverridableEffects()
        {
            return null;
        }

        public List<EnemyTypeId> GetUnaffectedUnits() =>
            null;

        private void ApplyLocalEffect()
        {
            _enemyMove.enabled = false;
            _enemyMove.IsActiveMove = false;

            _enemyAttack.enabled = false;

            _enemyAnimator.PlayStunEffectAnimation(true);
        }

        private void RemoveLocalEffect()
        {
            _enemyMove.enabled = true;

            if (!_enemyMove.TargetIsReached())
                _enemyMove.IsActiveMove = true;

            _enemyAttack.enabled = true;
            _enemyAttack.IsAttackingProcess = false;

            _enemyAnimator.PlayStunEffectAnimation(false);
        }
    }
}