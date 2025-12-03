using Enviroment;
using Infastructure.Services.BuildModeServices;
using UnityEngine;
using Zenject;

namespace Player
{
    public class IlluminateZone : MonoBehaviour
    {
        [SerializeField] private PlayerMove _playerMove;
        [SerializeField] private ObserverTrigger _observerTrigger;

        private IBuildingModeService _buildingModeService;

        [Inject]
        public void Construct(IBuildingModeService buildingModeService) =>
            _buildingModeService = buildingModeService;

        private void Start()
        {
            _observerTrigger.OnTriggerEnter += Enter;
            _observerTrigger.OnTriggerExit += Exit;

            _playerMove.AccelerationButtonUpHappened += Enter;
        }

        private void OnDestroy()
        {
            _observerTrigger.OnTriggerEnter -= Enter;
            _observerTrigger.OnTriggerExit -= Exit;

            _playerMove.AccelerationButtonUpHappened -= Enter;
        }

        private void Enter()
        {
            if (_playerMove.AccelerationPressedWithMove ||
                _observerTrigger.CurrentCollider == null ||
                _buildingModeService.IsBuildingState)
                return;

            if (_observerTrigger.CurrentCollider.TryGetComponent(out IlluminateObject illuminateObject))
                illuminateObject.Illuminate();
        }

        private void Exit()
        {
            if (_observerTrigger.CurrentCollider == null)
                return;

            if (_observerTrigger.CurrentCollider.TryGetComponent(out IlluminateObject illuminateObject))
                illuminateObject.Release();
        }
    }
}