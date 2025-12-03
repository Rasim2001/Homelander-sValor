using System;
using System.Collections;
using System.Collections.Generic;
using Infastructure;
using Player;
using UnityEngine;

namespace Enemy.Effects.Bleed
{
    public class BleedEffect : IEnemyEffect
    {
        public bool IsWorked { get; private set; }

        private readonly ICoroutineRunner _coroutineRunner;
        private readonly EnemyMove _enemyMove;
        private readonly Health _health;
        private readonly int _attackDamage = 5;

        private Coroutine _bleedingAttackCoroutine;

        public BleedEffect(ICoroutineRunner coroutineRunner, EnemyMove enemyMove, Health health)
        {
            _coroutineRunner = coroutineRunner;
            _enemyMove = enemyMove;
            _health = health;
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


        public List<Type> GetOverridableEffects() =>
            null;

        public List<EnemyTypeId> GetUnaffectedUnits() =>
            null;

        private void ApplyEffectLocal()
        {
            if (_bleedingAttackCoroutine != null)
                _coroutineRunner.StopCoroutine(_bleedingAttackCoroutine);

            _bleedingAttackCoroutine = _coroutineRunner.StartCoroutine(StartDamageCoroutine());
        }

        private void RemoveEffectLocal()
        {
            if (_bleedingAttackCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_bleedingAttackCoroutine);
                _bleedingAttackCoroutine = null;
            }
        }


        private IEnumerator StartDamageCoroutine()
        {
            while (IsWorked)
            {
                float damage = _enemyMove.IsActiveMove ? _enemyMove.Speed * _attackDamage : _attackDamage;
                _health.TakeDamage((int)damage);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}