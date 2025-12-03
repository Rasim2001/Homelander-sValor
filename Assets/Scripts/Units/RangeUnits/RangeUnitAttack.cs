using Units.Animators;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.StateMachineViews;
using UnityEngine;

namespace Units.RangeUnits
{
    public class RangeUnitAttack : UnitAttack
    {
        [SerializeField] private ArcherAnimator _archerAnimator;
        [SerializeField] private UnitStateMachineView _stateMachineView;

        [SerializeField] private RangeKeeper _rangeKeeper;
        [SerializeField] private ItemShooterBase _shooterBase;


        public override void OnAttackStarted()
        {
            _archerAnimator.ChangeAnimationSpeed(Random.Range(0.8f, 1.25f));

            base.OnAttackStarted();

            _shooterBase.Shoot(target);
        }


        protected override void StartAttack()
        {
            if (!_rangeKeeper.IsActiveHit || _rangeKeeper.IsAroundOfMainFlag())
            {
                base.StartAttack();

                _stateMachineView.ChangeState<AttackDefaultState>();
            }
        }
    }
}