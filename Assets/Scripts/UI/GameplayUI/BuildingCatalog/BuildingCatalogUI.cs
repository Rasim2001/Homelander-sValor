using Infastructure.Services.BuildingCatalog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.GameplayUI.BuildingCatalog
{
    public class BuildingCatalogUI : MonoBehaviour, ICatalog
    {
        [SerializeField] private Image _catalogImage;
        [SerializeField] private Button _openCatalogButton;
        [SerializeField] private TextMeshProUGUI _amountText;

        public Transform SubContainer;

        private ICatalogOpenService _catalogOpenService;
        private int _amount;

        [Inject]
        public void Construct(ICatalogOpenService catalogOpenService) =>
            _catalogOpenService = catalogOpenService;

        private void Start() =>
            _openCatalogButton.onClick.AddListener(OnToggleButtonClicked);

        private void OnDestroy() =>
            _openCatalogButton.onClick.RemoveListener(OnToggleButtonClicked);

        public void SetCatalogSprite(Sprite sprite) =>
            _catalogImage.sprite = sprite;

        public void OpenCatalog() =>
            SubContainer.localScale = new Vector3(1, 1, 1);

        public void CloseCatalog() =>
            SubContainer.localScale = new Vector3(0, 1, 1);

        public void AddCatalogItem()
        {
            _amount++;

            UpdateAmout(_amount.ToString());
        }

        public void RemoveCatalogItem()
        {
            _amount--;

            UpdateAmout(_amount.ToString());
        }

        private void UpdateAmout(string text) =>
            _amountText.text = text;

        private void OnToggleButtonClicked() =>
            _catalogOpenService.ToggleCatalog(this);
    }
}