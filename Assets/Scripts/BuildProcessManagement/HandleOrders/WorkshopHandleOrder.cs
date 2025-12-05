using System;
using BuildProcessManagement.WorkshopBuilding;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.AutomatizationService.Homeless;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.Unit;
using Player;
using Player.Orders;
using UI.GameplayUI.SpeachBubleUI;
using UI.Windows;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.HandleOrders
{
    public class WorkshopHandleOrder : MonoBehaviour, IHandleOrder
    {
        [SerializeField] private CurtainWorkshop _curtainWorkshop;
        [SerializeField] private OrderMarker _orderMarker;
        [SerializeField] private Workshop _workshop;


        public void Handle()
        {
            if (_orderMarker.IsStarted || _curtainWorkshop.IsShowed)
                return;

            GiveOrderToHomeless();
        }

        private void GiveOrderToHomeless() =>
            _workshop.SpawnItem();
    }
}