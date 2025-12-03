using Enemy.Effects;
using Enemy.Effects.Knockback;
using Enemy.Effects.Stun;
using HealthSystem;
using Infastructure.Services.Pool;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.Towers.CatapultTower
{
    public class CatapultBullet : MonoBehaviour
    {
        private const string EnemyLayer = "EnemyDamage";

        [SerializeField] private ObserverTrigger _observerTrigger;
        [SerializeField] private float _detectionRadius;

        public int Damage;

        private readonly Collider2D[] _enemies = new Collider2D[5];

        private PoolObjects<CatapultBullet> _catapultPoolObjects;
        private int _enemyLayer;

        [Inject]
        public void Construct(PoolObjects<CatapultBullet> catapultPoolObjects) =>
            _catapultPoolObjects = catapultPoolObjects;

        private void Awake() =>
            Initialize();

        private void Start() =>
            _observerTrigger.OnTriggerEnter += TriggerEnter;

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= TriggerEnter;

        private void Initialize() =>
            _enemyLayer = 1 << LayerMask.NameToLayer(EnemyLayer);

        private void TriggerEnter()
        {
            int size = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _detectionRadius,
                _enemies,
                _enemyLayer);

            for (int i = 0; i < size; i++)
            {
                EnemyEffectSystem enemyEffectSystem = _enemies[i].GetComponentInParent<EnemyEffectSystem>();
                enemyEffectSystem.AddEffect<StunEffect>(2);
                enemyEffectSystem.AddEffect<KnockbackEffect>(transform, 1);

                IHealth health = _enemies[i].GetComponent<IHealth>();
                health?.TakeDamage(Damage);
            }

            _catapultPoolObjects.ReturnObjectToPool(this);
        }
    }
}