using BuildProcessManagement.Towers;
using HealthSystem;
using Infastructure.Factories.GameFactories;
using Player.Orders;
using Units;
using UnityEngine;
using Zenject;

namespace Flag
{
    public class FlagActivator : MonoBehaviour
    {
        [Header("FlagPositionTransforms")]
        [SerializeField] private Transform _leftFlagTransform;
        [SerializeField] private Transform _rightFlagTransform;

        [Header("Main")]
        [SerializeField] private UniqueId _uniqueId;
        [SerializeField] private BarricadeVisibilityZone _barricadeVisibilityZone;
        [SerializeField] private OrderMarker _orderMarker;
        [SerializeField] private BuildingHealth _buildingHealth;

        private FlagSlotCoordinator _flagSlotCoordinator;
        private IGameFactory _gameFactory;

        [Inject]
        public void Construct(IGameFactory gameFactory) =>
            _gameFactory = gameFactory;

        private void OnDestroy()
        {
            if (_flagSlotCoordinator != null && _buildingHealth.CurrentHP <= 0)
                DestroyFlag();
        }

        public void Initialize(FlagSlotCoordinator flagSlotCoordinator)
        {
            _flagSlotCoordinator = flagSlotCoordinator;

            Register();
        }


        public void SpawnFlag()
        {
            GameObject flagObject =
                _gameFactory.CreateBarricadeFlag(GetPosition(), _uniqueId.Id);
            _flagSlotCoordinator = flagObject.GetComponent<FlagSlotCoordinator>();

            Register();
        }

        public FlagSlotCoordinator GetFlag() =>
            _flagSlotCoordinator;

        public bool HasFlag() =>
            _flagSlotCoordinator != null;


        public void ReleaseUnitFromBuildingFlag() =>
            _flagSlotCoordinator.ReleaseAll();

        public void DestroyFlag() => 
            Destroy(_flagSlotCoordinator.gameObject);

        private void Register()
        {
            int flipSideValue = transform.position.x > 0 ? -1 : 1;

            _flagSlotCoordinator.Initialize(_orderMarker, _buildingHealth, flipSideValue, transform.position.x);
            _barricadeVisibilityZone.Initialize(_flagSlotCoordinator);
        }

        private Vector3 GetPosition() =>
            transform.position.x > 0
                ? _leftFlagTransform.transform.position
                : _rightFlagTransform.transform.position;
    }
}