using System;
using System.Collections;
using System.Collections.Generic;
using Infastructure;
using Player;
using UnityEngine;

namespace Enemy.Effects.Damage
{
    public class FireEffect : IEnemyEffect
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Health _health;
        public bool IsWorked { get; private set; }

        private readonly int _attackDamage = 5;

        public FireEffect(ICoroutineRunner coroutineRunner, Health health)
        {
            _coroutineRunner = coroutineRunner;
            _health = health;
        }

        private Coroutine _fireAttackCoroutine;

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

        private void ApplyEffectLocal()
        {
            if (_fireAttackCoroutine != null)
                _coroutineRunner.StopCoroutine(_fireAttackCoroutine);

            _fireAttackCoroutine = _coroutineRunner.StartCoroutine(StartDamageCoroutine());
        }

        private void RemoveEffectLocal()
        {
            if (_fireAttackCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_fireAttackCoroutine);
                _fireAttackCoroutine = null;
            }
        }

        private IEnumerator StartDamageCoroutine()
        {
            while (IsWorked)
            {
                _health.TakeDamage(_attackDamage);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}