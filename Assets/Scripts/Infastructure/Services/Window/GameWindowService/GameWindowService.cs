using Infastructure.Factories.GameFactories;
using Infastructure.StaticData.Windows;
using UnityEngine;

namespace Infastructure.Services.Window.GameWindowService
{
    public class GameWindowService : IGameWindowService
    {
        private readonly IGameUIFactory _gameUIFactory;

        public GameWindowService(IGameUIFactory gameUIFactory) =>
            _gameUIFactory = gameUIFactory;

        public void Open(WindowId windowId)
        {
            switch (windowId)
            {
                case WindowId.GameSettingsWindow:
                    _gameUIFactory.CreateGameSettingsWindow(WindowId.GameSettingsWindow);
                    break;
                case WindowId.FinishTutorialWindow:
                    _gameUIFactory.CreateFinishTutorialWindow(WindowId.FinishTutorialWindow);
                    break;
                case WindowId.WinWindow:
                    _gameUIFactory.CreateWinWindow(WindowId.WinWindow);
                    break;
                case WindowId.DefeatWindow:
                    _gameUIFactory.CreateDefeatWindow(WindowId.DefeatWindow);
                    break;
            }
        }

        public GameObject OpenAndGet(WindowId windowId)
        {
            switch (windowId)
            {
                case WindowId.GameSettingsWindow:
                    return _gameUIFactory.CreateGameSettingsWindow(WindowId.GameSettingsWindow);
            }

            return null;
        }
    }
}