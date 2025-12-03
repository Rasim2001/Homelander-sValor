using _Tutorial;
using BuildProcessManagement.WorkshopBuilding;
using Infastructure.Services.Flag;
using Player.Orders;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Infastructure.Services.Tutorial.States
{
    public class WorkshopStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly IFlagTrackerService _flagTrackerService;
        private readonly ObserverTrigger _observerTrigger;
        private readonly PlayerInputOrders _playerInputOrders;

        private WorkshopInfo _hammerWorkshopInfo;
        private WorkshopInfo _bowWorkshopInfo;
        private WorkshopInfo _currentWorkshopInfo;

        private int _amountOfWorkshops = 2;

        public WorkshopStateTutorial(ITutorialStateMachine tutorialStateMachine,
            TutorialArrowHelper tutorialArrowHelper,
            ObserverTrigger observerTrigger,
            PlayerInputOrders playerInputOrders,
            IFlagTrackerService flagTrackerService)
        {
            _tutorialStateMachine = tutorialStateMachine;
            _tutorialArrowHelper = tutorialArrowHelper;
            _observerTrigger = observerTrigger;
            _playerInputOrders = playerInputOrders;
            _flagTrackerService = flagTrackerService;
        }


        public void Enter()
        {
            _observerTrigger.OnTriggerEnter += TriggerEnter;
            _observerTrigger.OnTriggerExit += TriggerExit;

            Transform bonfireTransform = _flagTrackerService.GetMainFlag();
            WorkshopInfo[] workshopInfos = bonfireTransform.GetComponentsInChildren<WorkshopInfo>();

            foreach (WorkshopInfo workshopInfo in workshopInfos)
            {
                if (workshopInfo.WorkshopItemId == WorkshopItemId.Hammer)
                    _hammerWorkshopInfo = workshopInfo;
                else if (workshopInfo.WorkshopItemId == WorkshopItemId.Arrow)
                    _bowWorkshopInfo = workshopInfo;
            }

            _currentWorkshopInfo = _hammerWorkshopInfo;

            TutorialHints tutorialHints = _currentWorkshopInfo.GetComponent<TutorialHints>();
            tutorialHints.Show();

            _tutorialArrowHelper.SetTarget(_currentWorkshopInfo.transform);
        }

        public void Exit()
        {
            _observerTrigger.OnTriggerEnter -= TriggerEnter;
            _observerTrigger.OnTriggerExit -= TriggerExit;

            TutorialHints previousHint = _currentWorkshopInfo.GetComponent<TutorialHints>();
            previousHint.Hide();

            _tutorialArrowHelper.SetTarget(null);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_observerTrigger.CurrentCollider != null &&
                    _observerTrigger.CurrentCollider.transform == _currentWorkshopInfo.transform)
                {
                    TutorialHints previousHint = _currentWorkshopInfo.GetComponent<TutorialHints>();
                    previousHint.Hide();

                    _currentWorkshopInfo = _bowWorkshopInfo;

                    TutorialHints currentHint = _currentWorkshopInfo.GetComponent<TutorialHints>();
                    currentHint.Show();
                    _tutorialArrowHelper.SetTarget(currentHint.transform);

                    _playerInputOrders.enabled = true;
                    _playerInputOrders.OnWorkshopHappenedOnTutorial?.Invoke();
                    _playerInputOrders.enabled = false;
                    _amountOfWorkshops--;

                    if (_amountOfWorkshops == 0)
                        _tutorialStateMachine.ChangeState<ProfessionStateTutorial>();
                }
            }
        }

        private void TriggerEnter()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _currentWorkshopInfo.transform)
            {
                Light2D light2D = _currentWorkshopInfo.GetComponent<Light2D>();
                light2D.enabled = true;
            }
        }

        private void TriggerExit()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _currentWorkshopInfo.transform)
            {
                Light2D light2D = _currentWorkshopInfo.GetComponent<Light2D>();
                light2D.enabled = false;
            }
        }
    }
}