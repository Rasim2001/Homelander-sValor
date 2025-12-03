using Units;
using UnityEngine;

namespace Infastructure.Services.MinimapManagement
{
    public interface IMinimapNotifierService
    {
        void BarricadeAttackedNotify(string uniqueId, Vector3 position);
        void BarricadeAttackedFinishedNotify(string uniqueId);
        void BarricadeDestroyedNotify(Vector3 position);
    }
}