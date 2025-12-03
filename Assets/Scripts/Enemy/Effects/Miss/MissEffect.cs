using System;
using System.Collections.Generic;

namespace Enemy.Effects.Miss
{
    public class MissEffect : IEnemyEffect
    {
        private readonly EnemyAttack _enemyAttack;

        public bool IsWorked { get; private set; }

        public MissEffect(EnemyAttack enemyAttack) =>
            _enemyAttack = enemyAttack;

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


        private void ApplyEffectLocal() =>
            _enemyAttack.IsMissActive = true;

        private void RemoveEffectLocal() =>
            _enemyAttack.IsMissActive = false;
    }
}