using _Tutorial;
using Chest;
using Player;
using Player.Orders;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Infastructure.Services.Tutorial.States
{
    public class ChestStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly TutorialArrowHelper _tutorialArrowHelper;
        private readonly PlayerMove _playerMove;
        private readonly PlayerInputOrders _playerInputOrders;
        private readonly ObserverTrigger _chestObserverTrigger;
        private readonly GameObject _chestObject;

        private bool _chestOpened;
        private TutorialHints _tutorialHints;

        public ChestStateTutorial(
            ITutorialStateMachine stateMachine,
            TutorialArrowHelper tutorialArrowHelper,
            PlayerMove playerMove,
            PlayerInputOrders playerInputOrders,
            ObserverTrigger chestObserverTrigger,
            GameObject chestObject)
        {
            _stateMachine = stateMachine;
            _tutorialArrowHelper = tutorialArrowHelper;
            _playerMove = playerMove;
            _playerInputOrders = playerInputOrders;
            _chestObserverTrigger = chestObserverTrigger;
            _chestObject = chestObject;
        }


        public void Enter()
        {
            _chestObserverTrigger.OnTriggerEnter += TriggerEnter;
            _chestObserverTrigger.OnTriggerExit += TriggerExit;

            _tutorialHints = _chestObject.GetComponent<TutorialHints>();
            _tutorialHints.Show();

            _tutorialArrowHelper.SetTarget(_tutorialHints.transform);
        }

        public void Exit()
        {
            _tutorialArrowHelper.SetTarget(null);

            _tutorialHints.Hide();

            _chestObserverTrigger.OnTriggerEnter -= TriggerEnter;
            _chestObserverTrigger.OnTriggerExit -= TriggerExit;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) && !_chestOpened)
            {
                if (_chestObserverTrigger.CurrentCollider != null &&
                    _chestObserverTrigger.CurrentCollider.TryGetComponent(out ChestActivator chestActivator))
                {
                    _chestOpened = true;
                    _playerMove.enabled = false;
                    chestActivator.Activate(_playerMove);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && _chestOpened)
            {
                _playerInputOrders.enabled = true;
                _playerMove.enabled = true;
                _playerInputOrders.OnCastSkillHappenedOnTutorial?.Invoke();
                _stateMachine.ChangeState<BindHomelessStateTutorial>();
            }
        }

        private void TriggerEnter()
        {
            if (_chestObserverTrigger.CurrentCollider != null &&
                _chestObserverTrigger.CurrentCollider.transform == _tutorialHints.transform)
            {
                Light2D light2D = _tutorialHints.GetComponent<Light2D>();
                light2D.enabled = true;
            }
        }

        private void TriggerExit()
        {
            if (_tutorialHints == null)
                return;

            if (_chestObserverTrigger.CurrentCollider != null &&
                _chestObserverTrigger.CurrentCollider.transform == _tutorialHints.transform)
            {
                Light2D light2D = _tutorialHints.GetComponent<Light2D>();
                light2D.enabled = false;
            }
        }
    }
}