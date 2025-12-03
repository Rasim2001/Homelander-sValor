using Units.Animators;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.UnitStates.FollowToPlayerStates
{
    public abstract class FollowToPlayerBaseState
    {
        protected readonly IUnitStateMachine _stateMachine;
        protected readonly Transform _unitTransform;
        protected readonly UnitFlip _unitFlip;
        protected readonly UnitStatus _unitStatus;
        protected readonly UnitAnimator _unitAnimator;
        protected readonly UnitMove _unitMove;

        protected FollowToPlayerBaseState(
            IUnitStateMachine stateMachine,
            Transform unitTransform,
            UnitFlip unitFlip,
            UnitStatus unitStatus,
            UnitAnimator unitAnimator,
            UnitMove unitMove)
        {
            _stateMachine = stateMachine;
            _unitTransform = unitTransform;
            _unitFlip = unitFlip;
            _unitStatus = unitStatus;
            _unitAnimator = unitAnimator;
            _unitMove = unitMove;
        }


        protected void Move(int direction, float distance, float randomFollowSpeed)
        {
            Vector3 targetPosition = _unitTransform.position + new Vector3(direction * distance, 0, 0);

            float step = randomFollowSpeed * Time.deltaTime;
            _unitTransform.position = Vector3.MoveTowards(_unitTransform.position, targetPosition, step);
        }
    }
}