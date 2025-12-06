using System;
using System.Collections.Generic;
using _Tutorial.NewTutorial;
using Bonfire;
using CutScenes;
using Infastructure.Services.CallNight;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.Tutorial.NewTutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.StaticDataService;
using Player.Orders;
using Zenject;

namespace Infastructure.Services.Tutorial
{
    public class TutorialService : ITutorialService, ITickable, IDisposable
    {
        private readonly IGameWindowService _gameWindowService;
        private readonly IInputService _inputService;
        private readonly ICristalTimeline _cristalTimeline;
        private readonly ITutorialArrowDisplayer _arrowDisplayer;
        private readonly IPlayerRegistryService _playerRegistryService;
        private readonly IStaticDataService _staticDataService;
        private readonly IUpgradeMainFlag _upgradeMainFlag;
        private readonly IBuilderCommandExecutor _builderCommandExecutor;
        private readonly ICallNightService _callNightService;
        private readonly ITutorialProgressService _tutorialProgressService;
        private readonly IEnemyWavesService _enemyWavesService;

        private ITutorialStateMachine _tutorialStateMachine;

        public TutorialService(
            IGameWindowService gameWindowService,
            IInputService inputService,
            ICristalTimeline cristalTimeline,
            ITutorialArrowDisplayer arrowDisplayer,
            IPlayerRegistryService playerRegistryService,
            IStaticDataService staticDataService,
            IUpgradeMainFlag upgradeMainFlag,
            IBuilderCommandExecutor builderCommandExecutor,
            ICallNightService callNightService,
            ITutorialProgressService tutorialProgressService,
            IEnemyWavesService enemyWavesService)
        {
            _gameWindowService = gameWindowService;
            _inputService = inputService;
            _cristalTimeline = cristalTimeline;
            _arrowDisplayer = arrowDisplayer;
            _playerRegistryService = playerRegistryService;
            _staticDataService = staticDataService;
            _upgradeMainFlag = upgradeMainFlag;
            _builderCommandExecutor = builderCommandExecutor;
            _callNightService = callNightService;
            _tutorialProgressService = tutorialProgressService;
            _enemyWavesService = enemyWavesService;
        }

        public void Initialize()
        {
            SubscribeOnUpdates();
            InitializeStateMachine();
        }

        public void Dispose() =>
            _enemyWavesService.OnWaveCompleted -= WaveCompleted;

        private void SubscribeOnUpdates() =>
            _enemyWavesService.OnWaveCompleted += WaveCompleted;

        private void InitializeStateMachine()
        {
            _tutorialStateMachine = new TutorialStateMachine();

            List<ITutorialState> state = new List<ITutorialState>()
            {
                new MovementTutorialState(_tutorialStateMachine, _gameWindowService, _inputService, _staticDataService),
                new CutSceneTutorialState(_tutorialStateMachine, _cristalTimeline, _arrowDisplayer, _gameWindowService,
                    _staticDataService),
                new AccelerationTutorialState(_tutorialProgressService, _tutorialStateMachine, _gameWindowService,
                    _playerRegistryService, _staticDataService),
                new MainFlagTutorialState(_tutorialProgressService, _tutorialStateMachine, _arrowDisplayer,
                    _gameWindowService, _staticDataService, _upgradeMainFlag),
                new MainFlagSecondTutorialState(_tutorialProgressService, _tutorialStateMachine, _arrowDisplayer,
                    _gameWindowService, _staticDataService, _upgradeMainFlag),
                new MainFlagThirdTutorialState(_tutorialProgressService, _tutorialStateMachine, _arrowDisplayer,
                    _gameWindowService, _staticDataService, _upgradeMainFlag),
                new BarricadeTutorialState(_tutorialProgressService, _tutorialStateMachine, _staticDataService,
                    _arrowDisplayer, _gameWindowService, _builderCommandExecutor),
                new CallNightTutorialState(_tutorialProgressService, _tutorialStateMachine, _gameWindowService,
                    _callNightService, _staticDataService),
                new AttackTutorialState(_tutorialProgressService, _tutorialStateMachine, _inputService,
                    _staticDataService, _gameWindowService),
                new TowerTutorialState(_tutorialProgressService, _tutorialStateMachine, _staticDataService,
                    _arrowDisplayer, _gameWindowService, _builderCommandExecutor),
                new UnknownTutorialState()
            };

            _tutorialStateMachine.Initialize(state);
        }

        public void ChangeState<TState>() where TState : ITutorialState =>
            _tutorialStateMachine.ChangeState<TState>();

        public void Tick() =>
            _tutorialStateMachine?.Update();

        private void WaveCompleted(int waveId)
        {
            if (waveId == 0)
                ChangeState<MainFlagSecondTutorialState>();
            else if (waveId == 1)
                ChangeState<MainFlagThirdTutorialState>();
        }
    }
}