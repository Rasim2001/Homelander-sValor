using TMPro;
using UnityEngine;

namespace Tooltip.UI
{
    public class TooltipUI : MonoBehaviour, ITooltip
    {
        [SerializeField] private RectTransform _tooltipRootTransform;
        [SerializeField] private RectTransform _bgTooltipTransform;
        [SerializeField] private TextMeshProUGUI _tooltipText;

        private void Awake() =>
            Hide();

        public void Show(string text, Vector2 position, TooltipPositionId tooltipPositionId, Rect iconRect,
            int customIndex)
        {
            _bgTooltipTransform.sizeDelta = new Vector2(300, 20);
            _tooltipText.rectTransform.sizeDelta = Vector2.zero;
            _tooltipText.text = text;

            if (_tooltipText.preferredWidth < 280)
            {
                _bgTooltipTransform.sizeDelta =
                    new Vector2(_tooltipText.preferredWidth + 30, _bgTooltipTransform.sizeDelta.y);
            }


            SetPivot(tooltipPositionId);

            Vector2 tooltipSize = _bgTooltipTransform.sizeDelta;
            Vector2 newPosition = position;

            switch (tooltipPositionId)
            {
                case TooltipPositionId.Above:
                    newPosition.y += iconRect.height * customIndex / 2 + 8;
                    break;
                case TooltipPositionId.Below:
                    newPosition.y -= iconRect.height * customIndex / 2 - 8;
                    break;
                case TooltipPositionId.Left:
                    newPosition.x -= iconRect.width * customIndex / 2;
                    break;
                case TooltipPositionId.Right:
                    newPosition.x += iconRect.width * customIndex / 2;
                    break;
            }

            newPosition = ClampToScreen(newPosition, tooltipSize);

            _bgTooltipTransform.position = newPosition;
            _bgTooltipTransform.gameObject.SetActive(true);
        }

        public void Hide() =>
            _bgTooltipTransform.gameObject.SetActive(false);

        private Vector2 ClampToScreen(Vector2 position, Vector2 size)
        {
            Vector2 clampedPosition = position;
            float halfWidth = size.x / 2;
            float halfHeight = size.y / 2;

            float screenWidth = _tooltipRootTransform.rect.width;
            float screenHeight = _tooltipRootTransform.rect.height;

            if (position.x + halfWidth > screenWidth)
                clampedPosition.x = screenWidth - halfWidth;

            if (position.x - halfWidth < 0)
                clampedPosition.x = halfWidth;

            if (position.y + halfHeight > screenHeight)
                clampedPosition.y = screenHeight - halfHeight;

            if (position.y - halfHeight < 0)
                clampedPosition.y = halfHeight;

            return clampedPosition;
        }

        private void SetPivot(TooltipPositionId tooltipPositionId)
        {
            switch (tooltipPositionId)
            {
                case TooltipPositionId.Above:
                    _bgTooltipTransform.pivot = new Vector2(0.5f, 0); // Нижний центр
                    break;
                case TooltipPositionId.Below:
                    _bgTooltipTransform.pivot = new Vector2(0.5f, 1); // Верхний центр
                    break;
                case TooltipPositionId.Left:
                    _bgTooltipTransform.pivot = new Vector2(1, 0.5f); // Правый центр
                    break;
                case TooltipPositionId.Right:
                    _bgTooltipTransform.pivot = new Vector2(0, 0.5f); // Левый центр
                    break;
            }
        }
    }
}