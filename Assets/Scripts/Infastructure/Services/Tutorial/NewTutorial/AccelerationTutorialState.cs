using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Windows;
using Player;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class AccelerationTutorialState : ITutorialState
    {
        private readonly ITutorialProgressService _tutorialProgressService;
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly IGameWindowService _gameWindowService;
        private readonly IPlayerRegistryService _playerRegistryService;

        private GameObject _forDeleteWindow;
        private PlayerMove _playerMove;

        public AccelerationTutorialState(ITutorialProgressService tutorialProgressService,
            ITutorialStateMachine tutorialStateMachine,
            IGameWindowService gameWindowService, IPlayerRegistryService playerRegistryService)
        {
            _tutorialProgressService = tutorialProgressService;
            _tutorialStateMachine = tutorialStateMachine;
            _gameWindowService = gameWindowService;
            _playerRegistryService = playerRegistryService;
        }

        public void Enter()
        {
            _playerMove = _playerRegistryService.Player.GetComponent<PlayerMove>();

            _tutorialProgressService.ReadyToUseAcceleration = true;
            _forDeleteWindow = _gameWindowService.OpenAndGet(WindowId.AccelerationTutorialWindow);
        }

        public void Exit()
        {
            Object.Destroy(_forDeleteWindow);
        }

        public void Update()
        {
            if (_playerMove.AccelerationPressedWithMove)
                _tutorialStateMachine.ChangeState<MainFlagTutorialState>();
        }
    }
}