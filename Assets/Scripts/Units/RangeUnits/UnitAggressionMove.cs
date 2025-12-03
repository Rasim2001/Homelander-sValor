using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.UnitStates;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;

namespace Units.RangeUnits
{
    public class UnitAggressionMove : MonoBehaviour
    {
        [SerializeField] private UnitStateMachineView _unitStateMachine;
        [SerializeField] private UnitStatus _unitStatus;
        [SerializeField] private UnitFlip _unitFlip;
        [SerializeField] private float _speed;

        public float ReachedDistance;
        public float DefenseDistance;
        
        public bool CanMove;

        [SerializeField] private Transform _target;

        private float _reachedDistanceDefault;
        [SerializeField] private bool _isMoving;

        private void Start()
        {
            _reachedDistanceDefault = ReachedDistance;

            EnableMove();
        }

        public void FollowTo(Transform target) =>
            _target = target;


        public void EnableMove()
        {
            CanMove = true;
            _isMoving = false;
        }

        public void DisableMove() =>
            CanMove = false;

        public void Release()
        {
            _target = null;
            _isMoving = false;

            if (!_unitStatus.IsDefensedFlag || _unitStatus.UnitTypeId == UnitTypeId.Archer)
                _unitStateMachine.ChangeState<RunState>();
        }

        public void SetDefaultReacherDistance() =>
            ReachedDistance = _reachedDistanceDefault;

        public void SetDefenseDistance() =>
            ReachedDistance = DefenseDistance;


        private void Update()
        {
            if (_target == null || _unitStatus.IsDefensedFlag || !CanMove)
                return;

            bool shouldMove = !TargetIsReached();

            if (shouldMove)
                MoveTo(_target);

            ChangeAnimation(shouldMove);
        }

        private void ChangeAnimation(bool shouldMove)
        {
            if (_isMoving == shouldMove)
                return;

            if (shouldMove)
                _unitStateMachine.ChangeState<RunDefaultState>();
            else
                _unitStateMachine.ChangeState<IdleDefaultState>();

            _isMoving = shouldMove;
        }


        private void MoveTo(Transform target)
        {
            float xValue = target.position.x - transform.position.x;

            float moveX = Mathf.Sign(xValue) *
                          Mathf.Min(Mathf.Abs(xValue), _speed * Time.deltaTime);

            _unitFlip.SetFlip(xValue < 0);
            transform.Translate(new Vector3(moveX, 0));
        }

        private bool TargetIsReached() =>
            Mathf.Abs(_target.position.x - transform.position.x) < ReachedDistance;
    }
}