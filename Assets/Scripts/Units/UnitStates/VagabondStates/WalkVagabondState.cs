using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.Vagabond;

namespace Units.UnitStates.VagabondStates
{
    public class WalkVagabondState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly VagabondMove _unitMove;
        private readonly UnitAnimator _unitAnimator;
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly UnitStaticData _unitStaticData;

        public WalkVagabondState(IUnitStateMachine unitStateMachine, ISafeBuildZone safeBuildZone,
            VagabondMove unitMove, UnitAnimator unitAnimator, UnitStaticData unitStaticData)
        {
            _unitStateMachine = unitStateMachine;
            _unitMove = unitMove;
            _unitAnimator = unitAnimator;
            _safeBuildZone = safeBuildZone;
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
                _unitStateMachine.ChangeState<IdleVagabondState>();
            else if (_safeBuildZone.IsNight)
                _unitStateMachine.ChangeState<FearVagabondState>();
            else
                _unitMove.Move();
        }

        private void ChangeSpeedAnimation() =>
            _unitAnimator.ChangeAnimationSpeed(1);
    }
}