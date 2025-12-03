using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyAgressionZone : MonoBehaviour
    {
        [SerializeField] private EnemyAttack _enemyAttack;
        [SerializeField] private EnemyMove _enemyMove;
        [SerializeField] private EnemyObserverTrigger _observerTrigger;

        [SerializeField] private Collider2D _currentHit;

        public Action OnFollowToTargetHappened;
        public Action OnReleaseHappened;

        private bool _hasTriggered;

        private void Start()
        {
            _observerTrigger.OnTriggerEnter += TriggerEnter;
            _observerTrigger.OnTriggerExit += TriggerExit;
        }


        private void OnDestroy()
        {
            _observerTrigger.OnTriggerEnter -= TriggerEnter;
            _observerTrigger.OnTriggerExit -= TriggerExit;
        }

        private void TriggerEnter()
        {
            _currentHit = _observerTrigger.GetActiveHitCollider();

            if (_currentHit == null)
            {
                CheckAndRelease();
                return;
            }

            OnFollowToTargetHappened?.Invoke();

            _enemyMove.FollowTo(_currentHit.transform);
            _enemyAttack.SetTarget(_currentHit.transform);
        }

        private void TriggerExit()
        {
            if (!gameObject.activeInHierarchy)
                return;

            _currentHit = null;
            
            ContinueFollowCoroutine();
        }


        private void ContinueFollowCoroutine()
        {
            if (_observerTrigger.HasAnyColliders())
                TriggerEnter();
            else
                CheckAndRelease();
        }

        private void CheckAndRelease()
        {
            OnReleaseHappened?.Invoke();

            _enemyAttack.SetTarget(null);
            _enemyMove.Release();
        }
    }
}