using Infastructure.Factories.ProjectFactories;
using Infastructure.Services.AssetProvider;
using Infastructure.Services.PlayerProgressService;
using Infastructure.Services.ProgressWatchers;
using Infastructure.Services.QuitGameApplicationService;
using Infastructure.Services.SaveLoadService;
using Infastructure.Services.Tutorial;
using Infastructure.Services.Tutorial.TutorialProgress;
using Infastructure.Services.Window.ProjectWindowService;
using Infastructure.States;
using Infastructure.StaticData.StaticDataService;
using UI.Windows;
using Zenject;

namespace Infastructure.CompositionRoot
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindQuitGameService();

            BindGameBootstraperFactory();

            BindCoroutineRunner();

            BindSceneLoader();

            BindGameStateMachine();

            BindStaticDataService();

            BindProjectUIFactory();

            BindProjectWindowService();

            BindPersistentProgressService();

            BindSaveLoadService();

            BindProgressWatchersService();

            BindTitleCurtain();

            BindAssetProvider();

            BindTutorialProgressService();
        }

        private void BindTutorialProgressService() =>
            Container.BindInterfacesAndSelfTo<TutorialProgressService>().AsSingle();

        private void BindQuitGameService() =>
            Container.BindInterfacesAndSelfTo<QuitGameService>().AsSingle();


        private void BindAssetProvider() =>
            Container.BindInterfacesAndSelfTo<AssetProviderService>().AsSingle();

        private void BindTitleCurtain()
        {
            Container
                .Bind<ICurtainWindow>()
                .To<CurtainWindow>()
                .FromComponentInNewPrefabResource(UIAssetPath.CurtainPath)
                .AsSingle();
        }


        private void BindProgressWatchersService() =>
            Container.BindInterfacesAndSelfTo<ProgressWatchersService>().AsSingle();


        private void BindPersistentProgressService() =>
            Container.BindInterfacesAndSelfTo<PersistentProgressService>().AsSingle();

        private void BindSaveLoadService() =>
            Container.BindInterfacesAndSelfTo<SaveLoadService>().AsSingle();

        private void BindProjectWindowService() =>
            Container.BindInterfacesAndSelfTo<ProjectWindowService>().AsSingle();

        private void BindProjectUIFactory() =>
            Container.BindInterfacesAndSelfTo<ProjectUIFactory>().AsSingle();

        private void BindStaticDataService() =>
            Container.BindInterfacesAndSelfTo<StaticDataService>().AsSingle();

        private void BindGameBootstraperFactory()
        {
            Container
                .BindFactory<GameBootstrapper, GameBootstrapper.Factory>()
                .FromComponentInNewPrefabResource(AssetsPath.GameBootstrapperPath);
        }

        private void BindCoroutineRunner()
        {
            Container
                .Bind<ICoroutineRunner>()
                .To<CoroutineRunner>()
                .FromComponentInNewPrefabResource(AssetsPath.CoroutineRunnerPath)
                .AsSingle();
        }

        private void BindSceneLoader() =>
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();

        private void BindGameStateMachine()
        {
            Container
                .Bind<IStateMachine>()
                .FromSubContainerResolve()
                .ByInstaller<GameStateMachineInstaller>()
                .AsSingle();
        }
    }
}