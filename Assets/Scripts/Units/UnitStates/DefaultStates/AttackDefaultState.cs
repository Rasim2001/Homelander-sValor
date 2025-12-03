using Units.Animators;

namespace Units.UnitStates.DefaultStates
{
    public class AttackDefaultState : IUnitState
    {
        private readonly IAttackAnimator _attackAnimator;

        public AttackDefaultState(IAttackAnimator attackAnimator) =>
            _attackAnimator = attackAnimator;

        public void Enter() =>
            _attackAnimator.PlayAttackAnimation(true);

        public void Update()
        {
            
        }

        public void Exit() =>
            _attackAnimator.PlayAttackAnimation(false);
    }
}