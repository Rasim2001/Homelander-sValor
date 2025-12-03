using Infastructure.StaticData.Unit;
using Units.Animators;

namespace Units.UnitStates
{
    public class WalkState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly UnitMove _unitMove;
        private readonly UnitAnimator _unitAnimator;
        private readonly UnitStaticData _unitStaticData;

        public WalkState(IUnitStateMachine unitStateMachine, UnitMove unitMove, UnitAnimator unitAnimator,
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

            _unitMove.SetSpeed(_unitStaticData.WalkSpeed);
            _unitAnimator.SetWalkAnimation(true);
        }

        public void Exit() =>
            _unitAnimator.SetWalkAnimation(false);

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