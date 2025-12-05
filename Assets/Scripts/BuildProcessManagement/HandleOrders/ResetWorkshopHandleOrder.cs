using BuildProcessManagement.WorkshopBuilding;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.Unit;
using Player;
using Player.Orders;
using UI.GameplayUI.SpeachBubleUI;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class ResetWorkshopHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private WorkshopReset _workshop;
        [SerializeField] private OrderMarker _orderMarker;

        private IPlayerRegistryService _playerRegistryService;
        private IUnitsRecruiterService _unitsRecruiterService;

        private SelectUnitArrow _selectUnitArrow;
        private SpeachBuble _speachBuble;

        [Inject]
        public void Construct(
            IPlayerRegistryService playerRegistryService,
            IUnitsRecruiterService unitsRecruiterService)
        {
            _unitsRecruiterService = unitsRecruiterService;
            _playerRegistryService = playerRegistryService;
        }

        private void Awake()
        {
            _speachBuble = _playerRegistryService.Player.GetComponentInChildren<SpeachBuble>();
            _selectUnitArrow = _playerRegistryService.Player.GetComponentInChildren<SelectUnitArrow>();
        }


        public void Handle()
        {
            int correctSelectableUnitIndex = _selectUnitArrow.IsActive()
                ? _selectUnitArrow.SelectableUnitIndex - 1
                : _selectUnitArrow.SelectableUnitIndex;

            UnitTypeId unitType = _unitsRecruiterService.GetUnitType(correctSelectableUnitIndex);

            if (unitType == UnitTypeId.Homeless || unitType == UnitTypeId.Unknow)
                _speachBuble.UpdateSpeach(SpeachBubleId.InvalidHomeless);
            else
            {
                UnitStatus unitStatus =
                    _unitsRecruiterService.ReleaseUnit(UnitTypeId.Anyone, correctSelectableUnitIndex);

                if (unitStatus != null)
                {
                    UnitStrategyBehaviour unitStrategyBehaviour =
                        unitStatus.GetComponentInChildren<UnitStrategyBehaviour>();
                    unitStrategyBehaviour.PlayResetBehavior(_workshop.transform.position.x);

                    _workshop.SpendCoins();
                    _selectUnitArrow.UnSelectUnit();
                }
            }
        }
    }
}