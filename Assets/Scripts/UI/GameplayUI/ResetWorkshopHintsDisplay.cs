using BuildProcessManagement.WorkshopBuilding;
using Infastructure.Services.PurchaseDelay;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Workshop;
using UI.GameplayUI.BuildingCoinsUIManagement;
using Units;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI
{
    public class ResetWorkshopHintsDisplay : HintsDisplayBase
    {
        [SerializeField] private KeyboardHintsUI _keyboardHintsUI;
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private BuildingCoinsUI _coinsUI;

        private IPurchaseDelayService _purchaseDelayService;
        private IStaticDataService _staticDataService;
        private bool _isVisible;


        [Inject]
        public void Construct(IPurchaseDelayService purchaseDelayService, IStaticDataService staticDataService)
        {
            _purchaseDelayService = purchaseDelayService;
            _staticDataService = staticDataService;
        }


        private void Start()
        {
            WorkshopStaticData workshopData = _staticDataService.ForWorkshop(WorkshopItemId.Reset);
            _coinsUI.UpdateCoinsUI(workshopData.CoinsValue);

            _purchaseDelayService.OnDelayExited += ExitDelay;
        }

        private void OnDestroy() =>
            _purchaseDelayService.OnDelayExited -= ExitDelay;


        protected override void Show(bool value)
        {
            _isVisible = value;

            _keyboardHintsUI.gameObject.SetActive(value);

            if (!_purchaseDelayService.DelayIsActive(_uniqueId.Id))
                _coinsUI.Show(value);
        }

        private void ExitDelay(string uniqueId)
        {
            if (_uniqueId.Id.Contains(uniqueId) && _isVisible)
                ShowHints();
        }
    }
}