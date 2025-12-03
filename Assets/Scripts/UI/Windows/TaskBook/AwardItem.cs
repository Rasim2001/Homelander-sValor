using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Mainflag
{
    public class AwardItem : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void SetIcon(Sprite icon) =>
            _iconImage.sprite = icon;

        public void SetAmount(int amount) =>
            _amountText.text = $"x{amount}";
    }
}