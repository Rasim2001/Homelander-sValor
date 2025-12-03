using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ImageButtonChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Sprite _selected;
        [SerializeField] private Sprite _unselected;

        [SerializeField] private Image _buttonImage;

        public void OnPointerEnter(PointerEventData eventData) =>
            _buttonImage.sprite = _selected;

        public void OnPointerExit(PointerEventData eventData) =>
            _buttonImage.sprite = _unselected;
    }
}