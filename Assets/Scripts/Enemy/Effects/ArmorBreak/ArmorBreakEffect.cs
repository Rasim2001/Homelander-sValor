using System;
using System.Collections.Generic;
using Player;

namespace Enemy.Effects.ArmorBreak
{
    public class ArmorBreakEffect : IEnemyEffect
    {
        public bool IsWorked { get; private set; }

        private readonly Health _health;

        public ArmorBreakEffect(Health health) =>
            _health = health;

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

        public List<Type> GetOverridableEffects() =>
            null;

        public List<EnemyTypeId> GetUnaffectedUnits() =>
            null;

        private void ApplyEffectLocal() =>
            _health.DamageMultiplier = 2;

        private void RemoveEffectLocal() =>
            _health.DamageMultiplier = 1;
    }
}