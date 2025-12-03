using Units.Animators;
using UnityEngine;

namespace Units.UnitStates.DefaultStates
{
    public class IdleDefaultState : IUnitState
    {
        private readonly UnitAnimator _unitAnimator;

        public IdleDefaultState(UnitAnimator unitAnimator) =>
            _unitAnimator = unitAnimator;

        public void Enter() =>
            _unitAnimator.SetIdleAnimation(true);

        public void Update()
        {
            
        }

        public void Exit() =>
            _unitAnimator.SetIdleAnimation(false);
    }
}