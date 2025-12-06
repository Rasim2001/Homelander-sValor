using System.Linq;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using Player;
using UI.Windows.Tutorial;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class AccelerationTutorialState : ITutorialState
    {
        private readonly ITutorialProgressService _tutorialProgressService;
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly IGameWindowService _gameWindowService;
        private readonly IPlayerRegistryService _playerRegistryService;
        private readonly IStaticDataService _staticDataService;

        private TutorialWindow _forDeleteWindow;
        private PlayerMove _playerMove;

        public AccelerationTutorialState(ITutorialProgressService tutorialProgressService,
            ITutorialStateMachine tutorialStateMachine,
            IGameWindowService gameWindowService, IPlayerRegistryService playerRegistryService,
            IStaticDataService staticDataService)
        {
            _tutorialProgressService = tutorialProgressService;
            _tutorialStateMachine = tutorialStateMachine;
            _gameWindowService = gameWindowService;
            _playerRegistryService = playerRegistryService;
            _staticDataService = staticDataService;
        }

        public void Enter()
        {
            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.AccelerationEvent)
                .Value;

            _playerMove = _playerRegistryService.Player.GetComponent<PlayerMove>();

            _tutorialProgressService.ReadyToUseAcceleration = true;
            _forDeleteWindow = _gameWindowService.OpenAndGet(WindowId.TutorialWindow).GetComponent<TutorialWindow>();
            _forDeleteWindow.Initialize(text);
        }

        public void Exit()
        {
            Object.Destroy(_forDeleteWindow.gameObject);
        }

        public void Update()
        {
            if (_playerMove.AccelerationPressedWithMove)
                _tutorialStateMachine.ChangeState<MainFlagTutorialState>();
        }
    }
}