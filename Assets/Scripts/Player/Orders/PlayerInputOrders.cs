using System;
using BuildProcessManagement.HandleOrders;
using CutScenes;
using Infastructure;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.CameraFocus;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PauseService;
using Infastructure.Services.Pool;
using Infastructure.Services.PurchaseDelay;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.Services.Tutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.UnitRecruiter;
using Player.Shoot;
using Player.Skills;
using UI.GameplayUI.CristalUI;
using UI.GameplayUI.TowerSelectionUI;
using Units;
using UnityEngine;
using Zenject;

namespace Player.Orders
{
    public class PlayerInputOrders : MonoBehaviour
    {
        [SerializeField] private Transform _castSkillPoint;
        [SerializeField] private PlayerFlip _playerFlip;
        [SerializeField] private PlayerMove _playerMove;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private SelectUnitArrow _selectUnitArrow;
        [SerializeField] private PlayerGiveOrderFx _playerGiveOrderFx;
        [SerializeField] private ObserverTrigger _orderObserverTrigger;


        public Action OnCastSkillHappenedOnTutorial;
        public Action OnCallUnitsHappenedOnTutorial;
        public Action OnBonfireUpgradeHappenedOnTutorial;
        public Action OnWorkshopHappenedOnTutorial;
        public Action OnReleaseHappendOnTutorial;

        public Action OnReleaseAllHappened;

        private IUnitsRecruiterService _unitsRecruiterService;
        private IInputService _inputService;
        private IFutureOrdersService _futureOrdersService;
        private ICoroutineRunner _coroutineRunner;
        private IHomelessOrdersService _homelessOrdersService;
        private ISkill _freezSkill;
        private IBuildingModeService _buildingModeService;
        private IBuildingModifyService _buildingModifyService;
        private IPurchaseDelayService _purchaseDelayService;
        private IPauseService _pauseService;
        private ITutorialProgressService _tutorialProgressService;
        private Cristal _cristal;

        private bool _readyUseSkill;
        private ICristalTimeline _cristalTimeline;
        private ICameraFocusService _cameraFocusService;
        private IBuilderCommandExecutor _builderCommandExecutor;
        private IPoolObjects<FreezParticleMarker> _freezParticlePool;
        private ISafeBuildZone _safeBuildZone;

        [Inject]
        public void Construct(
            IInputService inputService,
            IFutureOrdersService futureOrdersService,
            ICoroutineRunner coroutineRunner,
            IHomelessOrdersService homelessOrdersService,
            IBuildingModeService buildingModeService,
            IUnitsRecruiterService unitsRecruiterService,
            IBuildingModifyService buildingModifyService,
            IPurchaseDelayService purchaseDelayService,
            IPauseService pauseService,
            IGameUIFactory gameUIFactory,
            ICristalTimeline cristalTimeline,
            ICameraFocusService cameraFocusService,
            ITutorialProgressService tutorialProgressService,
            IBuilderCommandExecutor builderCommandExecutor,
            ISafeBuildZone safeBuildZone,
            IPoolObjects<FreezParticleMarker> freezParticlePool)
        {
            _safeBuildZone = safeBuildZone;
            _tutorialProgressService = tutorialProgressService;
            _freezParticlePool = freezParticlePool;
            _builderCommandExecutor = builderCommandExecutor;
            _cameraFocusService = cameraFocusService;
            _cristalTimeline = cristalTimeline;
            _inputService = inputService;
            _futureOrdersService = futureOrdersService;
            _coroutineRunner = coroutineRunner;
            _homelessOrdersService = homelessOrdersService;
            _buildingModeService = buildingModeService;
            _unitsRecruiterService = unitsRecruiterService;
            _buildingModifyService = buildingModifyService;
            _purchaseDelayService = purchaseDelayService;
            _pauseService = pauseService;
        }

        private void Awake() =>
            _freezSkill = new FreezSkill(_playerFlip, _castSkillPoint, _coroutineRunner, _freezParticlePool);

        public void InitCristal(Cristal cristal) =>
            _cristal = cristal;


        public void Update()
        {
            if (_playerMove.AccelerationPressedWithMove || _buildingModeService.IsBuildingState ||
                _pauseService.IsPaused || _cristalTimeline.IsPlaying || _cameraFocusService.PlayerDefeated)
                return;

            if (_inputService.SpacePressed && _tutorialProgressService.IsCallUnitsReadyToUse && _safeBuildZone.IsNight)
                CastSkill();
            else if (_inputService.CallUnitsPressed && _tutorialProgressService.IsCallUnitsReadyToUse)
                CallUnits();
            else if (_inputService.ReleaseUnitsPressed && _tutorialProgressService.IsReleaseUnitsReadyToUse)
                ReleaseUnits();
            else if (_inputService.SelectUnitPressed && _tutorialProgressService.IsSelectUnitsReadyToUse)
                _selectUnitArrow.SelectUnit();
            else if (_inputService.ExecuteOrderPressedDown && _orderObserverTrigger.CurrentCollider != null &&
                     _tutorialProgressService.IsGiveOrderReadyToUse)
                HandlePlayerOrder();
        }

        private void ReleaseUnits()
        {
            _playerAnimator.PlayReleaseAnimation();
            _unitsRecruiterService.ReleaseAll();

            _futureOrdersService.ContinueExecuteOrders();
            _homelessOrdersService.ContinueExecuteOrders();

            OnReleaseAllHappened?.Invoke();
        }

        private void HandlePlayerOrder()
        {
            UniqueId uniqueId = _orderObserverTrigger.CurrentCollider.GetComponent<UniqueId>();
            if (uniqueId != null && _purchaseDelayService.DelayIsActive(uniqueId.Id))
                return;

            TowerHintsDisplay towerHintsDisplay =
                _orderObserverTrigger.CurrentCollider.GetComponent<TowerHintsDisplay>();

            OrderMarker orderMarker = _orderObserverTrigger.CurrentCollider.GetComponent<OrderMarker>();

            if (!_buildingModifyService.IsActive && towerHintsDisplay != null && !orderMarker.IsStarted &&
                orderMarker.OrderID != OrderID.Heal)
                return;

            _playerAnimator.PlayGiveOrderAnimation();

            if (_orderObserverTrigger.CurrentCollider.TryGetComponent(out IHandleOrder handlerOrder))
            {
                if (IsBuildingOrder(orderMarker) || IsHealOrder(orderMarker))
                    _builderCommandExecutor.StartBuild(orderMarker);
                else
                    handlerOrder.Handle();
            }
        }

        private bool IsBuildingOrder(OrderMarker orderMarker)
        {
            return orderMarker != null && orderMarker.OrderID == OrderID.Build &&
                   orderMarker.IsStarted;
        }

        private bool IsHealOrder(OrderMarker orderMarker) =>
            orderMarker != null && orderMarker.OrderID == OrderID.Heal;


        private void CallUnits()
        {
            if (_playerAnimator.AnyStateIsActive)
                return;

            _playerGiveOrderFx.PlayFx();
            _unitsRecruiterService.InitializeUnits();
            _playerAnimator.PlaySummonAnimation();
        }

        private void CastSkill()
        {
            if (_cristal != null && _cristal.SkillIsReady() && !_playerAnimator.AnyStateIsActive)
            {
                _playerAnimator.PlayCastSkillAnimation();
                _freezSkill.CastSkill();
                _cristal.ResetCoolDown();
            }
        }
    }
}