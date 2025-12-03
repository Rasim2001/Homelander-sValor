using System.Collections;
using HealthSystem;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private EnemyFlip _enemyFlip;
        [SerializeField] private EnemyObserverTrigger _observerTrigger;
        [SerializeField] private EnemyMove _enemyMove;
        [SerializeField] private float _attackCooldown;

        [SerializeField] protected EnemyAnimator _enemyAnimator;

        public bool IsAttackingProcess { get; set; }
        public bool IsMissActive { get; set; }

        protected int Damage;

        private Transform _target;
        private Coroutine _coroutine;
        private EnemyInfo _enemyInfo;
        private float _attackCooldownDefault;

        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(IStaticDataService staticDataService) =>
            _staticDataService = staticDataService;

        private void Awake() =>
            _enemyInfo = GetComponent<EnemyInfo>();

        private void Start()
        {
            _attackCooldownDefault = _attackCooldown;

            EnemyStaticData enemyStaticData = _staticDataService.ForEnemy(_enemyInfo.EnemyTypeId);
            Damage = enemyStaticData.Damage;
        }

        private void Update()
        {
            UpdateCooldown();

            if (CanAttack() && _enemyMove.TargetIsReached())
                Attack();
        }


        public void OnAttackStarted() //calling from unity
        {
            if (_target == null)
                return;

            if (IsMissActive && Random.value < 0.5f)
                Debug.Log("Miss");
            else
            {
                IHealth health = _target.GetComponent<IHealth>();
                health?.TakeDamage(Damage);
            }

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        public void OnAttackEnded() //calling from unity
        {
            _attackCooldown = _attackCooldownDefault;
            IsAttackingProcess = false;

            _coroutine = StartCoroutine(WaitCoroutine());

            Collider2D activeHitCollider = _observerTrigger.GetActiveHitCollider();
            if (activeHitCollider != null)
                _target = activeHitCollider.transform;
        }

        private IEnumerator WaitCoroutine()
        {
            while (_attackCooldown >= 0)
                yield return null;

            if (!IsAttackingProcess)
                _enemyMove.IsActiveMove = true;
        }

        public void SetTarget(Transform target) =>
            _target = target;


        private bool CanAttack() =>
            !IsAttackingProcess && CooldownEnded() && _target != null;


        protected virtual void Attack()
        {
            SetFlip();

            _enemyAnimator.PlayAttackAnimation();
            IsAttackingProcess = true;
            _enemyMove.IsActiveMove = false;
        }

        private void SetFlip() =>
            _enemyFlip.SetFlip(_target.transform.position.x - transform.position.x);

        private void UpdateCooldown()
        {
            if (!CooldownEnded())
                _attackCooldown -= Time.deltaTime;
        }

        private bool CooldownEnded() =>
            _attackCooldown <= 0;
    }
}