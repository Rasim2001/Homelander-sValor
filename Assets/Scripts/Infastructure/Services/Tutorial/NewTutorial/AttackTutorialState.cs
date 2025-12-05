using System.Linq;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using UI.Windows.Tutorial;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class AttackTutorialState : ITutorialState
    {
        private readonly ITutorialProgressService _tutorialProgressService;
        private readonly ITutorialStateMachine _stateMachine;
        private readonly IInputService _inputService;
        private readonly IStaticDataService _staticDataService;
        private readonly IGameWindowService _gameWindowService;

        private GameObject _forDeleteWindow;

        public AttackTutorialState(ITutorialProgressService tutorialProgressService, ITutorialStateMachine stateMachine,
            IInputService inputService, IStaticDataService staticDataService, IGameWindowService gameWindowService)
        {
            _tutorialProgressService = tutorialProgressService;
            _stateMachine = stateMachine;
            _inputService = inputService;
            _staticDataService = staticDataService;
            _gameWindowService = gameWindowService;
        }

        public void Enter()
        {
            _tutorialProgressService.IsAttackReadyToUse = true;

            _forDeleteWindow = _gameWindowService.OpenAndGet(WindowId.AttackTutorialWindow);

            AttackTutorialWindow attackTutorialWindow = _forDeleteWindow.GetComponent<AttackTutorialWindow>();

            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.AttackStartEvent).Value;

            attackTutorialWindow.Initialize(text);
        }

        public void Exit() =>
            Object.Destroy(_forDeleteWindow);

        public void Update()
        {
            if (_inputService.ShootPressedDown)
                _stateMachine.ChangeState<UnknownTutorialState>();
        }
    }
}