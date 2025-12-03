using System;
using System.Collections;
using BuildProcessManagement;
using DG.Tweening;
using HealthSystem;
using Infastructure;
using Player.Orders;
using Units.Animators;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.StrategyBehaviour.HealManagement
{
    public class HealBuilding : IHealBuilding
    {
        private readonly Transform _unitTransform;
        private readonly UnitFlip _unitFlip;
        private readonly UnitStatus _unitStatus;
        private readonly BuilderActionHandler _builderActionHandler;
        private readonly BuilderAnimator _unitAnimator;
        private readonly UnitStateMachineView _unitStateMachineView;
        private readonly ICoroutineRunner _coroutineRunner;

        private OrderMarker _orderMarker;
        private Coroutine _healCoroutine;
        private HealingProgress _healingProgress;

        private BuildingHealth _health;
        private Tween _moveTween;
        private Action<OrderMarker> _onOrderCompleted;
        private Action _onContinueOrderHappened;

        public HealBuilding(
            Transform unitTransform,
            UnitFlip unitFlip,
            UnitStatus unitStatus,
            BuilderActionHandler builderActionHandler,
            BuilderAnimator unitAnimator,
            UnitStateMachineView unitStateMachineView,
            ICoroutineRunner coroutineRunner)
        {
            _unitTransform = unitTransform;
            _unitFlip = unitFlip;
            _unitStatus = unitStatus;
            _builderActionHandler = builderActionHandler;
            _unitAnimator = unitAnimator;
            _unitStateMachineView = unitStateMachineView;
            _coroutineRunner = coroutineRunner;
        }


        public void DoAction(
            OrderMarker orderMarker,
            float speed,
            int freePlaceIndex,
            Action<OrderMarker> onOrderCompleted,
            Action onContinueOrderHappened)
        {
            InitAndGetStatusHil(orderMarker, freePlaceIndex);
            _onOrderCompleted = onOrderCompleted;
            _onContinueOrderHappened = onContinueOrderHappened;

            float targetPositionX = orderMarker.Places[freePlaceIndex].ChopPlace.position.x;
            float distance = Mathf.Abs(targetPositionX - _unitTransform.position.x);

            SetCorrectFlip(targetPositionX);
            SetMove(speed, targetPositionX, distance);
        }

        public void StopAction()
        {
            if (_healCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_healCoroutine);
                _healCoroutine = null;
            }

            _moveTween?.Kill();

            if (_orderMarker != null)
            {
                SpeachBubleOrderUpdater speachBubleUpdater =
                    _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>();
                speachBubleUpdater.DisableSpeachBuble();

                ReleaseUnit();

                if (_healingProgress.HealIsFinished())
                {
                    _orderMarker.OrderID = OrderID.Build;

                    _onOrderCompleted.Invoke(_orderMarker);
                    //_onContinueOrderHappened.Invoke();
                }

                _orderMarker = null;
            }


            _unitAnimator.SetWorkingStateAnimation(false);
        }

        private void InitAndGetStatusHil(OrderMarker orderMarker, int freePlaceIndex)
        {
            _healingProgress = orderMarker.GetComponent<HealingProgress>();
            _health = orderMarker.GetComponentInChildren<BuildingHealth>();

            _builderActionHandler.OnWorkHappened += Heal;

            UniqueId uniqueId = orderMarker.GetComponent<UniqueId>();

            _orderMarker = orderMarker;
            _orderMarker.Places[freePlaceIndex].IsBusy = true;
            _orderMarker.IsStarted = true;

            _unitStatus.FreePlaceIndex = freePlaceIndex;
            _unitStatus.IsWorked = true;
            _unitStatus.OrderUniqueId = uniqueId.Id;
        }

        private void SetCorrectFlip(float targetPositionX)
        {
            float distanceDirection = targetPositionX - _unitTransform.position.x;
            _unitFlip.SetFlip(distanceDirection);
        }

        private void SetMove(float speed, float targetPositionX, float distance)
        {
            _unitAnimator.ResetAllAnimations();
            _unitAnimator.SetRunAnimation(true);

            _moveTween = _unitTransform
                .DOMoveX(targetPositionX, distance / speed).SetEase(Ease.Linear).OnUpdate(() =>
                {
                    if (_orderMarker == null || _orderMarker.OrderID == OrderID.Build)
                        RedefinitionOrder();
                })
                .OnComplete(() =>
                {
                    _unitAnimator.SetRunAnimation(false);
                    _healCoroutine = _coroutineRunner.StartCoroutine(StartHealing());
                });
        }

        private void RedefinitionOrder()
        {
            if (_healCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_healCoroutine);
                _healCoroutine = null;
            }

            _unitStatus.IsWorked = false;
            _unitStatus.OrderUniqueId = string.Empty;
            _unitStateMachineView.ChangeState<WalkState>();

            _moveTween.Kill();
            _unitTransform.DOKill();

            _onContinueOrderHappened.Invoke();
        }


        private IEnumerator StartHealing()
        {
            SpeachBubleOrderUpdater speachBubleUpdater =
                _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>();
            speachBubleUpdater.DisableSpeachBuble();

            _unitFlip.SetFlip(_orderMarker.Places[_unitStatus.FreePlaceIndex].Flip);

            _healingProgress.PlayHealFx();
            _unitAnimator.SetWorkingStateAnimation(true);

            while (!_healingProgress.HealIsFinished())
                yield return null;

            //_unitAnimator.SetWorkingStateSuccessfullyСompleted();

            float timeDelay = Random.Range(0.25f, 0.5f);
            yield return new WaitForSeconds(timeDelay);

            _unitStateMachineView.ChangeState<WalkState>();

            Finish();
            ReleaseUnit();

            _orderMarker.OrderID = OrderID.Build;
            _onOrderCompleted.Invoke(_orderMarker);
            _onContinueOrderHappened.Invoke();
        }

        private void Finish()
        {
            _healingProgress.RefreshHeal();

            IHealth health = _orderMarker.GetComponentInChildren<IHealth>();
            health.Reset();
        }

        private void ReleaseUnit()
        {
            _orderMarker.Places[_unitStatus.FreePlaceIndex].IsBusy = false;
            _orderMarker.IsStarted = false;
            _unitStatus.IsWorked = false;
            _unitStatus.OrderUniqueId = string.Empty;

            _unitAnimator.SetWorkingStateAnimation(false);

            _healingProgress.StopHealFx();
            _builderActionHandler.OnWorkHappened -= Heal;
        }

        private void Heal() =>
            _health.HealBuilding(10);
    }
}