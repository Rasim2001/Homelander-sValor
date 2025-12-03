using Infastructure.Services.HudFader;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Tooltip.UI
{
    public class TooltipTriggerRuntime : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _rectIcon;

        private TooltipPositionId _tooltipPositionId;
        private string _text;

        private ITooltip _tooltip;
        private IHudFaderService _hudFaderService;


        [Inject]
        public void Construct(ITooltip tooltip, IHudFaderService hudFaderService)
        {
            _tooltip = tooltip;
            _hudFaderService = hudFaderService;
        }

        public void Initialize(TooltipPositionId tooltipPositionId, string text)
        {
            _tooltipPositionId = tooltipPositionId;
            _text = text;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hudFaderService.ShowAll();

            _tooltip.Show(_text, transform.position, _tooltipPositionId, _rectIcon.rect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hudFaderService.HideAll();

            _tooltip.Hide();
        }
    }
}