using Player;
using Player.Orders;
using UnityEngine;

namespace Infastructure.Services.Tutorial.States
{
    public class UseAccelerationStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;
        private readonly PlayerMove _playerMove;
        private readonly PlayerInputOrders _playerInputOrders;
        private readonly GameObject _stateGameObject;

        private bool _hintIsActive;

        public UseAccelerationStateTutorial(
            ITutorialStateMachine stateMachine,
            PlayerMove playerMove,
            PlayerInputOrders playerInputOrders,
            GameObject stateGameObject)
        {
            _stateMachine = stateMachine;
            _playerMove = playerMove;
            _playerInputOrders = playerInputOrders;
            _stateGameObject = stateGameObject;
        }

        public void Enter()
        {
            _hintIsActive = true;
            _stateGameObject.SetActive(true);
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _playerMove.enabled = true;
            }


            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _playerMove.enabled = false;

                if (!_hintIsActive)
                    return;

                _hintIsActive = false;
                _stateGameObject.SetActive(false);
            }
        }
    }
}