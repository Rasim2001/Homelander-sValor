using Infastructure.StaticData.Unit;
using UnityEngine;

namespace Tooltip.UI
{
    public class UnitTooltipTrigger : TooltipTrigger
    {
        [SerializeField] private UnitTypeId _unitTypeId;

        private void Awake() =>
            TooltipText = StaticDataService.ForTooltip(_unitTypeId);
    }
}