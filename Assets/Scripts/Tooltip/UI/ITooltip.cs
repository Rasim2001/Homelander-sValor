using UnityEngine;

namespace Tooltip.UI
{
    public interface ITooltip
    {
        void Show(string text, Vector2 position, TooltipPositionId tooltipPositionId, Rect iconRect,
            int customIndex = 1);

        void Hide();
    }
}