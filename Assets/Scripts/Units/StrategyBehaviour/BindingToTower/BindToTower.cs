using System;
using BuildProcessManagement.Towers;
using BuildProcessManagement.Towers.SpawnOnTowers;
using DG.Tweening;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.StaticData.Unit;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.StrategyBehaviour.BindingToTower
{
    public class BindToTower : IBindToTower
    {
        private readonly UnitStaticData _unitStaticData;
        private readonly Transform _unitTransform;
        private readonly UnitStatus _unitStatus;
        private readonly UnitFlip _unitFlip;
        private readonly UnitStateMachineView _unitStateMachineView;

        public BindToTower(
            UnitStaticData unitStaticData,
            Transform unitTransform,
            UnitStatus unitStatus,
            UnitFlip unitFlip,
            UnitStateMachineView unitStateMachineView)
        {
            _unitStaticData = unitStaticData;
            _unitTransform = unitTransform;
            _unitStatus = unitStatus;
            _unitFlip = unitFlip;
            _unitStateMachineView = unitStateMachineView;
        }

        public void StopAction()
        {
            _unitStatus.GetComponentInChildren<SpeachBubleOrderUpdater>().DisableSpeachBuble();

            _unitStatus.IsWorked = false;
            _unitTransform.DOKill();
        }

        public void DoAction(IHomelessOrder order, float targetFreePositionX, Action onCompleted)
        {
            TowerUnitSpawner towerUnitSpawner = order as TowerUnitSpawner;

            UniqueId uniqueId = towerUnitSpawner.GetComponent<UniqueId>();
            _unitStatus.OrderUniqueId = uniqueId.Id;

            float speed = _unitStaticData.RunSpeed; //TODO:
            float directionDistanceToMove = targetFreePositionX - _unitTransform.position.x;
            float distance = Mathf.Abs(targetFreePositionX - _unitTransform.position.x);

            _unitStateMachineView.ChangeState<RunDefaultState>();
            _unitFlip.SetFlip(directionDistanceToMove);

            _unitStatus.IsWorked = true;

            _unitTransform.DOKill();
            _unitTransform
                .DOMoveX(targetFreePositionX, distance / speed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                    {
                        towerUnitSpawner.SpawnUnit();
                        onCompleted?.Invoke();
                    }
                );
        }
    }
}