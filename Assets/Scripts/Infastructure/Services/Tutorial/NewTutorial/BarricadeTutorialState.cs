using System;
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

            InitializeArrow();
            InitializeText();
        }

        public void Exit()
        {
            _builderCommandExecutor.OnBuildFinished -= BuildFinished;
            _builderCommandExecutor.OnBuildStarted -= StartBuild;
        }

        public void Update()
        {
        }

        private void StartBuild()
        {
            _amountOfBarricades++;

            if (_amountOfBarricades == 2)
                _stateMachine.ChangeState<CallNightTutorialState>();
            else
                _tutorialArrowDisplayer.Hide(_forDeleteDummy.transform);
        }

        private void BuildFinished() =>
            DestroyAsync().Forget();

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
            TutorialData tutorialData = _staticDataService.TutorialStaticData.TutorialObjects.FirstOrDefault(x =>
                x.TypeId == TutorialObjectTypeId.Barricade);

            _forDeleteDummy = new GameObject("DeleteDummy");
            _forDeleteDummy.transform.position = tutorialData.Position;

            _tutorialArrowDisplayer.Show(_forDeleteDummy.transform);
        }

        private async UniTask DestroyAsync()
        {
            string text = _staticDataService.TutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.BarricadeFinishBuildEvent).Value;

            _forDeleteWindow.Initialize(text);

            await UniTask.Delay(TimeSpan.FromSeconds(4));

            Object.Destroy(_forDeleteDummy);
            Object.Destroy(_forDeleteWindow.gameObject);
        }
    }
}