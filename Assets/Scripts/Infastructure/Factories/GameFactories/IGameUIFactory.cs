using Infastructure.StaticData.Windows;
using UnityEngine;

namespace Infastructure.Factories.GameFactories
{
    public interface IGameUIFactory
    {
        GameObject CreateGameSettingsWindow(WindowId windowId);
        void CreateMenuRootUI();
        void CreateFinishTutorialWindow(WindowId windowId);
        void CreateWinWindow(WindowId windowId);
        void CreateDefeatWindow(WindowId windowId);
        GameObject CreateCardsWindow(WindowId windowId);
        void CreateTaskBookWindow(WindowId windowId);
    }
}