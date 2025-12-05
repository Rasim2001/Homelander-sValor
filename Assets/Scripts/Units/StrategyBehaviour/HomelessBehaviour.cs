using System;
using System.Collections.Generic;
using BuildProcessManagement.Towers.SpawnOnTowers;
using BuildProcessManagement.WorkshopBuilding;
using Infastructure;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Units.StrategyBehaviour.BindingToTower;
using Units.StrategyBehaviour.BindingUnitToWorkshop;
using Zenject;

namespace Units.StrategyBehaviour
{
    public class HomelessBehaviour : UnitStrategyBehaviour
    {
        private IBindUnitToWorkshop _becomingVendor;
        private IBindToTower _bindToTower;
        private ICoroutineRunner _coroutineRunner;

        private Dictionary<Type, IHomelessBehavior> _homelessBehaviors;
        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(ICoroutineRunner coroutineRunner, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _coroutineRunner = coroutineRunner;
        }

        public override void StopAllActions()
        {
            base.StopAllActions();

            unitStatus.OrderUniqueId = string.Empty;

            _becomingVendor.StopAction();
            _bindToTower.StopAction();
        }

        public override void Awake()
        {
            base.Awake();

            UnitStaticData unitStaticData = _staticDataService.ForUnit(UnitTypeId.Homeless);

            _becomingVendor = new BindUnitToWorkshop(
                unitStaticData,
                WorkshopService,
                UnitsRecruiterService,
                unitTransform,
                unitStatus,
                unitFlip,
                stateMachine,
                _coroutineRunner);

            _bindToTower = new BindToTower(unitStaticData, unitTransform, unitStatus, unitFlip, stateMachine);

            _homelessBehaviors = new Dictionary<Type, IHomelessBehavior>()
            {
                { typeof(TowerUnitSpawner), _bindToTower },
                { typeof(Workshop), _becomingVendor },
            };
        }

        public void PlayHomelessOrderBehavior(IHomelessOrder order, float positionX, Action onCompleted = null)
        {
            if (_homelessBehaviors.TryGetValue(order.GetType(), out IHomelessBehavior behavior))
                behavior.DoAction(order, positionX, onCompleted);
        }
    }
}