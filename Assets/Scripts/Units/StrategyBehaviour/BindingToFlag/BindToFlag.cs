using System;
using DG.Tweening;
using Flag;
using Infastructure.StaticData.Unit;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.StrategyBehaviour.BindingToFlag
{
    public class BindToFlag : IBindToFlag
    {
        private readonly Transform _unitTransform;
        private readonly UnitFlip _unitFlip;
        private readonly UnitStatus _unitStatus;
        private readonly UnitStateMachineView _unitStateMachineView;
        private readonly UnitStaticData _unitStaticData;

        private FlagSlotCoordinator _flagSlotCoordinator;

        public BindToFlag(Transform unitTransform, UnitFlip unitFlip, UnitStatus unitStatus,
            UnitStateMachineView unitStateMachineView, UnitStaticData unitStaticData)
        {
            _unitTransform = unitTransform;
            _unitFlip = unitFlip;
            _unitStatus = unitStatus;
            _unitStateMachineView = unitStateMachineView;
            _unitStaticData = unitStaticData;
        }

        public void StopAction()
        {
            if (_flagSlotCoordinator != null)
            {
                _flagSlotCoordinator.UnBindUnit(_unitTransform, _unitStatus.UnitTypeId);
                _flagSlotCoordinator.RelocateUnits();
                _flagSlotCoordinator = null;
            }

            _unitStatus.IsWorked = false;
            _unitTransform.DOKill();
        }

        public void DoAction(FlagSlotCoordinator flagSlotCoordinator, float targetFreePositionX, int flipSideValue,
            Action onCompleted)
        {
            if (Mathf.Approximately(_unitTransform.position.x, targetFreePositionX))
            {
                onCompleted?.Invoke();
                return;
            }

            float speed = _unitStaticData.RunSpeed;
            float directionDistanceToMove = targetFreePositionX - _unitTransform.position.x;
            float distance = Mathf.Abs(targetFreePositionX - _unitTransform.position.x);

            _unitTransform.DOKill();

            _flagSlotCoordinator = flagSlotCoordinator;
            _unitStatus.IsWorked = true;

            _unitStateMachineView.ChangeState<RunDefaultState>();

            _unitFlip.SetFlip(directionDistanceToMove);

            _unitTransform
                .DOMoveX(targetFreePositionX, distance / speed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _unitStateMachineView.ChangeState<IdleDefaultState>();
                    _unitFlip.SetFlip(flipSideValue);

                    onCompleted?.Invoke();
                });
        }
    }
}