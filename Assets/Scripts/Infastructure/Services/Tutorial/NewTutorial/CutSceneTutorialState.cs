using _Tutorial.NewTutorial;
using CutScenes;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Windows;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class CutSceneTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly ICristalTimeline _cristalTimeline;
        private readonly ITutorialArrowDisplayer _tutorialArrowDisplayer;
        private readonly IGameWindowService _windowService;

        private GameObject _forDeleteWindow;

        public CutSceneTutorialState(
            ITutorialStateMachine stateMachine, ICristalTimeline cristalTimeline,
            ITutorialArrowDisplayer tutorialArrowDisplayer,
            IGameWindowService windowService)
        {
            _stateMachine = stateMachine;
            _cristalTimeline = cristalTimeline;
            _tutorialArrowDisplayer = tutorialArrowDisplayer;
            _windowService = windowService;
        }

        public void Enter()
        {
            _cristalTimeline.OnPlayFinishHappened += FinishCutscene;
            _cristalTimeline.OnPlayStartHappened += StartCutScene;

            _forDeleteWindow = _windowService.OpenAndGet(WindowId.CutsceneTutorialWindow);
            
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
            Object.Destroy(_forDeleteWindow);
        }
    }
}