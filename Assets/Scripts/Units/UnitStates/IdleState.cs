using Units.Animators;
using UnityEngine;

namespace Units.UnitStates
{
    public class IdleState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly UnitAnimator _unitAnimator;

        private float _idleTime;

        public IdleState(IUnitStateMachine unitStateMachine, UnitAnimator unitAnimator)
        {
            _unitStateMachine = unitStateMachine;
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
            if (_idleTime > 0)
                _idleTime -= Time.deltaTime;
            else
                _unitStateMachine.ChangeState<WalkState>();
        }
    }
}