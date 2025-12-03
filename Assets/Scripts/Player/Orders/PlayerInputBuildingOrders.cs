using BuildProcessManagement;
using CameraManagement;
using CutScenes;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.CameraFocus;
using Infastructure.Services.ECSInput;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PauseService;
using UI.GameplayUI.TowerSelectionUI;
using UI.GameplayUI.TowerSelectionUI.MoveItems;
using UnityEngine;
using Zenject;

namespace Player.Orders
{
    public class PlayerInputBuildingOrders : MonoBehaviour, IEcsWatcher
    {
        [SerializeField] private ObserverTrigger _observerTrigger;
        [SerializeField] private PlayerMove _playerMove;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private MoveBuildingUI _moveBuildingUI;

        public float BuildModeDelay = 1;

        private IInputService _inputService;
        private IOrderSelectionUIService _orderSelectionUIService;
        private IBuildingModeService _buildingModeService;
        private IBuildingModeUIService _buildingModeUIService;
        private IBuildingModifyService _buildingModifyService;
        private IBuilderCommandExecutor _builderCommandExecutor;
        private IBuildingModeConfigurationService _configurationService;

        private float _currentTimeDelay;
        private CinemachineFollow _cameraFollow;
        private ICristalTimeline _cristalTimeline;
        private IPauseService _pauseService;
        private ICameraFocusService _cameraFocusService;


        [Inject]
        public void Construct(
            IInputService inputService,
            IOrderSelectionUIService orderSelectionUIService,
            IBuildingModeService buildingModeService,
            IBuildingModeUIService buildingModeUIService,
            IBuildingModifyService buildingModifyService,
            IBuilderCommandExecutor builderCommandExecutor,
            IBuildingModeConfigurationService configurationService,
            ICristalTimeline cristalTimeline,
            IPauseService pauseService,
            ICameraFocusService cameraFocusService)
        {
            _cameraFocusService = cameraFocusService;
            _pauseService = pauseService;
            _cristalTimeline = cristalTimeline;
            _inputService = inputService;
            _orderSelectionUIService = orderSelectionUIService;
            _buildingModeService = buildingModeService;
            _buildingModeUIService = buildingModeUIService;
            _buildingModifyService = buildingModifyService;
            _builderCommandExecutor = builderCommandExecutor;
            _configurationService = configurationService;
        }

        private void Start()
        {
            _cameraFollow = Camera.main.GetComponent<CinemachineFollow>();
            _buildingModeService.StopBuildingState();

            _observerTrigger.OnTriggerExit += ExitModify;
            _builderCommandExecutor.OnBuildHappened += ExitModify;
        }

        private void OnDestroy()
        {
            _observerTrigger.OnTriggerExit -= ExitModify;
            _builderCommandExecutor.OnBuildHappened -= ExitModify;
        }

        private void Update()
        {
            if (_cristalTimeline.IsPlaying || _pauseService.IsPaused ||
                _cameraFocusService.PlayerDefeated || _playerAnimator.AnyStateIsActive)
                return;

            HandleAcceleration();

            HandleOccupyGrid();

            HandleBuildingMode();

            PlayBuildModeAnimation();
        }


        public bool CanUseEcsMenu() =>
            !_buildingModeService.IsBuildingState && !_buildingModifyService.IsActive;

        public void EcsCancel()
        {
            _buildingModeService.StopBuildingState();
            _buildingModifyService.ExitModify();

            ResetDelayTimer();
        }

        private void ExitModify()
        {
            if (_buildingModeService.IsBuildingState)
                return;

            ResetDelayTimer();
            _buildingModifyService.ExitModify();
        }


        private void PlayBuildModeAnimation() =>
            _playerAnimator.PlayBuildModeAnimation(_buildingModeService.IsBuildingState ||
                                                   _buildingModifyService.IsActive);

        private void HandleBuildingMode()
        {
            Collider2D currentCollider = _observerTrigger.CurrentCollider;

            if (IsHealingProcess(currentCollider))
                return;

            if (CanEnterInBuildingMode())
                EnterBuildingMode();
            else if (CanModifyBuild())
                UpdateModifyBuilding();
            else if (_buildingModeService.IsBuildingState)
                UpdateBuildingMode();
            else if ((_inputService.ExecuteOrderButtonUp ||
                      _inputService.MoveKeysButtonDown) &&
                     !_buildingModifyService.IsActive)
                ResetDelayTimer();
        }

