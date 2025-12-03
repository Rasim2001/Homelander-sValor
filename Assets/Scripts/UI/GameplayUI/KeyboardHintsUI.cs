using TMPro;
using UnityEngine;

namespace UI.GameplayUI
{
    public class KeyboardHintsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private string[] _variants;

        public void UpdateText(int index) =>
            _textMeshProUGUI.text = _variants[index];
    }
}