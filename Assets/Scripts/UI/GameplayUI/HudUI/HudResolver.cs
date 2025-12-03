using Infastructure.Services.BuildingCatalog;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.HudFader;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI.HudUI
{
    public class HudResolver : MonoBehaviour
    {
        [Header("HudFader")]
        [SerializeField] private CanvasGroup _coinsGroup;
        [SerializeField] private CanvasGroup _unitsGroup;
        [SerializeField] private CanvasGroup _buildingsGroup;

        [Header("BuildingCatalogUI")]
        [SerializeField] private Transform _leftContainer;

        private IHudFaderService _hudFaderService;
        private IBuildingCatalogService _buildingCatalogService;
        private IBuildingModeConfigurationService _buildingModeConfigurationService;
        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(IHudFaderService hudFaderService, IBuildingCatalogService buildingCatalogService,
            IBuildingModeConfigurationService buildingModeConfigurationService, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _buildingModeConfigurationService = buildingModeConfigurationService;
            _hudFaderService = hudFaderService;
            _buildingCatalogService = buildingCatalogService;
        }


        private void Start()
        {
            _buildingCatalogService.Initialize(_leftContainer);

            _hudFaderService.Register(HudId.Coins, _coinsGroup);
            _hudFaderService.Register(HudId.Units, _unitsGroup);
            _hudFaderService.Register(HudId.Buildings, _buildingsGroup);
            _hudFaderService.DoFade(HudId.Units);

            if (_staticDataService.CheatStaticData.GetSchemes)
                InitSchemes();
        }

        private void InitSchemes()
        {
            for (int i = 0; i < 5; i++)
            {
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.Baricade);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.BowWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.HammerWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.ShielderWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.ResetWorkshop);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.TowerBow);
                _buildingModeConfigurationService.CreateItemUI(BuildingTypeId.TowerTar);

                _buildingCatalogService.CreateCatalog(BuildingTypeId.Baricade);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.BowWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.HammerWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.ShielderWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.ResetWorkshop);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.TowerBow);
                _buildingCatalogService.CreateCatalog(BuildingTypeId.TowerTar);
            }

            _hudFaderService.Show(HudId.Buildings);
            _hudFaderService.DoFade(HudId.Buildings);
        }
    }
}