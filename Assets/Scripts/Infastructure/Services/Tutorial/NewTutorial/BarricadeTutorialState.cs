using System;
using System.Collections.Generic;
using System.Linq;
using _Tutorial;
using _Tutorial.NewTutorial;
using Cysharp.Threading.Tasks;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using Player.Orders;
using UI.Windows.Tutorial;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class BarricadeTutorialState : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly IStaticDataService _staticDataService;
        private readonly ITutorialArrowDisplayer _tutorialArrowDisplayer;
        private readonly IGameWindowService _gameWindowService;
        private readonly ITutorialProgressService _tutorialProgressService;
        private readonly IBuilderCommandExecutor _builderCommandExecutor;

        private BarricadeTutorialWindow _forDeleteWindow;
        private GameObject _forDeleteDummy;

        private int _amountOfBarricades;
        private List<TutorialData> _tutorialDatas;

        public BarricadeTutorialState(
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

            _tutorialProgressService.IsBuildingStateReadyToUse = true;

            _tutorialDatas = _staticDataService.TutorialStaticData.TutorialObjects.Where(x =>
                x.TypeId == TutorialObjectTypeId.Barricade).ToList();

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
            _amountOfBarricades++;

            _tutorialArrowDisplayer.Hide(_forDeleteDummy.transform);

            if (_amountOfBarricades == 2)
                _stateMachine.ChangeState<CallNightTutorialState>();
        }

        private void BuildFinished()
        {
            InitializeArrow();

            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.BarricadeFinishBuildEvent).Value;

            string buildingText = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.BarricadeBuildingEvent).Value;

            text += $"<br>{buildingText} : {_amountOfBarricades}/2";

            _forDeleteWindow.Initialize(text);
        }

        private void InitializeText()
        {
            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.BarricadeStartBuildEvent).Value;

            _forDeleteWindow = _gameWindowService
                .OpenAndGet(WindowId.BarricadeTutorialWindow).GetComponent<BarricadeTutorialWindow>();
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