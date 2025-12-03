using System;
using Units.RangeUnits;
using UnityEngine;

namespace BuildProcessManagement.Towers.CatapultTower
{
    public class Bowl : MonoBehaviour, IBowl
    {
        private static readonly int ShootHash = Animator.StringToHash("Shoot");

        [SerializeField] private ItemShooterBase _shooterBase;
        [SerializeField] private Animator _animator;

        private Transform _target;

        private SpriteRenderer _spriteRenderer;

        private void Awake() =>
            _spriteRenderer = GetComponent<SpriteRenderer>();

        public void SetFlipX(bool value) =>
            _spriteRenderer.flipX = value;

        public void SetTarget(Transform target) =>
            _target = target;

        public void PlayShootAnimation() =>
            _animator.SetTrigger(ShootHash);

        public void OnShootHappened() //calling from unity
            => _shooterBase.Shoot(_target);
    }
}