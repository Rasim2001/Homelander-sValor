using Infastructure.Services.SafeBuildZoneTracker;
using Units.Animators;

namespace Units.UnitStates.VagabondStates
{
    public class FearVagabondState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly VagabondAnimator _unitAnimator;

        public FearVagabondState(IUnitStateMachine unitStateMachine, ISafeBuildZone safeBuildZone,
            VagabondAnimator unitAnimator)
        {
            _unitStateMachine = unitStateMachine;
            _safeBuildZone = safeBuildZone;
            _unitAnimator = unitAnimator;
        }

        public void Enter() =>
            _unitAnimator.PlayFearAnimation(true);

        public void Update()
        {
            if (_safeBuildZone.IsNight)
                return;

            _unitStateMachine.ChangeState<WalkVagabondState>();
        }

        public void Exit() =>
            _unitAnimator.PlayFearAnimation(false);
    }
}