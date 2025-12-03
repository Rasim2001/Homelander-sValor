using System;
using System.Collections.Generic;
using System.Linq;
using HealthSystem;
using UnityEngine;

namespace Enemy
{
    public class EnemyObserverTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;

        [SerializeField] private List<Collider2D> _triggeredHits = new List<Collider2D>();

        public Collider2D BuildingHit;

        public Action OnTriggerEnter;
        public Action OnTriggerExit;

        public Collider2D GetActiveHitCollider()
        {
            if (BuildingHit != null)
                return BuildingHit;

            return _triggeredHits.Count > 0 ? FindNearestHit() : null;
        }

        public LayerMask GetLayerMask() =>
            _layerMask;

        public void SetLayerMask(LayerMask layerMask) =>
            _layerMask = layerMask;

        public bool HasAnyColliders() =>
            _triggeredHits.Count > 0 || BuildingHit != null;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_layerMask != (_layerMask | (1 << other.gameObject.layer)))
                return;

            if (!_triggeredHits.Contains(other))
                _triggeredHits.Add(other);

            if (other.GetComponent<BuildingHealth>())
                BuildingHit = other;

            OnTriggerEnter?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_layerMask != (_layerMask | (1 << other.gameObject.layer)))
                return;

            if (other.GetComponent<BuildingHealth>())
                BuildingHit = null;

            if (_triggeredHits.Contains(other))
                _triggeredHits.Remove(other);

            OnTriggerExit?.Invoke();
        }

        private Collider2D FindNearestHit()
        {
            return _triggeredHits
                .Where(hit => hit != null)
                .OrderBy(hit => Mathf.Abs(hit.transform.position.x - transform.position.x))
                .FirstOrDefault();
        }
    }
}