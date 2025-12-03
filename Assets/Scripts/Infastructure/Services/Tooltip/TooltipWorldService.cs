using Infastructure.Services.Pool;
using Tooltip;
using Tooltip.World;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.Tooltip
{
    public class TooltipWorldService : ITooltipWorldService
    {
        private readonly IPoolObjects<TooltipWorld> _tooltipWorldPool;
        private TooltipWorld _tooltipWorld;

        public TooltipWorldService(IPoolObjects<TooltipWorld> tooltipWorldPool) =>
            _tooltipWorldPool = tooltipWorldPool;


        public void Show(string text, Vector2 position, TooltipPositionId tooltipPositionId, Vector2 iconSize)
        {
            _tooltipWorld = _tooltipWorldPool.GetObjectFromPool();
            _tooltipWorld.SetText(text);

            Vector2 newPosition = position;
            Vector3 sizeText = _tooltipWorld.GetSize();

            switch (tooltipPositionId)
            {
                case TooltipPositionId.Above:
                    newPosition.y += iconSize.y / 2 + sizeText.y / 2 + 0.2f;
                    break;
                case TooltipPositionId.Below:
                    newPosition.y -= (iconSize.y / 2 + sizeText.y / 2 + 0.2f);
                    break;
                case TooltipPositionId.Left:
                    newPosition.x -= (iconSize.x / 2 + sizeText.x / 2 + 0.2f);
                    break;
                case TooltipPositionId.Right:
                    newPosition.x += iconSize.x / 2 + sizeText.x / 2 + 0.2f;
                    break;
            }

            _tooltipWorld.transform.position = newPosition;
        }

        public void Hide()
        {
            if (_tooltipWorld != null)
                _tooltipWorldPool.ReturnObjectToPool(_tooltipWorld);

            _tooltipWorld = null;
        }
    }
}