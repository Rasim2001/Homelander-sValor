using UnityEngine;

namespace Units.Shielder
{
    public class CheckAttackRange : MonoBehaviour
    {
        [SerializeField] private UnitAttack _unitAttack;
        [SerializeField] private UnitObserverTrigger _triggerObserver;
        [SerializeField] private CircleCollider2D _circleCollider;

        [SerializeField] private float _defaultDistance;
        [SerializeField] private float _defenseDistance;

        public bool IsRetreating;

        public bool HasAnyHits() =>
            _triggerObserver.GetNearestHit();

        private void Awake()
        {
            _triggerObserver.OnTriggerEnter += TriggerEnter;
            _triggerObserver.OnTriggerExit += TriggerExit;
        }

        private void OnDestroy()
        {
            _triggerObserver.OnTriggerEnter -= TriggerEnter;
            _triggerObserver.OnTriggerExit -= TriggerExit;
        }

        public void SetDefaultReachDistance() =>
            _circleCollider.radius = _defaultDistance;

        public void SetDefenseDistance() =>
            _circleCollider.radius = _defenseDistance;

        private void TriggerEnter() =>
            _unitAttack.EnableAttack();

        private void TriggerExit()
        {
            if (IsRetreating)
                return;

            if (!_triggerObserver.HasAnyHits())
                _unitAttack.DisableAttack();
        }
    }
}