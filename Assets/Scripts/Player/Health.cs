using System;
using DG.Tweening;
using HealthSystem;
using UnityEngine;

namespace Player
{
    public class Health : MonoBehaviour, IHealth
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public int DamageMultiplier { get; set; } = 1;
        [field: SerializeField] public int CurrentHP { get; set; }
        [field: SerializeField] public int MaxHp { get; set; }
        public bool IsDeath { get; set; }

        public Action OnDeathHappened;

        private BoxCollider2D _healthCollider;

        private void Awake() =>
            _healthCollider = GetComponent<BoxCollider2D>();

        public void Initialize(int hp)
        {
            MaxHp = hp;
            CurrentHP = hp;
        }

        public void Reset() =>
            CurrentHP = MaxHp;

        public void TakeDamage(int damage)
        {
            if (IsDeath)
                return;

            CurrentHP -= damage * DamageMultiplier;

            if (CurrentHP <= 0)
                Death();
            else
                ShowTakeDamage();
        }

        private void Death()
        {
            IsDeath = true;
            _healthCollider.enabled = false;

            OnDeathHappened?.Invoke();
        }

        private void ShowTakeDamage()
        {
            if (_spriteRenderer == null)
                return;

            DOTween.Kill(gameObject);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_spriteRenderer.DOColor(Color.red, 0.1f));
            sequence.Append(_spriteRenderer.DOColor(Color.white, 0.1f))
                .SetTarget(gameObject);
        }

        private void OnDestroy()
        {
            if (_spriteRenderer != null)
                DOTween.Kill(gameObject);
        }
    }
}