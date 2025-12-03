using HealthSystem;
using Infastructure.Services.EnemyWaves;
using UnityEngine;
using Zenject;

namespace Enemy.Bomber
{
    public class EnemyBomberAttack : EnemyAttack
    {
        [SerializeField] private GameObject _explosionFxPrefab;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _detectionRadius;

        private readonly Collider2D[] _enemies = new Collider2D[20];

        private IWaveEnemiesCountService _waveEnemiesCountService;
        private bool _isExploding;

        [Inject]
        public void Construct(IWaveEnemiesCountService waveEnemiesCountService) =>
            _waveEnemiesCountService = waveEnemiesCountService;

        protected override void Attack()
        {
            if (_isExploding)
                return;

            base.Attack();
        }

        public void OnExplode() //call from unity animation
        {
            Explode();
            DestroyEnemy();
        }

        private void Explode()
        {
            _isExploding = true;

            Instantiate(_explosionFxPrefab, new Vector3(transform.position.x, -3, 0), Quaternion.identity);

            int size = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _detectionRadius,
                _enemies,
                _layerMask);

            for (int i = 0; i < size; i++)
            {
                IHealth health = _enemies[i].GetComponent<IHealth>();

                if (health != null)
                    health.TakeDamage(Damage);
            }
        }

        private void DestroyEnemy()
        {
            _waveEnemiesCountService.NumberOfEnemiesOnWave--;
            _waveEnemiesCountService.Enemies.Remove(gameObject);

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}