using HealthSystem;
using Infastructure.Services.Pool;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits
{
    public class ArrowDamage : MonoBehaviour
    {
        [SerializeField] private ObserverTrigger _observerTrigger;

        public int Damage;
        public bool IsHittingToGround;
        public ArrowRotation ArrowRotation => _arrowRotation;
        public Rigidbody2D Rigidbody => _rigidbody2D;

        private IPoolObjects<ArrowDamage> _arrowPoolObjects;
        private Coroutine _destroyObjectCoroutine;

        private Rigidbody2D _rigidbody2D;
        private ArrowRotation _arrowRotation;

        [Inject]
        public void Construct(IPoolObjects<ArrowDamage> arrowPoolObjects) =>
            _arrowPoolObjects = arrowPoolObjects;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _arrowRotation = GetComponent<ArrowRotation>();
        }

        private void Start() =>
            _observerTrigger.OnTriggerEnter += TriggerEnter;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= TriggerEnter;

        private void TriggerEnter()
        {
            if (IsHittingToGround)
                return;

            if (_observerTrigger.CurrentCollider.TryGetComponent(out IHealth health))
            {
                health.TakeDamage(Damage);
                _arrowPoolObjects.ReturnObjectToPool(this);
            }
        }
    }
}