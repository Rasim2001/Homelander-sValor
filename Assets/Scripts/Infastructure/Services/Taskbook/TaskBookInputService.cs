using Infastructure.Factories.GameFactories;
using Infastructure.Services.ECSInput;
using Infastructure.Services.InputPlayerService;
using Infastructure.StaticData.Windows;
using UnityEngine;
using Zenject;

namespace Infastructure.Services.Taskbook
{
    public class TaskBookInputService : ITickable
    {
        private readonly IInputService _inputService;
        private readonly IGameUIFactory _gameUIFactory;
        private readonly IEcsWatchersService _ecsWatchersService;

        public TaskBookInputService(
            IInputService inputService,
            IGameUIFactory gameUIFactory,
            IEcsWatchersService ecsWatchersService)
        {
            _inputService = inputService;
            _gameUIFactory = gameUIFactory;
            _ecsWatchersService = ecsWatchersService;
        }

        public void Tick()
        {
            if (_inputService.TaskBookPressed && _ecsWatchersService.CanOpenWindow())
                _gameUIFactory.CreateTaskBookWindow(WindowId.TaskBookWindow);
        }
    }
}