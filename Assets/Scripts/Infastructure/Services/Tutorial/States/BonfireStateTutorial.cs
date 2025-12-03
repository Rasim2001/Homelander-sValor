using _Tutorial;
using Infastructure.Services.Flag;
using Player.Orders;
using UI.GameplayUI;
using UnityEngine;

namespace Infastructure.Services.Tutorial.States
{
    public class BonfireStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly IFlagTrackerService _flagTrackerService;
        private readonly ObserverTrigger _observerTrigger;
        private readonly PlayerInputOrders _playerInputOrders;

        private TutorialHints _tutorialHints;

        public BonfireStateTutorial(ITutorialStateMachine tutorialStateMachine,
            TutorialArrowHelper tutorialArrowHelper,
            IFlagTrackerService flagTrackerService,
            ObserverTrigger observerTrigger,
            PlayerInputOrders playerInputOrders)
        {
            _tutorialStateMachine = tutorialStateMachine;
            _tutorialArrowHelper = tutorialArrowHelper;
            _flagTrackerService = flagTrackerService;
            _observerTrigger = observerTrigger;
            _playerInputOrders = playerInputOrders;
        }

        public void Enter()
        {
            _tutorialHints = _flagTrackerService.GetMainFlag().GetComponent<TutorialHints>();
            _tutorialHints.Show();

            _observerTrigger.OnTriggerEnter += TriggerEnter;

            _tutorialArrowHelper.SetTarget(_tutorialHints.transform);
        }

        public void Exit()
        {
            _tutorialHints.Hide();

            _playerInputOrders.enabled = false;

            _tutorialArrowHelper.SetTarget(null);

            _observerTrigger.OnTriggerEnter -= TriggerEnter;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_observerTrigger.CurrentCollider != null)
                {
                    if (_observerTrigger.CurrentCollider.transform == _flagTrackerService.GetMainFlag())
                    {
                        _playerInputOrders.enabled = true;
                        _playerInputOrders.OnBonfireUpgradeHappenedOnTutorial?.Invoke();
                        _tutorialStateMachine.ChangeState<WorkshopStateTutorial>();
                    }
                }
            }
        }

        private void TriggerEnter()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _tutorialHints.transform)
            {
                /*BaseCoinsDisplay baseCoinsDisplay =
                    _observerTrigger.CurrentCollider.GetComponentInChildren<BaseCoinsDisplay>();
                baseCoinsDisplay.Show(true, true);*/
            }
        }
    }
}