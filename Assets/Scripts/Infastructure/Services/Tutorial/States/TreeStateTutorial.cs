using _Tutorial;
using Infastructure.Services.NearestBuildFind;
using Player.Orders;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Infastructure.Services.Tutorial.States
{
    public class TreeStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly ITutorialCheckerService _tutorialCheckerService;
        private readonly INearestBuildFindService _nearestBuildFindService;
        private readonly PlayerInputOrders _playerInputOrders;
        private readonly ObserverTrigger _observerTrigger;
        private readonly GameObject _shiftGameObject;

        private TutorialHints _tutorialHints;

        private bool _accelerationHintsIsActive;

        public TreeStateTutorial(ITutorialStateMachine tutorialStateMachine,
            TutorialArrowHelper tutorialArrowHelper,
            ITutorialCheckerService tutorialCheckerService,
            INearestBuildFindService nearestBuildFindService,
            PlayerInputOrders playerInputOrders,
            ObserverTrigger observerTrigger,
            GameObject shiftGameObject)
        {
            _tutorialStateMachine = tutorialStateMachine;
            _tutorialArrowHelper = tutorialArrowHelper;
            _tutorialCheckerService = tutorialCheckerService;
            _nearestBuildFindService = nearestBuildFindService;
            _playerInputOrders = playerInputOrders;
            _observerTrigger = observerTrigger;
            _shiftGameObject = shiftGameObject;
        }

        public void Enter()
        {
            _observerTrigger.OnTriggerEnter += TriggerEnter;
            _observerTrigger.OnTriggerExit += TriggerExit;

            _tutorialHints = _nearestBuildFindService.GetNearestTree();
            _tutorialHints.Show();

            _tutorialArrowHelper.SetTarget(_tutorialHints.transform);
            _tutorialCheckerService.ReadyToUseAcceleration = true;
            _shiftGameObject.SetActive(true);
            _accelerationHintsIsActive = true;
        }

        public void Exit()
        {
            _observerTrigger.OnTriggerEnter -= TriggerEnter;
            _observerTrigger.OnTriggerExit -= TriggerExit;

            _tutorialHints.Hide();
            _playerInputOrders.enabled = false;

            _shiftGameObject.SetActive(false);
            _tutorialArrowHelper.SetTarget(null);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_observerTrigger.CurrentCollider != null &&
                    _observerTrigger.CurrentCollider.transform == _tutorialHints.transform)
                {
                    _playerInputOrders.enabled = true;
                    _playerInputOrders.OnWorkshopHappenedOnTutorial?.Invoke();
                    _tutorialStateMachine.ChangeState<StoneStateTutorial>();
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (!_accelerationHintsIsActive)
                    return;

                _accelerationHintsIsActive = false;
                _shiftGameObject.SetActive(false);
            }
        }

        private void TriggerEnter()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _tutorialHints.transform)
            {
                Light2D light2D = _tutorialHints.GetComponentInChildren<Light2D>();
                light2D.enabled = true;
            }
        }

        private void TriggerExit()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _tutorialHints.transform)
            {
                Light2D light2D = _tutorialHints.GetComponentInChildren<Light2D>();
                light2D.enabled = false;
            }
        }
    }
}