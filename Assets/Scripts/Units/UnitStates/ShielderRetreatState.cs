using Flag;
using Infastructure.Services.Flag;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.RangeUnits;
using Units.Shielder;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.UnitStates
{
    public class ShielderRetreatState : IUnitState
    {
        private readonly IFlagTrackerService _flagTrackerService;
        private readonly AttackOptionBase _attackOptionBase;
        private readonly UnitMove _unitMove;
        private readonly UnitStatus _unitStatus;
        private readonly ShielderAnimator _shielderAnimator;
        private readonly UnitAttack _unitAttack;
        private FlagSlotCoordinator _lastFlagSlotCoordinator;
        private readonly UnitFlip _unitFlip;
        private readonly UnitAggressionMove _unitAggressionMove;
        private readonly UnitStaticData _unitStaticData;
        private CheckAttackRange _checkAttackRange;

        private bool _isBindedFlag;
        private UnitAggressionZoneBase _unitAggressionZoneBase;

        public ShielderRetreatState(IFlagTrackerService flagTrackerService,
            AttackOptionBase attackOptionBase,
            UnitMove unitMove,
            UnitStatus unitStatus,
            ShielderAnimator shielderAnimator,
            UnitAttack unitAttack,
            UnitFlip unitFlip,
            UnitAggressionMove unitAggressionMove,
            UnitStaticData unitStaticData,
            CheckAttackRange checkAttackRange,
            UnitAggressionZoneBase unitAggressionZoneBase)
        {
            _unitAggressionZoneBase = unitAggressionZoneBase;
            _checkAttackRange = checkAttackRange;
            _flagTrackerService = flagTrackerService;
            _attackOptionBase = attackOptionBase;
            _unitMove = unitMove;
            _unitStatus = unitStatus;
            _shielderAnimator = shielderAnimator;
            _unitAttack = unitAttack;
            _unitFlip = unitFlip;
            _unitAggressionMove = unitAggressionMove;
            _unitStaticData = unitStaticData;
        }


        public void Enter()
        {
            _checkAttackRange.SetDefaultReachDistance();
            _unitAggressionMove.SetDefaultReacherDistance();

            if (!_flagTrackerService.LastFlagIsMainFlag(_unitMove.transform.position.x > 0))
                TryRetreat();
            else
                Release();
        }

        public void Update()
        {
            if (_lastFlagSlotCoordinator == null || _isBindedFlag)
                return;

            if (Mathf.Abs(_lastFlagSlotCoordinator.transform.position.x - _unitMove.transform.position.x) < 1)
                BindToFlag();
            else
                Retreat();
        }

        public void Exit()
        {
            _checkAttackRange.IsRetreating = false;
            _shielderAnimator.SetRetreatAnimation(false);

            if (_lastFlagSlotCoordinator != null)
                _lastFlagSlotCoordinator.OnDestroyHappened -= Release;
        }

        private void TryRetreat()
        {
            _lastFlagSlotCoordinator = _flagTrackerService.GetLastFlag(_unitMove.transform.position.x > 0)
                .GetComponent<FlagSlotCoordinator>();
            _lastFlagSlotCoordinator.OnDestroyHappened += Release;

            _checkAttackRange.IsRetreating = true;
            _attackOptionBase.SetAttackZone(false);
            _unitMove.ChangeTargetPosition();

            _unitAttack.Release();
            _unitAggressionMove.Release();

            _shielderAnimator.ResetAllAnimations();
            _unitMove.SetSpeed(_unitStaticData.RetreatSpeed);
            _shielderAnimator.ChangeAnimationSpeed(0.5f);
            _shielderAnimator.SetRetreatAnimation(true);
        }

        private void Release()
        {
            _unitStatus.IsDefensedFlag = false;
            _unitStatus.IsWorked = false;

            _unitAttack.OnAttackEnded();
            _unitAggressionZoneBase.CheckAndRelease();
            _attackOptionBase.SetAttackZone(true);
        }

        private void Retreat()
        {
            _unitMove.Move();
            _unitFlip.SetFlip(_unitMove.transform.position.x < 0);
        }

        private void BindToFlag()
        {
            if (_isBindedFlag)
                return;

            _isBindedFlag = true;

            _unitStatus.IsDefensedFlag = false;
            _unitStatus.IsWorked = false;

            _lastFlagSlotCoordinator.BindUnitToSlot(_unitStatus.transform, _unitStatus.UnitTypeId);
            _lastFlagSlotCoordinator.RelocateUnits();

            if (_lastFlagSlotCoordinator.HasEnemiesAroundBarricade)
                _lastFlagSlotCoordinator.PrepareForDefense();
        }
    }
}