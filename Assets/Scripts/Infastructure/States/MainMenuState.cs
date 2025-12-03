using DG.Tweening;
using Infastructure.Factories.ProjectFactories;
using Infastructure.StaticData.Windows;
using Zenject;

namespace Infastructure.States
{
    public class MainMenuState : IState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IProjectUIFactory _projectUIFactory;
        private readonly ICoroutineRunner _coroutineRunner;

        public MainMenuState(ISceneLoader sceneLoader, IProjectUIFactory projectUIFactory,
            ICoroutineRunner coroutineRunner)
        {
            _sceneLoader = sceneLoader;
            _projectUIFactory = projectUIFactory;
            _coroutineRunner = coroutineRunner;
        }

        public void Enter()
        {
            DOTween.KillAll();

            _sceneLoader.Load(AssetsPath.MainMenuScene, OnLoaded);
        }

        public void Exit()
        {
        }

        private void OnLoaded()
        {
            _coroutineRunner.StopAllCoroutines();

            _projectUIFactory.CreateMainMenuRootUI();
            _projectUIFactory.CreateMenuWindow(WindowId.MainMenuWindow);
        }

        public class Factory : PlaceholderFactory<MainMenuState>
        {
        }
    }
}