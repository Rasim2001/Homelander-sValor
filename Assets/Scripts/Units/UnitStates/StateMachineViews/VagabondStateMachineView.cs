using System.Collections.Generic;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Units.Animators;
using Units.UnitStates.VagabondStates;
using Units.UnitStatusManagement;
using Units.Vagabond;
using UnityEngine;
using Zenject;

namespace Units.UnitStates.StateMachineViews
{
    public class VagabondStateMachineView : MonoBehaviour
    {
        [SerializeField] private UnitStatus _unitStatus;
        [SerializeField] private VagabondMove _vagabondMove;
        [SerializeField] protected VagabondAnimator _unitAnimator;

        private IUnitStateMachine _stateMachine;
        private ISafeBuildZone _safeBuildZone;
        private IStaticDataService _staticDataService;

        [Inject]
        public void Construct(ISafeBuildZone safeBuildZone, IStaticDataService staticDataService)
        {
            _safeBuildZone = safeBuildZone;
            _staticDataService = staticDataService;
        }

        public void Initialize()
        {
            UnitStaticData unitData = _staticDataService.ForUnit(_unitStatus.UnitTypeId);

            _stateMachine = new UnitStateMachine();

            List<IUnitState> states = new List<IUnitState>()
            {
                new IdleVagabondState(_stateMachine, _safeBuildZone, _unitAnimator),
                new WalkVagabondState(_stateMachine, _safeBuildZone, _vagabondMove, _unitAnimator, unitData),
                new FearVagabondState(_stateMachine, _safeBuildZone, _unitAnimator),
                new ScaryRunVagabondState(_stateMachine, _unitAnimator, _vagabondMove, unitData)
            };

            _stateMachine.Initialize(states);

            ChangeState<ScaryRunVagabondState>();
        }

        private void Update() =>
            _stateMachine?.Update();

        public void ChangeState<TState>() where TState : IUnitState =>
            _stateMachine.ChangeState<TState>();
    }
}