using System;
using Infastructure.Data;
using Infastructure.Services.CameraFocus;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PauseService;
using Infastructure.Services.SaveLoadService;
using Infastructure.Services.Tutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerMove : MonoBehaviour, ISavedProgress
    {
        [SerializeField] private TirednessProgressBar _tirednessProgressBar;
        [SerializeField] private PlayerAnimator _playerAnimator;
        public bool AccelerationPressedWithMove =>
            _inputService.AccelerationPressed &&
            _inputService.MoveKeysPressed &&
            _tutorialProgressService.ReadyToUseAcceleration;

        public float Speed;
        public float AccelerationTime;

        public Action AccelerationButtonUpHappened;

        private IInputService _inputService;
        private SpriteRenderer _spriteRenderer;

        private float _defaultSpeed;
        private float _defaultAccelerationTime;
        private IPauseService _pauseService;
        private ICameraFocusService _cameraFocusService;
        private ITutorialProgressService _tutorialProgressService;


        [Inject]
        public void Construct(
            IInputService inputService,
            ITutorialProgressService tutorialProgressService,
            IPauseService pauseService,
            ICameraFocusService cameraFocusService)
        {
            _cameraFocusService = cameraFocusService;
            _pauseService = pauseService;
            _inputService = inputService;
            _tutorialProgressService = tutorialProgressService;
        }


        private void Awake() =>
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        private void Start()
        {
            _defaultSpeed = Speed;
            _defaultAccelerationTime = AccelerationTime;
        }

        private void Update()
        {
            if (_pauseService.IsPaused || _cameraFocusService.PlayerDefeated)
                return;

            if (_inputService.AccelerationButtonUp || _inputService.MoveKeysButtonUp)
                AccelerationButtonUpHappened?.Invoke();

            Animate();
            UpdateTirednessProgressBar();

            if (_playerAnimator.AnyStateIsActive)
                return;

            Move();
            Accelerate();
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.flipX = progress.WorldData.FlipX;

            if (progress.WorldData.Position != null)
                transform.position = progress.WorldData.Position.AsUnityVector();
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            progress.WorldData.Position = transform.position.AsVectorData();
            progress.WorldData.FlipX = spriteRenderer.flipX;
        }


        public bool IsMoving()
        {
            return _inputService.MoveKeysPressed &&
                   _playerAnimator.AnyStateIsActive == false &&
                   AccelerationTime > 0.1f &&
                   !_playerAnimator.TiredIsActive &&
                   !_cameraFocusService.PlayerDefeated;
        }

        public void ReduceSpeed() =>
            Speed = _defaultSpeed / 2;

        public void SetDefaultSpeed() =>
            Speed = _defaultSpeed;

        private void UpdateTirednessProgressBar()
        {
            float tiredPercentage = AccelerationTime / _defaultSpeed;
            _tirednessProgressBar.UpdateProgressBar(tiredPercentage);

            if (_inputService.AccelerationPressed && _tutorialProgressService.ReadyToUseAcceleration)
            {
                if (tiredPercentage > 0.6f)
                    _tirednessProgressBar.SetAlpha(0f);
                else if (tiredPercentage > 0.3f)
                {
                    float alpha = Mathf.InverseLerp(0.6f, 0.3f, tiredPercentage);
                    _tirednessProgressBar.SetAlpha(alpha);
                }
                else
                    _tirednessProgressBar.SetAlpha(1f);
            }
            else
            {
                float currentAlpha = _tirednessProgressBar.CurrentAlpha;
                float newAlpha = Mathf.Lerp(currentAlpha, 0f, Time.deltaTime * 2);
                _tirednessProgressBar.SetAlpha(newAlpha);
            }
        }

        private void Animate()
        {
            AccelerationTime = AccelerationPressedWithMove && _tutorialProgressService.ReadyToUseAcceleration
                ? Mathf.Max(0, AccelerationTime - Time.deltaTime)
                : Mathf.Min(_defaultAccelerationTime, AccelerationTime + Time.deltaTime);

            _playerAnimator.SetAccelerationTime(AccelerationTime);


            _playerAnimator.PlayWalkAnimation(_inputService.MoveKeysPressed &&
                                              (!_inputService.AccelerationPressed ||
                                               !_tutorialProgressService.ReadyToUseAcceleration));

            if (_tutorialProgressService.ReadyToUseAcceleration)
                _playerAnimator.PlayRunAnimation(_inputService.AccelerationPressed &&
                                                 _inputService.MoveKeysPressed &&
                                                 AccelerationTime > 0.1f);
        }

        private void Move()
        {
            if (AccelerationTime < 0.1f || _playerAnimator.TiredIsActive)
                return;

            if (_inputService.MoveKeysPressed)
            {
                Vector3 movementVector = new Vector3(_inputService.AxisX, 0, 0);
                movementVector.Normalize();

                transform.Translate(movementVector * (Speed * Time.deltaTime));
            }
        }

        private void Accelerate()
        {
            if (AccelerationTime < 0.1f || _playerAnimator.IsBuildingModeActive)
                return;

            if (AccelerationPressedWithMove && _tutorialProgressService.ReadyToUseAcceleration)
                Speed = _defaultSpeed * 2;
            else
                Speed = _defaultSpeed;
        }
    }
}