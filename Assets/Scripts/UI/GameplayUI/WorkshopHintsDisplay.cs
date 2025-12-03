using BuildProcessManagement.WorkshopBuilding;
using Enviroment;
using Infastructure.Services.PurchaseDelay;
using UI.GameplayUI.BuildingCoinsUIManagement;
using Units;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI
{
    public class WorkshopHintsDisplay : HintsDisplayBase
    {
        private const int VendorIndex = 0;
        private const int CoinsIndex = 1;

        [SerializeField] private CurtainWorkshop _curtainWorkshop;
        [SerializeField] private IlluminateObject _illuminateObject;
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private KeyboardHintsUI _keyboardHintsUI;
        [SerializeField] private BuildingCoinsUI _coinsUI;
        [SerializeField] private Workshop _workshop;

        private IPurchaseDelayService _purchaseDelayService;
        private ObserverTrigger _observerTrigger;

        private bool _isVisible;


        [Inject]
        public void Construct(IPurchaseDelayService purchaseDelayService) =>
            _purchaseDelayService = purchaseDelayService;


        protected override void Awake()
        {
            base.Awake();

            _observerTrigger = GetComponent<ObserverTrigger>();
        }

        private void Start()
        {
            _purchaseDelayService.OnDelayExited += ExitDelay;
            _curtainWorkshop.OnVisibilityChanged += VisibilityChanged;
        }

        private void OnDestroy()
        {
            _purchaseDelayService.OnDelayExited -= ExitDelay;
            _curtainWorkshop.OnVisibilityChanged -= VisibilityChanged;
        }


        public void UpdateHints()
        {
            _keyboardHintsUI.UpdateText(_workshop.HasVendor ? CoinsIndex : VendorIndex);
            //_coinsUI.Show();
        }

        protected override void Show(bool value)
        {
            _isVisible = value;

            if (_curtainWorkshop.IsShowed && _workshop.HasVendor && _isVisible)
                return;

            _keyboardHintsUI.gameObject.SetActive(value);
            _keyboardHintsUI.UpdateText(_workshop.HasVendor ? CoinsIndex : VendorIndex);

            if (_workshop.HasVendor && !_purchaseDelayService.DelayIsActive(_uniqueId.Id))
                _coinsUI.Show(value);
        }

        private void ExitDelay(string uniqueId)
        {
            if (_uniqueId.Id.Contains(uniqueId) && _isVisible)
                ShowHints();
        }

        private void VisibilityChanged()
        {
            if (_observerTrigger.CurrentCollider != null)
                Show(!_curtainWorkshop.IsShowed);
        }
    }
}