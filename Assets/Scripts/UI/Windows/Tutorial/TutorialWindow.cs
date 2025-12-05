using TMPro;
using UnityEngine;

namespace UI.Windows.Tutorial
{
    public class TutorialWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void Initialize(string text) =>
            _text.text = text;
    }
}