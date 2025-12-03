using _Tutorial;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Unit;
using Infastructure.StaticData.Windows;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using UnityEngine;

namespace Infastructure.Services.Tutorial.States
{
    public class UnknowState : ITutorialState
    {
        private readonly IGameWindowService _gameWindowService;
        private readonly TutorialView _tutorialView;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly IGameFactory _gameFactory;

        public UnknowState(IGameWindowService gameWindowService, TutorialView tutorialView,
            TutorialArrowHelper tutorialArrowHelper, IGameFactory gameFactory)
        {
            _gameWindowService = gameWindowService;
            _tutorialView = tutorialView;
            _tutorialArrowHelper = tutorialArrowHelper;
            _gameFactory = gameFactory;
        }

        public void Enter()
        {
            _tutorialView.gameObject.SetActive(false);
            _tutorialArrowHelper.gameObject.SetActive(false);
            _gameWindowService.Open(WindowId.FinishTutorialWindow);

            for (int i = 0; i < 8; i++)
            {
                GameObject homelessObject = _gameFactory.CreateUnit(UnitTypeId.Homeless);

                UnitStateMachineView unitStateMachineView = homelessObject.GetComponent<UnitStateMachineView>();
                unitStateMachineView.ChangeState<WalkState>();
            }
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}