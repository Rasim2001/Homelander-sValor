using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Player.Orders
{
    [RequireComponent(typeof(UniqueId))]
    public class OrderMarker : MonoBehaviour
    {
        public OrderID OrderID;
        public List<OrderPlace> Places;

        public bool IsStarted;
        public bool IsMarkered;
        
        
    }

    [Serializable]
    public class OrderPlace
    {
        public Transform ChopPlace;
        public bool IsBusy;
        public bool Flip;
    }
}