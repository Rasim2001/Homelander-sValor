using CameraManagement;
using DG.Tweening;
using Player;
using Player.Orders;
using UnityEngine;

namespace Infastructure.Services.Tutorial.States
{
    public class MovementStateTutorial : ITutorialState
    {
        private readonly ITutorialStateMachine _stateMachine;

        private readonly PlayerMove _playerMove;
        private readonly PlayerInputOrders _playerInputOrders;
        private readonly Transform _moveTextTransform;
        private readonly GameObject _movementStateGameObject;

        private CinemachineFollow _cinemachineFollowCameras;

        public MovementStateTutorial(
            ITutorialStateMachine stateMachine,
            PlayerMove playerMove,
            PlayerInputOrders playerInputOrders,
            Transform moveTextTransform,
            GameObject movementStateGameObject)
        {
            _stateMachine = stateMachine;
            _playerMove = playerMove;
            _playerInputOrders = playerInputOrders;
            _moveTextTransform = moveTextTransform;
            _movementStateGameObject = movementStateGameObject;
        }

        public void Enter()
        {
            _cinemachineFollowCameras = Camera.main.GetComponent<CinemachineFollow>();
            _cinemachineFollowCameras.NearCamera.gameObject.SetActive(false);
            _cinemachineFollowCameras.FarCamera.gameObject.SetActive(true);

            _movementStateGameObject.SetActive(true);

            _moveTextTransform.localScale = Vector3.zero;
            _moveTextTransform.DOScale(Vector3.one, 0.25f);
            _playerMove.enabled = false;
            _playerInputOrders.enabled = false;
        }

        public void Exit()
        {
            _moveTextTransform.DOScale(Vector3.zero, 0.1f)
                .OnComplete(() => _movementStateGameObject.SetActive(false));

            _cinemachineFollowCameras.NearCamera.gameObject.SetActive(true);
            _cinemachineFollowCameras.FarCamera.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                _playerMove.enabled = true;
                _stateMachine.ChangeState<ChestStateTutorial>();
            }
        }
    }
}