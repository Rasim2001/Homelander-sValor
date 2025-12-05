using Infastructure.Services.InputPlayerService;
using Infastructure.Services.Tutorial.TutorialProgress;
using UnityEngine;
using Zenject;

namespace Player.Shoot
{
    public class PlayerAttack : MonoBehaviour
    {
        private const int ANIMATION_SPEED_ON = 1;
        private const int ANIMATION_SPEED_OFF = 0;

        private readonly float _maxDistance = 8;
        private readonly float _minDistance = 2;

        private readonly float _minHoldTime = 0;
        private readonly float _maxHoldTime = 0.5f;

        private PlayerFlip _playerFlip;
        private PlayerAnimator _playerAnimator;
        private PlayerShooter _playerShooter;

        private float _mouseHoldTime;
        private Vector3 _targetPosition;

        private bool _needToShoot;

        private float _shootPointX;
        private IInputService _inputService;
        private ITutorialProgressService _tutorialProgressService;

        [Inject]
        public void Construct(IInputService inputService, ITutorialProgressService tutorialProgressService)
        {
            _tutorialProgressService = tutorialProgressService;
            _inputService = inputService;
        }

        private void Awake()
        {
            _playerFlip = GetComponent<PlayerFlip>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _playerShooter = GetComponent<PlayerShooter>();

            _shootPointX = _playerShooter.ShootPoint.transform.position.x;
        }


        private void Update()
        {
            if (!_tutorialProgressService.IsAttackReadyToUse)
                return;

            if (CanShoot())
                TryAim();
            else if (_inputService.ShootPressedUp)
                TryShoot();
            else if (_inputService.ShootPressed)
                UpdateTime();
        }

        private void TryAim()
        {
            _needToShoot = false;
            _playerAnimator.PlayAimAndShootAnimation();
        }

        private bool CanShoot()
        {
            return _inputService.ShootPressedDown &&
                   !_playerAnimator.AnyStateIsActive &&
                   !_playerAnimator.IsBuildingModeActive;
        }

        public void StartAiming()
        {
            if (_needToShoot)
                ExecuteShot();
            else
                _playerAnimator.SetSpeed(ANIMATION_SPEED_OFF);
        }


        private void TryShoot()
        {
            if (IsAnimationSpeedOff())
            {
                _playerAnimator.SetSpeed(ANIMATION_SPEED_ON);

                ExecuteShot();
            }
            else
                _needToShoot = true;
        }

        private bool IsAnimationSpeedOff() =>
            _playerAnimator.GetSpeed() == 0;

        private void ExecuteShot()
        {
            PrepareTargetPosition();
            Shoot();
            ResetTime();
        }

        private void Shoot()
        {
            _playerShooter.ShootPoint.transform.localPosition = SetCorrectShootPoint();
            _playerShooter.Shoot(_targetPosition);
        }

        private void PrepareTargetPosition()
        {
            float normalizedHold = Mathf.InverseLerp(_minHoldTime, _maxHoldTime, _mouseHoldTime);
            float distance = Mathf.Lerp(_minDistance, _maxDistance, normalizedHold);

            float targetPositionX = transform.position.x - _playerFlip.FlipValue() * distance;

            _targetPosition = new Vector3(targetPositionX, transform.position.y, transform.position.z);
        }


        private void ResetTime() =>
            _mouseHoldTime = 0;

        private void UpdateTime() =>
            _mouseHoldTime += Time.deltaTime;

        private Vector3 SetCorrectShootPoint()
        {
            return new Vector3(-_playerFlip.FlipValue() * _shootPointX,
                _playerShooter.ShootPoint.transform.localPosition.y);
        }
    }
}