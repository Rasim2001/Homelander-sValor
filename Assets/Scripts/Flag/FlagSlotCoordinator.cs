using System;
using System.Collections.Generic;
using HealthSystem;
using Infastructure.Services.AutomatizationService.Builders;
using Infastructure.Services.Flag;
using Infastructure.StaticData.Unit;
using Player.Orders;
using Units;
using Units.Animators;
using Units.RangeUnits;
using Units.Shielder;
using Units.StrategyBehaviour;
using Units.UnitStates;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace Flag
{
    public class FlagSlotCoordinator : MonoBehaviour
    {
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private List<FlagSlotInfo> _flags;

        private readonly List<Transform> _unBindedBuilders = new List<Transform>();
        public UniqueId UniqueId => _uniqueId;

        private IFlagTrackerService _flagTrackerService;
        private IExecuteOrdersService _executeOrdersService;
        private IFlagDefenseHandler _defenseHandler;

        private int _flipSideValue;
        private int _waitableAmountOfBuilders;

        private OrderMarker _orderMarker;
        private BuildingHealth _buildingHealth;
        private IFutureOrdersService _futureOrdersService;

        public Action OnDestroyHappened;

        public bool HasEnemiesAroundBarricade { get; set; }


        [Inject]
        public void Construct(
            IFlagTrackerService flagTrackerService,
            IExecuteOrdersService executeOrdersService,
            IFlagDefenseHandler defenseHandler,
            IFutureOrdersService futureOrdersService)
        {
            _flagTrackerService = flagTrackerService;
            _executeOrdersService = executeOrdersService;
            _defenseHandler = defenseHandler;
            _futureOrdersService = futureOrdersService;
        }

        public void Initialize(OrderMarker orderMarker, BuildingHealth buildingHealth, int flipSideValue,
            float barricadePositionX)
        {
            _orderMarker = orderMarker;
            _buildingHealth = buildingHealth;
            _flipSideValue = flipSideValue;

            _flagTrackerService.RegisterFlag(transform, barricadePositionX);
        }

        public bool IsBarricadeDamaged() =>
            _buildingHealth.CurrentHP != _buildingHealth.MaxHp;


        public OrderMarker GetBarricadeOrderMarker() =>
            _orderMarker;

        private void OnDestroy()
        {
            OnDestroyHappened?.Invoke();

            _flagTrackerService.DeleteFlag(transform);

            if (IsBarricadeDamaged())
            {
                foreach (FlagSlotInfo flagSlotInfo in _flags)
                    _defenseHandler.StartRetreat(flagSlotInfo);
            }
            else
                ReleaseAll();
        }

        public void Relax()
        {
            foreach (FlagSlotInfo slotInfo in _flags)
            {
                for (int i = 0; i < slotInfo.BindedUnits.Count; i++)
                {
                    Animator animator = slotInfo.BindedUnits[i].GetComponent<Animator>();
                    animator.Play(UnitStatesPath.IdleHash);
                }
            }
        }


        public void RelocateUnits()
        {
            if (HasEnemiesAroundBarricade)
                return;

            float index = 0;

            foreach (FlagSlotInfo slotInfo in _flags)
            {
                foreach (Transform unitTransform in slotInfo.BindedUnits)
                {
                    CheckAttackRange attackRange = unitTransform.GetComponentInChildren<CheckAttackRange>();
                    if (attackRange != null)
                        attackRange.SetDefaultReachDistance();

                    _defenseHandler.RelocateAfterFight(unitTransform);

                    UnitStatus unitStatus = unitTransform.GetComponent<UnitStatus>();
                    unitStatus.IsDefensedFlag = false;

                    UnitAggressionMove unitAggressionMove = unitTransform.GetComponent<UnitAggressionMove>();
                    unitAggressionMove?.DisableMove();

                    UnitStrategyBehaviour unitStrategyBehaviour =
                        unitTransform.GetComponentInChildren<UnitStrategyBehaviour>();

                    float positionX = transform.position.x + index * _flipSideValue;
                    unitStrategyBehaviour.PlayBindToFlagBehaviour(this, positionX, _flipSideValue);

                    index += 0.4f;
                }
            }
        }


        public void BindUnitToSlot(Transform unitTransform, UnitTypeId typeId)
        {
            UnitStatus unitStatus = unitTransform.GetComponent<UnitStatus>();

            foreach (FlagSlotInfo slotInfo in _flags)
            {
                if (slotInfo.UnitTypeId == typeId && !slotInfo.BindedUnits.Contains(unitTransform))
                {
                    unitStatus.BindedToFlagUniqueId = UniqueId.Id;
                    slotInfo.BindedUnits.Add(unitTransform);
                }
            }
        }

        public void UnBindUnit(Transform unitTransform, UnitTypeId typeId)
        {
            UnitStatus unitStatus = unitTransform.GetComponent<UnitStatus>();

            foreach (FlagSlotInfo slotInfo in _flags)
            {
                if (slotInfo.UnitTypeId == typeId && slotInfo.BindedUnits.Contains(unitTransform))
                {
                    slotInfo.BindedUnits.Remove(unitTransform);
                    unitStatus.BindedToFlagUniqueId = string.Empty;
                    unitStatus.IsDefensedFlag = false;

                    if (typeId == UnitTypeId.Archer || typeId == UnitTypeId.Shielder)
                    {
                        UnitAggressionMove unitAggressionMove = unitStatus.GetComponent<UnitAggressionMove>();
                        unitAggressionMove.enabled = false;

                        unitAggressionMove.Release();
                        unitAggressionMove.SetDefaultReacherDistance();

                        UnitAttack unitAttack = unitStatus.GetComponent<UnitAttack>();
                        unitAttack.Release();
                    }
                }
            }
        }

        public void PrepareForDefense()
        {
            float offset = 0.5f;

            foreach (FlagSlotInfo slotInfo in _flags)
            {
                if (_unBindedBuilders.Count > 0)
                {
                    foreach (Transform unBindedBuilder in _unBindedBuilders)
                    {
                        if (unBindedBuilder == null)
                            continue;

                        UnitStrategyBehaviour unitStrategyBehaviour =
                            unBindedBuilder.GetComponentInChildren<UnitStrategyBehaviour>();

                        unitStrategyBehaviour.StopAllActions();

                        BindUnitToSlot(unBindedBuilder, UnitTypeId.Builder);
                        InitHealBuild();
                    }

                    _unBindedBuilders.Clear();
                }


                for (int i = 0; i < slotInfo.BindedUnits.Count; i++)
                {
                    offset += slotInfo.OffsetBetweenUnits;

                    UnitStatus unitStatus = slotInfo.BindedUnits[i].GetComponent<UnitStatus>();

                    if (unitStatus.IsDefensedFlag)
                        continue;

                    unitStatus.IsDefensedFlag = true;

                    _defenseHandler.RelocateAfterFight(slotInfo.BindedUnits[i]);

                    UnitStrategyBehaviour unitStrategyBehaviour =
                        slotInfo.BindedUnits[i].GetComponentInChildren<UnitStrategyBehaviour>();

                    UnitAttack unitAttack = slotInfo.BindedUnits[i].GetComponent<UnitAttack>();
                    unitAttack?.OnAttackEnded();

                    UnitAnimator unitAnimator = slotInfo.BindedUnits[i].GetComponent<UnitAnimator>();
                    unitAnimator.SetIdleAnimation(false);

                    float positionX = _orderMarker.transform.position.x + offset * _flipSideValue;

                    int savedIndex = i;
                    unitStrategyBehaviour.PlayBindToFlagBehaviour(this, positionX, _flipSideValue,
                        () => _defenseHandler.PrepareToDefense(slotInfo.BindedUnits[savedIndex]));
                }

                offset += slotInfo.OffsetNextGroups;
            }
        }


        public void HealBarricade()
        {
            if (_buildingHealth.MaxHp == _buildingHealth.CurrentHP)
                return;

            foreach (FlagSlotInfo slotInfo in _flags)
            {
                if (slotInfo.UnitTypeId == UnitTypeId.Builder)
                {
                    for (int i = 0; i < slotInfo.BindedUnits.Count; i++)
                    {
                        int freePlaceIndex = _executeOrdersService.FreePlaceIndex(_orderMarker);
                        if (freePlaceIndex == -1)
                            break;

                        Transform currentBuilder = slotInfo.BindedUnits[i];
                        BuilderBehaviour builderBehaviour =
                            currentBuilder.GetComponentInChildren<BuilderBehaviour>();

                        UnBindUnit(currentBuilder, UnitTypeId.Builder);
                        _unBindedBuilders.Add(currentBuilder);

                        _executeOrdersService.ExecuteOrder(builderBehaviour, _orderMarker, freePlaceIndex,
                            _ =>
                            {
                                currentBuilder.GetComponent<UnitMove>().enabled = false;

                                BindUnitToSlot(currentBuilder, UnitTypeId.Builder);
                                WaitAndRelocateUnits(1);

                                _futureOrdersService.RemoveCompletedOrder(_orderMarker);
                            }, _futureOrdersService.ContinueExecuteOrders);
                    }

                    RelocateUnits();
                }
            }
        }


        private void WaitAndRelocateUnits(int count)
        {
            _waitableAmountOfBuilders++;

            if (_waitableAmountOfBuilders == count)
            {
                _waitableAmountOfBuilders = 0;

                RelocateUnits();
                //InitHealBuild();
            }
        }

        public void ReleaseAll()
        {
            foreach (FlagSlotInfo slotInfo in _flags)
            {
                foreach (Transform unit in slotInfo.BindedUnits)
                {
                    UnitStatus unitStatus = unit.GetComponent<UnitStatus>();

                    unitStatus.BindedToFlagUniqueId = string.Empty;
                    unitStatus.IsDefensedFlag = false;
                    unitStatus.IsWorked = false;

                    if (unitStatus.UnitTypeId == UnitTypeId.Archer || unitStatus.UnitTypeId == UnitTypeId.Shielder)
                    {
                        UnitAggressionMove unitAggressionMove = unitStatus.GetComponent<UnitAggressionMove>();
                        unitAggressionMove.enabled = false;
                        unitAggressionMove.Release();
                        unitAggressionMove.SetDefaultReacherDistance();

                        UnitAttack unitAttack = unitStatus.GetComponent<UnitAttack>();
                        unitAttack.Release();

                        CheckAttackRange attackRange = unitStatus.GetComponentInChildren<CheckAttackRange>();
                        attackRange?.SetDefaultReachDistance(); //? because modify build(destroy)
                    }

                    unitStatus.Release();
                }
            }

            foreach (FlagSlotInfo slotInfo in _flags)
                slotInfo.BindedUnits.Clear();
        }

        private void InitHealBuild()
        {
            _orderMarker.OrderID = OrderID.Build;
            _orderMarker.IsStarted = false;
        }
    }
}