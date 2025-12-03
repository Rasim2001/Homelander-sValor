using System;
using System.Collections;
using CameraManagement;
using Cinemachine;
using DG.Tweening;
using Enemy;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.Flag;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Windows;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Infastructure.Services.CameraFocus
{
    public class CameraFocusService : ICameraFocusService, ITickable, IDisposable
    {
        public bool PlayerDefeated { get; set; }

        public event Action OnDefeatHappened;

        private readonly IGameWindowService _gameWindowService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IWaveEnemiesCountService _waveEnemiesCountService;
        private readonly IFlagTrackerService _flagTrackerService;

        private readonly Volume _globalVolume;

        private Coroutine _handleDefeatCoroutine;

        private CinemachineFollow _cinemachineFollow;

        private CinemachineVirtualCamera _nearCamera;
        private CinemachineVirtualCamera _farCamera;

        private float _defaultNearSize;
        private float _defaultFarSize;

        public CameraFocusService(
            IGameWindowService gameWindowService,
            ICoroutineRunner coroutineRunner,
            IWaveEnemiesCountService waveEnemiesCountService,
            IFlagTrackerService flagTrackerService,
            Volume globalVolume)
        {
            _gameWindowService = gameWindowService;
            _coroutineRunner = coroutineRunner;
            _waveEnemiesCountService = waveEnemiesCountService;
            _flagTrackerService = flagTrackerService;
            _globalVolume = globalVolume;
        }

        public void Initialize(CinemachineFollow cinemachineFollow)
        {
            _cinemachineFollow = cinemachineFollow;

            _nearCamera = _cinemachineFollow.GetNearCamera();
            _defaultNearSize = _nearCamera.m_Lens.OrthographicSize;

            _farCamera = _cinemachineFollow.GetFarCamera();
            _defaultFarSize = _farCamera.m_Lens.OrthographicSize;
        }

        public void ShowMainFlagDestruction()
        {
            PlayerDefeated = true;
            OnDefeatHappened?.Invoke();

            if (_globalVolume.profile.TryGet(out Vignette vignette))
                vignette.active = true;

            _cinemachineFollow.FocusOnMainFlag();

            if (_handleDefeatCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_handleDefeatCoroutine);
                _handleDefeatCoroutine = null;
            }

            _handleDefeatCoroutine = _coroutineRunner.StartCoroutine(StartHandleDefeatCoroutine());
        }

        private IEnumerator StartHandleDefeatCoroutine()
        {
            yield return new WaitForSeconds(1.5f);

            foreach (GameObject gameObject in _waveEnemiesCountService.Enemies)
            {
                gameObject.GetComponent<EnemyAttack>().enabled = false;
                gameObject.GetComponent<EnemyMove>().enabled = false;

                gameObject.GetComponent<EnemyAnimator>().PlayFlagDestroyedAnimation();
            }

            Transform mainFlag = _flagTrackerService.GetMainFlag();
            mainFlag.DOScale(Vector3.zero, 1f).SetEase(Ease.InBounce);

            yield return new WaitForSeconds(2);

            _gameWindowService.Open(WindowId.DefeatWindow);
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetZoomCamera(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SetZoomCamera(1.15f);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SetZoomCamera(1.3f);
        }

        public void Dispose() =>
            PlayerDefeated = false;

        private void SetZoomCamera(float x)
        {
            _nearCamera.m_Lens.OrthographicSize = _defaultNearSize * x;
            _farCamera.m_Lens.OrthographicSize = _defaultFarSize * x;
        }
    }
}