using Flag;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.SpeachBuble.Player;
using Infastructure.StaticData.Unit;
using Player;
using UI.GameplayUI.SpeachBubleUI;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class FlagslotHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private FlagSlotCoordinator _flagSlotCoordinator;

        private IPlayerRegistryService _playerRegistryService;
        private IUnitsRecruiterService _unitsRecruiterService;

        private SpeachBuble _speachBuble;
        private SelectUnitArrow _selectUnitArrow;

        [Inject]
        public void Construct(IPlayerRegistryService playerRegistryService, IUnitsRecruiterService unitsRecruiterService)
        {
            _playerRegistryService = playerRegistryService;
            _unitsRecruiterService = unitsRecruiterService;
        }

        public void Awake()
        {
            _speachBuble = _playerRegistryService.Player.GetComponentInChildren<SpeachBuble>();
            _selectUnitArrow = _playerRegistryService.Player.GetComponentInChildren<SelectUnitArrow>();
        }

        public void Handle()
        {
            BindToFlag();
            _unitsRecruiterService.RelocateRemainingUnitsToPlayer();
        }

        private void BindToFlag()
        {
            int correctSelectableUnitIndex = _selectUnitArrow.IsActive()
                ? _selectUnitArrow.SelectableUnitIndex - 1
                : _selectUnitArrow.SelectableUnitIndex;

            UnitTypeId unitType = _unitsRecruiterService.GetUnitType(correctSelectableUnitIndex);

            if (unitType == UnitTypeId.Homeless || unitType == UnitTypeId.Unknow)
            {
                _speachBuble.UpdateSpeach(SpeachBubleId.InvalidHomeless);
                return;
            }

            UnitStatus unitStatus =
                _unitsRecruiterService.ReleaseUnit(UnitTypeId.Anyone, correctSelectableUnitIndex);

            if (unitStatus != null)
            {
                _flagSlotCoordinator.BindUnitToSlot(unitStatus.transform, unitStatus.UnitTypeId);
                _flagSlotCoordinator.RelocateUnits();

                if (_flagSlotCoordinator.HasEnemiesAroundBarricade)
                    _flagSlotCoordinator.PrepareForDefense();

                _selectUnitArrow.UnSelectUnit();
            }
        }
    }
}