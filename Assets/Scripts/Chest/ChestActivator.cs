using Player;
using Player.Orders;
using UI.GameplayUI.CristalUI;
using UnityEngine;

namespace Chest
{
    public class ChestActivator : MonoBehaviour
    {
        [SerializeField] private GameObject _firstHints;
        [SerializeField] private GameObject _secondHints;
        [SerializeField] private GameObject _cristralPrefab;

        public void Activate(PlayerMove playerMove)
        {
            /*GameObject cristalObject = Instantiate(_cristralPrefab, transform.position, Quaternion.identity);
            Cristal cristal = cristalObject.GetComponent<Cristal>();
            PlayerInputOrders playerInputOrders = playerMove.GetComponent<PlayerInputOrders>();

            cristal.Initialize(playerMove);
            playerInputOrders.InitCristal(cristal);

            _firstHints.SetActive(false);
            _secondHints.SetActive(true);*/
        }
    }
}