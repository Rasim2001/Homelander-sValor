using System;
using Infastructure.Services.QuitGameApplicationService;
using Infastructure.States;
using UnityEngine;
using Zenject;

namespace Infastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private IStateMachine _stateMachine;
        private IQuitGameService _quitGameService;

        [Inject]
        private void Construct(IStateMachine stateMachine, IQuitGameService quitGameService)
        {
            _quitGameService = quitGameService;
            _stateMachine = stateMachine;
        }

        private void Start()
        {
            _stateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }

        private void OnApplicationQuit() =>
            _quitGameService.QuitGame();

        public class Factory : PlaceholderFactory<GameBootstrapper>
        {
        }
    }
}