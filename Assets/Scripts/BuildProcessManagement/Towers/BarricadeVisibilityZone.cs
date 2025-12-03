using System;
using System.Collections;
using Flag;
using HealthSystem;
using Infastructure.Services.MinimapManagement;
using Units;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.Towers
{
    public class BarricadeVisibilityZone : MonoBehaviour
    {
        [SerializeField] private BuildingHealth _buildingHealth;
        [SerializeField] private UnitObserverTrigger _observerTrigger;
        [SerializeField] private UniqueId _uniqueId;
        public bool HasVisiableEnemy => _observerTrigger.GetNearestHit();

        private FlagSlotCoordinator _flagSlotCoordinator;
        private Coroutine _coroutine;
        private IMinimapNotifierService _minimapNotifierService;


        [Inject]
        public void Construct(IMinimapNotifierService minimapNotifierService) =>
            _minimapNotifierService = minimapNotifierService;

        public void Initialize(FlagSlotCoordinator flagSlotCoordinator) =>
            _flagSlotCoordinator = flagSlotCoordinator;

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

        private void TriggerEnter()
        {
            if (_flagSlotCoordinator.HasEnemiesAroundBarricade)
                return;

            _minimapNotifierService.BarricadeAttackedNotify(_uniqueId.Id, transform.position);
            _flagSlotCoordinator.HasEnemiesAroundBarricade = true;
            _flagSlotCoordinator.PrepareForDefense();

            StopHealCoroutine();
        }

        private void TriggerExit()
        {
            if (_buildingHealth.IsDeath)
                _minimapNotifierService.BarricadeAttackedFinishedNotify(_uniqueId.Id);

            if (_observerTrigger.GetNearestHit() || _buildingHealth.IsDeath)
                return;

            _minimapNotifierService.BarricadeAttackedFinishedNotify(_uniqueId.Id);
            _flagSlotCoordinator.HasEnemiesAroundBarricade = false;
            _flagSlotCoordinator.RelocateUnits();

            _coroutine = StartCoroutine(HealBarricadeCoroutine());
        }

        private IEnumerator HealBarricadeCoroutine()
        {
            _flagSlotCoordinator.Relax();
            yield return new WaitForSeconds(2f);
            _flagSlotCoordinator.HealBarricade();
        }

        private void StopHealCoroutine()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }
}