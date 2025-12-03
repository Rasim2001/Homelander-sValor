using System;
using System.Collections;
using Infastructure.Services.Flag;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits
{
    public class RangeKeeper : MonoBehaviour
    {
        [SerializeField] private UnitAttack _unitAttack;
        [SerializeField] private UnitAggressionMove _unitAggressionMove;
        [SerializeField] private ObserverTrigger _observerTrigger;

        public Action OnRangeBreakHappened;
        public bool IsActiveHit;

        private IFlagTrackerService _flagTrackerService;
        private Coroutine _tryReleaseCoroutine;

        [Inject]
        public void Construct(IFlagTrackerService flagTrackerService) =>
            _flagTrackerService = flagTrackerService;

        private void Start()
        {
            _observerTrigger.OnTriggerEnter += TriggerEnter;
            _observerTrigger.OnTriggerExit += TriggerExit;
        }

        private void OnDestroy()
        {
            _observerTrigger.OnTriggerEnter -= TriggerEnter;
            _observerTrigger.OnTriggerExit -= TriggerExit;
        }

        private void OnDisable() =>
            IsActiveHit = false;

        private void TriggerEnter()
        {
            if (!IsAroundOfMainFlag())
            {
                if (_tryReleaseCoroutine != null)
                {
                    StopCoroutine(_tryReleaseCoroutine);
                    _tryReleaseCoroutine = null;
                }

                _tryReleaseCoroutine = StartCoroutine(TryReleaseCoroutine());
            }
        }

        private void TriggerExit()
        {
            if (gameObject.activeInHierarchy)
            {
                IsActiveHit = false;
                OnRangeBreakHappened?.Invoke();
            }
        }


        public bool IsAroundOfMainFlag() =>
            Mathf.Abs(_flagTrackerService.GetMainFlag().position.x - _unitAttack.transform.position.x) < 5;

        private IEnumerator TryReleaseCoroutine()
        {
            IsActiveHit = true;

            while (!_unitAggressionMove.CanMove)
                yield return null;

            _unitAttack.Release();
            _unitAggressionMove.Release();
        }
    }
}