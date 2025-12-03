using _Tutorial;
using Infastructure.Services.NearestBuildFind;
using Player.Orders;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Infastructure.Services.Tutorial.States
{
    public class TowerStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly INearestBuildFindService _nearestBuildFindService;
        private readonly PlayerInputOrders _playerInputOrders;
        private readonly ObserverTrigger _observerTrigger;
        private TutorialHints _tutorialHints;

        public TowerStateTutorial(ITutorialStateMachine tutorialStateMachine,
            TutorialArrowHelper tutorialArrowHelper,
            INearestBuildFindService nearestBuildFindService,
            PlayerInputOrders playerInputOrders,
            ObserverTrigger observerTrigger)
        {
            _tutorialStateMachine = tutorialStateMachine;
            _tutorialArrowHelper = tutorialArrowHelper;
            _nearestBuildFindService = nearestBuildFindService;
            _playerInputOrders = playerInputOrders;
            _observerTrigger = observerTrigger;
        }

        public void Enter()
        {
            _observerTrigger.OnTriggerEnter += TriggerEnter;
            _observerTrigger.OnTriggerExit += TriggerExit;

            _tutorialHints = _nearestBuildFindService.GetNearestTower();
            _tutorialHints.Show();

            _tutorialArrowHelper.SetTarget(_tutorialHints.transform);
        }

        public void Exit()
        {
            _observerTrigger.OnTriggerEnter -= TriggerEnter;
            _observerTrigger.OnTriggerExit -= TriggerExit;

            _tutorialArrowHelper.SetTarget(null);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_observerTrigger.CurrentCollider != null &&
                    _observerTrigger.CurrentCollider.transform == _tutorialHints.transform)
                {
                    _tutorialHints.Hide();

                    _playerInputOrders.enabled = true;
                    _playerInputOrders.OnWorkshopHappenedOnTutorial?.Invoke();
                    _tutorialStateMachine.ChangeState<BindWarrioirsStateTutorial>();
                }
            }
        }

        private void TriggerEnter()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _tutorialHints.transform)
            {
                Light2D light2D = _tutorialHints.GetComponent<Light2D>();
                light2D.enabled = true;
            }
        }

        private void TriggerExit()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _tutorialHints.transform)
            {
                Light2D light2D = _tutorialHints.GetComponent<Light2D>();
                light2D.enabled = false;
            }
        }
    }
}