using BuildProcessManagement;
using Infastructure.StaticData.Building;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI.BuildingCoinsUIManagement
{
    public class BuildCoinsUIRegistrator : MonoBehaviour
    {
        [SerializeField] private BuildInfo _buildInfo;
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;

        private IStaticDataService _staticData;

        [Inject]
        public void Construct(IStaticDataService staticData) =>
            _staticData = staticData;


        public void Register()
        {
            if (_buildInfo.NextBuildingLevelId == BuildingLevelId.Unknow)
                return;

            BuildingUpgradeData buildingUpgradeNextData =
                _staticData.ForBuilding(_buildInfo.BuildingTypeId, _buildInfo.NextBuildingLevelId);

            if (buildingUpgradeNextData != null)
                _buildingCoinsUI.UpdateCoinsUI(buildingUpgradeNextData.CoinsValue);
        }
    }
}