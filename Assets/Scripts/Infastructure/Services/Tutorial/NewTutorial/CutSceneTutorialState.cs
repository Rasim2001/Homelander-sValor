using System.Linq;
using _Tutorial.NewTutorial;
using CutScenes;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using UI.Windows.Tutorial;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class CutSceneTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly ICristalTimeline _cristalTimeline;
        private readonly ITutorialArrowDisplayer _tutorialArrowDisplayer;
        private readonly IGameWindowService _windowService;
        private readonly IStaticDataService _staticDataService;

        private TutorialWindow _forDeleteWindow;

        public CutSceneTutorialState(ITutorialStateMachine stateMachine, ICristalTimeline cristalTimeline,
            ITutorialArrowDisplayer tutorialArrowDisplayer,
            IGameWindowService windowService, IStaticDataService staticDataService)
        {
            _stateMachine = stateMachine;
            _cristalTimeline = cristalTimeline;
            _tutorialArrowDisplayer = tutorialArrowDisplayer;
            _windowService = windowService;
            _staticDataService = staticDataService;
        }

        public void Enter()
        {
            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.CutSceneEvent)
                .Value;

            _cristalTimeline.OnPlayFinishHappened += FinishCutscene;
            _cristalTimeline.OnPlayStartHappened += StartCutScene;

            _forDeleteWindow = _windowService.OpenAndGet(WindowId.TutorialWindow).GetComponent<TutorialWindow>();
            _forDeleteWindow.Initialize(text);

            _tutorialArrowDisplayer.Show(_cristalTimeline.CristalTransform);
        }

        public void Exit()
        {
            _cristalTimeline.OnPlayFinishHappened -= FinishCutscene;
            _cristalTimeline.OnPlayStartHappened -= StartCutScene;
        }

        public void Update()
        {
        }

        private void FinishCutscene() =>
            _stateMachine.ChangeState<AccelerationTutorialState>();

        private void StartCutScene()
        {
            _tutorialArrowDisplayer.Hide(_cristalTimeline.CristalTransform);
            Object.Destroy(_forDeleteWindow.gameObject);
        }
    }
}