using _Tutorial.NewTutorial;
using Bonfire;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class MainFlagSecondTutorialState : MainFlagTutorialStateBase
    {
        public MainFlagSecondTutorialState(
            ITutorialProgressService tutorialProgressService,
            ITutorialStateMachine stateMachine,
            ITutorialArrowDisplayer tutorialArrowDisplayer,
            IGameWindowService gameWindowService,
            IStaticDataService staticDataService,
            IUpgradeMainFlag upgradeMainFlag)
            : base(tutorialProgressService, stateMachine, tutorialArrowDisplayer,
                gameWindowService, staticDataService, upgradeMainFlag)
        {
        }

        protected override TutorialEventData StartEvent =>
            TutorialEventData.MainFlagSecondStartBuildEvent;

        protected override TutorialEventData FinishEvent =>
            TutorialEventData.MainFlagSecondFinishBuildEvent;

        protected override float FinishDelaySeconds => 2f;

        protected override void OnUpgradeFinished() =>
            StateMachine.ChangeState<UnknownTutorialState>();
    }
}