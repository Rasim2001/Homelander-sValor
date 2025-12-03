using System;
using DG.Tweening;
using Player;
using UnityEngine;

namespace CutScenes
{
    public class PlayerSignal
    {
        private readonly Transform _playerTransform;
        private readonly Transform _point;

        public PlayerSignal(Transform playerTransform, Transform point)
        {
            _point = point;
            _playerTransform = playerTransform;
        }

        public void MoveToTarget(Action onComplete)
        {
            PlayerMove playerMove = _playerTransform.GetComponent<PlayerMove>();
            PlayerAnimator playerAnimator = _playerTransform.GetComponent<PlayerAnimator>();

            playerMove.SetDefaultSpeed();
            playerMove.enabled = false;

            float speed = playerMove.Speed;
            float distance = Mathf.Abs(_playerTransform.position.x - _point.position.x);
            float time = distance / speed;

            playerAnimator.PlayForceIdleAnimation();

            _playerTransform.DOMove(_point.position, time).SetEase(Ease.Linear).SetDelay(1f)
                .OnStart(() =>
                {
                    playerAnimator.ResetForceIdleTrigger();
                    playerAnimator.PlayWalkAnimation(true);
                })
                .OnComplete(() =>
                {
                    playerAnimator.PlayWalkAnimation(false);
                    playerAnimator.PlayFlyAnimation(true);

                    onComplete?.Invoke();
                });
        }

        public void ReturnMoveToDefault()
        {
            PlayerAnimator playerAnimator = _playerTransform.GetComponent<PlayerAnimator>();
            playerAnimator.PlayFlyAnimation(false);

            /*float downPosition = _playerTransform.position.y - 0.5f;
            _playerTransform.DOMoveY(downPosition, 1).SetEase(Ease.Linear).SetDelay(0.5f).OnComplete(() =>
            {
                PlayerMove playerMove = _playerTransform.GetComponent<PlayerMove>();
                playerMove.enabled = true;

                onComplete.Invoke();
            });*/
        }

        public void EnableMovement()
        {
            PlayerMove playerMove = _playerTransform.GetComponent<PlayerMove>();
            playerMove.enabled = true;
        }
    }
}