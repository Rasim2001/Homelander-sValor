using UnityEngine;

namespace Units.TargetingSystem
{
    public interface ITargetPriority
    {
        float Evaluate(Collider2D target, Vector3 position);
    }
}