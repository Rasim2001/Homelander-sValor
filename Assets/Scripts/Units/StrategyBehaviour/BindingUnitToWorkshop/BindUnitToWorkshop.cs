using System;
using System.Collections;
using BuildProcessManagement.WorkshopBuilding;
using DG.Tweening;
using Infastructure;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Unit;
using Units.UnitStates;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.StrategyBehaviour.BindingUnitToWorkshop
{
    public class BindUnitToWorkshop : IBindUnitToWorkshop
    {
        private readonly IWorkshopService _workshopService;
        private readonly IUnitsRecruiterService _unitsRecruiterService;
        private readonly ICoroutineRunner _coroutineRunner;

        private readonly UnitStaticData _unitStaticData;
        private readonly HomelessBehaviour _homelessBehaviour;
        private readonly Transform _unitTransform;
        private readonly UnitFlip _unitFlip;
        private readonly UnitStateMachineView _unitStateMachineView;
        private readonly UnitStatus _unitStatus;

        private Coroutine _tryBindCoroutine;
        private Workshop _workshop;
        private Action _onCompleted;

        public BindUnitToWorkshop(
            UnitStaticData unitStaticData,
            IWorkshopService workshopService,
            IUnitsRecruiterService unitsRecruiterService,
            Transform unitTransform,
            UnitStatus unitStatus,
            UnitFlip unitFlip,
            UnitStateMachineView unitStateMachineView,
            ICoroutineRunner coroutineRunner)
        {
            _unitStaticData = unitStaticData;
            _workshopService = workshopService;
            _unitsRecruiterService = unitsRecruiterService;
            _unitTransform = unitTransform;
            _unitFlip = unitFlip;
            _unitStateMachineView = unitStateMachineView;
            _unitStatus = unitStatus;
            _coroutineRunner = coroutineRunner;
        }

        public void StopAction()
        {
            _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>()?.DisableSpeachBuble();
            _unitStatus.IsWorked = false;

            _unitTransform.DOKill();
        }

        public void DoAction(IHomelessOrder order, float targetFreePositionX, Action onCompleted)
        {
            Workshop workshop = order as Workshop;

            UniqueId uniqueId = workshop.GetComponent<UniqueId>();
            _unitStatus.OrderUniqueId = uniqueId.Id;

            _onCompleted = onCompleted;
            _workshop = workshop;

            _unitStatus.IsWorked = true;

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
                    if (!_workshop.HasVendor)
                        BecomeVendor();
                    else
                        GetProfession();

                    _onCompleted?.Invoke();
                });
        }

        private void BecomeVendor()
        {
            _workshop.HasVendor = true;
            _workshop.CreateVendor();
        }


        private void GetProfession()
        {
            if (_workshop.IsEmpty())
                return;

            _workshop.ReduceIndex();
            _workshop.ReduceItemsAmount();

            WorkshopInfo workshopInfo = _workshop.GetComponent<WorkshopInfo>();

            GameObject unitObject = _workshopService.SpawnUnitWithProfession(workshopInfo.WorkshopItemId);
            unitObject.transform.position = _unitTransform.position;


            if (_tryBindCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_tryBindCoroutine);
                _tryBindCoroutine = null;
            }

            _tryBindCoroutine = _coroutineRunner.StartCoroutine(TryBindCoroutine(unitObject));
        }

        private void TryBindToPlayer(GameObject newUnit, bool previousUnitBindedToPlayer)
        {
            if (previousUnitBindedToPlayer)
            {
                UnitStatus unitStatus = newUnit.GetComponent<UnitStatus>();

                _unitsRecruiterService.AddUnitToList(unitStatus);
                _unitsRecruiterService.BindUnitToPlayer(unitStatus);
                _unitsRecruiterService.RelocateRemainingUnitsToPlayer();
            }
            else
            {
                UnitStateMachineView unitStateMachineView = newUnit.GetComponent<UnitStateMachineView>();
                unitStateMachineView.ChangeState<WalkState>();
            }
        }

        private IEnumerator TryBindCoroutine(GameObject unitObject)
        {
            HomelessBehaviour behaviourForDelete = _unitTransform.GetComponentInChildren<HomelessBehaviour>();
            BoxCollider2D homelessCollider = behaviourForDelete.GetComponent<BoxCollider2D>();
            bool previousUnitBindedToPlayer = PreviousUnitBindedToPlayer(homelessCollider);

            yield return new WaitForSeconds(0.5f);

            TryBindToPlayer(unitObject, previousUnitBindedToPlayer);
        }

        private bool PreviousUnitBindedToPlayer(BoxCollider2D homelessCollider) =>
            homelessCollider.enabled == false;
    }
}