using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayUI.BuildingCatalog
{
    public class CatalogItemUI : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        public void UpdateIcon(Sprite sprite) =>
            _iconImage.sprite = sprite;
    }
}