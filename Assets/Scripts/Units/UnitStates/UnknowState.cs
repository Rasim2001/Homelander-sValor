using UnityEngine;

namespace Units.UnitStates
{
    public class UnknowState : IUnitState
    {
        private readonly Animator _animator;

        public UnknowState(Animator animator) =>
            _animator = animator;

        public void Enter()
        {
        }

        public void Update()
        {
        }

        public void Exit()
        {
        }
    }
}