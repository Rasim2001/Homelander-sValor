using UnityEngine;

namespace Units.RangeUnits.AttackOnTower
{
    public class UnitAttackOnTowerBase : MonoBehaviour
    {
        [SerializeField] protected Animator _animator;

        [SerializeField] protected Collider2D _target;

        protected UnitObserverTrigger _unitObserverTrigger;


        public void Initialize(UnitObserverTrigger unitObserverTrigger)
        {
            _unitObserverTrigger = unitObserverTrigger;

            _unitObserverTrigger.OnTriggerEnter += Enter;
            _unitObserverTrigger.OnTriggerExit += Exit;
        }


        protected virtual void OnDestroy()
        {
            if (_unitObserverTrigger == null)
                return;

            _unitObserverTrigger.OnTriggerEnter -= Enter;
            _unitObserverTrigger.OnTriggerExit -= Exit;

            _target = null;
        }


        protected virtual void Enter()
        {
        }

        protected virtual void Shoot()
        {
        }

        protected virtual void Exit() =>
            Enter();
    }
}