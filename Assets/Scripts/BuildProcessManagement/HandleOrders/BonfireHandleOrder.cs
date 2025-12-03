using System;
using Bonfire;
using Infastructure.Services.PlayerRegistry;
using Infastructure.StaticData.SpeachBuble.Player;
using UI.GameplayUI.BuildingCoinsUIManagement;
using UI.GameplayUI.SpeachBubleUI;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class BonfireHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;
        [SerializeField] private BonfireMarker _bonfireMarker;

        private IUpgradeMainFlag _upgradeMainFlag;
        private IPlayerRegistryService _playerRegistryService;

        private SpeachBuble _speachBuble;

        [Inject]
        public void Construct(IUpgradeMainFlag upgradeMainFlag, IPlayerRegistryService playerRegistryService)
        {
            _playerRegistryService = playerRegistryService;
            _upgradeMainFlag = upgradeMainFlag;
        }

        private void Awake() =>
            _speachBuble = _playerRegistryService.Player.GetComponentInChildren<SpeachBuble>();


        public void Handle()
        {
            if (!_upgradeMainFlag.HasUpgrade())
                return;

            if (!_upgradeMainFlag.IsEnoughCoins())
                _speachBuble.UpdateSpeach(SpeachBubleId.Coins);
            else
                _upgradeMainFlag.Upgrade(_bonfireMarker, _buildingCoinsUI);
        }
    }
}