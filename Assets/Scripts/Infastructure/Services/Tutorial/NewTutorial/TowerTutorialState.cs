using System.Collections.Generic;
using System.Linq;
using _Tutorial;
using _Tutorial.NewTutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using Player.Orders;
using UI.Windows.Tutorial;
using UnityEngine;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class TowerTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly IStaticDataService _staticDataService;
        private readonly ITutorialArrowDisplayer _tutorialArrowDisplayer;
        private readonly IGameWindowService _gameWindowService;
        private readonly ITutorialProgressService _tutorialProgressService;
        private readonly IBuilderCommandExecutor _builderCommandExecutor;

        private TutorialWindow _forDeleteWindow;
        private GameObject _forDeleteDummy;

        private int _amountOfTowers;
        private List<TutorialData> _tutorialDatas;

        public TowerTutorialState(
            ITutorialProgressService tutorialProgressService,
            ITutorialStateMachine stateMachine,
            IStaticDataService staticDataService,
            ITutorialArrowDisplayer tutorialArrowDisplayer,
            IGameWindowService gameWindowService,
            IBuilderCommandExecutor builderCommandExecutor)
        {
            _tutorialProgressService = tutorialProgressService;
            _builderCommandExecutor = builderCommandExecutor;
            _stateMachine = stateMachine;
            _staticDataService = staticDataService;
            _tutorialArrowDisplayer = tutorialArrowDisplayer;
            _gameWindowService = gameWindowService;
        }

        public void Enter()
        {
            _builderCommandExecutor.OnBuildFinished += BuildFinished;
            _builderCommandExecutor.OnBuildStarted += StartBuild;

            _tutorialDatas = _staticDataService.TutorialStaticData.TutorialObjects.Where(x =>
                x.TypeId == TutorialObjectTypeId.Tower).ToList();

            InitializeArrow();
            InitializeText();
        }

        public void Exit()
        {
            Object.Destroy(_forDeleteDummy);
            Object.Destroy(_forDeleteWindow.gameObject);

            _builderCommandExecutor.OnBuildFinished -= BuildFinished;
            _builderCommandExecutor.OnBuildStarted -= StartBuild;
        }

        public void Update()
        {
        }

        private void StartBuild()
        {
            _amountOfTowers++;

            _tutorialArrowDisplayer.Hide(_forDeleteDummy.transform);

            if (_amountOfTowers == 2)
                _stateMachine.ChangeState<CallNightTutorialState>();
        }

        private void BuildFinished()
        {
            InitializeArrow();

            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.TowerFinishBuildEvent).Value;

            string buildingText = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.BuildingEvent).Value;

            text += $"<br>{buildingText} : {_amountOfTowers}/2";

            _forDeleteWindow.Initialize(text);
        }

        private void InitializeText()
        {
            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.TowerStartBuildEvent).Value;

            _forDeleteWindow = _gameWindowService
                .OpenAndGet(WindowId.TutorialWindow).GetComponent<TutorialWindow>();
            _forDeleteWindow.Initialize(text);
        }

        private void InitializeArrow()
        {
            TutorialData tutorialData = _tutorialDatas.FirstOrDefault();
            if (tutorialData == null)
                return;

            _tutorialDatas.Remove(tutorialData);

            _forDeleteDummy = new GameObject("DeleteDummy");
            _forDeleteDummy.transform.position = tutorialData.Position;

            _tutorialArrowDisplayer.Show(_forDeleteDummy.transform);
        }
    }
}