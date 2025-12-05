using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.UnitEvacuationService;
using Infastructure.Services.UnitRecruiter;
using Infastructure.Services.UnitTrackingService;
using Infastructure.StaticData.Unit;
using Player;
using Units.HomelessUnits;
using Units.StrategyBehaviour;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStates.VagabondStates;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace Units
{
    public class UnitDeath : MonoBehaviour
    {
        [SerializeField] private UnitStatus _unitStatus;
        [SerializeField] private Health _health;

        private IGameFactory _gameFactory;
        private IFutureOrdersService _futureOrdersService;
        private IEvacuationService _evacuationService;
        private IUnitsTrackerService _unitsTrackerService;
        private IUnitsRecruiterService _unitsRecruiterService;

        [Inject]
        public void Construct(IGameFactory gameFactory, IFutureOrdersService futureOrdersService,
            IEvacuationService evacuationService, IUnitsTrackerService unitsTrackerService,
            IUnitsRecruiterService unitsRecruiterService)
        {
            _unitsRecruiterService = unitsRecruiterService;
            _gameFactory = gameFactory;
            _futureOrdersService = futureOrdersService;
            _evacuationService = evacuationService;
            _unitsTrackerService = unitsTrackerService;
        }

        private void Start() =>
            _health.OnDeathHappened += Death;

        private void OnDestroy() =>
            _health.OnDeathHappened -= Death;

        private void Death()
        {
            /*GameObject unit = _gameFactory.CreateUnit(UnitTypeId.Vagabond);
            unit.transform.position = transform.position;

            VagabondStateMachineView vagabondStateMachineView = unit.GetComponent<VagabondStateMachineView>();
            vagabondStateMachineView.ChangeState<ScaryRunVagabondState>();*/

            _unitsRecruiterService.RemoveUnitFromAllLists(_unitStatus);

            if (_unitStatus.UnitTypeId == UnitTypeId.Builder)
                _futureOrdersService.RemoveBuilder(_unitStatus);

            _evacuationService.RemoveUnit(_unitStatus);
            _unitsTrackerService.RemoveUnit(_unitStatus.UnitTypeId);

            UnitStrategyBehaviour unitStrategyBehaviour = _unitStatus.GetComponentInChildren<UnitStrategyBehaviour>();
            unitStrategyBehaviour.StopAllActions();

            Destroy(gameObject);
        }

        public void DeathFromResetWorkshop()
        {
            if (_unitStatus.UnitTypeId == UnitTypeId.Builder)
                _futureOrdersService.RemoveBuilder(_unitStatus);

            _evacuationService.RemoveUnit(_unitStatus);
            _unitsTrackerService.RemoveUnit(_unitStatus.UnitTypeId);

            Destroy(gameObject);
        }
    }
}