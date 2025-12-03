using HealthSystem;
using UnityEngine;

namespace Units.TargetingSystem.Priorities
{
    public class HighestHPPriority : ITargetPriority
    {
        public float Evaluate(Collider2D target, Vector3 position)
        {
            if (target == null)
                return 0;

            if (target.TryGetComponent(out IHealth health))
                return -health.MaxHp;

            return 0;
        }
    }
}