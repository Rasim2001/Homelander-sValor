using System;
using BuildProcessManagement.WorkshopBuilding;
using Flag;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.StrategyBehaviour.BindingToFlag;
using Units.StrategyBehaviour.BindingToResetWorkshop;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace Units.StrategyBehaviour
{
    public class UnitStrategyBehaviour : MonoBehaviour
    {
        [SerializeField] protected UnitAnimator unitAnimator;
        [SerializeField] protected UnitStateMachineView stateMachine;
        [SerializeField] protected Transform unitTransform;
        [SerializeField] protected UnitFlip unitFlip;
        [SerializeField] protected UnitStatus unitStatus;

        protected IWorkshopService WorkshopService;
        protected IUnitsRecruiterService UnitsRecruiterService;

        private IBindToFlag _bindToFlag;
        private IBindToResetWorkshop _bindToResetWorkshop;
        private IStaticDataService _staticDataService;


        [Inject]
        public void Construct(IWorkshopService workshopService, IUnitsRecruiterService unitsRecruiterService,
            IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            WorkshopService = workshopService;
            UnitsRecruiterService = unitsRecruiterService;
        }

        public virtual void Awake()
        {
            UnitStaticData unitStaticData = _staticDataService.ForUnit(unitStatus.UnitTypeId);

            _bindToFlag = new BindToFlag(unitTransform, unitFlip, unitStatus, stateMachine, unitStaticData);

            _bindToResetWorkshop = new BindToResetWorkshop(
                WorkshopService,
                UnitsRecruiterService,
                unitTransform,
                unitStatus,
                unitFlip,
                stateMachine,
                unitStaticData);
        }

        public virtual void StopAllActions() =>
            _bindToFlag.StopAction();

        public void PlayResetBehavior(float positionX, Action onCompleted = null) =>
            _bindToResetWorkshop.DoAction(positionX, onCompleted);

        public void PlayBindToFlagBehaviour(FlagSlotCoordinator flagSlotCoordinator, float positionX, int flipSideValue,
            Action OnCompleted = null) =>
            _bindToFlag.DoAction(flagSlotCoordinator, positionX, flipSideValue, OnCompleted);
    }
}