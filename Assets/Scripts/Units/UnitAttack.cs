using Units.RangeUnits;
using UnityEngine;

namespace Units
{
    public class UnitAttack : MonoBehaviour
    {
        [SerializeField] protected UnitAggressionMove _unitAggressionMove;
        [SerializeField] protected Transform target;
        [SerializeField] protected bool _isAttacking;
        [SerializeField] protected bool _attackIsActive;

        [SerializeField] private UnitFlip _unitFlip;

        private void Update()
        {
            if (CanAttack())
            {
                SetFlip();
                StartAttack();
            }
        }

        public virtual void EnableAttack() =>
            _attackIsActive = true;

        public virtual void DisableAttack() =>
            _attackIsActive = false;

        public void SetTarget(Transform newTarget) =>
            target = newTarget;

        public void Release()
        {
            target = null;

            _isAttacking = false;
            _unitAggressionMove.EnableMove();

            _attackIsActive = false;
        }


        public virtual void OnAttackStarted()
        {
        }

        public virtual void OnAttackEnded()
        {
            _unitAggressionMove.EnableMove();
            _isAttacking = false;
        }

        protected virtual void StartAttack()
        {
            _isAttacking = true;
            _unitAggressionMove.DisableMove();
        }

        private void SetFlip() =>
            _unitFlip.SetFlip(target.transform.position.x - transform.position.x);


        private bool CanAttack() =>
            target != null && !_isAttacking && _attackIsActive;
    }
}