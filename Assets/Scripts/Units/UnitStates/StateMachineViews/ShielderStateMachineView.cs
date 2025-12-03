using Infastructure.Services.Flag;
using Units.Animators;
using Units.RangeUnits;
using Units.Shielder;
using Units.UnitStates.DefaultStates;
using UnityEngine;
using Zenject;

namespace Units.UnitStates.StateMachineViews
{
    public class ShielderStateMachineView : UnitStateMachineView
    {
        [SerializeField] private UnitAggressionMove _unitAggressionMove;
        [SerializeField] private UnitAttack _unitAttack;
        [SerializeField] private AttackOptionBase _attackOptionBase;
        [SerializeField] private CheckAttackRange _checkAttackRange;
        [SerializeField] private UnitAggressionZoneBase _unitAggressionZoneBase;

        private IFlagTrackerService _flagTrackerService;

        [Inject]
        public void Construct(IFlagTrackerService flagTrackerService) =>
            _flagTrackerService = flagTrackerService;

        public override void Initialize()
        {
            base.Initialize();

            ShielderAnimator shielderAnimator = UnitAnimator as ShielderAnimator;

            AddToCurrentStates(new ShielderRetreatState(
                _flagTrackerService,
                _attackOptionBase,
                UnitMove,
                UnitStatus,
                shielderAnimator,
                _unitAttack,
                UnitFlip,
                _unitAggressionMove,
                UnitData,
                _checkAttackRange,
                _unitAggressionZoneBase));

            AddToCurrentStates(new AttackDefaultState(shielderAnimator));
        }
    }
}