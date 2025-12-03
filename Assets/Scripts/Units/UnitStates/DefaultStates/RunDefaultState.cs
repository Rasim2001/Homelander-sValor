using Units.Animators;

namespace Units.UnitStates.DefaultStates
{
    public class RunDefaultState : IUnitState
    {
        private readonly UnitAnimator _unitAnimator;

        public RunDefaultState(UnitAnimator unitAnimator) =>
            _unitAnimator = unitAnimator;

        public void Enter() =>
            _unitAnimator.SetRunAnimation(true);

        public void Update()
        {
        }

        public void Exit() =>
            _unitAnimator.SetRunAnimation(false);
    }
}