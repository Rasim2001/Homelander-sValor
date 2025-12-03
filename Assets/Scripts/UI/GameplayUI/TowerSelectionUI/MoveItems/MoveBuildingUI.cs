using Infastructure.Services.BuildModeServices;
using Zenject;

namespace UI.GameplayUI.TowerSelectionUI.MoveItems
{
    public class MoveBuildingUI : MoveUIItemsBase
    {
        private IBuildingModeConfigurationService _configurationService;

        [Inject]
        public void Construct(IBuildingModeConfigurationService configurationService) =>
            _configurationService = configurationService;

        public override void ReInitialize()
        {
            base.ReInitialize();

            _configurationService.CorrectIndex = 0;
        }

        public void UpdateNumberOfItems() =>
            NumberOfItems = _configurationService.BuildingTypeInfos.Count;
    }
}