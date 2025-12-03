using Bonfire;
using Infastructure.Services.PurchaseDelay;
using UI.GameplayUI.BuildingCoinsUIManagement;
using Units;
using UnityEngine;
using Zenject;

namespace UI.GameplayUI
{
    public class MainFlagHintsDisplay : HintsDisplayBase
    {
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;
        [SerializeField] private UniqueId _uniqueId;

        private IUpgradeMainFlag _upgradeMainFlag;
        private IPurchaseDelayService _purchaseDelayService;


        private bool _isVisible;


        [Inject]
        public void Construct(IUpgradeMainFlag upgradeMainFlag, IPurchaseDelayService purchaseDelayService)
        {
            _upgradeMainFlag = upgradeMainFlag;
            _purchaseDelayService = purchaseDelayService;
        }

        private void Start() =>
            _purchaseDelayService.OnDelayExited += ExitDelay;

        private void OnDestroy() =>
            _purchaseDelayService.OnDelayExited -= ExitDelay;

        public override void ShowHints()
        {
            if (_upgradeMainFlag.HasUpgrade() &&
                !_purchaseDelayService.DelayIsActive(_uniqueId.Id) &&
                !buildingModeService.IsBuildingState)
                Show(true);
        }

        protected override void Show(bool isTrue)
        {
            _isVisible = isTrue;

            if (_isVisible)
                _buildingCoinsUI.Show();
            else
                _buildingCoinsUI.Hide();
        }

        private void ExitDelay(string uniqueId)
        {
            if (_uniqueId.Id.Contains(uniqueId) && _isVisible)
                ShowHints();
        }
    }
}