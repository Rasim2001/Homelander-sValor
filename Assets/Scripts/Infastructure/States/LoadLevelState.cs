using DG.Tweening;
using UI.Windows;
using UnityEngine;
using Zenject;

namespace Infastructure.States
{
    public class LoadLevelState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly ICurtainWindow _curtainWindow;

        public LoadLevelState(IStateMachine stateMachine, ISceneLoader sceneLoader, ICurtainWindow curtainWindow)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _curtainWindow = curtainWindow;
        }

        public void Enter()
        {
            DOTween.Clear();

            _curtainWindow.Show();
            _sceneLoader.Load(AssetsPath.GameScene, OnLoaded);
        }

        private void OnLoaded()
        {
        }

        public void Exit()
        {
        }


        public class Factory : PlaceholderFactory<IStateMachine, LoadLevelState>
        {
        }
    }
}