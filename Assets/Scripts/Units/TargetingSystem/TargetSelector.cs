using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units.TargetingSystem
{
    public class TargetSelector
    {
        private readonly List<ITargetPriority> _priorities = new List<ITargetPriority>();

        public void AddPriority(ITargetPriority priority) =>
            _priorities.Add(priority);

        public void Cleanup() =>
            _priorities.Clear();

        public Collider2D GetBestTarget(List<Collider2D> targets, Vector3 position)
        {
            if (targets == null || targets.Count == 0)
                return null;

            return targets
                .OrderBy(target => _priorities.Sum(priority => priority.Evaluate(target, position)))
                .FirstOrDefault();
        }
    }
}