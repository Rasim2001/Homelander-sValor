using System.Collections.Generic;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.UnitRecruiter;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Player;
using Units.Animators;
using Units.UnitStates.DefaultStates;
using Units.UnitStates.FollowToPlayerStates;
using Units.UnitStatusManagement;
using UnityEngine;
using Zenject;

namespace Units.UnitStates.StateMachineViews
{
    public class UnitStateMachineView : MonoBehaviour
    {
        [SerializeField] protected UnitAnimator UnitAnimator;
        [SerializeField] protected UnitDeath UnitDeath;
        [SerializeField] protected UnitMove UnitMove;
        [SerializeField] protected UnitStatus UnitStatus;
        [SerializeField] protected Animator Animator;
        [SerializeField] protected UnitFlip UnitFlip;

        [SerializeField] private Transform _unitTranform;

        protected IUnitStateMachine StateMachine;
        protected IStaticDataService StaticDataService;
        protected UnitStaticData UnitData;
        
        private IPlayerRegistryService _playerRegistryService;
        private IUnitsRecruiterService _unitsRecruiterService;


        [Inject]
        public void Construct(IStaticDataService staticDataService, IPlayerRegistryService playerRegistryService,
            IUnitsRecruiterService unitsRecruiterService)
        {
            _unitsRecruiterService = unitsRecruiterService;
            StaticDataService = staticDataService;

            _playerRegistryService = playerRegistryService;
        }


        public virtual void Initialize()
        {
            UnitData = StaticDataService.ForUnit(UnitStatus.UnitTypeId);

            StateMachine = new UnitStateMachine();

            AutomaticAttackZone automaticAttackZone =
                _playerRegistryService.Player.GetComponentInChildren<AutomaticAttackZone>();

            List<IUnitState> states = new List<IUnitState>()
            {
                new UnknowState(Animator),

                new IdleDefaultState(UnitAnimator),
                new RunDefaultState(UnitAnimator),

                new IdleState(StateMachine, UnitAnimator),
                new WalkState(StateMachine, UnitMove, UnitAnimator, UnitData),
                new RunState(StateMachine, UnitMove, UnitAnimator, UnitData),

                new IdleTowardsPlayer(StateMachine, automaticAttackZone, _unitTranform, UnitFlip, UnitStatus,
                    UnitAnimator, UnitMove),
                new RunTowardsPlayer(StateMachine, _unitsRecruiterService, _unitTranform, UnitFlip, UnitStatus,
                    UnitAnimator, UnitMove,
                    UnitData),
                new FastRunTowardsPlayer(StateMachine, _unitsRecruiterService, _unitTranform, UnitFlip, UnitStatus,
                    UnitAnimator, UnitMove,
                    UnitData),
            };

            StateMachine.Initialize(states);

            ChangeState<IdleState>();
        }

        protected void AddToCurrentStates(IUnitState unitState) =>
            StateMachine.AddToCurrentStates(unitState);

        private void Update() =>
            StateMachine.Update();

        public void ChangeState<TState>() where TState : IUnitState =>
            StateMachine.ChangeState<TState>();
    }
}