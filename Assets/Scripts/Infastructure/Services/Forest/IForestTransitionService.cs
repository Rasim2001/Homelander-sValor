using UnityEngine;

namespace Infastructure.Services.Forest
{
    public interface IForestTransitionService
    {
        void Initialize(Transform playerTransform);
        void SubscribeUpdates();
        void Cleanup();
    }
}