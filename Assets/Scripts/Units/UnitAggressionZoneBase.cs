using System;
using Infastructure.Services.Flag;
using Player;
using Units.RangeUnits;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace Units
{
    public class UnitAggressionZoneBase : MonoBehaviour
    {
        [SerializeField] protected UnitStatus UnitStatus;
        [SerializeField] protected UnitAggressionMove UnitAggressionMove;
        [SerializeField] protected UnitAttack UnitAttack;
        [SerializeField] protected UnitObserverTrigger UnitObserverTrigger;
        [SerializeField] protected ObserverTrigger DefeatMainFlagObserverTrigger;

        [SerializeField] private Health _health;

        private IFlagTrackerService _flagTrackerService;

        public Action<UnitStatus> OnReleaseHappened;

        [Inject]
        public void Construct(IFlagTrackerService flagTrackerService) =>
            _flagTrackerService = flagTrackerService;


        public void SetAggressionTarget(Transform target)
        {
            UnitAggressionMove.enabled = true;
            UnitAggressionMove.FollowTo(target);
            UnitAttack.SetTarget(target);
        }

        protected virtual void Start()
        {
            DefeatMainFlagObserverTrigger.OnTriggerEnter += TriggerEnter;

            UnitObserverTrigger.OnTriggerEnter += TriggerEnter;
            UnitObserverTrigger.OnTriggerExit += TriggerExit;
        }


        protected virtual void OnDestroy()
        {
            DefeatMainFlagObserverTrigger.OnTriggerEnter -= TriggerEnter;

            UnitObserverTrigger.OnTriggerEnter -= TriggerEnter;
            UnitObserverTrigger.OnTriggerExit -= TriggerExit;
        }

        protected virtual void TriggerEnter()
        {
            if (_health.IsDeath)
                return;

            Collider2D target = UnitObserverTrigger.GetNearestHit();

            if (target != null)
                SetAggressionTarget(target.transform);
        }


        protected bool IsAroundOfMainFlag() =>
            Mathf.Abs(_flagTrackerService.GetMainFlag().position.x - UnitAttack.transform.position.x) < 5;

        private void TriggerExit()
        {
            if (_health.IsDeath)
                return;

            CheckAndRelease();
        }

        public void CheckAndRelease()
        {
            if (UnitObserverTrigger.GetNearestHit() != null)
                TriggerEnter();
            else if (!UnitStatus.IsWorked || UnitStatus.PlayerMove != null)
                Release();
        }

        public void ReleaseFromAutomaticZoneAttack()
        {
            UnitAggressionMove.enabled = false;
            UnitAggressionMove.Release();
            UnitAttack.Release();
        }


        private void Release()
        {
            UnitAggressionMove.enabled = false;
            UnitAggressionMove.Release();
            UnitAttack.Release();

            OnReleaseHappened?.Invoke(UnitStatus);
        }
    }
}