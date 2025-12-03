using System;
using System.Collections;
using System.Collections.Generic;
using BuildProcessManagement;
using BuildProcessManagement.ResourceElements;
using DG.Tweening;
using Grid;
using Infastructure;
using Infastructure.Services.Pool;
using Infastructure.StaticData.RecourceElements;
using Infastructure.StaticData.StaticDataService;
using Loots;
using Player.Orders;
using UI.GameplayUI;
using Units.Animators;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Units.StrategyBehaviour.DigManagement
{
    public class Dig : IDig
    {
        private readonly Transform _unitTransform;
        private readonly UnitFlip _unitFlip;
        private readonly UnitStatus _unitStatus;
        private readonly BuilderActionHandler _builderActionHandler;
        private readonly BuilderAnimator _unitAnimator;
        private readonly UnitStateMachineView _unitStateMachineView;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly List<OrderMarker> _ordersList = new List<OrderMarker>();

        private OrderMarker _orderMarker;
        private ResourceDestruction _recourseDestruction;
        private readonly IStaticDataService _staticDataService;
        private readonly IGridMap _gridMap;
        private Coroutine _finishCoroutine;
        private Action<OrderMarker> _onOrderCompleted;
        private Action _onContinueOrderHappened;

        private Coroutine _digCoroutine;
        private Tween _moveTween;
        private int _indexForDelete = -1;

        public Dig(
            Transform unitTransform,
            UnitFlip unitFlip,
            UnitStatus unitStatus,
            BuilderActionHandler builderActionHandler,
            BuilderAnimator unitAnimator,
            UnitStateMachineView unitStateMachineView,
            ICoroutineRunner coroutineRunner,
            IStaticDataService staticDataService,
            IGridMap gridMap)
        {
            _unitTransform = unitTransform;
            _unitFlip = unitFlip;
            _unitStatus = unitStatus;
            _builderActionHandler = builderActionHandler;
            _unitAnimator = unitAnimator;
            _unitStateMachineView = unitStateMachineView;
            _coroutineRunner = coroutineRunner;
            _staticDataService = staticDataService;
            _gridMap = gridMap;
        }


        public void DoAction(
            OrderMarker orderMarker,
            float speed,
            int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted,
            Action onContinueOrderHappened)
        {
            InitAndGetStatusDig(orderMarker, freePlaceIndex);

            _onOrderCompleted = onOrderCompleted;
            _onContinueOrderHappened = onContinueOrderHappened;

            float targetPositionX = orderMarker.Places[freePlaceIndex].ChopPlace.position.x;
            float distance = Mathf.Abs(targetPositionX - _unitTransform.position.x);

            SetCorrectFlip(targetPositionX);
            SetMove(speed, targetPositionX, distance);
        }


        public void StopAction()
        {
            if (_digCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_digCoroutine);
                _digCoroutine = null;
            }

            _moveTween?.Kill();


            if (_orderMarker != null)
            {
                SpeachBubleOrderUpdater speachBubleUpdater =
                    _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>();
                speachBubleUpdater.DisableSpeachBuble();

                ReleaseUnit();

                _orderMarker = null;
            }

            _unitAnimator.SetWorkingStateAnimation(false);
        }

        private void InitAndGetStatusDig(OrderMarker orderMarker, int freePlaceIndex)
        {
            _recourseDestruction = orderMarker.GetComponent<ResourceDestruction>();
            _builderActionHandler.OnWorkHappened += _recourseDestruction.UpdateDestructionProgress;

            _orderMarker = orderMarker;

            if (!_ordersList.Contains(_orderMarker))
                _ordersList.Add(_orderMarker);

            UniqueId uniqueId = orderMarker.GetComponent<UniqueId>();

            if (orderMarker.TryGetComponent(out ResourceHintsDisplay hintsDisplay))
                hintsDisplay.HideHints();

            _orderMarker.Places[freePlaceIndex].IsBusy = true;
            _unitStatus.FreePlaceIndex = freePlaceIndex;
            _unitStatus.IsWorked = true;
            _unitStatus.OrderUniqueId = uniqueId.Id;
        }

        private IEnumerator StartDig()
        {
            SpeachBubleOrderUpdater speachBubleUpdater =
                _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>();
            speachBubleUpdater.DisableSpeachBuble();

            _unitFlip.SetFlip(_orderMarker.Places[_unitStatus.FreePlaceIndex].Flip);

            _unitAnimator.SetWorkingStateAnimation(true);

            while (!_recourseDestruction.IsDestroyed())
                yield return null;

            //_unitAnimator.SetWorkingStateSuccessfullyСompleted();

            float timeDelay = Random.Range(0.25f, 0.5f);
            yield return new WaitForSeconds(timeDelay);

            _unitStateMachineView.ChangeState<WalkState>();

            ReleaseUnit();
            DestroyObject();
            _onContinueOrderHappened?.Invoke();
        }


        private void SetMove(float speed, float targetPositionX, float distance)
        {
            _unitAnimator.SetRunAnimation(true);

            _moveTween = _unitTransform
                .DOMoveX(targetPositionX, distance / speed).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _unitAnimator.SetRunAnimation(false);
                    _digCoroutine = _coroutineRunner.StartCoroutine(StartDig());
                });
        }


        private void SetCorrectFlip(float targetPositionX)
        {
            bool flipValie = targetPositionX - _unitTransform.position.x < 0;
            _unitFlip.SetFlip(flipValie);
        }


        private void ReleaseUnit()
        {
            _orderMarker.Places[_unitStatus.FreePlaceIndex].IsBusy = false;
            _unitStatus.IsWorked = false;
            _unitStatus.OrderUniqueId = string.Empty;

            _unitAnimator.SetWorkingStateAnimation(false);

            _builderActionHandler.OnWorkHappened -= _recourseDestruction.UpdateDestructionProgress;
        }

        private void DestroyObject()
        {
            _indexForDelete = _ordersList.IndexOf(_orderMarker);
            _onOrderCompleted.Invoke(_orderMarker);

            if (_indexForDelete == -1)
                return;

            OrderMarker currentOrderMarker = _ordersList[_indexForDelete];
            _ordersList.RemoveAt(_indexForDelete);

            ResourceInfo resourceInfo = currentOrderMarker.GetComponent<ResourceInfo>();
            ResourceData buildingData = _staticDataService.ForResource(resourceInfo.ResourceId);
            _gridMap.ClearCells((int)currentOrderMarker.transform.position.x, buildingData);

            DestroyResource destroyResource = currentOrderMarker.GetComponent<DestroyResource>();
            destroyResource.DestroyElement();
        }
    }
}