using Tooltip;
using UnityEngine;

namespace Infastructure.Services.Tooltip
{
    public interface ITooltipWorldService
    {
        void Show(string text, Vector2 position, TooltipPositionId tooltipPositionId, Vector2 iconSize);
        void Hide();
    }
}