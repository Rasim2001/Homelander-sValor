using HealthSystem;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.StateMachineViews;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using UnknowState = Units.UnitStates.UnknowState;

namespace Units.Shielder
{
    public class ShielderUnitAttack : UnitAttack
    {
        [SerializeField] private ShielderAnimator _shielderAnimator;
        [SerializeField] private UnitStateMachineView _stateMachineView;
        [SerializeField] private UnitStatus _unitStatus;

        public int Damage;

        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(IStaticDataService staticDataService) =>
            _staticDataService = staticDataService;

        private void Start() =>
            Damage = _staticDataService.ForUnit(UnitTypeId.Shielder).Damage;

        public override void OnAttackStarted()
        {
            _shielderAnimator.ChangeAnimationSpeed(Random.Range(0.8f, 1.25f));

            if (target == null)
                return;

            IHealth health = target.GetComponentInChildren<IHealth>();
            health?.TakeDamage(Damage);
        }

        protected override void StartAttack()
        {
            base.StartAttack();

            _stateMachineView.ChangeState<AttackDefaultState>();
        }

        public override void DisableAttack()
        {
            base.DisableAttack();

            if (_unitStatus.IsDefensedFlag)
            {
                OnAttackEnded();
                _stateMachineView.ChangeState<UnknowState>();
            }
        }
    }
}