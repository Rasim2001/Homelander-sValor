using Infastructure.Services.Flag;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.StrategyBehaviour;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Units
{
    public class UnitMove : MonoBehaviour
    {
        [SerializeField] protected UnitAnimator _animator;
        [SerializeField] protected UnitStatus _unitStatus;
        [SerializeField] protected UnitStrategyBehaviour _strategyBehaviour;

        protected IFlagTrackerService FlagTrackerService;
        protected float TargetFreePositionX;

        private IStaticDataService _staticDataService;
        private UnitStaticData _unitStaticData;
        private float _randomFollowSpeed;

        public float _speed;
        private float _randomSpeedTime;
        private UnitFlip _unitFlip;


        [Inject]
        public void Construct(IFlagTrackerService flagTrackerService, IStaticDataService staticDataService)
        {
            FlagTrackerService = flagTrackerService;
            _staticDataService = staticDataService;
        }

        private void Awake()
        {
            _unitFlip = GetComponent<UnitFlip>();

            _unitStaticData = _staticDataService.ForUnit(_unitStatus.UnitTypeId);
        }

        private void OnEnable()
        {
            if (FlagTrackerService.GetMainFlag() != null)
                ChangeTargetPosition();
        }

        private void Update()
        {
            if (_unitStatus.PlayerMove == null)
                return;

            if (FollowTimeIsEnded())
                ChangeFollowTimeSpeed();
            else
                UpdateFollowTimeSpeed();
        }


        public virtual void ChangeTargetPosition()
        {
            float flagPositionX = FlagTrackerService.GetClosestFlagPositionX(transform.position.x);

            float leftBorder = transform.position.x >= 0 ? flagPositionX - 5f : flagPositionX - 1f;
            float rightBorder = transform.position.x > 0 ? flagPositionX + 1f : flagPositionX + 5f;

            TargetFreePositionX = FindPrefferedTarget(leftBorder, rightBorder);
        }

        public void Move()
        {
            if (_animator.IsStandartAnimationSpeed() == false)
                _animator.ChangeAnimationSpeed(1);

            float directionDistanceToMove = TargetFreePositionX - transform.position.x;

            _unitFlip.SetFlip(directionDistanceToMove);

            float moveDistance = Mathf.Sign(directionDistanceToMove) *
                                 Mathf.Min(Mathf.Abs(directionDistanceToMove), _speed * Time.deltaTime);

            transform.Translate(new Vector2(moveDistance, 0));
        }

        public void SetSpeed(float value) =>
            _speed = value;

        protected float FindPrefferedTarget(float leftBorder, float rightBorder)
        {
            float prefferedTarget = Random.Range(leftBorder, rightBorder);

            if (Mathf.Abs(TargetFreePositionX - prefferedTarget) < _unitStaticData.MinimalDistanceMove)
                return FindPrefferedTarget(leftBorder, rightBorder);

            return prefferedTarget;
        }

        private void ChangeFollowTimeSpeed()
        {
            ChangeSpeedAndTime();
            ChangeAnimationSpeed();
        }

        private void ChangeSpeedAndTime()
        {
            _randomFollowSpeed = Random.Range(_unitStaticData.MinAdditionalFollowSpeed,
                _unitStaticData.MaxAdditionalFollowSpeed);
            _randomSpeedTime = Random.Range(2f, 3f);
        }

        private void ChangeAnimationSpeed()
        {
            float range = _unitStaticData.MaxAdditionalFollowSpeed - _unitStaticData.MinAdditionalFollowSpeed;
            if (range == 0f)
                range = 1f;

            float randomSpeedInPercentage = (_randomFollowSpeed - _unitStaticData.MinAdditionalFollowSpeed) / range;
            float randomAnimationSpeed = Mathf.Lerp(_unitStaticData.MinAnimationSpeed,
                _unitStaticData.MaxAnimationSpeed, randomSpeedInPercentage);

            _animator.ChangeAnimationSpeed(randomAnimationSpeed);
        }

        private bool FollowTimeIsEnded() =>
            _randomSpeedTime <= 0;

        private void UpdateFollowTimeSpeed() =>
            _randomSpeedTime -= Time.deltaTime;


        public bool IsPathReached() =>
            Mathf.Abs(transform.position.x - TargetFreePositionX) < Mathf.Epsilon;


        public float GetRandomSpeed() =>
            _randomFollowSpeed + _speed;
    }
}