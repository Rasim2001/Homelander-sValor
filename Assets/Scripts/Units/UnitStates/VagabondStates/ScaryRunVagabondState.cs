using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.Vagabond;

namespace Units.UnitStates.VagabondStates
{
    public class ScaryRunVagabondState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly VagabondAnimator _unitAnimator;
        private readonly VagabondMove _vagabondMove;
        private readonly UnitStaticData _unitData;

        public ScaryRunVagabondState(
            IUnitStateMachine unitStateMachine,
            VagabondAnimator unitAnimator,
            VagabondMove vagabondMove,
            UnitStaticData unitData)
        {
            _unitStateMachine = unitStateMachine;
            _unitAnimator = unitAnimator;
            _vagabondMove = vagabondMove;
            _unitData = unitData;
        }

        public void Enter()
        {
            _vagabondMove.ChangeTargetPosition();
            _vagabondMove.SetSpeed(_unitData.RunSpeed);

            _unitAnimator.PlayScaryRunAnimation(true);
        }

        public void Update()
        {
            if (_vagabondMove.IsPathReached())
                _unitStateMachine.ChangeState<FearVagabondState>();
            else
                _vagabondMove.Move();
        }

        public void Exit() =>
            _unitAnimator.PlayScaryRunAnimation(false);
    }
}