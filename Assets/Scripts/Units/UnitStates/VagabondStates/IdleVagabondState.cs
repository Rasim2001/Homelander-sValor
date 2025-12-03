using Infastructure.Services.SafeBuildZoneTracker;
using Units.Animators;
using UnityEngine;

namespace Units.UnitStates.VagabondStates
{
    public class IdleVagabondState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly UnitAnimator _unitAnimator;

        private float _idleTime;

        public IdleVagabondState(IUnitStateMachine unitStateMachine, ISafeBuildZone safeBuildZone,
            UnitAnimator unitAnimator)
        {
            _unitStateMachine = unitStateMachine;
            _safeBuildZone = safeBuildZone;
            _unitAnimator = unitAnimator;
        }

        public void Enter()
        {
            _idleTime = Random.Range(2f, 5f);

            _unitAnimator.SetIdleAnimation(true);
        }

        public void Exit() =>
            _unitAnimator.SetIdleAnimation(false);

        public void Update()
        {
            if (_safeBuildZone.IsNight)
                _unitStateMachine.ChangeState<FearVagabondState>();

            if (_idleTime > 0)
                _idleTime -= Time.deltaTime;
            else
                _unitStateMachine.ChangeState<WalkVagabondState>();
        }
    }
}