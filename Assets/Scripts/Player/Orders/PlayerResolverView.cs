using Grid;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.UnitRecruiter;
using UI.GameplayUI;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.SpeachBubleUI;
using UI.GameplayUI.TowerSelectionUI.MoveItems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Player.Orders
{
    public class PlayerResolverView : MonoBehaviour
    {
        [Header("BuilderCommand")]
        [SerializeField] private SpeachBuble _speachBuble;
        [SerializeField] private SelectUnitArrow _selectUnitArrow;

        [Header("BuildingMode")]
        [SerializeField] private SpriteRenderer _ghostSpriteRender;

        [Header("BuildingModeUI")]
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;
        [SerializeField] private Image _fillImage;
        [SerializeField] private MoveBuildingUI _moveBuildingUI;
        [SerializeField] private BuildHintsUI _buildHintsUI;
        [SerializeField] private Transform _buildingModeContainer;

        [Header("UnitRecruiter")]
        [SerializeField] private PlayerFlip _playerFlip;
        [SerializeField] private PlayerMove _playerMove;

        [Header("BuildingModify")]
        [SerializeField] private ShowPriceZone _showPriceZone;
        [SerializeField] private ObserverTrigger _buildingObserverTrigger;

        private IBuilderCommandExecutor _builderCommandExecutor;
        private IBuildingModeService _buildingModeService;
        private IBuildingModeUIService _buildingModeUIService;
        private IUnitsRecruiterService _unitsRecruiterService;
        private IBuildingModifyService _buildingModifyService;
        private IBuildingModeConfigurationService _configurationService;

        [Inject]
        public void Construct(
            IBuilderCommandExecutor builderCommandExecutor,
            IBuildingModeService buildingModeService,
            IBuildingModeUIService buildingModeUIService,
            IUnitsRecruiterService unitsRecruiterService,
            IBuildingModifyService buildingModifyService,
            IBuildingModeConfigurationService configurationService)
        {
            _builderCommandExecutor = builderCommandExecutor;
            _buildingModeService = buildingModeService;
            _buildingModeUIService = buildingModeUIService;
            _unitsRecruiterService = unitsRecruiterService;
            _buildingModifyService = buildingModifyService;
            _configurationService = configurationService;
        }

        public void Awake() =>
            Resolve();

        private void Resolve()
        {
            _unitsRecruiterService.Initialize(_playerFlip, _playerMove);

            _builderCommandExecutor.Initialize(_speachBuble, _selectUnitArrow);

            _buildingModeService.Initialize(
                _moveBuildingUI,
                _buildHintsUI,
                _ghostSpriteRender,
                _buildingCoinsUI,
                _speachBuble);

            _buildingModifyService.Initialize(_showPriceZone, _buildingObserverTrigger);

            _buildingModeUIService.Initialize(_fillImage);
            _configurationService.Initialize(_buildingModeContainer, _moveBuildingUI);
        }
    }
}