using System.Collections.Generic;
using Units.TargetingSystem.Priorities;
using UnityEngine;

namespace Units.TargetingSystem
{
    public class TargetSelection
    {
        private readonly TargetSelector selector = new TargetSelector();

        public Collider2D GetNearestHit(List<Collider2D> triggeredHits, Vector3 position)
        {
            selector.Cleanup();
            selector.AddPriority(new NearestPriority());

            return selector.GetBestTarget(triggeredHits, position);
        }

        public Collider2D GetFurthestHit(List<Collider2D> triggeredHits, Vector3 position)
        {
            selector.Cleanup();
            selector.AddPriority(new FurthestPriority());

            return selector.GetBestTarget(triggeredHits, position);
        }

        public Collider2D GetHighestHPTarget(List<Collider2D> triggeredHits, Vector3 position)
        {
            selector.Cleanup();
            selector.AddPriority(new HighestHPPriority());

            return selector.GetBestTarget(triggeredHits, position);
        }

        public Collider2D GetLowlestHPTarget(List<Collider2D> triggeredHits, Vector3 position)
        {
            selector.Cleanup();
            selector.AddPriority(new LowestHPPriority());

            return selector.GetBestTarget(triggeredHits, position);
        }
    }
}