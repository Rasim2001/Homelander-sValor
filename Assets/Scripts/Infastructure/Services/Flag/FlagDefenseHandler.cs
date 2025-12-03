using Flag;
using Infastructure.StaticData.Unit;
using Units;
using Units.RangeUnits;
using Units.Shielder;
using Units.UnitStates;
using Units.UnitStates.BuilderStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Infastructure.Services.Flag
{
    public class FlagDefenseHandler : IFlagDefenseHandler
    {
        public void PrepareToDefense(Transform unit)
        {
            if (unit.TryGetComponent(out AttackOptionBase attackOption))
                attackOption.PrepareDefense();
        }

        public void RelocateAfterFight(Transform unit)
        {
            if (unit.TryGetComponent(out AttackOptionBase attackOption))
                attackOption.SetAttackZone(false);
        }

        public void StartRetreat(FlagSlotInfo slotInfo)
        {
            foreach (Transform unit in slotInfo.BindedUnits)
            {
                if (unit == null)
                    return;

                UnitStatus unitStatus = unit.GetComponent<UnitStatus>();

                if (unitStatus.UnitTypeId == UnitTypeId.Archer)
                {
                    AttackOptionBase attackOption = unitStatus.GetComponent<AttackOptionBase>();
                    attackOption.SetAttackZone(true);

                    ArcherRetreat retreat = unitStatus.GetComponent<ArcherRetreat>();
                    retreat.Retreat();
                }
                else if (unitStatus.UnitTypeId == UnitTypeId.Shielder)
                {
                    UnitStateMachineView unitStateMachineView = unit.GetComponent<UnitStateMachineView>();
                    unitStateMachineView.ChangeState<ShielderRetreatState>();
                }
                else if (unitStatus.UnitTypeId == UnitTypeId.Builder)
                {
                    UnitStateMachineView unitStateMachineView = unit.GetComponent<UnitStateMachineView>();
                    unitStateMachineView.ChangeState<BuilderRetreatState>();
                }
            }
        }
    }
}