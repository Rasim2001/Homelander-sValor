using System;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.PurchaseDelay;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Workshop;
using UI.GameplayUI;
using UI.GameplayUI.BuildingCoinsUIManagement;
using Units;
using Units.UnitStates;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class Workshop : MonoBehaviour, IHomelessOrder
    {
        [SerializeField] private CurtainWorkshop _curtainWorkshop;
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private WorkshopHintsDisplay _workshopHintsDisplay;
        [SerializeField] private BuildingCoinsUI _buildingCoinsUI;
        [SerializeField] Transform _vendorPositionPoint;
        [SerializeField] private WorkshopInfo _workshopInfo;

        public bool HasVendor { get; set; }
        public Action OnCreateVendorHappened;

        private IWorkshopItemCreator _workshopItemCreator;
        private IWorkshopService _workshopService;
        private IPersistentProgressService _progressService;
        private IStaticDataService _staticDataService;
        private IHomelessOrdersService _homelessOrdersService;
        private IPurchaseDelayService _purchaseDelayService;

        private int _index;
        private Coroutine _coroutine;
        private Animator _vendorAnimator;

        [Inject]
        public void Construct(
            IWorkshopService workshopService,
            IStaticDataService staticDataService,
            IPersistentProgressService progressService,
            IHomelessOrdersService homelessOrdersService,
            IPurchaseDelayService purchaseDelayService)
        {
            _workshopService = workshopService;
            _staticDataService = staticDataService;
            _progressService = progressService;
            _homelessOrdersService = homelessOrdersService;
            _purchaseDelayService = purchaseDelayService;
        }

        private void Awake() =>
            _workshopItemCreator = _workshopService.Initialize(_workshopInfo.WorkshopItemId);


        public int NumberOfOrders() =>
            HasVendor ? _index : 1;

        public bool HasAvailableSlot()
        {
            if (HasVendor)
                return !IsEmpty();

            return !HasVendor;
        }

        public void CreateVendor()
        {
            WorkshopStaticData workshopData = _staticDataService.ForWorkshop(_workshopInfo.WorkshopItemId);

            _buildingCoinsUI.UpdateCoinsUI(workshopData.CoinsValue);
            _workshopHintsDisplay.UpdateHints();

            GameObject vendorObject =
                _workshopService.CreateVendor(_vendorPositionPoint.position, _workshopInfo.VendorTypeId);

            _vendorAnimator = vendorObject.GetComponent<Animator>();
            OnCreateVendorHappened?.Invoke();
        }

        public void SpawnItem()
        {
            WorkshopStaticData workshopData = _staticDataService.ForWorkshop(_workshopInfo.WorkshopItemId);

            if (HasAvailableItem() && IsEnoughCoins(workshopData))
            {
                SpendCoins(workshopData);
                CreateItem();

                _homelessOrdersService.ContinueExecuteOrders();
            }
        }

        public void ReduceItemsAmount() =>
            _workshopItemCreator.Reduce();

        public void ReduceIndex() =>
            _index--;

        public bool IsEmpty() =>
            _index <= 0;

        private void CreateItem()
        {
            _workshopItemCreator.CreateItem(_workshopInfo.TargetsPoint[_index].position);
            _index++;

            _purchaseDelayService.AddDelay(_uniqueId.Id);
            _vendorAnimator.Play(UnitStatesPath.VendorHappyHash, 0, 0);
        }

        private void SpendCoins(WorkshopStaticData workshopData)
        {
            _buildingCoinsUI.PlaySpendAnimation(workshopData.CoinsValue, transform);
            _buildingCoinsUI.Hide();

            _progressService.PlayerProgress.CoinData.Spend(workshopData.CoinsValue);
        }


        private bool HasAvailableItem() =>
            _index < _workshopInfo.TargetsPoint.Count;

        private bool IsEnoughCoins(WorkshopStaticData workshopData) =>
            _progressService.PlayerProgress.CoinData.IsEnoughCoins(workshopData.CoinsValue);
    }
}