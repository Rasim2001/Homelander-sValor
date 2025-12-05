using System.Collections.Generic;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.Flag;
using Infastructure.Services.NearestBuildFind;
using Infastructure.Services.Tutorial;
using Infastructure.Services.Tutorial.States;
using Infastructure.Services.Window.GameWindowService;
using Player;
using Player.Orders;
using UnityEngine;
using Zenject;

namespace _Tutorial
{
    public class TutorialView : MonoBehaviour
    {
        [SerializeField] private TutorialArrowHelper _tutorialArrowHelper;
        [SerializeField] private PlayerFlip _playerFlip;
        [SerializeField] private ObserverTrigger _observerTrigger;
        [SerializeField] private PlayerMove _playerMove;
        [SerializeField] private PlayerInputOrders _playerInputOrders;
        [SerializeField] private Transform _unitsRecruiterTransform;

        [Header("MovementStateSettings")]
        [SerializeField] private GameObject _movementStateGameObject;
        [SerializeField] private Transform _movementTexTransform;

        [Header("ShiftHintsSettings")]
        [SerializeField] private GameObject _shiftGameObject;

        private ITutorialStateMachine _tutorialStateMachine;
        private ITutorialSpawnService _tutorialSpawnService;
        private IFlagTrackerService _flagTrackerService;
        private INearestBuildFindService _nearestBuildFindService;
        private ITutorialService _tutorialService;
        private IGameWindowService _gameWindowService;
        private IGameFactory _gameFactory;

        private List<ITutorialState> _states;

        [Inject]
        public void Construct(
            ITutorialSpawnService tutorialSpawnService,
            IFlagTrackerService flagTrackerService,
            INearestBuildFindService nearestBuildFindService,
            ITutorialService tutorialService,
            IGameWindowService gameWindowService,
            IGameFactory gameFactory)
        {
            _tutorialSpawnService = tutorialSpawnService;
            _flagTrackerService = flagTrackerService;
            _nearestBuildFindService = nearestBuildFindService;
            _tutorialService = tutorialService;
            _gameWindowService = gameWindowService;
            _gameFactory = gameFactory;
        }

        private void Awake()
        {
            _tutorialStateMachine = new TutorialStateMachine();

            _states = new List<ITutorialState>()
            {
                new MovementStateTutorial(
                    _tutorialStateMachine,
                    _playerMove,
                    _playerInputOrders,
                    _movementTexTransform,
                    _movementStateGameObject),

                new ChestStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _playerMove,
                    _playerInputOrders,
                    _observerTrigger,
                    _tutorialSpawnService.ChestObject),

                new BindHomelessStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _playerFlip,
                    _unitsRecruiterTransform,
                    _playerInputOrders,
                    _tutorialSpawnService.HomelessList),

                new BonfireStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _flagTrackerService,
                    _observerTrigger,
                    _playerInputOrders),

                new WorkshopStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _observerTrigger,
                    _playerInputOrders,
                    _flagTrackerService),

                new ProfessionStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _observerTrigger,
                    _playerInputOrders,
                    _flagTrackerService),

                new StoneStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _nearestBuildFindService,
                    _playerInputOrders,
                    _observerTrigger),

                new TreeStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _tutorialService,
                    _nearestBuildFindService,
                    _playerInputOrders,
                    _observerTrigger,
                    _shiftGameObject),

                new TowerStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _nearestBuildFindService,
                    _playerInputOrders,
                    _observerTrigger),

                new BindWarrioirsStateTutorial(
                    _tutorialStateMachine,
                    _tutorialArrowHelper,
                    _playerFlip,
                    _unitsRecruiterTransform,
                    _playerInputOrders,
                    _tutorialSpawnService.WarriorsList),

                new FightWithEnemyStateTutorial(
                    _tutorialStateMachine,
                    _playerMove,
                    _playerInputOrders,
                    _tutorialArrowHelper,
                    _tutorialSpawnService.FightPointObject,
                    _tutorialSpawnService.EnemyObject),

                new UnknowState(_gameWindowService, this, _tutorialArrowHelper, _gameFactory)
            };
        }


        public void Initialize() =>
            _tutorialStateMachine.Initialize(_states);

        private void Update() =>
            _tutorialStateMachine.Update();
    }
}