using Infastructure.StaticData.Unit;
using Units.Animators;
using UnityEngine;

namespace Units.UnitStates
{
    public class RunState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly UnitMove _unitMove;
        private readonly UnitAnimator _unitAnimator;
        private readonly UnitStaticData _unitStaticData;

        public RunState(IUnitStateMachine unitStateMachine, UnitMove unitMove, UnitAnimator unitAnimator,
            UnitStaticData unitStaticData)
        {
            _unitStateMachine = unitStateMachine;
            _unitMove = unitMove;
            _unitAnimator = unitAnimator;
            _unitStaticData = unitStaticData;
        }

        public void Enter()
        {
            ChangeSpeedAnimation();

            _unitMove.ChangeTargetPosition();
            _unitMove.SetSpeed(_unitStaticData.RunSpeed);

            _unitAnimator.SetRunAnimation(true);
        }

        public void Exit() =>
            _unitAnimator.SetRunAnimation(false);

        public void Update()
        {
            if (_unitMove.IsPathReached())
                _unitStateMachine.ChangeState<IdleState>();
            else
                _unitMove.Move();
        }

        private void ChangeSpeedAnimation() =>
            _unitAnimator.ChangeAnimationSpeed(1);
    }
}