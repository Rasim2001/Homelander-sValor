using Player;
using Units.Animators;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.UnitStates.FollowToPlayerStates
{
    public class IdleTowardsPlayer : FollowToPlayerBaseState, IUnitState
    {
        private readonly AutomaticAttackZone _playerAutomaticAttackZone;

        public IdleTowardsPlayer(IUnitStateMachine stateMachine, AutomaticAttackZone playerAutomaticAttackZone,
            Transform unitTransform, UnitFlip unitFlip,
            UnitStatus unitStatus, UnitAnimator unitAnimator, UnitMove unitMove) : base(stateMachine, unitTransform,
            unitFlip, unitStatus, unitAnimator, unitMove)
        {
            _playerAutomaticAttackZone = playerAutomaticAttackZone;
        }

        public void Enter()
        {
            _unitAnimator.SetIdleAnimation(true);
            _playerAutomaticAttackZone.TryAttackAgain();
        }

        public void Exit()
        {
            _unitAnimator.SetIdleAnimation(false);
        }

        public void Update()
        {
            if (_unitStatus.PlayerMove == null)
                return;

            Vector3 targetPosition =
                _unitStatus.PlayerMove.transform.position +
                new Vector3(_unitStatus.OffsetX * _unitStatus.PlayerFlip.FlipValue(), 0, 0);

            int direction = targetPosition.x - _unitTransform.position.x > 0 ? 1 : -1;
            float distance = Mathf.Abs(targetPosition.x - _unitTransform.position.x);

            _unitFlip.SetFlip(distance, direction, _unitStatus.PlayerFlip);

            ChangePlayerFollowState(distance);
        }


        private void ChangePlayerFollowState(float distance)
        {
            if (distance > 0.1f && _unitStatus.PlayerMove.IsMoving())
            {
                if (_unitStatus.PlayerMove.AccelerationPressedWithMove && _unitStatus.PlayerMove.IsMoving())
                    _stateMachine.ChangeState<FastRunTowardsPlayer>();
                else
                    _stateMachine.ChangeState<RunTowardsPlayer>();
            }
        }
    }
}