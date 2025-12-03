using UnityEngine;

namespace BuildProcessManagement.Towers.BallistaTar
{
    public interface ICarriage
    {
        void Shoot();
        void Reload();
        Transform CarriageTransform { get; }
    }
}