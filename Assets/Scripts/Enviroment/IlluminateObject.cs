using BuildProcessManagement;
using Infastructure.Services.ResourceLimiter;
using Infastructure.StaticData.Building;
using Player.Orders;
using Tooltip.World;
using UnityEngine;
using Zenject;

namespace Enviroment
{
    public class IlluminateObject : MonoBehaviour
    {
        private OrderMarker _orderMarker;
        private IResourceLimiterService _resourceLimiterService;
        private TooltipTriggerWorld[] _tooltipTriggerWorldArray;

        [Inject]
        public void Construct(IResourceLimiterService resourceLimiterService) =>
            _resourceLimiterService = resourceLimiterService;

        private void Awake()
        {
            InitOrderMarker();
            InitTooltipTriggerWorld();
        }

        private void InitTooltipTriggerWorld() =>
            _tooltipTriggerWorldArray = GetComponentsInChildren<TooltipTriggerWorld>();

        private void InitOrderMarker()
        {
            _orderMarker = GetComponent<OrderMarker>();

            if (TryGetComponent(out BuildInfo buildInfo))
            {
                if (buildInfo.NextBuildingLevelId == BuildingLevelId.Unknow)
                    enabled = false;
            }
        }

        public void Illuminate()
        {
            if (_orderMarker == null)
                return;

            if (_orderMarker.IsStarted)
                return;

            if (_orderMarker.OrderID == OrderID.Chop || _orderMarker.OrderID == OrderID.Dig)
            {
                if (!_resourceLimiterService.IsActive(_orderMarker))
                    return;
            }

            foreach (TooltipTriggerWorld tooltip in _tooltipTriggerWorldArray)
            {
                tooltip.IsTriggeredWithPlayer = true;
                tooltip.ShowIlluminate();
            }
        }

        public void Release()
        {
            if (_orderMarker == null)
                return;

            if (_orderMarker.OrderID == OrderID.Chop || _orderMarker.OrderID == OrderID.Dig)
            {
                if (!_resourceLimiterService.IsActive(_orderMarker))
                    return;
            }

            foreach (TooltipTriggerWorld tooltip in _tooltipTriggerWorldArray)
            {
                tooltip.IsTriggeredWithPlayer = false;
                tooltip.HideIlluminate();
            }
        }
    }
}