using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.StaticData.SpeachBuble.Units;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Player.Orders;
using Units.Animators;

namespace Units.UnitStates.BuilderStates
{
    public class BuilderScaryRunState : IUnitState
    {
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly IStaticDataService _staticDataService;

        private readonly SpeachBubleOrderUpdater _speachBubleUpdater;
        private readonly UnitMove _unitMove;
        private readonly UnitAnimator _unitAnimator;
        private readonly UnitStaticData _unitStaticData;

        public BuilderScaryRunState(
            IUnitStateMachine unitStateMachine,
            IStaticDataService staticDataService,
            SpeachBubleOrderUpdater speachBubleUpdater,
            UnitMove unitMove,
            UnitAnimator unitAnimator,
            UnitStaticData unitStaticData)
        {
            _unitStateMachine = unitStateMachine;
            _staticDataService = staticDataService;
            _speachBubleUpdater = speachBubleUpdater;
            _unitMove = unitMove;
            _unitAnimator = unitAnimator;
            _unitStaticData = unitStaticData;
        }

        public void Enter()
        {
            SpeachBubleOrderConfig orderConfig = _staticDataService.ForSpeachBuble(OrderID.ScaryRun);
            _speachBubleUpdater.UpdateSpeachBuble(orderConfig.Sprite);

            ChangeSpeedAnimation();

            _unitMove.ChangeTargetPosition();
            _unitMove.SetSpeed(_unitStaticData.RunSpeed);

            _unitAnimator.SetRunAnimation(true);
        }

        public void Exit()
        {
            _speachBubleUpdater.DisableSpeachBuble();
            _unitAnimator.SetRunAnimation(false);
        }

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