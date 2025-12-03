using System.Collections.Generic;
using System.Linq;
using Enemy.Effects;
using Enemy.Effects.ArmorBreak;
using Enemy.Effects.Bleed;
using Enemy.Effects.Knockback;
using HealthSystem;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Units;
using UnityEngine;
using Zenject;

namespace BuildProcessManagement.Towers.BallistaBow
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Spear : MonoBehaviour
    {
        [SerializeField] private UnitObserverTrigger _observerTrigger;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _speed = 15;

        public bool IsHittingToGround { get; set; }

        public int Damage;

        private readonly List<Collider2D> alreadyHitColliders = new List<Collider2D>();
        private bool _isActiveTarget;
        private int _direction;

        private IStaticDataService _staticDataService;


        [Inject]
        public void Construct(IStaticDataService staticDataService) =>
            _staticDataService = staticDataService;

        private void Start()
        {
            _direction = transform.position.x > 0 ? 1 : -1;
            _spriteRenderer.flipX = _direction == 1;

            _observerTrigger.OnTriggerEnter += TriggerEnter;

            Damage = _staticDataService.ForUnit(UnitTypeId.Ballistaman).Damage;
        }

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= TriggerEnter;

        public void Shoot()
        {
            transform.SetParent(null);

            _isActiveTarget = true;
        }

        private void Update()
        {
            if (_isActiveTarget)
                transform.position +=
                    transform.right * (_speed * Time.deltaTime * _direction);
        }

        private void TriggerEnter()
        {
            if (IsHittingToGround)
                return;

            List<Collider2D> allHits = _observerTrigger.GetAllHits().ToList();

            foreach (Collider2D hit in allHits)
            {
                if (hit.TryGetComponent(out IHealth health) && !alreadyHitColliders.Contains(hit))
                {
                    IEnemyEffectSystem effectSystem = hit.GetComponentInParent<IEnemyEffectSystem>();

                    if (effectSystem != null)
                    {
                        effectSystem.AddEffect<ArmorBreakEffect>(10);
                        effectSystem.AddEffect<BleedEffect>();
                        effectSystem.AddEffect<KnockbackEffect>(transform, 1);
                    }

                    health.TakeDamage(Damage);

                    alreadyHitColliders.Add(hit);
                }
            }
        }
    }
}