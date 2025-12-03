using System.Collections;
using Cysharp.Threading.Tasks;
using Infastructure.Services.CoinsCreator;
using Infastructure.Services.Pool;
using Infastructure.Services.ResourceLimiter;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.StaticDataService;
using Loots;
using Player.Orders;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.ResourceElements
{
    public class DestroyResource : MonoBehaviour
    {
        private IResourceLimiterService _resourceLimiterService;
        private IStaticDataService _staticDataService;

        private OrderMarker _orderMarker;
        private ResourceInfo _resourceInfo;
        private ICoinsCreatorService _coinsCreatorService;

        private void Awake()
        {
            _orderMarker = GetComponent<OrderMarker>();
            _resourceInfo = GetComponent<ResourceInfo>();
        }

        [Inject]
        public void Construct(
            IResourceLimiterService resourceLimiterService,
            IStaticDataService staticDataService,
            ICoinsCreatorService coinsCreatorService)
        {
            _coinsCreatorService = coinsCreatorService;
            _staticDataService = staticDataService;
            _resourceLimiterService = resourceLimiterService;
        }

        public virtual void DestroyElement()
        {
            ResourceData resourceData = _staticDataService.ForResource(_resourceInfo.ResourceId);
            _coinsCreatorService.CreateCoinsAsync(transform.position, resourceData.CoinsValue).Forget();

            _resourceLimiterService.RemoveResource(_orderMarker);
        }
    }
}