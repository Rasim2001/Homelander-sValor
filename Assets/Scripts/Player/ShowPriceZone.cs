using Bonfire;
using BuildProcessManagement;
using BuildProcessManagement.ResourceElements;
using HealthSystem;
using Player.Orders;
using UnityEngine;

namespace Player
{
    public class ShowPriceZone : MonoBehaviour
    {
        [SerializeField] private PlayerMove _playerMove;
        [SerializeField] private ObserverTrigger _observerTrigger;

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
            if (_playerMove.AccelerationPressedWithMove || _observerTrigger.CurrentCollider == null)
                return;

            ShowCoins();
        }

        private void Exit()
        {
            if (_observerTrigger.CurrentCollider == null)
                return;

            HideCoins();
        }

        public void ShowCoins()
        {
            /*if (_observerTrigger.CurrentCollider.TryGetComponent(out UpgradeMainFlag upgradeBonfire))
            {
                if (upgradeBonfire.HasUpgrades())
                {


                    return;
                }

            }*/

            BuildingHealth buildingHealth = _observerTrigger.CurrentCollider.GetComponentInChildren<BuildingHealth>();
            RepairIconDisplay repairIconDisplay =
                _observerTrigger.CurrentCollider.GetComponentInChildren<RepairIconDisplay>();

            if (buildingHealth != null && buildingHealth.MaxHp != buildingHealth.CurrentHP)
            {
                repairIconDisplay.Show();
                return;
            }

            /*BaseCoinsDisplay coinsDisplay =
                _observerTrigger.CurrentCollider.GetComponentInChildren<BaseCoinsDisplay>();

            if (coinsDisplay == null)
                return;

            if (coinsDisplay.TryGetComponent(out OrderSelectionUI orderSelectionId))
            {
                if (orderSelectionId.OrderSelectionId != OrderSelectionId.LevelUp)
                    return;
            }*/

            /*OrderMarker orderMarker = _observerTrigger.CurrentCollider.GetComponent<OrderMarker>();

            if (orderMarker != null && (orderMarker.IsMarkered || orderMarker.IsStarted))
                return;*/
        }


        private void HideCoins()
        {
        }
    }
}