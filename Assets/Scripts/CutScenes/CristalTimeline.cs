using System;
using System.Collections.Generic;
using CameraManagement;
using Cinemachine;
using CutScenes.MicroInfastructure;
using DayCycle;
using Infastructure;
using Infastructure.Factories.GameFactories;
using Infastructure.Services.PlayerProgressService;
using Infastructure.StaticData.StaticDataService;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace CutScenes
{
    public class CristalTimeline : MonoBehaviour, ICristalTimeline
    {
        [FoldoutGroup("CristalEmbersParticles")] [SerializeField]
        private ParticleSystem _cristalEmbersParticles;

        [FoldoutGroup("LightRing")] [SerializeField]
        private Light2D _light2D;

        [FoldoutGroup("CristalSpawnPoint")] [SerializeField]
        private Transform _spawnPoint;

        [FoldoutGroup("StoneSignalData")] [SerializeField]
        private List<StoneSignalData> _stoneSignals;

        [FoldoutGroup("TimelineCamera")] [SerializeField]
        private CinemachineVirtualCamera _timelineCamera;

        [FoldoutGroup("Other")] [SerializeField]
        private ObserverTrigger _observerTrigger;

        [FoldoutGroup("Other")] [SerializeField]
        private Transform _movePoint;
        public bool IsPlaying { get; private set; }

        private ICoroutineRunner _coroutineRunner;
        private Transform _playerTransform;

        private PlayerSignal _playerSignal;
        private CameraSignal _cameraSignal;
        private CristalEmbersParticlesSignal _cristalEmbersParticlesSignal;
        private LightRingSignal _lightRing;
        private CristalSignal _cristalSignal;
        private StonesSignal _stonesSignal;
        private IGameFactory _gameFactory;
        private IPersistentProgressService _persistentProgressService;
        private IStaticDataService _staticDataService;

        private bool _isTriggered;
        private bool _skipCutscene;

        [Inject]
        public void Construct(ICoroutineRunner coroutineRunner, IGameFactory gameFactory,
            IPersistentProgressService persistentProgressService, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _persistentProgressService = persistentProgressService;
            _gameFactory = gameFactory;
            _coroutineRunner = coroutineRunner;
        }

        private void Awake() =>
            _skipCutscene = _staticDataService.CheatStaticData.SkipCutScene;

        public void Initialize(Transform playerTransform) =>
            _playerTransform = playerTransform;

        private void Start()
        {
            InitPlayerSignal();
            InitCameraSignal();
            InitCristalEmbersParticlesSignal();
            InitLightRingSignal();
            InitCristalSignal();
            InitStonesSignal();

            InitPosition();

            _observerTrigger.OnTriggerEnter += TriggerEnter;
        }

        private void OnDestroy() =>
            _observerTrigger.OnTriggerEnter -= TriggerEnter;

        private void InitStonesSignal() =>
            _stonesSignal = new StonesSignal(_stoneSignals, _spawnPoint.position, _coroutineRunner);

        private void InitCristalSignal() =>
            _cristalSignal = new CristalSignal(_gameFactory, this, _coroutineRunner, _playerTransform, _spawnPoint);

        private void InitLightRingSignal() =>
            _lightRing = new LightRingSignal(_light2D);

        private void InitCristalEmbersParticlesSignal() =>
            _cristalEmbersParticlesSignal = new CristalEmbersParticlesSignal(_cristalEmbersParticles, _coroutineRunner);

        private void InitPosition()
        {
            float targetPositionX = _skipCutscene ? -70f : _playerTransform.position.x + 20;

            transform.position = new Vector3(targetPositionX, _playerTransform.position.y);
        }


        private void InitCameraSignal()
        {
            CinemachineFollow cinemachineFollow = Camera.main.GetComponent<CinemachineFollow>();
            CinemachineVirtualCamera cutSceneNearCamera = cinemachineFollow.GetCutSceneNearCamera();

            _cameraSignal = new CameraSignal(_playerTransform, cutSceneNearCamera, _timelineCamera);
        }

        private void InitPlayerSignal() =>
            _playerSignal = new PlayerSignal(_playerTransform, _movePoint);


        private void TriggerEnter()
        {
            if (_isTriggered || !_persistentProgressService.PlayerProgress.CutSceneData.Active || _skipCutscene)
                return;

            _isTriggered = true;

            DayCycleUpdater cycleUpdater = Camera.main.GetComponentInChildren<DayCycleUpdater>();
            cycleUpdater?.FreezTime();

            IsPlaying = true;

            TimelineSequence timelineSequence = new TimelineSequence(_coroutineRunner);

            timelineSequence
                .Add(onComplete => _playerSignal.MoveToTarget(onComplete))
                .Join(_stonesSignal.MoveWaveStones)
                .Join(_cameraSignal.StopCamera)
                .Join(_cameraSignal.ActivateTimelineCamera).WithDelay(1)
                .Join(_cameraSignal.Shake).WithDelay(1)
                .Add(onComplete => _cristalEmbersParticlesSignal.PlayParticles(onComplete))
                .Add(_lightRing.PlayLightRing)
                .Join(_cameraSignal.CancelShake)
                .Join(_stonesSignal.StopMoveStones)
                .Add(onComplete => _cristalSignal.PlayCristalAnimation(onComplete))
                .Add(() => ExitTimeline(cycleUpdater))
                .Execute();
        }

        private void ExitTimeline(DayCycleUpdater cycleUpdater)
        {
            TimelineSequence timelineSequence = new TimelineSequence(_coroutineRunner);

            timelineSequence
                .Add(_playerSignal.ReturnMoveToDefault)
                .Add(_playerSignal.EnableMovement).WithDelay(0.15f)
                .Add(() => IsPlaying = false)
                .Add(_cameraSignal.MoveToPlayer)
                .Add(cycleUpdater.UnFreezTime)
                .Add(_lightRing.StopLightRing)
                .Execute();
        }
    }
}