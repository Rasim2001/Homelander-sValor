using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.Window;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Windows;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class MovementTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly IGameWindowService _windowService;
        private readonly IInputService _inputService;

        private GameObject _forDeleteWindow;

        public MovementTutorialState(ITutorialStateMachine stateMachine, IGameWindowService windowService,
            IInputService inputService)
        {
            _stateMachine = stateMachine;
            _windowService = windowService;
            _inputService = inputService;
        }

        public void Enter() =>
            _forDeleteWindow = _windowService.OpenAndGet(WindowId.MovementTutorialWindow);

        public void Update()
        {
            if (_inputService.MoveKeysPressed)
                _stateMachine.ChangeState<CutSceneTutorialState>();
        }

        public void Exit() =>
            Object.Destroy(_forDeleteWindow);
    }
}