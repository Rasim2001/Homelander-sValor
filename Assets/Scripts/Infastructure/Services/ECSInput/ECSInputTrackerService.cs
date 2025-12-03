using System.Collections.Generic;
using Infastructure.Services.BuildModeServices;
using Infastructure.Services.InputPlayerService;
using Infastructure.Services.PauseService;
using Infastructure.Services.Window.GameWindowService;
using Infastructure.StaticData.Windows;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.ECSInput
{
    public class ECSInputTrackerService : IECSInputTrackerService, ITickable
    {
        private readonly IInputService _inputService;
        private readonly IPauseService _pauseService;
        private readonly IGameWindowService _gameWindowService;
        private readonly IEcsWatchersService _ecsWatchersService;


        private GameObject _ecsWindow;

        public ECSInputTrackerService(
            IInputService inputService,
            IPauseService pauseService,
            IEcsWatchersService ecsWatchersService,
            IGameWindowService gameWindowService)
        {
            _inputService = inputService;
            _pauseService = pauseService;
            _ecsWatchersService = ecsWatchersService;
            _gameWindowService = gameWindowService;
        }


        public void Tick()
        {
            if (_inputService.ECSPressed)
            {
                if (_ecsWatchersService.CanOpenWindow())
                    HandleEscWindow();
                else
                    CloseEcsWindow();
            }
        }

        private void CloseEcsWindow()
        {
            IEnumerable<IEcsWatcher> watchersCopy = new List<IEcsWatcher>(_ecsWatchersService.EcsWatchers);
            foreach (IEcsWatcher ecsWatcher in watchersCopy)
                ecsWatcher.EcsCancel();

            IEnumerable<IEcsWatcherWindow> watchersWindowCopy =
                new List<IEcsWatcherWindow>(_ecsWatchersService.EcsWatchersWindows);

            foreach (IEcsWatcherWindow watcherWindow in watchersWindowCopy)
                watcherWindow.EcsCancel();
        }


        private void HandleEscWindow()
        {
            if (!_pauseService.IsPaused)
                _ecsWindow = _gameWindowService.OpenAndGet(WindowId.GameSettingsWindow);
        }
    }
}