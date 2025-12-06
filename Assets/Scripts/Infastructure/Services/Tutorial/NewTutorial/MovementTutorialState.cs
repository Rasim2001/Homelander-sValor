using System.Linq;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.Window;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using UI.Windows.Tutorial;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class MovementTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly IGameWindowService _windowService;
        private readonly IInputService _inputService;
        private readonly IStaticDataService _staticDataService;

        private TutorialWindow _forDeleteWindow;

        public MovementTutorialState(ITutorialStateMachine stateMachine, IGameWindowService windowService,
            IInputService inputService, IStaticDataService staticDataService)
        {
            _stateMachine = stateMachine;
            _windowService = windowService;
            _inputService = inputService;
            _staticDataService = staticDataService;
        }

        public void Enter()
        {
            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.MovementEvent)
                .Value;

            _forDeleteWindow = _windowService.OpenAndGet(WindowId.TutorialWindow).GetComponent<TutorialWindow>();
            _forDeleteWindow.Initialize(text);
        }

        public void Update()
        {
            if (_inputService.MoveKeysPressed)
                _stateMachine.ChangeState<CutSceneTutorialState>();
        }

        public void Exit() =>
            Object.Destroy(_forDeleteWindow.gameObject);
    }
}