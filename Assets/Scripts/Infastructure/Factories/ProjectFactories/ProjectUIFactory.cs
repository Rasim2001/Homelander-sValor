using Infastructure.StaticData.StaticDataService;
using Infastructure.StaticData.Windows;
using UnityEngine;
using Zenject;

namespace Infastructure.Factories.ProjectFactories
{
    public class ProjectUIFactory : IProjectUIFactory
    {
        private readonly DiContainer _diContainer;
        private readonly IStaticDataService _staticData;

        private GameObject _mainMenuUIRoot;

        public ProjectUIFactory(DiContainer diContainer, IStaticDataService staticDataService)
        {
            _diContainer = diContainer;
            _staticData = staticDataService;
        }

        public void CreateMainMenuRootUI()
        {
            GameObject prefab = Resources.Load<GameObject>(UIAssetPath.MainMenuUIRootPath);
            _mainMenuUIRoot = Object.Instantiate(prefab);
        }

        public void CreateMenuWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            _diContainer.InstantiatePrefab(windowConfig.Prefab, _mainMenuUIRoot.transform);
        }

        public void CreateMenuSettingsWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            _diContainer.InstantiatePrefab(windowConfig.Prefab, _mainMenuUIRoot.transform);
        }
        
        public void CreateIntroTutorialWindow(WindowId windowId)
        {
            WindowConfig windowConfig = _staticData.ForWindow(windowId);
            _diContainer.InstantiatePrefab(windowConfig.Prefab, _mainMenuUIRoot.transform);
        }
    }
}