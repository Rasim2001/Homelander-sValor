using Infastructure.Services.CallNight;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Windows;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class CallNightTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly IGameWindowService _gameWindowService;
        private readonly ICallNightService _callNightService;
        private readonly ITutorialProgressService _tutorialProgressService;

        private GameObject _forDeleteWindow;

        public CallNightTutorialState(ITutorialProgressService tutorialProgressService, ITutorialStateMachine stateMachine,
            IGameWindowService gameWindowService, ICallNightService callNightService)
        {
            _tutorialProgressService = tutorialProgressService;
            _stateMachine = stateMachine;
            _gameWindowService = gameWindowService;
            _callNightService = callNightService;
        }

        public void Enter()
        {
            _callNightService.OnCallNightHappened += OnCallNight;

            _tutorialProgressService.IsCallingNightReadyToUse = true;
            _forDeleteWindow = _gameWindowService.OpenAndGet(WindowId.CallingNightTutorialWindow);
        }

        private void OnCallNight() =>
            _stateMachine.ChangeState<UnknownTutorialState>();

        public void Exit()
        {
            Object.Destroy(_forDeleteWindow);

            _callNightService.OnCallNightHappened -= OnCallNight;
        }

        public void Update()
        {
        }
    }
}