using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.Services.Flag;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Unit;
using Player.Orders;
using Units;
using Units.RangeUnits;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace Player
{
    public class AutomaticAttackZone : MonoBehaviour
    {
        [SerializeField] private PlayerInputOrders _playerInput;
        [SerializeField] private UnitObserverTrigger _observerTrigger;

        [SerializeField] private List<UnitStatus> _units = new List<UnitStatus>();

        private IUnitsRecruiterService _unitsRecruiterService;
        private IInputService _inputService;
        private IFlagTrackerService _flagTrackerService;

        [Inject]
        public void Construct(
            IInputService inputService,
            IFlagTrackerService flagTrackerService,
            IUnitsRecruiterService unitsRecruiterService)
        {
            _inputService = inputService;
            _flagTrackerService = flagTrackerService;
            _unitsRecruiterService = unitsRecruiterService;
        }

        private void Start()
        {
            _playerInput.OnReleaseAllHappened += ReleaseAll;

            _observerTrigger.OnTriggerEnter += Enter;
            _observerTrigger.OnTriggerExit += Exit;

            _unitsRecruiterService.OnRemoveHappened += RemoveUnitFromList;
        }


        private void OnDestroy()
        {
            _playerInput.OnReleaseAllHappened -= ReleaseAll;

            _observerTrigger.OnTriggerEnter -= Enter;
            _observerTrigger.OnTriggerExit -= Exit;

            _unitsRecruiterService.OnRemoveHappened -= RemoveUnitFromList;
        }

        public void TryAttackAgain()
        {
            if (!_observerTrigger.HasAnyHits())
                return;

            Enter();
        }


        private void RemoveUnitFromList(UnitStatus unitStatus)
        {
            if (!_units.Contains(unitStatus))
                return;

            ReleaseUnit(unitStatus);

            _units.Remove(unitStatus);
        }


        private void Enter()
        {
            if (_unitsRecruiterService.AllUnits.Count <= 0)
                return;

            List<UnitStatus> newWarriors = _unitsRecruiterService.AllUnits
                .Where(unit => unit != null &&
                               !_units.Contains(unit) &&
                               UnitIsWarrior(unit))
                .ToList();

            if (newWarriors.Count > 0)
                _units.AddRange(newWarriors);

            foreach (UnitStatus unitStatus in _units)
            {
                _unitsRecruiterService.ReleaseWarriorUnitFromAutomaticZone(unitStatus);
                ReleaseAndBindingToPlayer(unitStatus);

                if (unitStatus.IsAutomaticBindedToPlayer)
                    continue;

                UnitAggressionZoneBase unitAggressionZoneBase =
                    unitStatus.GetComponentInChildren<UnitAggressionZoneBase>();

                unitStatus.IsAutomaticBindedToPlayer = true;
                unitAggressionZoneBase.OnReleaseHappened += ReleaseAndBindingToPlayer;
            }
        }


        private void Exit()
        {
            if (!_observerTrigger.HasAnyHits() && _units != null && RunningToFlag())
                BindUnitsToPlayer();
        }

        private bool UnitIsWarrior(UnitStatus unit) =>
            unit.UnitTypeId == UnitTypeId.Shielder || unit.UnitTypeId == UnitTypeId.Archer;


        private void ReleaseAndBindingToPlayer(UnitStatus unitStatus)
        {
            Collider2D enemyCollider = _observerTrigger.GetNearestHit();
            Health health = enemyCollider?.GetComponent<Health>();

            if (HasAnyHits(enemyCollider, health))
                FindAnyEnemies(enemyCollider.transform);
            else
                BindToPlayer(unitStatus);
        }


        private bool HasAnyHits(Collider2D enemyCollider, Health health) =>
            enemyCollider != null && health != null && !health.IsDeath;

        private void BindToPlayer(UnitStatus unitStatus)
        {
            _unitsRecruiterService.AddUnitToList(unitStatus);
            _unitsRecruiterService.BindUnitToPlayer(unitStatus);
            _unitsRecruiterService.RelocateRemainingUnitsToPlayerWithSort();
        }


        private void BindUnitsToPlayer()
        {
            if (_units.Count == 0)
                return;

            foreach (UnitStatus unitStatus in _units)
            {
                if (unitStatus.TryGetComponent(out AttackOptionBase attackOption))
                    attackOption.SetAttackZone(false);
            }
        }

        private void FindAnyEnemies(Transform enemyTransform)
        {
            if (_units.Count == 0)
                return;

            foreach (UnitStatus unitStatus in _units)
            {
                UnitAggressionZoneBase unitAggressionZoneBase =
                    unitStatus.GetComponentInChildren<UnitAggressionZoneBase>();

                unitAggressionZoneBase?.SetAggressionTarget(enemyTransform);
            }
        }


        private void ReleaseAll()
        {
            if (_units.Count == 0)
                return;

            foreach (UnitStatus unitStatus in _units)
                ReleaseUnit(unitStatus);

            _units.Clear();
        }

        private void ReleaseUnit(UnitStatus unitStatus)
        {
            UnitAggressionZoneBase unitAggressionZoneBase =
                unitStatus.GetComponentInChildren<UnitAggressionZoneBase>();

            unitStatus.GetComponent<BindToPlayerMarker>().Hide();
            unitStatus.IsAutomaticBindedToPlayer = false;
            unitAggressionZoneBase.OnReleaseHappened -= ReleaseAndBindingToPlayer;
        }

        private bool RunningToFlag() =>
            (_inputService.AxisX > 0 && transform.position.x - _flagTrackerService.GetMainFlag().position.x < 0) ||
            (_inputService.AxisX < 0 && transform.position.x - _flagTrackerService.GetMainFlag().position.x > 0);

        private bool IsNotMoving() =>
            Mathf.Abs(_inputService.AxisX) < 0.1f;
    }
}