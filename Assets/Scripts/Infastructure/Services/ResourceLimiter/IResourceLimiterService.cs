using System;
using Player.Orders;
using UnityEngine;

namespace Infastructure.Services.ResourceLimiter
{
    public interface IResourceLimiterService
    {
        void AddResource(OrderMarker orderMarker);
        void RemoveResource(OrderMarker orderMarker);
        bool IsActive(OrderMarker orderMarker);
        Action<bool, Transform> OnResourceChanged { get; set; }
        void CleanUp();
    }
}