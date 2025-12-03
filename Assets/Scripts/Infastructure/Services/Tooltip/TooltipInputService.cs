using Tooltip.World;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.Tooltip
{
    public class TooltipInputService : ITooltipInputService, IInitializable, ITickable
    {
        private const string FlagHintsLayer = "FlagHints";

        private readonly Collider2D[] results = new Collider2D[1];

        private Camera _mainCamera;
        private GameObject lastHitObject;
        private int _layerMask;

        public void Initialize()
        {
            _mainCamera = Camera.main;

            _layerMask = 1 << LayerMask.NameToLayer(FlagHintsLayer);
        }

        public void Tick()
        {
            Vector2 worldPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int hitCount = Physics2D.OverlapPointNonAlloc(worldPoint, results, _layerMask);

            GameObject currentHitObject = null;
            if (hitCount > 0 && results[0] != null)
                currentHitObject = results[0].gameObject;

            if (currentHitObject != lastHitObject)
            {
                if (lastHitObject != null)
                {
                    TooltipTriggerWorld handler = lastHitObject.GetComponent<TooltipTriggerWorld>();
                    if (handler != null)
                        handler.MouseExitCustom();
                }

                if (currentHitObject != null)
                {
                    TooltipTriggerWorld handler = currentHitObject.GetComponent<TooltipTriggerWorld>();
                    if (handler != null)
                        handler.MouseEnterCustom();
                }

                lastHitObject = currentHitObject;
            }
        }
    }
}