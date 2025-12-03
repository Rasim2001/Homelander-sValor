using System;
using System.Collections.Generic;
using Enemy.Effects.Fear;
using Player;
using UnityEngine;

namespace Enemy.Effects.Convert
{
    public class ConvertEffect : IEnemyEffect, IEffectUpdater
    {
        public bool IsWorked { get; private set; }

        private readonly CircleCollider2D _enemyObserverTriggerCollider;
        private readonly BoxCollider2D _healthCollider;

        private readonly EnemyObserverTrigger _enemyObserverTrigger;
        private readonly Health _health;
        private readonly EnemyAgressionZone _aggressionZone;
        private readonly EnemyMove _enemyMove;
        private readonly EnemyFlip _enemyFlip;
        private readonly EnemyAnimator _enemyAnimator;
        private readonly EnemyAttack _enemyAttack;

        private readonly Material _convertMaterial;
        private readonly SpriteRenderer _spriteRenderer;

        private readonly Transform _enemyTransform;

        private int _unitLayer;
        private int _enemyLayer;

        private LayerMask _unitLayerMask;
        private LayerMask _enemyLayerMask;

        private bool _isReleased = true;

        private Material _defaultMaterialInstance;
        private Material _convertMaterialInstance;


        public ConvertEffect(EnemyObserverTrigger enemyObserverTrigger,
            Health health,
            EnemyAgressionZone aggressionZone,
            EnemyMove enemyMove,
            EnemyFlip enemyFlip,
            EnemyAnimator enemyAnimator,
            EnemyAttack enemyAttack,
            Material convertMaterial,
            SpriteRenderer spriteRenderer)
        {
            _enemyObserverTrigger = enemyObserverTrigger;
            _health = health;
            _aggressionZone = aggressionZone;
            _enemyMove = enemyMove;
            _enemyFlip = enemyFlip;
            _enemyAnimator = enemyAnimator;
            _enemyAttack = enemyAttack;
            _convertMaterial = convertMaterial;
            _spriteRenderer = spriteRenderer;

            _enemyObserverTriggerCollider = enemyObserverTrigger.GetComponent<CircleCollider2D>();
            _healthCollider = health.GetComponent<BoxCollider2D>();

            _enemyTransform = _enemyMove.transform;

            InitializeMaterial();
            InitializeLayers();
        }

        public List<Type> GetOverridableEffects()
        {
            return new List<Type>()
            {
                typeof(FearEffect),
            };
        }

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
            if (!_isReleased || !IsWorked)
                return;

            int directionX = _enemyTransform.position.x < 0 ? -1 : 1;
            Vector3 moveVector = new Vector3(directionX * (_enemyMove.Speed * Time.deltaTime), 0);
            _enemyTransform.Translate(moveVector);
        }

        private void InitializeMaterial()
        {
            _defaultMaterialInstance = new Material(_spriteRenderer.material);
            _convertMaterialInstance = new Material(_convertMaterial);

            _spriteRenderer.material = _defaultMaterialInstance;
        }

        private void InitializeLayers()
        {
            _enemyLayer = LayerMask.NameToLayer("EnemyDamage");
            _unitLayer = LayerMask.NameToLayer("PlayerDamage");

            _enemyLayerMask = _enemyObserverTrigger.GetLayerMask();
            _unitLayerMask = 1 << _enemyLayer;
        }


        private void RemoveEffectLocal()
        {
            _spriteRenderer.material = _defaultMaterialInstance;

            _aggressionZone.OnFollowToTargetHappened -= FollowToTarget;
            _aggressionZone.OnReleaseHappened -= Release;

            SetEffectState(_enemyLayer, _enemyLayerMask);
            FollowToTarget();
        }

        private void ApplyEffectLocal()
        {
            _spriteRenderer.material = _convertMaterialInstance;

            _aggressionZone.OnFollowToTargetHappened += FollowToTarget;
            _aggressionZone.OnReleaseHappened += Release;

            SetEffectState(_unitLayer, _unitLayerMask);
            Release();
        }

        private void SetEffectState(int layer, LayerMask layerMask)
        {
            ToggleColliders(false);

            _enemyObserverTrigger.SetLayerMask(layerMask);
            _health.gameObject.layer = layer;

            ToggleColliders(true);
        }

        private void ToggleColliders(bool state)
        {
            _enemyAttack.enabled = state;

            _healthCollider.enabled = state;
            _enemyObserverTriggerCollider.enabled = state;
        }

        private void FollowToTarget()
        {
            _isReleased = false;

            _enemyAttack.IsAttackingProcess = false;
            _enemyMove.enabled = true;

            _enemyAnimator.PlayRunAway(false);
        }

        private void Release()
        {
            _isReleased = true;

            _enemyMove.enabled = false;
            _enemyMove.IsActiveMove = true;

            _enemyAttack.IsAttackingProcess = false;

            _enemyAnimator.PlayRunAway(true);
            _enemyFlip.SetFlip(_enemyTransform.position.x < 0);
        }
    }
}