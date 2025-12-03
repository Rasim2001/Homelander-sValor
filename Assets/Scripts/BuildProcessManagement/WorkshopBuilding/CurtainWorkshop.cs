using System;
using System.Collections;
using Units;
using UnityEngine;

namespace BuildProcessManagement.WorkshopBuilding
{
    public class CurtainWorkshop : MonoBehaviour
    {
        [SerializeField] private Workshop _workshop;

        private static readonly int IsShowedHash = Animator.StringToHash("IsShowed");
        private static readonly int InitHideTriggerHash = Animator.StringToHash("InitHideTrigger");

        public Action OnVisibilityChanged;

        private UnitObserverTrigger _observerTrigger;
        private Animator _animator;
        private Coroutine _waitOpenCoroutine;

        public bool IsShowed => _animator.GetBool(IsShowedHash);

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _observerTrigger = GetComponent<UnitObserverTrigger>();
        }

        private void Start()
        {
            _workshop.OnCreateVendorHappened += VendorSpawned;

            _observerTrigger.OnTriggerEnter += TriggerEnter;
            _observerTrigger.OnTriggerExit += TriggerExit;
        }

        private void OnDestroy()
        {
            _workshop.OnCreateVendorHappened -= VendorSpawned;

            _observerTrigger.OnTriggerEnter -= TriggerEnter;
            _observerTrigger.OnTriggerExit -= TriggerExit;
        }


        private void Show(bool value)
        {
            if (!_workshop.HasVendor)
                return;

            _animator.SetBool(IsShowedHash, value);
            OnVisibilityChanged?.Invoke();
        }

        public void OnHideHappened()
        {
            if (!_observerTrigger.HasAnyHits())
                return;

            if (!IsShowed)
                Show(true);
        }

        private void VendorSpawned()
        {
            _animator.SetTrigger(InitHideTriggerHash);

            OnVisibilityChanged?.Invoke();
        }


        private void TriggerEnter()
        {
            if (!IsShowed)
                Show(true);
        }

        private void TriggerExit()
        {
            if (_observerTrigger.HasAnyHits() || !_workshop.HasVendor)
                return;

            StopWaitCoroutine();

            _waitOpenCoroutine = StartCoroutine(WaitOpenCoroutine());
        }

        private IEnumerator WaitOpenCoroutine()
        {
            yield return new WaitForSeconds(2f);

            if (!_observerTrigger.HasAnyHits())
                Show(false);
        }

        private void StopWaitCoroutine()
        {
            if (_waitOpenCoroutine == null)
                return;

            StopCoroutine(_waitOpenCoroutine);
            _waitOpenCoroutine = null;
        }
    }
}