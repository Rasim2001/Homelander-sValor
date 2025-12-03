using TMPro;
using UnityEngine;

namespace UI.GameplayUI.TowerSelectionUI
{
    public class BuildingItemsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _amountText;

        public void UpdateAmout(string text) =>
            _amountText.text = text;
    }
}