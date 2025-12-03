using System;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.PurchaseDelay;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Workshop;
using UI.GameplayUI.BuildingCoinsUIManagement;
using Units;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class WorkshopReset : MonoBehaviour
    {
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;

        private IStaticDataService _staticDataService;
        private IPersistentProgressService _progressService;
        private IPurchaseDelayService _purchaseDelayService;

        private WorkshopStaticData _workshopData;
        private UniqueId _uniqueId;

        [Inject]
        public void Construct(
            IStaticDataService staticDataService,
            IPurchaseDelayService purchaseDelayService,
            IPersistentProgressService progressService)
        {
            _staticDataService = staticDataService;
            _progressService = progressService;
            _purchaseDelayService = purchaseDelayService;
        }

        private void Awake() =>
            _uniqueId = GetComponent<UniqueId>();

        private void Start() =>
            _workshopData = _staticDataService.ForWorkshop(WorkshopItemId.Reset);

        public void SpendCoins()
        {
            _purchaseDelayService.AddDelay(_uniqueId.Id);

            _buildingCoinsUI.PlaySpendAnimation(_workshopData.CoinsValue, transform);
            _buildingCoinsUI.Hide();

            _progressService.PlayerProgress.CoinData.Spend(_workshopData.CoinsValue);
        }
    }
}