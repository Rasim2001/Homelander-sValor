using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.StaticData.Unit;
using Player;
using Player.CallUnits;
using Units;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Infastructure.Services.UnitRecruiter
{
    public class UnitsRecruiterService : IUnitsRecruiterService
    {
        public List<UnitStatus> AllUnits { get; set; } = new List<UnitStatus>();

        public event Action<UnitStatus> OnRemoveHappened;

        private readonly IHomelessOrdersService _homelessOrdersService;
        private readonly IGameFactory _gameFactory;

        private readonly List<UnitStatus> _builders = new List<UnitStatus>();
        private readonly List<UnitStatus> _arhers = new List<UnitStatus>();
        private readonly List<UnitStatus> _homeless = new List<UnitStatus>();
        private readonly List<UnitStatus> _shielders = new List<UnitStatus>();

        private readonly float _originalOffsetDistance = 1;
        private readonly float _offsexBetweenUnits = 0.5f;

        private ICallingUnits _callingUnits;
        private float _offsetDistanceFollowing = 1;

        private PlayerMove _playerMove;
        private PlayerFlip _playerFlip;

        public UnitsRecruiterService(IGameFactory gameFactory, IHomelessOrdersService homelessOrdersService)
        {
            _homelessOrdersService = homelessOrdersService;
            _gameFactory = gameFactory;
        }

        public void Initialize(PlayerFlip playerFlip, PlayerMove playerMove)
        {
            _playerFlip = playerFlip;
            _playerMove = playerMove;

            _offsetDistanceFollowing = _originalOffsetDistance;

            _callingUnits = new SingleCalling(this, _gameFactory, _playerFlip);
            _callingUnits.Initialize();
        }


        public void InitializeUnits()
        {
            _callingUnits.DetectUnits();

            BindAllSelectableUnits();
        }

        public void ReInitializeUnits(List<UnitStatus> units)
        {
            foreach (UnitStatus unitStatus in units)
                AddUnitToList(unitStatus);

            BindAllSelectableUnits();
        }

        public void InitializeSavedUnits(List<UnitStatus> units)
        {
            foreach (UnitStatus unit in units)
                AddUnitToList(unit);

            BindAllSelectableUnits();
        }

        public void AddUnitToList(UnitStatus unitStatus)
        {
            if (unitStatus.UnitTypeId == UnitTypeId.Builder && !_builders.Contains(unitStatus))
                _builders.Add(unitStatus);
            else if (unitStatus.UnitTypeId == UnitTypeId.Archer && !_arhers.Contains(unitStatus))
                _arhers.Add(unitStatus);
            else if (unitStatus.UnitTypeId == UnitTypeId.Homeless && !_homeless.Contains(unitStatus))
            {
                _homelessOrdersService.RemoveHomelessTemp(unitStatus);
                _homeless.Add(unitStatus);
            }

            else if (unitStatus.UnitTypeId == UnitTypeId.Shielder && !_shielders.Contains(unitStatus))
                _shielders.Add(unitStatus);

            if (!AllUnits.Contains(unitStatus))
                AllUnits.Add(unitStatus);
        }


        public void RelocateRemainingUnitsToPlayer()
        {
            _offsetDistanceFollowing = _originalOffsetDistance;

            foreach (UnitStatus unitStatus in AllUnits)
            {
                unitStatus.BindToPlayer(_playerMove, _offsetDistanceFollowing);
                _offsetDistanceFollowing += _offsexBetweenUnits;
            }
        }

        public void RelocateRemainingUnitsToPlayerWithSort()
        {
            _offsetDistanceFollowing = _originalOffsetDistance;

            IEnumerable<UnitStatus>[] unitGroups = { _shielders, _arhers, _builders, _homeless };

            foreach (IEnumerable<UnitStatus> unitGroup in unitGroups)
            {
                foreach (UnitStatus unitStatus in unitGroup.ToList())
                {
                    unitStatus.BindToPlayer(_playerMove, _offsetDistanceFollowing);
                    _offsetDistanceFollowing += _offsexBetweenUnits;
                }
            }
        }

        public int FindUnitIndexByType(UnitTypeId unitTypeId)
        {
            int index = -1;

            for (int i = 0; i < AllUnits.Count; i++)
            {
                if (AllUnits[i].UnitTypeId == unitTypeId)
                    return i;
            }

            return index;
        }

        public UnitStatus ReleaseUnit(UnitTypeId unitTypeId, int index = 0)
        {
            UnitStatus currentUnitStatus = null;

            switch (unitTypeId)
            {
                case UnitTypeId.Builder:
                    currentUnitStatus = ReleaseUnitFromList(_builders);
                    break;
                case UnitTypeId.Archer:
                    currentUnitStatus = ReleaseUnitFromList(_arhers);
                    break;
                case UnitTypeId.Homeless:
                    currentUnitStatus = ReleaseUnitFromList(_homeless);
                    break;
                case UnitTypeId.Shielder:
                    currentUnitStatus = ReleaseUnitFromList(_shielders);
                    break;
                case UnitTypeId.Anyone:
                    currentUnitStatus = ReleaseUnitFromList(AllUnits, index);
                    break;
            }

            RemoveUnitFromAllLists(currentUnitStatus);

            return currentUnitStatus;
        }

        public UnitTypeId GetUnitType(int index) =>
            index == AllUnits.Count ? UnitTypeId.Unknow : AllUnits[index].UnitTypeId;


        public void ReleaseAll()
        {
            foreach (UnitStatus unitStatus in _builders)
            {
                unitStatus.GetComponent<UnitMove>().ChangeTargetPosition();
                unitStatus.GetComponent<BindToPlayerMarker>().Hide();

                unitStatus.Release();
            }

            foreach (UnitStatus unitStatus in _arhers)
            {
                unitStatus.GetComponent<UnitMove>().ChangeTargetPosition();
                unitStatus.Release();
                unitStatus.GetComponent<BindToPlayerMarker>().Hide();

                unitStatus.GetComponent<AttackOptionBase>().SetAttackZone(true);
            }


            foreach (UnitStatus unitStatus in _shielders)
            {
                unitStatus.GetComponent<UnitMove>().ChangeTargetPosition();
                unitStatus.Release();
                unitStatus.GetComponent<BindToPlayerMarker>().Hide();

                unitStatus.GetComponent<AttackOptionBase>().SetAttackZone(true);
            }

            foreach (UnitStatus unitStatus in _homeless)
            {
                unitStatus.Release();
                unitStatus.GetComponent<BindToPlayerMarker>().Hide();
            }


            _builders.Clear();
            _arhers.Clear();
            _homeless.Clear();
            _shielders.Clear();

            AllUnits.Clear();
        }

        public void ReleaseWarriorUnitFromAutomaticZone(UnitStatus unitStatus)
        {
            _arhers.Remove(unitStatus);
            _shielders.Remove(unitStatus);

            AllUnits.Remove(unitStatus);

            unitStatus.GetComponent<AttackOptionBase>().SetAttackZone(true);
            unitStatus.GetComponent<UnitMove>().ChangeTargetPosition();
            unitStatus.ReleaseFromAutomaticZone();
        }


        public void BindUnitToPlayer(UnitStatus unitStatus)
        {
            if (unitStatus == null)
            {
                Debug.LogError("Bind unit to player is not avaliable");
                return;
            }

            unitStatus.GetComponent<BindToPlayerMarker>().Show();

            if (unitStatus.TryGetComponent(out AttackOptionBase attackOption))
                attackOption.SetAttackZone(false);

            UnitStrategyBehaviour unitStrategyBehaviour = unitStatus.GetComponentInChildren<UnitStrategyBehaviour>();
            unitStrategyBehaviour.StopAllActions();

            UnitMove unitMove = unitStatus.GetComponent<UnitMove>();
            unitMove.enabled = true;

            unitStatus.BindToPlayer(_playerMove, _offsetDistanceFollowing);


            Health health = unitStatus.GetComponentInChildren<Health>(); // TODO: Пересмотреть решение 
            if (health != null)
            {
                health.OnDeathHappened += () =>
                {
                    int index = AllUnits.IndexOf(unitStatus);

                    if (index != -1)
                    {
                        ReleaseUnit(UnitTypeId.Anyone, index);
                        RelocateRemainingUnitsToPlayer();
                    }
                };
            }

            _offsetDistanceFollowing += _offsexBetweenUnits;
        }

        public void RemoveUnitFromAllLists(UnitStatus unitStatus)
        {
            _arhers.Remove(unitStatus);
            _builders.Remove(unitStatus);
            _homeless.Remove(unitStatus);
            _shielders.Remove(unitStatus);

            AllUnits.Remove(unitStatus);

            OnRemoveHappened?.Invoke(unitStatus);
        }

        private void BindAllSelectableUnits()
        {
            List<UnitStatus> newAllUnits = new List<UnitStatus>();

            _offsetDistanceFollowing = _originalOffsetDistance;

            IEnumerable<UnitStatus>[] unitGroups = { _shielders, _arhers, _builders, _homeless };

            foreach (IEnumerable<UnitStatus> unitGroup in unitGroups)
            {
                foreach (UnitStatus unitStatus in unitGroup.ToList())
                {
                    BindUnitToPlayer(unitStatus);

                    newAllUnits.Add(unitStatus);
                }
            }

            AllUnits = new List<UnitStatus>(newAllUnits);
        }

        private UnitStatus ReleaseUnitFromList(List<UnitStatus> unitList, int index = 0)
        {
            if (unitList.Count == 0)
                return null;

            UnitStatus currentUnitStatus = unitList[index];
            currentUnitStatus.Release();
            currentUnitStatus.GetComponent<BindToPlayerMarker>().Hide();
            currentUnitStatus.GetComponent<AttackOptionBase>()?.SetAttackZone(true);
            currentUnitStatus.GetComponent<UnitMove>().enabled = false;

            return currentUnitStatus;
        }
    }
}