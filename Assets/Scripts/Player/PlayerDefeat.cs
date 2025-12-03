using System;
using Infastructure.Services.CameraFocus;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerDefeat : MonoBehaviour
    {
        private PlayerAnimator _playerAnimator;
        private ICameraFocusService _cameraFocusService;

        [Inject]
        public void Construct(ICameraFocusService cameraFocusService) =>
            _cameraFocusService = cameraFocusService;

        private void Awake() =>
            _playerAnimator = GetComponent<PlayerAnimator>();

        private void Start() =>
            _cameraFocusService.OnDefeatHappened += Defeat;

        private void OnDestroy() =>
            _cameraFocusService.OnDefeatHappened -= Defeat;

        private void Defeat() =>
            _playerAnimator.PlayForceIdleAnimation();
    }
}