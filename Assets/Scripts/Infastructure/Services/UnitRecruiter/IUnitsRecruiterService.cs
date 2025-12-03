using System;
using System.Collections.Generic;
using Infastructure.StaticData.Unit;
using Player;
using Units.UnitStatusManagement;

namespace Infastructure.Services.UnitRecruiter
{
    public interface IUnitsRecruiterService
    {
        List<UnitStatus> AllUnits { get; }
        void Initialize(PlayerFlip playerFlip, PlayerMove playerMove);
        void InitializeUnits();
        void ReInitializeUnits(List<UnitStatus> units);
        void InitializeSavedUnits(List<UnitStatus> units);
        void AddUnitToList(UnitStatus unitStatus);
        void RelocateRemainingUnitsToPlayer();
        int FindUnitIndexByType(UnitTypeId unitTypeId);
        void ReleaseAll();
        void BindUnitToPlayer(UnitStatus unitStatus);
        UnitTypeId GetUnitType(int index);
        UnitStatus ReleaseUnit(UnitTypeId unitTypeId, int index = 0);
        void RemoveUnitFromAllLists(UnitStatus unitStatus);
        event Action<UnitStatus> OnRemoveHappened;
        void ReleaseWarriorUnitFromAutomaticZone(UnitStatus unitStatus);
        void RelocateRemainingUnitsToPlayerWithSort();
    }
}