        private bool IsHealingProcess(Collider2D currentCollider)
        {
            return currentCollider != null && currentCollider.TryGetComponent(out OrderMarker orderMarker) &&
                   orderMarker.OrderID == OrderID.Heal;
        }

        private void UpdateModifyBuilding()
        {
            if (TimeIsEnded())
            {
                _buildingModifyService.StartModify();

                _playerMove.ReduceSpeed();
                _cameraFollow.SetFarCamera();
            }
            else
                UpdateTime();
        }

        private bool CanModifyBuild()
        {
            if (_observerTrigger.CurrentCollider == null)
                return false;

            TowerHintsDisplay towerHintsDisplay = _observerTrigger.CurrentCollider.GetComponent<TowerHintsDisplay>();
            OrderMarker orderMarker = _observerTrigger.CurrentCollider.GetComponent<OrderMarker>();

            return _inputService.ExecuteOrderPressed &&
                   towerHintsDisplay != null &&
                   orderMarker != null && !orderMarker.IsMarkered && !orderMarker.IsStarted &&
                   !_buildingModifyService.IsActive &&
                   !_playerMove.IsMoving();
        }

        private bool CanEnterInBuildingMode()
        {
            return _inputService.ExecuteOrderPressed &&
                   _observerTrigger.CurrentCollider == null &&
                   _configurationService.HasBuilding() &&
                   !_playerMove.IsMoving();
        }

        private void UpdateBuildingMode()
        {
            if (_orderSelectionUIService.LeftArrowPressed && _moveBuildingUI.CanMoveLeft())
            {
                _buildingModeUIService.NavigateSelection(-1);
                _buildingModeService.RegistBuild();
                _moveBuildingUI.MoveLeft();
            }

            else if (_orderSelectionUIService.RightArrowPressed && _moveBuildingUI.CanMoveRight())
            {
                _buildingModeUIService.NavigateSelection(1);
                _buildingModeService.RegistBuild();
                _moveBuildingUI.MoveRight();
            }

            int currentPosition = Mathf.RoundToInt(transform.position.x);

            _buildingModeService.MoveGhost(new Vector3(transform.position.x, -2.75f, 0));
            _buildingModeService.PaintGhost(currentPosition);
        }

        private void EnterBuildingMode()
        {
            if (TimeIsEnded())
            {
                _buildingModeService.StartBuildingState();
                _buildingModeService.RegistBuild();
                _buildingModeUIService.SelectBuildItem();

                _playerMove.ReduceSpeed();
                _cameraFollow.SetFarCamera();
            }
            else
                UpdateTime();
        }

        private void HandleOccupyGrid()
        {
            if (_inputService.ExecuteOrderPressedDown && _buildingModeService.IsBuildingState)
            {
                int currentPosition = Mathf.RoundToInt(transform.position.x);

                if (_buildingModeService.CanOccupyCells(currentPosition))
                    ResetDelayTimer();
            }
        }

        private void HandleAcceleration()
        {
            if (_inputService.AccelerationPressedDown)
            {
                _buildingModeService.StopBuildingState();
                _buildingModifyService.ExitModify();

                ResetDelayTimer();
            }
        }


        private bool TimeIsEnded() =>
            _currentTimeDelay >= BuildModeDelay && !_buildingModeService.IsBuildingState;

        private void UpdateTime()
        {
            if (_currentTimeDelay < BuildModeDelay)
                _currentTimeDelay += Time.deltaTime;

            if (_currentTimeDelay / BuildModeDelay > 0.4f)
                UpdateBuildBarUI();
        }

        private void UpdateBuildBarUI()
        {
            float progress = Mathf.Clamp01(_currentTimeDelay / BuildModeDelay);

            _buildingModeUIService.UpdateProgressUI(progress);
        }

        private void ResetDelayTimer()
        {
            _playerMove.SetDefaultSpeed();
            _currentTimeDelay = 0;

            _buildingModeUIService.ResetProgress();
            _cameraFollow.SetNearCamera();
        }
    }
}