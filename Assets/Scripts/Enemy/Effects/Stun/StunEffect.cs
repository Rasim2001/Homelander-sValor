using System;
using System.Collections.Generic;
using Enemy.Effects.Convert;
using Enemy.Effects.Fear;
using Enemy.Effects.Knockback;
using Enemy.Effects.Miss;
using Enemy.Effects.Slowdown;
using UnityEngine;

namespace Enemy.Effects.Stun
{
    public class StunEffect : IEnemyEffect
    {
        public bool IsWorked { get; private set; }

        private readonly EnemyMove _enemyMove;
        private readonly EnemyAttack _enemyAttack;
        private readonly EnemyAnimator _enemyAnimator;

        public StunEffect(EnemyMove enemyMove, EnemyAttack enemyAttack, EnemyAnimator enemyAnimator)
        {
            _enemyMove = enemyMove;
            _enemyAttack = enemyAttack;
            _enemyAnimator = enemyAnimator;
        }

        public void ApplyEffect()
        {
            IsWorked = true;

            _enemyMove.enabled = false;
            _enemyAttack.enabled = false;

            _enemyAnimator.PlayStunEffectAnimation(true);
        }

        public void RemoveEffect()
        {
            IsWorked = false;

            _enemyMove.enabled = true;
            _enemyAttack.enabled = true;

            _enemyAnimator.PlayStunEffectAnimation(false);
        }

        public List<Type> GetOverridableEffects()
        {
            return new List<Type>()
            {
                typeof(SlowdownEffect),
                typeof(FearEffect),
                typeof(MissEffect),
                typeof(ConvertEffect),
                typeof(KnockbackEffect),
            };
        }

        public List<EnemyTypeId> GetUnaffectedUnits() =>
            null;
    }
}