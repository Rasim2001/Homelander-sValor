using System;
using BuildProcessManagement.WorkshopBuilding;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Unit;
using Player;
using Player.Orders;
using UI.GameplayUI.SpeachBubleUI;
using UI.Windows;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class WorkshopHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private CurtainWorkshop _curtainWorkshop;
        [SerializeField] private OrderMarker _orderMarker;
        [SerializeField] private Workshop _workshop;

        private IPlayerRegistryService _playerRegistryService;
        private IUnitsRecruiterService _unitsRecruiterService;

        private SelectUnitArrow _selectUnitArrow;
        private IHomelessOrdersService _homelessOrdersService;

        [Inject]
        public void Construct(
            IPlayerRegistryService playerRegistryService,
            IUnitsRecruiterService unitsRecruiterService,
            IHomelessOrdersService homelessOrdersService)
        {
            _homelessOrdersService = homelessOrdersService;
            _unitsRecruiterService = unitsRecruiterService;
            _playerRegistryService = playerRegistryService;
        }

        private void Awake() =>
            _selectUnitArrow = _playerRegistryService.Player.GetComponentInChildren<SelectUnitArrow>();

        public void Handle()
        {
            if (_orderMarker.IsMarkered || _curtainWorkshop.IsShowed)
                return;

            GiveOrderToHomeless();
        }

        private void GiveOrderToHomeless()
        {
            if (_selectUnitArrow.IsActive())
            {
                int correctIndex = _selectUnitArrow.SelectableUnitIndex - 1;
                UnitTypeId unitType = _unitsRecruiterService.GetUnitType(correctIndex);

                if (unitType == UnitTypeId.Homeless)
                {
                    if (_workshop.HasVendor && (_workshop.IsEmpty() || correctIndex == -1))
                        _workshop.SpawnItem();

                    if (correctIndex != -1)
                        BindToWorkshop(correctIndex);

                    _selectUnitArrow.UnSelectUnit();
                }
            }
            else
            {
                int index = _unitsRecruiterService.FindUnitIndexByType(UnitTypeId.Homeless);

                if (_workshop.HasVendor && (_workshop.IsEmpty() || index == -1))
                    _workshop.SpawnItem();

                if (index != -1)
                    BindToWorkshop(index);
            }

            _unitsRecruiterService.RelocateRemainingUnitsToPlayer();
        }

        private void BindToWorkshop(int correctIndex)
        {
            UnitStatus unitStatus = _unitsRecruiterService.ReleaseUnit(UnitTypeId.Anyone, correctIndex);

            SpriteRenderer homelessSpriteRender = unitStatus.GetComponent<SpriteRenderer>();
            homelessSpriteRender.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            HomelessBehaviour homelessBehaviour =
                unitStatus.GetComponentInChildren<HomelessBehaviour>();

            homelessBehaviour
                .PlayHomelessOrderBehavior(_workshop, _workshop.transform.position.x,
                    () => { _homelessOrdersService.CompleteOrder(_workshop, unitStatus); });
        }
    }
}