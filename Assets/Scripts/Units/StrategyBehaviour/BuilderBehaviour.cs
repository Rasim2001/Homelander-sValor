using System;
using FogOfWar;
using Grid;
using Infastructure;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.BuildingRegistry;
using Infastructure.Services.Fence;
using Infastructure.Services.MarkerSignCoordinator;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.Pool;
using Infastructure.StaticData.StaticDataService;
using Loots;
using Player.Orders;
using Units.Animators;
using Units.StrategyBehaviour.BuildManagement;
using Units.StrategyBehaviour.ChopManagement;
using Units.StrategyBehaviour.DigManagement;
using Units.StrategyBehaviour.HealManagement;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using UnityEngine;
using Zenject;

namespace Units.StrategyBehaviour
{
    public class BuilderBehaviour : UnitStrategyBehaviour
    {
        [SerializeField] private UnitStateMachineView _unitStateMachineView;
        [SerializeField] private BuilderActionHandler _builderActionHandler;

        private IBuild _build;
        private IChopWood _chopWood;
        private IDig _dig;
        private IHealBuilding _heal;

        private ICoroutineRunner _coroutineRunner;
        private IPoolObjects<CoinLoot> _poolLoot;
        private IMarkerSignCoordinatorService _markerSignService;
        private IPersistentProgressService _progressService;
        private IHomelessOrdersService _homelessOrdersService;
        private IStaticDataService _staticDataService;
        private IFogOfWarMinimap _fogOfWarMinimap;
        private IGridMap _gridMap;
        private IBuildingRegistryService _buildingRegistryService;
        private IFenceService _fenceService;


        [Inject]
        public void Construct(
            ICoroutineRunner coroutineRunner,
            IPoolObjects<CoinLoot> poolLoot,
            IMarkerSignCoordinatorService markerSignService,
            IPersistentProgressService progressService,
            IHomelessOrdersService homelessOrdersService,
            IStaticDataService staticDataService,
            IGridMap gridMap,
            IFogOfWarMinimap fogOfWarMinimap,
            IBuildingRegistryService buildingRegistryService,
            IFenceService fenceService
        )
        {
            _fenceService = fenceService;
            _buildingRegistryService = buildingRegistryService;
            _coroutineRunner = coroutineRunner;
            _poolLoot = poolLoot;
            _markerSignService = markerSignService;
            _progressService = progressService;
            _homelessOrdersService = homelessOrdersService;
            _staticDataService = staticDataService;
            _gridMap = gridMap;
            _fogOfWarMinimap = fogOfWarMinimap;
        }

        public override void StopAllActions()
        {
            base.StopAllActions();

            _chopWood.StopAction();
            _dig.StopAction();
            _build.StopAction();
            _heal.StopAction();

            unitStatus.FreePlaceIndex = -1;
        }

        public override void Awake()
        {
            base.Awake();

            BuilderAnimator builderAnimator = unitAnimator as BuilderAnimator;

            _chopWood = new ChopWood(
                unitTransform,
                unitFlip,
                unitStatus,
                _builderActionHandler,
                builderAnimator,
                _unitStateMachineView,
                _coroutineRunner,
                _staticDataService,
                _gridMap);

            _dig = new Dig(unitTransform,
                unitFlip,
                unitStatus,
                _builderActionHandler,
                builderAnimator,
                _unitStateMachineView,
                _coroutineRunner,
                _staticDataService,
                _gridMap);

            _build = new Build(unitTransform,
                unitFlip,
                unitStatus,
                _builderActionHandler,
                builderAnimator,
                _unitStateMachineView,
                _coroutineRunner,
                _markerSignService,
                _progressService,
                _homelessOrdersService,
                _fogOfWarMinimap,
                _buildingRegistryService,
                _fenceService);

            _heal = new HealBuilding(unitTransform,
                unitFlip,
                unitStatus,
                _builderActionHandler,
                builderAnimator,
                _unitStateMachineView,
                _coroutineRunner);
        }

        public void PlayChopWoodBehaviour(OrderMarker orderMarker, float speed, int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted, Action onContinueOrderHappened)
        {
            stateMachine.ChangeState<UnknowState>();
            _chopWood.DoAction(orderMarker, speed, freePlaceIndex, onOrderCompleted, onContinueOrderHappened);
        }

        public void PlayBuildBehaviour(OrderMarker orderMarker, float speed, int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted, Action onContinueOrderHappened)
        {
            stateMachine.ChangeState<UnknowState>();
            _build.DoAction(speed, freePlaceIndex, orderMarker, onOrderCompleted, onContinueOrderHappened);
        }


        public void PlayDigBehaviour(OrderMarker orderMarker, float speed, int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted, Action onContinueOrderHappened)
        {
            stateMachine.ChangeState<UnknowState>();
            _dig.DoAction(orderMarker, speed, freePlaceIndex, onOrderCompleted, onContinueOrderHappened);
        }

        public void PlayHealBehaviour(OrderMarker orderMarker, float speed, int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted, Action onContinueOrderHappened)
        {
            stateMachine.ChangeState<UnknowState>();
            _heal.DoAction(orderMarker, speed, freePlaceIndex, onOrderCompleted, onContinueOrderHappened);
        }
    }
}