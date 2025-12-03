using System;
using Infastructure.Services.Flag;
using Units.UnitStates.BuilderStates;
using Zenject;

namespace Units.UnitStates.StateMachineViews
{
    public class BuilderStateMachineView : UnitStateMachineView
    {
        private IFlagTrackerService _flagTrackerService;
        private SpeachBubleOrderUpdater _speachBubleOrderUpdater;

        [Inject]
        public void Construct(IFlagTrackerService flagTrackerService) =>
            _flagTrackerService = flagTrackerService;

        private void Awake() =>
            _speachBubleOrderUpdater = GetComponentInChildren<SpeachBubleOrderUpdater>();

        public override void Initialize()
        {
            base.Initialize();

            AddToCurrentStates(
                new BuilderRetreatState(
                    StateMachine,
                    _flagTrackerService,
                    UnitMove,
                    UnitStatus,
                    UnitAnimator,
                    UnitData));


            AddToCurrentStates(
                new BuilderScaryRunState(
                    StateMachine,
                    StaticDataService,
                    _speachBubleOrderUpdater,
                    UnitMove,
                    UnitAnimator,
                    UnitData));
        }
    }
}