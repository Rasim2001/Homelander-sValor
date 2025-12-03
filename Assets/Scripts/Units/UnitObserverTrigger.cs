using System;
using System.Collections.Generic;
using Units.TargetingSystem;
using UnityEngine;

namespace Units
{
    public class UnitObserverTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private List<Collider2D> _triggeredHits = new List<Collider2D>();

        public Action OnTriggerEnter;
        public Action OnTriggerExit;

        private TargetSelection _targetSelection;
        private bool _isTriggered;

        private void Start() =>
            _targetSelection = new TargetSelection();

        public Collider2D GetNearestHit() =>
            _targetSelection.GetHighestHPTarget(_triggeredHits, transform.position);

        public List<Collider2D> GetAllHits() =>
            _triggeredHits;

        public bool HasAnyHits() =>
            _triggeredHits != null && _triggeredHits.Count > 0;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_layerMask != (_layerMask | (1 << other.gameObject.layer)))
                return;

            if (!_triggeredHits.Contains(other))
                _triggeredHits.Add(other);

            OnTriggerEnter?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_layerMask != (_layerMask | (1 << other.gameObject.layer)))
                return;

            if (_triggeredHits.Contains(other))
                _triggeredHits.Remove(other);

            OnTriggerExit?.Invoke();
        }
    }
}