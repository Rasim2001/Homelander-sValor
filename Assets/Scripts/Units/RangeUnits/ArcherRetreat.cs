using Flag;
using Infastructure.Services.Flag;
using Units.Animators;
using Units.UnitStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace Units.RangeUnits
{
    public class ArcherRetreat : MonoBehaviour
    {
        [SerializeField] private ArcherAnimator _archerAnimator;
        [SerializeField] private UnitStateMachineView _unitStateMachineView;
        [SerializeField] private UnitStatus _unitStatus;
        [SerializeField] private AttackOptionBase _attackOptionBase;
        [SerializeField] private UnitAttack _unitAttack;
        [SerializeField] private UnitAggressionMove _unitAggressionMove;

        private IFlagTrackerService _flagTrackerService;
        private FlagSlotCoordinator _lastFlagSlotCoordinator;

        [Inject]
        public void Construct(IFlagTrackerService flagTrackerService) =>
            _flagTrackerService = flagTrackerService;

        public void Retreat()
        {
            _archerAnimator.ResetAllAnimations();

            if (!_flagTrackerService.LastFlagIsMainFlag(transform.position.x > 0))
            {
                _lastFlagSlotCoordinator = _flagTrackerService.GetLastFlag(transform.position.x > 0)
                    .GetComponent<FlagSlotCoordinator>();

                _lastFlagSlotCoordinator.OnDestroyHappened += () =>
                {
                    _unitAttack.OnAttackEnded();

                    _unitStatus.IsDefensedFlag = false;
                    _unitStatus.IsWorked = false;
                    _lastFlagSlotCoordinator = null;
                };
            }
            else
            {
                _unitAttack.OnAttackEnded();

                _unitStatus.IsDefensedFlag = false;
                _unitStatus.IsWorked = false;
            }
        }

        public void Update()
        {
            if (_lastFlagSlotCoordinator == null)
                return;

            BindToFlag();
        }

        private void BindToFlag()
        {
            if (Mathf.Abs(_lastFlagSlotCoordinator.transform.position.x - transform.position.x) < 1)
            {
                _unitAttack.Release();
                _unitAggressionMove.Release();
                _attackOptionBase.SetAttackZone(false);

                _lastFlagSlotCoordinator.BindUnitToSlot(transform, _unitStatus.UnitTypeId);
                _lastFlagSlotCoordinator.RelocateUnits();

                if (_lastFlagSlotCoordinator.HasEnemiesAroundBarricade)
                {
                    _unitStatus.IsDefensedFlag = false;
                    _unitStatus.IsWorked = false;

                    _unitStateMachineView.ChangeState<UnknowState>();
                    _lastFlagSlotCoordinator.PrepareForDefense();
                }

                _lastFlagSlotCoordinator = null;
            }
        }
    }
}