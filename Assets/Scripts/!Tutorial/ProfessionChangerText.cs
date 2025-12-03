using TMPro;
using UnityEngine;

namespace _Tutorial
{
    public class ProfessionChangerText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private const string TargetText = "Новобранец станет <b>Рабочим</b>,\nесли дать ему молот.";

        public void Change() =>
            _text.text = TargetText;
    }
}