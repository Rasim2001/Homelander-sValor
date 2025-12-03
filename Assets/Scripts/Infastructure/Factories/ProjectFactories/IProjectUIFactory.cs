using Infastructure.StaticData.Windows;

namespace Infastructure.Factories.ProjectFactories
{
    public interface IProjectUIFactory
    {
        void CreateMenuWindow(WindowId windowId);
        void CreateMainMenuRootUI();
        void CreateMenuSettingsWindow(WindowId windowId);
        void CreateIntroTutorialWindow(WindowId windowId);
    }
}