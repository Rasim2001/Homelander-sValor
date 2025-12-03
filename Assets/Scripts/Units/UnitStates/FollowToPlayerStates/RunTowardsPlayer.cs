using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.UnitStates.FollowToPlayerStates
{
    public class RunTowardsPlayer : FollowToPlayerBaseState, IUnitState
    {
        private readonly IUnitsRecruiterService _unitsRecruiterService;
        private readonly UnitStaticData _unitStaticData;

        public RunTowardsPlayer(
            IUnitStateMachine stateMachine,
            IUnitsRecruiterService unitsRecruiterService,
            Transform unitTransform,
            UnitFlip unitFlip,
            UnitStatus unitStatus,
            UnitAnimator animator,
            UnitMove unitMove,
            UnitStaticData unitStaticData) : base(stateMachine, unitTransform, unitFlip, unitStatus, animator, unitMove)
        {
            _unitsRecruiterService = unitsRecruiterService;
            _unitStaticData = unitStaticData;
        }

        public void Enter()
        {
            _unitAnimator.SetRunAnimation(true);
            _unitMove.SetSpeed(_unitStaticData.RunTowardPlayerSpeed);
        }

        public void Exit()
        {
            _unitAnimator.SetRunAnimation(false);
        }

        public void Update()
        {
            if (_unitStatus.PlayerMove == null)
                return;

            float breakingBindDistance =
                Mathf.Abs(_unitStatus.PlayerMove.transform.position.x - _unitTransform.position.x);

            if (breakingBindDistance > 10)
                _unitsRecruiterService.ReleaseUnit(_unitStatus.UnitTypeId);
            else
            {
                Vector3 targetPosition =
                    _unitStatus.PlayerMove.transform.position +
                    new Vector3(_unitStatus.OffsetX * _unitStatus.PlayerFlip.FlipValue(), 0, 0);

                int direction = targetPosition.x - _unitTransform.position.x > 0 ? 1 : -1;
                float distance = Mathf.Abs(targetPosition.x - _unitTransform.position.x);

                _unitFlip.SetFlip(distance, direction, _unitStatus.PlayerFlip);

                ChangePlayerFollowState(distance);
                Move(direction, distance, _unitMove.GetRandomSpeed());
            }
        }


        private void ChangePlayerFollowState(float distance)
        {
            if (distance < 0.1f && !_unitStatus.PlayerMove.IsMoving())
                _stateMachine.ChangeState<IdleTowardsPlayer>();
            else if (_unitStatus.PlayerMove.IsMoving() && _unitStatus.PlayerMove.AccelerationPressedWithMove)
                _stateMachine.ChangeState<FastRunTowardsPlayer>();
        }
    }
}