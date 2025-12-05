using System;
using Infastructure.Services.EnemyWaves;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PlayerRegistry;
using Infastructure.Services.SafeBuildZoneTracker;
using Infastructure.Services.Tutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using Player;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.CallNight
{
    public class CallNightService : ITickable, IDisposable, ICallNightService
    {
        private readonly IInputService _inputService;
        private readonly ISafeBuildZone _safeBuildZone;
        private readonly IEnemyWavesService _enemyWavesService;
        private readonly IPlayerRegistryService _playerRegistryService;
        private readonly ITutorialProgressService _tutorialProgressService;

        public event Action OnCallNightHappened;

        private CallNightUI _callNightUI;
        private readonly float _pressDuration = 2;

        private float _currentTime = 0;

        private bool _holding;

        public CallNightService(IInputService inputService, ISafeBuildZone safeBuildZone,
            IEnemyWavesService enemyWavesService, IPlayerRegistryService playerRegistryService,
            ITutorialProgressService tutorialProgressService)
        {
            _inputService = inputService;
            _safeBuildZone = safeBuildZone;
            _enemyWavesService = enemyWavesService;
            _playerRegistryService = playerRegistryService;
            _tutorialProgressService = tutorialProgressService;
        }

        public void SubscribeUpdates() =>
            _playerRegistryService.OnInitialized += OnPlayerInitialized;


        public void Dispose() =>
            _playerRegistryService.OnInitialized -= OnPlayerInitialized;

        private void OnPlayerInitialized() =>
            _callNightUI = _playerRegistryService.Player.GetComponentInChildren<CallNightUI>();

        public void Tick()
        {
            if (_safeBuildZone.IsNight || !_tutorialProgressService.IsCallingNightReadyToUse)
                return;

            if (_inputService.SpacePressed)
                _holding = true;

            if (_inputService.SpacePressedUp)
                Clear();

            if (_holding)
            {
                _currentTime += Time.deltaTime;

                _callNightUI.SetValue(_currentTime / _pressDuration);

                if (_currentTime >= _pressDuration)
                {
                    _enemyWavesService.ForceNight();
                    OnCallNightHappened?.Invoke();

                    Clear();
                }
            }
        }

        private void Clear()
        {
            _holding = false;
            _currentTime = 0;
            _callNightUI.SetValue(0);
        }
    }
}