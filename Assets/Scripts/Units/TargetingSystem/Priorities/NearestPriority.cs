using UnityEngine;

namespace Units.TargetingSystem.Priorities
{
    public class NearestPriority : ITargetPriority
    {
        public float Evaluate(Collider2D target, Vector3 position) =>
            Mathf.Abs(target.transform.position.x - position.x);
    }
}