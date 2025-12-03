using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject[] _arrows;

        private readonly Color _normalColor = new Color(128f / 255f, 128f / 255f, 128f / 255f, 1);
        private readonly Color _targetColor = Color.white;

        private Button _button;

        private void Awake() =>
            _button = GetComponent<Button>();

        private void Start() =>
            _button.targetGraphic.color = _normalColor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetActive(true);
            _button.targetGraphic.color = _targetColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetActive(false);
            _button.targetGraphic.color = _normalColor;
        }

        private void SetActive(bool value)
        {
            foreach (GameObject arrow in _arrows)
                arrow.SetActive(value);
        }
    }
}