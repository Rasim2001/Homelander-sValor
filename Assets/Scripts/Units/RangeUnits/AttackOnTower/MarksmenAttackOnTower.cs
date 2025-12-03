using Units.UnitStates;
using UnityEngine;

namespace Units.RangeUnits.AttackOnTower
{
    public class MarksmenAttackOnTower : MonoBehaviour
    {
        [SerializeField] private ItemShooterBase _shooterBase;
        [SerializeField] private Animator _animator;

        private Collider2D _target;
        private UnitObserverTrigger _unitObserverTrigger;

        private Coroutine _coroutine;
        private bool _isShooting;


        public void Initialize(UnitObserverTrigger unitObserverTrigger)
        {
            _unitObserverTrigger = unitObserverTrigger;

            _unitObserverTrigger.OnTriggerEnter += Enter;
            _unitObserverTrigger.OnTriggerExit += Exit;
        }


        private void OnDestroy()
        {
            if (_unitObserverTrigger == null)
                return;

            _unitObserverTrigger.OnTriggerEnter -= Enter;
            _unitObserverTrigger.OnTriggerExit -= Exit;
        }


        public void OnAttackStarted() //calling from unity
            => Shoot();

        public void OnAttackEnded() //calling from unity
            => _target = _unitObserverTrigger.GetNearestHit();

        public void Enter()
        {
            _target = _unitObserverTrigger.GetNearestHit();

            _animator.Play(_target == null ? UnitStatesPath.IdleHash : UnitStatesPath.AttackHash);
        }

        private void Shoot()
        {
            if (_target != null)
                _shooterBase.Shoot(_target.transform);
        }

        private void Exit() =>
            Enter();
    }
}