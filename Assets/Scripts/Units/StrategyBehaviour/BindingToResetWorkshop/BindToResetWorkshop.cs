using System;
using BuildProcessManagement.WorkshopBuilding;
using DG.Tweening;
using Infastructure.Services.UnitEvacuationService;
using Infastructure.Services.UnitRecruiter;
using Infastructure.Services.UnitTrackingService;
using Infastructure.StaticData.Unit;
using Units.UnitStates;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.StrategyBehaviour.BindingToResetWorkshop
{
    public class BindToResetWorkshop : IBindToResetWorkshop
    {
        private readonly IWorkshopService _workshopService;
        private readonly IUnitsRecruiterService _unitsRecruiterService;
        private readonly Transform _unitTransform;
        private readonly UnitStatus _unitStatus;
        private readonly UnitFlip _unitFlip;
        private readonly UnitStateMachineView _unitStateMachineView;
        private readonly UnitStaticData _unitStaticData;

        private Action _onCompleted;


        public BindToResetWorkshop(IWorkshopService workshopService,
            IUnitsRecruiterService unitsRecruiterService,
            Transform unitTransform,
            UnitStatus unitStatus,
            UnitFlip unitFlip,
            UnitStateMachineView unitStateMachineView,
            UnitStaticData unitStaticData)
        {
            _workshopService = workshopService;
            _unitsRecruiterService = unitsRecruiterService;
            _unitTransform = unitTransform;
            _unitStatus = unitStatus;
            _unitFlip = unitFlip;
            _unitStateMachineView = unitStateMachineView;
            _unitStaticData = unitStaticData;
        }

        public void StopAction() =>
            _unitTransform.DOKill();

        public void DoAction(float targetFreePositionX, Action onCompleted)
        {
            _onCompleted = onCompleted;

            float speed = _unitStaticData.RunSpeed;
            float distance = Mathf.Abs(targetFreePositionX - _unitTransform.position.x);

            SetCorrectFlip(targetFreePositionX);
            SetMove(speed, targetFreePositionX, distance);
        }

        private void SetCorrectFlip(float targetPositionX)
        {
            bool flipValue = targetPositionX - _unitTransform.position.x < 0;
            _unitFlip.SetFlip(flipValue);
        }

        private void SetMove(float speed, float targetPositionX, float distance)
        {
            _unitStateMachineView.ChangeState<RunDefaultState>();

            _unitTransform
                .DOMoveX(targetPositionX, distance / speed).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    GameObject unitObject = CreateHomeless();
                    TryBindToPlayer(unitObject);
                    DestroyUnit();

                    _onCompleted?.Invoke();
                });
        }


        private GameObject CreateHomeless()
        {
            GameObject unitObject = _workshopService.SpawnUnitWithProfession(WorkshopItemId.Reset);
            unitObject.transform.position = _unitTransform.position;

            return unitObject;
        }

        private void TryBindToPlayer(GameObject newUnit)
        {
            UnitStatus unitStatus = newUnit.GetComponent<UnitStatus>();

            _unitsRecruiterService.AddUnitToList(unitStatus);
            _unitsRecruiterService.BindUnitToPlayer(unitStatus);
            _unitsRecruiterService.RelocateRemainingUnitsToPlayer();
        }

        private void DestroyUnit()
        {
            UnitDeath unitDeath = _unitStatus.GetComponent<UnitDeath>();
            unitDeath.DeathFromResetWorkshop();
        }
    }
}