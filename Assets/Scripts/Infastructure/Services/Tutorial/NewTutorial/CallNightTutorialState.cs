using System.Linq;
using Infastructure.Services.CallNight;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using UI.Windows.Tutorial;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class CallNightTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly IGameWindowService _gameWindowService;
        private readonly ICallNightService _callNightService;
        private readonly IStaticDataService _staticDataService;
        private readonly ITutorialProgressService _tutorialProgressService;

        private TutorialWindow _forDeleteWindow;

        public CallNightTutorialState(ITutorialProgressService tutorialProgressService,
            ITutorialStateMachine stateMachine,
            IGameWindowService gameWindowService, ICallNightService callNightService,
            IStaticDataService staticDataService)
        {
            _tutorialProgressService = tutorialProgressService;
            _stateMachine = stateMachine;
            _gameWindowService = gameWindowService;
            _callNightService = callNightService;
            _staticDataService = staticDataService;
        }

        public void Enter()
        {
            _callNightService.OnCallNightHappened += OnCallNight;

            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.CallNightEvent)
                .Value;

            _tutorialProgressService.IsCallingNightReadyToUse = true;
            _forDeleteWindow = _gameWindowService.OpenAndGet(WindowId.TutorialWindow).GetComponent<TutorialWindow>();
            _forDeleteWindow.Initialize(text);
        }

        private void OnCallNight() =>
            _stateMachine.ChangeState<UnknownTutorialState>();

        public void Exit()
        {
            Object.Destroy(_forDeleteWindow.gameObject);

            _callNightService.OnCallNightHappened -= OnCallNight;
        }

        public void Update()
        {
        }
    }
}