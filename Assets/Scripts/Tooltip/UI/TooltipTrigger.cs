using Infastructure.Services.HudFader;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Tooltip.UI
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected TooltipPositionId _tooltipPositionId;
        [SerializeField] protected RectTransform _rectIcon;

        protected IStaticDataService StaticDataService;
        protected string TooltipText;
        protected ITooltip Tooltip;

        private IHudFaderService _hudFaderService;


        [Inject]
        public void Construct(IStaticDataService staticDataService, ITooltip tooltip, IHudFaderService hudFaderService)
        {
            StaticDataService = staticDataService;

            Tooltip = tooltip;
            _hudFaderService = hudFaderService;
        }


        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _hudFaderService.ShowAll();

            Tooltip.Show(TooltipText, transform.position, _tooltipPositionId, _rectIcon.rect);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _hudFaderService.HideAll();

            Tooltip.Hide();
        }
    }
}