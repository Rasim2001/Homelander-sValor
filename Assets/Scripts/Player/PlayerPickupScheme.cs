using Infastructure.Services.BuildingCatalog;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.HudFader;
using Schemes;
using UI.GameplayUI.HudUI;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerPickupScheme : MonoBehaviour
    {
        [SerializeField] private ObserverTrigger _observerTrigger;

        private IBuildingModeConfigurationService _buildingModeConfigurationService;
        private IBuildingCatalogService _buildingCatalogService;
        private IHudFaderService _hudFaderService;


        [Inject]
        public void Construct(
            IBuildingModeConfigurationService buildingModeConfigurationService,
            IBuildingCatalogService buildingCatalogService, IHudFaderService hudFaderService)
        {
            _buildingModeConfigurationService = buildingModeConfigurationService;
            _buildingCatalogService = buildingCatalogService;
            _hudFaderService = hudFaderService;
        }

        private void Start() =>
            _observerTrigger.OnTriggerEnter += Enter;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= Enter;

        private void Enter()
        {
            if (_observerTrigger.CurrentCollider.TryGetComponent(out Scheme scheme))
            {
                if (scheme.CanCollect)
                {
                    _buildingModeConfigurationService.CreateItemUI(scheme.BuildingTypeId);
                    _buildingCatalogService.CreateCatalog(scheme.BuildingTypeId);

                    _hudFaderService.Show(HudId.Buildings);
                    _hudFaderService.DoFade(HudId.Buildings);


                    Destroy(_observerTrigger.CurrentCollider.gameObject);
                }
            }
        }
    }
}