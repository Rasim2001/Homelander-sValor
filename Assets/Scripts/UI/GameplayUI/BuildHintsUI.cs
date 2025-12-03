using TMPro;
using UnityEngine;

namespace UI.GameplayUI
{
    public class BuildHintsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

        public void UpdateText(string text) =>
            _textMeshProUGUI.text = text;
    }
}