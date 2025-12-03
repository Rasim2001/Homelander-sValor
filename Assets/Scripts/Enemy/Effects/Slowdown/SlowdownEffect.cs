using System;
using System.Collections.Generic;
using Infastructure.StaticData.Enemy;

namespace Enemy.Effects.Slowdown
{
    public class SlowdownEffect : IEnemyEffect
    {
        private readonly EnemyMove _enemyMove;
        private readonly EnemyAnimator _enemyAnimator;
        private readonly EnemyStaticData _enemyStaticData;

        private float _defaultSpeed;
        private float _defaultAnimatorSpeed;

        public bool IsWorked { get; private set; }

        public SlowdownEffect(EnemyMove enemyMove, EnemyAnimator enemyAnimator, EnemyStaticData enemyStaticData)
        {
            _enemyMove = enemyMove;
            _enemyAnimator = enemyAnimator;
            _enemyStaticData = enemyStaticData;

            Initialize();
        }

        public List<Type> GetOverridableEffects() =>
            null;

        public List<EnemyTypeId> GetUnaffectedUnits()
        {
            return new List<EnemyTypeId>()
            {
                //EnemyTypeId.Marauder
            };
        }

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

        private void Initialize()
        {
            _defaultSpeed = _enemyStaticData.Speed;
            _defaultAnimatorSpeed = _enemyAnimator.GetAnimatorSpeed();
        }

        private void ApplyEffectLocal()
        {
            _enemyMove.Speed = _defaultSpeed / 2;
            _enemyAnimator.SetAnimatorSpeed(_defaultAnimatorSpeed / 2);
        }

        private void RemoveEffectLocal()
        {
            _enemyMove.Speed = _defaultSpeed;
            _enemyAnimator.SetAnimatorSpeed(_defaultAnimatorSpeed);
        }
    }
}