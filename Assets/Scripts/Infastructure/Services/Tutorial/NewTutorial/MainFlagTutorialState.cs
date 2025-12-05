using System;
using System.Linq;
using _Tutorial;
using _Tutorial.NewTutorial;
using Bonfire;
using Cysharp.Threading.Tasks;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Tutorial;
using Infastructure.StaticData.Windows;
using UI.Windows.Tutorial;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infastructure.Services.Tutorial.NewTutorial
{
    public class MainFlagTutorialState : ITutorialState
    {
        private readonly ITutorialProgressService _tutorialProgressService;
        private readonly ITutorialStateMachine _stateMachine;
        private readonly ITutorialArrowDisplayer _tutorialArrowDisplayer;
        private readonly IGameWindowService _gameWindowService;
        private readonly IUpgradeMainFlag _upgradeMainFlag;
        private readonly IStaticDataService _staticDataService;

        private MainFlagTutorialWindow _forDeleteWindow;
        private GameObject _forDeleteDummy;


        public MainFlagTutorialState(ITutorialProgressService tutorialProgressService,
            ITutorialStateMachine stateMachine,
            ITutorialArrowDisplayer tutorialArrowDisplayer, IGameWindowService gameWindowService,
            IStaticDataService staticDataService, IUpgradeMainFlag upgradeMainFlag)
        {
            _staticDataService = staticDataService;
            _upgradeMainFlag = upgradeMainFlag;
            _tutorialProgressService = tutorialProgressService;
            _stateMachine = stateMachine;
            _tutorialArrowDisplayer = tutorialArrowDisplayer;
            _gameWindowService = gameWindowService;
        }

        public void Enter()
        {
            _upgradeMainFlag.OnUpgradeHappened += UpgradeMainFlagStarted;
            _upgradeMainFlag.OnUpgradeFinished += UpgradeMainFlagFinished;

            Initialize();

            _tutorialArrowDisplayer.Show(_forDeleteDummy.transform);
        }

        public void Exit()
        {
            _upgradeMainFlag.OnUpgradeHappened -= UpgradeMainFlagStarted;
            _upgradeMainFlag.OnUpgradeFinished -= UpgradeMainFlagFinished;

            Object.Destroy(_forDeleteWindow.gameObject);
            Object.Destroy(_forDeleteDummy);
        }

        public void Update()
        {
        }

        private void Initialize()
        {
            TutorialStaticData tutorialStaticData = _staticDataService.TutorialStaticData;
            string text = tutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.MainFlagStartBuildEvent)
                .Value;

            Vector3 position = tutorialStaticData.TutorialObjects.FirstOrDefault(x =>
                x.TypeId == TutorialObjectTypeId.MainFlag).Position;

            _forDeleteDummy = new GameObject("DeleteDummy");
            _forDeleteDummy.transform.position = position;

            _tutorialProgressService.IsGiveOrderReadyToUse = true;
            _forDeleteWindow = _gameWindowService.OpenAndGet(WindowId.MainFlagTutorialWindow)
                .GetComponent<MainFlagTutorialWindow>();
            _forDeleteWindow.Initialize(text);
        }


        private void UpgradeMainFlagStarted() =>
            _tutorialArrowDisplayer.Hide(_forDeleteDummy.transform);

        private void UpgradeMainFlagFinished() =>
            DestroyAsync().Forget();

        private async UniTask DestroyAsync()
        {
            TutorialStaticData tutorialStaticData = _staticDataService.TutorialStaticData;

            string text = tutorialStaticData.Infos
                .FirstOrDefault(x => x.Key == TutorialEventData.MainFlagFinishBuildEvent)
                .Value;

            _forDeleteWindow.Initialize(text);

            await UniTask.Delay(TimeSpan.FromSeconds(4));

            _stateMachine.ChangeState<BarricadeTutorialState>();
        }
    }
}