using Infastructure.Services.Flag;
using Infastructure.StaticData.Enemy;
using Infastructure.StaticData.StaticDataService;
using UnityEngine;
using Zenject;

namespace Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        [SerializeField] private EnemyFlip _enemyFlip;
        [SerializeField] private Transform _target;
        [SerializeField] private float _targetReachedDistance = 1;
        public float Speed { get; set; }
        public bool IsActiveMove;

        private EnemyInfo _enemyInfo;

        private Transform _flagTransform;
        private float distance;
        private IFlagTrackerService _flagTrackerService;
        private IStaticDataService _staticDataService;


        [Inject]
        public void Construct(IFlagTrackerService flagTrackerService, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _flagTrackerService = flagTrackerService;
        }

        private void Awake() =>
            _enemyInfo = GetComponent<EnemyInfo>();

        private void Start()
        {
            EnemyStaticData enemyStaticData = _staticDataService.ForEnemy(_enemyInfo.EnemyTypeId);
            Speed = enemyStaticData.Speed;

            IsActiveMove = true;
            _flagTransform = _flagTrackerService.GetMainFlag().transform;

            Release();
        }

        private void LateUpdate()
        {
            if (!IsActiveMove || _target == null || TargetIsReached())
                return;

            MoveTo(_target);
        }

        public void FollowTo(Transform target) =>
            _target = target;

        public void Release() =>
            _target = _flagTransform;

        public bool TargetIsReached() =>
            Mathf.Abs(_target.transform.position.x - transform.position.x) < _targetReachedDistance;


        private void MoveTo(Transform target)
        {
            float xValue = target.position.x - transform.position.x;

            float moveX = Mathf.Sign(xValue) *
                          Mathf.Min(Mathf.Abs(xValue), Speed * Time.deltaTime);

            _enemyFlip.SetFlip(xValue < 0);
            transform.Translate(new Vector3(moveX, 0));
        }
    }
}