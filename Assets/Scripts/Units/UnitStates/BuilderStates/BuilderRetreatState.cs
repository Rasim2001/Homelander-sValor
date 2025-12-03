using Flag;
using Infastructure.Services.Flag;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.UnitStatusManagement;

namespace Units.UnitStates.BuilderStates
{
    public class BuilderRetreatState : IUnitState
    {
        private readonly UnitMove _unitMove;
        private readonly UnitStatus _unitStatus;
        private readonly UnitAnimator _unitAnimator;
        private readonly UnitStaticData _unitData;
        private readonly IUnitStateMachine _unitStateMachine;
        private readonly IFlagTrackerService _flagTrackerService;
        private FlagSlotCoordinator _lastFlagSlotCoordinator;

        public BuilderRetreatState(IUnitStateMachine unitStateMachine,
            IFlagTrackerService flagTrackerService,
            UnitMove unitMove,
            UnitStatus unitStatus,
            UnitAnimator unitAnimator,
            UnitStaticData unitData)
        {
            _unitMove = unitMove;
            _unitStatus = unitStatus;
            _unitAnimator = unitAnimator;
            _unitData = unitData;
            _unitStateMachine = unitStateMachine;
            _flagTrackerService = flagTrackerService;
        }


        public void Enter()
        {
            _unitMove.ChangeTargetPosition();
            _unitMove.enabled = true;
            _unitMove.SetSpeed(_unitData.RetreatSpeed);

            _unitAnimator.ResetAllAnimations();

            if (!_flagTrackerService.LastFlagIsMainFlag(_unitMove.transform.position.x > 0))
            {
                _lastFlagSlotCoordinator = _flagTrackerService.GetLastFlag(_unitMove.transform.position.x > 0)
                    .GetComponent<FlagSlotCoordinator>();

                _lastFlagSlotCoordinator.BindUnitToSlot(_unitStatus.transform, _unitStatus.UnitTypeId);
                _lastFlagSlotCoordinator.RelocateUnits();
            }
            else
                _unitStateMachine.ChangeState<RunState>();
        }

        public void Update()
        {
        }

        public void Exit()
        {
            _unitStatus.IsDefensedFlag = false;
            _unitStatus.IsWorked = false;
        }
    }
}