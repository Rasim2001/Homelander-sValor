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
    public abstract class MainFlagTutorialStateBase : ITutorialState
    {
        protected readonly ITutorialProgressService TutorialProgressService;
        protected readonly ITutorialStateMachine StateMachine;
        protected readonly ITutorialArrowDisplayer TutorialArrowDisplayer;
        protected readonly IGameWindowService GameWindowService;
        protected readonly IUpgradeMainFlag UpgradeMainFlag;
        protected readonly IStaticDataService StaticDataService;

        protected TutorialWindow Window;
        protected GameObject Dummy;

        protected MainFlagTutorialStateBase(
            ITutorialProgressService tutorialProgressService,
            ITutorialStateMachine stateMachine,
            ITutorialArrowDisplayer tutorialArrowDisplayer,
            IGameWindowService gameWindowService,
            IStaticDataService staticDataService,
            IUpgradeMainFlag upgradeMainFlag)
        {
            TutorialProgressService = tutorialProgressService;
            StateMachine = stateMachine;
            TutorialArrowDisplayer = tutorialArrowDisplayer;
            GameWindowService = gameWindowService;
            StaticDataService = staticDataService;
            UpgradeMainFlag = upgradeMainFlag;
        }

        public virtual void Enter()
        {
            UpgradeMainFlag.OnUpgradeHappened += OnUpgradeStarted;
            UpgradeMainFlag.OnUpgradeFinished += OnUpgradeFinished;

            Initialize();
            TutorialArrowDisplayer.Show(Dummy.transform);
        }

        public virtual void Exit()
        {
            UpgradeMainFlag.OnUpgradeHappened -= OnUpgradeStarted;
            UpgradeMainFlag.OnUpgradeFinished -= OnUpgradeFinished;

            Object.Destroy(Window.gameObject);
            Object.Destroy(Dummy);
        }

        public virtual void Update()
        {
        }


        protected virtual void Initialize()
        {
            TutorialStaticData data = StaticDataService.TutorialStaticData;

            string text = data.Infos
                .FirstOrDefault(x => x.Key == StartEvent)
                .Value;

            Vector3 position = data.TutorialObjects
                .FirstOrDefault(x => x.TypeId == TutorialObjectTypeId.MainFlag)
                .Position;

            Dummy = new GameObject("DeleteDummy");
            Dummy.transform.position = position;

            OnAfterDummyCreated(data);

            Window = GameWindowService.OpenAndGet(WindowId.TutorialWindow)
                .GetComponent<TutorialWindow>();
            Window.Initialize(text);
        }

        protected virtual void OnUpgradeStarted() =>
            TutorialArrowDisplayer.Hide(Dummy.transform);

        protected virtual void OnUpgradeFinished() =>
            DestroyAsync().Forget();

        protected virtual async UniTask DestroyAsync()
        {
            TutorialStaticData data = StaticDataService.TutorialStaticData;

            string text = data.Infos
                .FirstOrDefault(x => x.Key == FinishEvent)
                .Value;

            Window.Initialize(text);

            await UniTask.Delay(TimeSpan.FromSeconds(FinishDelaySeconds));

            StateMachine.ChangeState<BarricadeTutorialState>();
        }

        protected virtual void OnAfterDummyCreated(TutorialStaticData data)
        {
        }

        protected abstract TutorialEventData StartEvent { get; }
        protected abstract TutorialEventData FinishEvent { get; }
        protected abstract float FinishDelaySeconds { get; }
    }
}