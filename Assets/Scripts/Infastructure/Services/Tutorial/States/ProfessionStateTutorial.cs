using _Tutorial;
using BuildProcessManagement.WorkshopBuilding;
using Infastructure.Services.Flag;
using Player.Orders;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Infastructure.Services.Tutorial.States
{
    public class ProfessionStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _tutorialStateMachine;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly ObserverTrigger _observerTrigger;
        private readonly PlayerInputOrders _playerInputOrders;

        private readonly IFlagTrackerService _flagTrackerService;
        private WorkshopInfo _hammerWorkshopInfo;

        public ProfessionStateTutorial(ITutorialStateMachine tutorialStateMachine,
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
            }

            ProfessionChangerText professionChangerText = _hammerWorkshopInfo.GetComponent<ProfessionChangerText>();
            professionChangerText.Change();

            TutorialHints tutorialHints = _hammerWorkshopInfo.GetComponent<TutorialHints>();
            tutorialHints.Show();

            _tutorialArrowHelper.SetTarget(tutorialHints.transform);
        }

        public void Exit()
        {
            _observerTrigger.OnTriggerEnter -= TriggerEnter;
            _observerTrigger.OnTriggerExit -= TriggerExit;

            _playerInputOrders.enabled = false;
            _tutorialArrowHelper.SetTarget(null);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_observerTrigger.CurrentCollider.transform == _hammerWorkshopInfo.transform)
                {
                    TutorialHints previousHint = _hammerWorkshopInfo.GetComponent<TutorialHints>();
                    previousHint.Hide();

                    _playerInputOrders.enabled = true;
                    _playerInputOrders.OnWorkshopHappenedOnTutorial?.Invoke();
                    _tutorialStateMachine.ChangeState<TreeStateTutorial>();
                }
            }
        }

        private void TriggerEnter()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _hammerWorkshopInfo.transform)
            {
                Light2D light2D = _hammerWorkshopInfo.GetComponent<Light2D>();
                light2D.enabled = true;
            }
        }

        private void TriggerExit()
        {
            if (_observerTrigger.CurrentCollider != null &&
                _observerTrigger.CurrentCollider.transform == _hammerWorkshopInfo.transform)
            {
                Light2D light2D = _hammerWorkshopInfo.GetComponent<Light2D>();
                light2D.enabled = false;
            }
        }
    }
}