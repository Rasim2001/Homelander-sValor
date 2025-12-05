using Player.Orders;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class ResourceHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private OrderMarker _orderMarker;

        private IBuilderCommandExecutor _builderCommandExecutor;

        [Inject]
        public void Construct(IBuilderCommandExecutor builderCommandExecutor) =>
            _builderCommandExecutor = builderCommandExecutor;

        public void Handle()
        {
            //_builderCommandExecutor.StartHarvest(_orderMarker);
        }
    }
}