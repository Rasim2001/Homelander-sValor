using Infastructure.Data;
using Infastructure.Services.ProgressWatchers;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.Services.SaveLoadService;
using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Unit;
using Player;
using Units.UnitStates;
using Units.UnitStates.FollowToPlayerStates;
using Units.UnitStates.StateMachineViews;
using UnityEngine;
using Zenject;

namespace Units.UnitStatusManagement
{
    public class UnitStatus : MonoBehaviour, ISavedProgress
    {
        [field: SerializeField] public string OrderUniqueId { get; set; }
        [field: SerializeField] public bool IsWorked { get; set; }
        [field: SerializeField] public bool IsDefensedFlag { get; set; }
        [field: SerializeField] public bool IsAutomaticBindedToPlayer { get; set; }
        [field: SerializeField] public UnitTypeId UnitTypeId { get; set; }
        [field: SerializeField] public string BindedToFlagUniqueId { get; set; }
        [field: SerializeField] public PlayerMove PlayerMove { get; private set; }
        [field: SerializeField] public PlayerFlip PlayerFlip { get; private set; }
        [field: SerializeField] public PlayerAnimator PlayerAnimator;
        
        public float OffsetX { private set; get; }
        public int FreePlaceIndex = -1;

        private IProgressWatchersService _progressWatchersService;
        private ISafeBuildZone _safeBuildZone;
        private IStaticDataService _staticDataService;

        private UnitStateMachineView _unitStateMachine;

        [Inject]
        public void Construct(IProgressWatchersService progressWatchersService, ISafeBuildZone safeBuildZone)
        {
            _safeBuildZone = safeBuildZone;
            _progressWatchersService = progressWatchersService;
        }

        private void Awake() =>
            _unitStateMachine = GetComponent<UnitStateMachineView>();


        public void LoadProgress(PlayerProgress progress)
        {
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            progress.UnitDataListWrapper.Units.Add(new UnitData(UnitTypeId, transform.position,
                PlayerMove != null, BindedToFlagUniqueId, OrderUniqueId, FreePlaceIndex));
        }

        public void BindToPlayer(PlayerMove playerMove, float offsetX)
        {
            if (playerMove == null)
                return;

            PlayerMove = playerMove;
            OffsetX = offsetX;

            PlayerAnimator = playerMove.GetComponent<PlayerAnimator>();

            PlayerFlip = PlayerMove.GetComponent<PlayerFlip>();

            Vector3 targetPosition =
                PlayerMove.transform.position +
                new Vector3(OffsetX * PlayerFlip.FlipValue(), 0, 0);

            float distance = Mathf.Abs(targetPosition.x - transform.position.x);

            if (distance > 0.1f)
                _unitStateMachine.ChangeState<RunTowardsPlayer>();
        }

        public void Release()
        {
            PlayerMove = null;
            PlayerFlip = null;
            PlayerAnimator = null;

            if (_safeBuildZone.IsNight)
                _unitStateMachine.ChangeState<RunState>();
            else
                _unitStateMachine.ChangeState<WalkState>();
        }

        public void ReleaseFromAutomaticZone()
        {
            PlayerMove = null;
            PlayerFlip = null;
            PlayerAnimator = null;
        }

        public bool IsBusy() =>
            IsWorked || PlayerMove;

        private void OnDestroy() =>
            _progressWatchersService.Release(this);

        public bool IsBindedToPlayer() =>
            PlayerMove != null;
    }
}