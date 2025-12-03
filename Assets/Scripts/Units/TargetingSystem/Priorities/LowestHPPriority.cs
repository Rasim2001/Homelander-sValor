using HealthSystem;
using UnityEngine;

namespace Units.TargetingSystem.Priorities
{
    public class LowestHPPriority : ITargetPriority
    {
        public float Evaluate(Collider2D target, Vector3 position)
        {
            if (target.TryGetComponent(out IHealth health))
                return health.MaxHp;

            return 0;
        }
    }
}