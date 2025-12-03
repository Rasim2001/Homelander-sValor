using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.StaticDataService;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.ResourceElements
{
    public class ResourceCoinsUIRegistrator : MonoBehaviour
    {
        [SerializeField] private ResourceInfo _resourceInfo;
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;

        private IStaticDataService _staticData;

        [Inject]
        public void Construct(IStaticDataService staticData) =>
            _staticData = staticData;

        private void Start()
        {
            ResourceData resourceData = _staticData.ForResource(_resourceInfo.ResourceId);

            if (resourceData != null)
                _buildingCoinsUI.UpdateCoinsUI(resourceData.CoinsValue);
        }
    }
}